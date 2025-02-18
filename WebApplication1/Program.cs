using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using WebApplication1.Dtos;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);


var msSqlContainer = new MsSqlBuilder()
    .WithPortBinding(666, 1433)
    .Build();
await msSqlContainer.StartAsync();
Console.WriteLine($"Connectionstring: {msSqlContainer.GetConnectionString()}");

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(msSqlContainer.GetConnectionString()));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/Persons", async (MyDbContext context) =>
{
    var persons = await context.Persons
        .Include(x => x.PersonCompanies)
        .ThenInclude(pc => pc.Company)
        .ToListAsync();

    // map to persondto including company dtos
    var personDtos = persons.Select(p => new PersonDto
    {
        Id = p.Id,
        Name = p.Name,
        ConcurrencyToken = p.ConcurrencyToken,
        Companies = p.PersonCompanies.Select(c => new CompanyDto
        {
            Id = c.Id,
            CompanyId = c.CompanyId,
            Name = c.Company.Name,
            FromDate = c.FromDate,
            ToDate = c.ToDate,
            ConcurrencyToken = c.ConcurrencyToken
        }).ToList()
    }).ToList();

    return personDtos;
});

// put endpoint on person including personcompany
app.MapPut("/Persons/{id}", async (MyDbContext context, int id, PersonDto personDto) =>
{
    var person = await context.Persons
        .Include(x => x.PersonCompanies)
        .ThenInclude(pc => pc.Company)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id);
    if (person == null)
    {
        return Results.NotFound();
    }
    person.Name = personDto.Name;
    person.ConcurrencyToken = personDto.ConcurrencyToken;
    // update companies
    foreach (var companyDto in personDto.Companies)
    {
        var personCompany = person.PersonCompanies.FirstOrDefault(x => x.Id == companyDto.Id);
        if (personCompany == null)
        {
            person.PersonCompanies.Add(new PersonCompany
            {
                CompanyId = companyDto.CompanyId,
                FromDate = companyDto.FromDate,
                ToDate = companyDto.ToDate
            });
        }
        else
        {
            personCompany.FromDate = companyDto.FromDate;
            personCompany.ToDate = companyDto.ToDate;
            personCompany.CompanyId = companyDto.CompanyId;
            personCompany.Company.Name = companyDto.Name;
            personCompany.ConcurrencyToken = companyDto.ConcurrencyToken;
        }
    }

    try
    {
        context.Update(person);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        return Results.Conflict(new { message = "Concurrency conflict occurred.", details = ex.Message });
    }
});

// get endpoint for companies
app.MapGet("/Companies", async (MyDbContext context) =>
{
    var companies = await context.Companies.ToListAsync();
    return companies;
});

// post endpoint for company
app.MapPost("/Companies", async (MyDbContext context, CompanyDto companyDto) =>
{
    var company = new Company(companyDto.Name)
    {
        ConcurrencyToken = companyDto.ConcurrencyToken
    };
    context.Companies.Add(company);
    await context.SaveChangesAsync();
    return Results.Created($"/Companies/{company.Id}", company);
});

using (var context = new MyDbContext(builder.Services.BuildServiceProvider()
    .GetRequiredService<DbContextOptions<MyDbContext>>()))
{
    context.Database.EnsureCreated();
}

await app.RunAsync();

await msSqlContainer.StopAsync();

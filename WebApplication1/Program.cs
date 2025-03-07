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

    foreach (var companyDto in personDto.Companies)
    {
        var personCompany = person.PersonCompanies.FirstOrDefault(x => x.Id == companyDto.Id);
        // add new companies to person
        if (personCompany == null)
        {
            person.PersonCompanies.Add(new PersonCompany
            {
                CompanyId = companyDto.CompanyId,
                FromDate = companyDto.FromDate,
                ToDate = companyDto.ToDate
            });
        }
        // update existing companies
        else
        {
            personCompany.FromDate = companyDto.FromDate;
            personCompany.ToDate = companyDto.ToDate;
            personCompany.CompanyId = companyDto.CompanyId;
            personCompany.Company.Name = companyDto.Name;
            personCompany.ConcurrencyToken = companyDto.ConcurrencyToken;
        }
    }

    // remove personcompany from person when company not in dto
    foreach (var personCompany in person.PersonCompanies.ToList())
    {
        if (!personDto.Companies.Any(x => x.Id == personCompany.Id))
        {
            context.Remove(personCompany);
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

// only update assignments of companies to persons
app.MapPost("/Persons/{id}/Companies", async (MyDbContext context, int id, PersonCompaniesDto personCompaniesDto) =>
{
    var person = await context.Persons
        .Include(x => x.PersonCompanies)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id);

    if (person == null)
    {
        return Results.NotFound($"Person with Id {id}");
    }

    person.ConcurrencyToken = personCompaniesDto.ConcurrencyToken;

    var personState = context.Entry(person).State;

    foreach (var companyId in personCompaniesDto.CompanyIds)
    {
        var company = await context.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        if (company == null)
        {
            return Results.NotFound($"Company with Id {companyId}");
        }

        // ne need to add company if already exists
        if (person.PersonCompanies.Any(x => x.CompanyId == companyId))
        {
            continue;
        }

        // add company to person
        var newPersonCompany = new PersonCompany
        {
            CompanyId = companyId
        };

        person.PersonCompanies.Add(newPersonCompany);

        //context.Entry(newPersonCompany).State = EntityState.Added;

        person.ConcurrencyToken = personCompaniesDto.ConcurrencyToken;
    }

    // remove company from person when not in dto
    foreach (var personCompany in person.PersonCompanies.ToList())
    {
        if (!personCompaniesDto.CompanyIds.Contains(personCompany.CompanyId))
        {
            context.Remove(personCompany);
            //context.Entry(personCompany).State = EntityState.Deleted;
        }
    }

    try
    {
        //context.Entry(person).State = EntityState.Modified;
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

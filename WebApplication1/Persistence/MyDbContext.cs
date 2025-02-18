using Microsoft.EntityFrameworkCore;
using System.IO.Pipelines;
using WebApplication1.Models;

public class MyDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Company> Companies { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);

        // Seed data for Companies
        modelBuilder.Entity<Company>().HasData(
            new Company("Acme Corp") { Id = 1, ConcurrencyToken = Guid.NewGuid().ToByteArray() },
            new Company("Globex Inc") { Id = 2, ConcurrencyToken = Guid.NewGuid().ToByteArray() }
        );

        // Seed data for Persons
        modelBuilder.Entity<Person>().HasData(
            new Person("John Doe") { Id = 1, ConcurrencyToken = Guid.NewGuid().ToByteArray() },
            new Person("Jane Smith") { Id = 2, ConcurrencyToken = Guid.NewGuid().ToByteArray() }
        );

        // Seed data for PersonCompany
        modelBuilder.Entity<PersonCompany>().HasData(
            new PersonCompany { Id = 1, CompanyId = 1, PersonId = 1, FromDate = new DateTime(2020, 1, 1), ToDate = null, ConcurrencyToken = Guid.NewGuid().ToByteArray() },
            new PersonCompany { Id = 2, CompanyId = 2, PersonId = 1, FromDate = new DateTime(2020, 1, 1), ToDate = null, ConcurrencyToken = Guid.NewGuid().ToByteArray() }
        );



        base.OnModelCreating(modelBuilder);
    }
}

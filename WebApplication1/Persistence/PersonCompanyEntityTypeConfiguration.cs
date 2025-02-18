using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Models;

namespace WebApplication1.Persistence
{
    public class PersonCompanyEntityTypeConfiguration : IEntityTypeConfiguration<PersonCompany>
    {
        public void Configure(EntityTypeBuilder<PersonCompany> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.Property(x => x.ConcurrencyToken).IsRowVersion();
        }
    }
}

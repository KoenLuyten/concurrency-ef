namespace WebApplication1.Models
{
    public class PersonCompany
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool IsCurrent => ToDate == null;

        public byte[] ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}

namespace WebApplication1.Dtos
{
    public class CompanyDto
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime FromDate { get; set; } = DateTime.Now;

        public DateTime? ToDate { get; set; }

        public bool? IsCurrent { get; set; } = null;

        public byte[] ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}

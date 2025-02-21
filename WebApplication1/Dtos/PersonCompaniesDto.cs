namespace WebApplication1.Dtos
{
    public class PersonCompaniesDto
    {
        public List<int> CompanyIds { get; set; }

        public byte[]? ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}

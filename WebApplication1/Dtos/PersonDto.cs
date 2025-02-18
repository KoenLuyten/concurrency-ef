using WebApplication1.Models;

namespace WebApplication1.Dtos
{
    public class PersonDto
    {
        public int? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public IEnumerable<CompanyDto> Companies { get; set; } = [];

        public byte[] ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}

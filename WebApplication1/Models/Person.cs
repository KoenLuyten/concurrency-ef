namespace WebApplication1.Models
{
    public class Person(string name)
    {
        public int Id { get; set; }

        public string Name { get; set; } = name;

        public ICollection<PersonCompany> PersonCompanies { get; set; } = new List<PersonCompany>();

        public byte[] ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}

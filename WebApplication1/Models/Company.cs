namespace WebApplication1.Models
{
    public class Company(string name)
    {
        public int Id { get; set; }

        public string Name { get; set; } = name;        

        public byte[] ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}

namespace Pronia1.Models
{
    public class Size :BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ProductSize> productSizes { get; set; }
    }
}

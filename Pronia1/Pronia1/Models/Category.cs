namespace Pronia1.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        //relational
        public ICollection<Product> Products { get; set; }



    }
}

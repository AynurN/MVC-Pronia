namespace Pronia1.Models
{
    public class ProductImage : BaseEntity
    {
        public string ImageURL { get; set; }
        public bool? isPrimary { get; set; }
        //relational
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}

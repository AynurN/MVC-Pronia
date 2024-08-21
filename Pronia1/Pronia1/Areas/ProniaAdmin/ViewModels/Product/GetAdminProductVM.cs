using Pronia1.Models;

namespace Pronia1.Areas.ProniaAdmin.ViewModels
{
    public class GetAdminProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
 
        //relational
        public string CategoryName { get; set; }
        public string ProductImage { get; set; }
    }
}

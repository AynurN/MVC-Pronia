using Pronia1.Models;
using System.ComponentModel.DataAnnotations;

namespace Pronia1.Areas.ProniaAdmin.ViewModels
{
    public class CreateAdminProductVM
    {
        public IFormFile MainPhoto { get; set; }
        public IFormFile HoverPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<int>? TagIds { get; set; }
    }
}

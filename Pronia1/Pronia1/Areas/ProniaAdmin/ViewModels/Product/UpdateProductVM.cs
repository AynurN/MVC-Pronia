using Microsoft.Build.Framework;
using Pronia1.Models;

namespace Pronia1.Areas.ProniaAdmin.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public int? CategoryId { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<int>? TagIds { get; set; }
    }
}

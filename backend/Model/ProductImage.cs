using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    public class ProductImage
    {
        [Key, Required]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string brand { get; set; } = string.Empty;
        public int size { get; set; }
        public decimal price { get; set; }

        public IFormFile image { get; set; }
    }
}

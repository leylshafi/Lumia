using Lumia.Models.Base;

namespace Lumia.Models
{
    public class Portfolio:BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

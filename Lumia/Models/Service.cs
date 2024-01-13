using Lumia.Models.Base;

namespace Lumia.Models
{
    public class Service:BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}

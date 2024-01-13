using Lumia.Models.Base;

namespace Lumia.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public List<Portfolio>? Portfolios { get; set; }
    }
}

using Lumia.Models;

namespace Lumia.Areas.Admin.ViewModels
{
    public class UpdatePortfolioVM
    {
        public string? ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
    }
}

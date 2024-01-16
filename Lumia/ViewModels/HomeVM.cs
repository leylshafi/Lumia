using Lumia.Models;

namespace Lumia.ViewModels
{
    public class HomeVM
    {
        public List<Service> Services { get; set; }
        public List<Portfolio> Portfolios { get; set; }
        public Dictionary<string, string> Setting { get; set; }
    }
}

using Lumia.Data;
using Lumia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM vm = new()
            {
                Services = await _context.Services.ToListAsync(),
                Portfolios = await _context.Portfolios.Include(p=>p.Category).ToListAsync(),
                Setting = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value)
        };
            return View(vm);
        }
    }
}
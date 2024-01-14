using Lumia.Areas.Admin.ViewModels;
using Lumia.Data;
using Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Categories.CountAsync();
            var categories = await _context.Categories.Skip(page*2).Take(2).Include(c=>c.Portfolios).ToListAsync();
            PaginationVM<Category> vm = new()
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 2),
                Items = categories
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();
            bool result = await _context.Categories.AnyAsync(c=>c.Name==categoryVM.Name);
            if(result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }
            Category category = new()
            {
                Name = categoryVM.Name,
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (existed is null) return NotFound();
            UpdateCategoryVM vm = new()
            {
                Name = existed.Name,
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult>Update(int id, UpdateCategoryVM categoryVM)
        {
            if(id<= 0) return BadRequest(); 
            var existed = await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (existed is null) return NotFound();
            if (!ModelState.IsValid) return View(categoryVM);
            if(await _context.Categories.AnyAsync(c=>c.Name==categoryVM.Name && c.Id != id))
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(categoryVM);
            }
            existed.Name = categoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Details(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Categories.Include(c=>c.Portfolios).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            return View(existed);
        }

        public async Task<IActionResult>Delete(int id)
        {
            if(id<=0) return BadRequest();
            var existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            _context.Categories.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

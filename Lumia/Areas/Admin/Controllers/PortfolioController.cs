using Lumia.Areas.Admin.ViewModels;
using Lumia.Data;
using Lumia.Models;
using Lumia.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PortfolioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PortfolioController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Portfolios.CountAsync();
            var portfolios = await _context.Portfolios.Include(p=>p.Category).Skip(page * 2).Take(2).ToListAsync();
            PaginationVM<Portfolio> vm = new()
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 2),
                Items = portfolios
            };
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            CreatePortfolioVM vm = new()
            {
                Categories = await _context.Categories.ToListAsync(),
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult>Create(CreatePortfolioVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }
            if (!vm.Photo.ValdiateType())
            {
                ModelState.AddModelError("Photo", "Incorrect file type");
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }
            if (!vm.Photo.ValdiateSize())
            {
                ModelState.AddModelError("Photo", "Incorrect file size");
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }
            bool result = await _context.Portfolios.AnyAsync(p => p.Name == vm.Name);
            if(result)
            {
                ModelState.AddModelError("Name", "This portfolio already exists");
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }

            Portfolio portfolio = new() 
            {
                 Name= vm.Name,
                 Description= vm.Description,
                 CategoryId= vm.CategoryId,
                 ImageUrl = await vm.Photo.CreateFile(_env.WebRootPath,"assets","img")
            };

            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            UpdatePortfolioVM vm = new()
            {
                Name = existed.Name,
                Description = existed.Description,
                CategoryId = existed.CategoryId,
                ImageUrl = existed.ImageUrl,
                Categories = await _context.Categories.ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult>Update(int id, UpdatePortfolioVM vm)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }
            if(await _context.Portfolios.AnyAsync(p=>p.Name==vm.Name && p.Id != id))
            {
                ModelState.AddModelError("Name", "This portfolio already exists");
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }
            if(vm.Photo is not null)
            {
                if (!vm.Photo.ValdiateType())
                {
                    ModelState.AddModelError("Photo", "Incorrect file type");
                    vm.Categories = await _context.Categories.ToListAsync();
                    return View(vm);
                }
                if (!vm.Photo.ValdiateSize())
                {
                    ModelState.AddModelError("Photo", "Incorrect file size");
                    vm.Categories = await _context.Categories.ToListAsync();
                    return View(vm);
                }
                await existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ImageUrl = await vm.Photo.CreateFile(_env.WebRootPath, "assets", "img");
            }
            existed.Name = vm.Name;
            existed.Description = vm.Description;
            existed.CategoryId = vm.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Details(int id)
        {
            if (id <= 0) return BadRequest();
            var existed= await _context.Portfolios.Include(p=>p.Category).FirstOrDefaultAsync(p=>p.Id== id);
            if (existed is null) return NotFound();
            return View(existed);
        }

        public async Task<IActionResult>Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Portfolios.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            await existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
            _context.Portfolios.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

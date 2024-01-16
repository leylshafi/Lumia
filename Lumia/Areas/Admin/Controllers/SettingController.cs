using Lumia.Areas.Admin.ViewModels;
using Lumia.Data;
using Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;

namespace Lumia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Settings.CountAsync();
            var settings = await _context.Settings.Skip(page*2).Take(2).ToListAsync();
            PaginationVM<Setting> vm = new()
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 2),
                Items = settings
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Create(CreateSettingVM settingVM)
        {
            if (!ModelState.IsValid) return View();
            if(await _context.Settings.AnyAsync(s=>s.Key== settingVM.Key))
            {
                ModelState.AddModelError("Key", "This key is already exists");
                return View();
            }
            Setting setting = new()
            {
                Key = settingVM.Key,
                Value = settingVM.Value
            };
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Settings.FirstOrDefaultAsync(s=>s.Id== id);
            if (existed is null) return NotFound();
            UpdateSettingVM vm = new()
            {
                Key = existed.Key,
                Value = existed.Value
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult>Update(int id, UpdateSettingVM vm)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            if (!ModelState.IsValid) return View(vm);
            if (await _context.Settings.AnyAsync(s => s.Key == vm.Key && s.Id!=id))
            {
                ModelState.AddModelError("Key", "This key is already exists");
                return View();
            }
            existed.Key = vm.Key;
            existed.Value = vm.Value;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult>Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            _context.Settings.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

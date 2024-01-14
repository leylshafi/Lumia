using Lumia.Areas.Admin.ViewModels;
using Lumia.Data;
using Lumia.Models;
using Lumia.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumia.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ServiceController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;
        public ServiceController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page)
		{
			double count = await _context.Services.CountAsync();
			var services = await _context.Services.Skip(page*2).Take(2).ToListAsync();
			PaginationVM<Service> vm = new()
			{
				CurrentPage = page,
				TotalPage = Math.Ceiling(count / 2),
				Items = services
			};
			return View(vm);
		}

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult>Create(CreateServiceVM serviceVM)
		{
			if (!ModelState.IsValid) return View();
			if (!serviceVM.Photo.ValdiateType())
			{
				ModelState.AddModelError("Photo", "Incorrect file type");
				return View();
			}
            if (!serviceVM.Photo.ValdiateSize())
            {
                ModelState.AddModelError("Photo", "Incorrect file size");
                return View();
            }

			if(await _context.Services.AnyAsync(s => s.Name == serviceVM.Name))
			{
				ModelState.AddModelError("Name", "This service already exists");
				return View();
			}

			Service service = new Service()
			{
				Name = serviceVM.Name,
				Description = serviceVM.Description,
				ImageUrl = await serviceVM.Photo.CreateFile(_env.WebRootPath, "assets", "img")
			};
			await _context.Services.AddAsync(service);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }

		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			var existed= await _context.Services.FirstOrDefaultAsync(s=>s.Id== id);
			if (existed is null) return NotFound();
			UpdateServiceVM vm = new()
			{
				Name = existed.Name,
				Description = existed.Description,
				ImageUrl = existed.ImageUrl,
			};
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult>Update(int id, UpdateServiceVM serviceVM)
		{
            if (id <= 0) return BadRequest();
            var existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
			if (!ModelState.IsValid) return View(serviceVM);
			if(await _context.Services.AnyAsync(s=>s.Name==serviceVM.Name && s.Id != id))
			{
				ModelState.AddModelError("Name", "This service already exists");
				return View(serviceVM);
			}
			if(serviceVM.Photo is not null)
			{
                if (!serviceVM.Photo.ValdiateType())
                {
                    ModelState.AddModelError("Photo", "Incorrect file type");
                    return View();
                }
                if (!serviceVM.Photo.ValdiateSize())
                {
                    ModelState.AddModelError("Photo", "Incorrect file size");
                    return View();
                }
				await existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
				existed.ImageUrl = await serviceVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
            }
			
			existed.Name = serviceVM.Name;
			existed.Description = serviceVM.Description;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }

		public async Task<IActionResult>Details(int id)
		{
			if(id<=0) return BadRequest();
            var existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
			return View(existed);
        }

		public async Task<IActionResult>Delete(int id)
		{
            if (id <= 0) return BadRequest();
            var existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
			await existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
			_context.Services.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }
	}
}

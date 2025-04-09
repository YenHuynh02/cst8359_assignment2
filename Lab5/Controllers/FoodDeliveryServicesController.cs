using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    public class FoodDeliveryServicesController : Controller
    {
        private readonly DealsFinderDbContext _context;
        private readonly IWebHostEnvironment _env;

        public FoodDeliveryServicesController(DealsFinderDbContext context)
        {
            _context = context;
        }

        // GET: FoodDeliveryServices
        public async Task<IActionResult> Index()
        {
            var viewModel = new DealsViewModel
            {
                Customers = await _context.Customers.ToListAsync(),
                FoodDeliveryServices = await _context.FoodDeliveryServices.ToListAsync(),
                Subscriptions = await _context.Subscriptions.ToListAsync()
            };
            return View(viewModel);
        }

        // GET: FoodDeliveryServices/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodDeliveryService = await _context.FoodDeliveryServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foodDeliveryService == null)
            {
                return NotFound();
            }

            return View(foodDeliveryService);
        }

        public async Task<IActionResult> Create(string id)
        {
            var fds = await _context.FoodDeliveryServices.FindAsync(id);
            if (fds == null)
                return NotFound();

            var model = new FileInputViewModel
            {
                FoodDeliveryServiceId = fds.Id,
                FoodDeliveryServiceTitle = fds.Title
            };

            return View(model);
        }

        // POST: FoodDeliveryService/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileInputViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var fds = await _context.FoodDeliveryServices.FindAsync(model.FoodDeliveryServiceId);
            if (fds == null)
                return NotFound();

            if (model.File != null && model.File.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var filePath = Path.Combine(uploads, model.File.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var service = new FoodDeliveryService
                {
                    Id = fds.Id,
                    Title = model.File.FileName
                };
                _context.FoodDeliveryServices.Add(service);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = fds.Id });
            }

            return View(model);
        }

        // GET: FoodDeliveryServices/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodDeliveryService = await _context.FoodDeliveryServices.FindAsync(id);
            if (foodDeliveryService == null)
            {
                return NotFound();
            }
            return View(foodDeliveryService);
        }

        // POST: FoodDeliveryServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] FoodDeliveryService foodDeliveryService)
        {
            if (id != foodDeliveryService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodDeliveryService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodDeliveryServiceExists(foodDeliveryService.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(foodDeliveryService);
        }

        // GET: FoodDeliveryServices/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fds = await _context.FoodDeliveryServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fds == null)
            {
                return NotFound();
            }

            return View(fds);
        }

        // POST: FoodDeliveryService/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var fds = await _context.FoodDeliveryServices.FindAsync(id);
            if (fds != null)
            {
                _context.FoodDeliveryServices.Remove(fds);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodDeliveryServiceExists(string id)
        {
            return _context.FoodDeliveryServices.Any(e => e.Id == id);
        }
    }
}

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
        public async Task<IActionResult> Index(string? selectedFoodDeliveryServiceId)
        {
            var viewModel = new DealsViewModel
            {
                Customers = await _context.Customers.ToListAsync(),
                FoodDeliveryServices = await _context.FoodDeliveryServices.ToListAsync(),
                Subscriptions = await _context.Subscriptions.ToListAsync()
            };
            if (!string.IsNullOrEmpty(selectedFoodDeliveryServiceId)) { }
            {
                var selectedFoodDeliveryService = await _context.FoodDeliveryServices
            .FirstOrDefaultAsync(f => f.Id == selectedFoodDeliveryServiceId);

                var selectedSubscriptions = _context.Subscriptions
                    .Where(s => s.FoodDeliveryServiceId == selectedFoodDeliveryServiceId)
                    .Select(s => _context.Customers.FirstOrDefault(c => c.Id == s.CustomerId).FullName)
                    .ToList();

                ViewBag.SelectedFoodDeliveryService = selectedFoodDeliveryService;
                ViewBag.SelectedSubscriptions = selectedSubscriptions;
            }
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

        // GET: FoodDeliveryServices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodDeliveryServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] FoodDeliveryService foodDeliveryService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foodDeliveryService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foodDeliveryService);
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

            var store = await _context.FoodDeliveryServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var store = await _context.FoodDeliveryServices.FindAsync(id);
            if (store != null)
            {
                _context.FoodDeliveryServices.Remove(store);
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

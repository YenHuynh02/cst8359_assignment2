using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.ComponentModel;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    
    public class DealsController : Controller
    {
        private readonly DealsFinderDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string dealContainerName = "dealsimages";
        private readonly IWebHostEnvironment _env;




        public DealsController(DealsFinderDbContext context, BlobServiceClient blobServiceClient, IWebHostEnvironment env)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _env = env;
        }
        public IActionResult OnGet()
        {
            return View();
        }

        // GET: Deals
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var fds = await _context.FoodDeliveryServices.FindAsync(id);
            if (fds == null)
                return NotFound();

            var deals = await _context.Deals.Where(f => f.FoodDeliveryServiceId == id).ToListAsync();

            var viewModel = new DealsPostsViewModel
            {
                FoodDeliveryService = fds,
                Deals = deals
            };
            return View(viewModel);
        }

        // GET: Deals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals
                .FirstOrDefaultAsync(m => m.FoodDeliveryServiceId == id);
            if (deal == null)
            {
                return NotFound();
            }

            return View(deal);
        }

        // GET: Deals/Create
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

        // POST: Deals/Create
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
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var filePath = Path.Combine(uploads, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var deal = new Deal
                {
                    FoodDeliveryServiceId = fds.Id,
                    DealTitle = uniqueFileName,
                    ImageURL = "/uploads/" + uniqueFileName // Store relative path
                };

                _context.Deals.Add(deal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { id = fds.Id });
        }







        // GET: Deals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals.FindAsync(id);
            if (deal == null)
            {
                return NotFound();
            }
            return View(deal);
        }

        // POST: Deals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DealTitle,ImageURL,FoodDeliveryServiceId")] Deal deal)
        {
            if (id != deal.FoodDeliveryServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DealExists(deal.FoodDeliveryServiceId))
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
            return View(deal);
        }

        // GET: Flyers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var deal = await _context.Deals
                .Include(f => f.FoodDeliveryService)
                .FirstOrDefaultAsync(m => m.FoodDeliveryServiceId == id);
            if (deal == null)
                return NotFound();

            return View(deal);
        }

        // POST: Flyers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var deal = await _context.Deals.FindAsync(id);

            if (deal == null)
                return NotFound();

            // Delete the file from server if required
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, deal.DealTitle);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            var fdsId = deal.FoodDeliveryServiceId;
            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = fdsId });
        }

        private bool DealExists(string id)
        {
            return _context.Deals.Any(e => e.FoodDeliveryServiceId == id);
        }

        
    }
}
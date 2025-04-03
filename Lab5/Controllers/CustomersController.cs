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
    public class CustomersController : Controller
    {
        private readonly DealsFinderDbContext _context;

        public CustomersController(DealsFinderDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int? selectedCustomerId)
        {
            //return View(await _context.Customers.ToListAsync());

            // Get all customers
            var customers = await _context.Customers.ToListAsync();
            ViewBag.Customers = customers; // Store for display

            // Store selected customer's subscriptions in ViewBag
            if (selectedCustomerId.HasValue)
            {
                var selectedCustomer = await _context.Customers
                    .Include(c => c.Subscriptions)
                        .ThenInclude(s => s.FoodDeliveryService)
                    .FirstOrDefaultAsync(c => c.Id == selectedCustomerId);

                if (selectedCustomer != null)
                {
                    ViewBag.SelectedCustomer = selectedCustomer;
                    ViewBag.SelectedSubscriptions = selectedCustomer.Subscriptions
                        .Select(s => s.FoodDeliveryService.Title) // Assuming there's a Name property
                        .ToList();
                }
            }
            return View(customers); // Pass a list of customers as the model

        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


        public async Task<IActionResult> EditSubscription(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Get all stores
            var stores = await _context.FoodDeliveryServices.ToListAsync();
            // Get subscriptions for this customer
            var customerSubscriptions = await _context.Subscriptions
                .Where(s => s.CustomerId == id)
                .ToListAsync();

            // Separate subscribed and unsubscribed stores
            var subscribedStores = (from st in stores
                                    join subs in customerSubscriptions on st.Id equals subs.ServiceId
                                    select new FoodDeliveryServiceSubscriptionViewModel
                                    {
                                        FoodDeliveryServiceId = st.Id,
                                        Title = st.Title,
                                        IsSubscribed = true
                                    }).ToList();

            var unsubscribedStores = stores
                .Where(st => !customerSubscriptions.Any(cs => cs.ServiceId == st.Id))
                .Select(st => new FoodDeliveryServiceSubscriptionViewModel
                {
                    FoodDeliveryServiceId = st.Id,
                    Title = st.Title,
                    IsSubscribed = false
                })
                .OrderBy(s => s.Title)
                .ToList();

            var viewModel = new CustomerSubscriptionViewModel
            {
                Customer = customer,
                Subscriptions = subscribedStores.Concat(unsubscribedStores)
            };

            // Return the correct view with the correct model
            return View(viewModel); 
        }


        public async Task<IActionResult> AddSubscription(int customerId, string storeId)
        {
            var subscription = new Subscription { CustomerId = customerId, ServiceId = storeId };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditSubscription), new { id = customerId });
        }


        public async Task<IActionResult> RemoveSubscription(int customerId, string storeId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.CustomerId == customerId && s.ServiceId == storeId);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EditSubscription), new { id = customerId });
        }
    }
}

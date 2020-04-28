using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace SportsStore.Areas.Employee.Controllers
{
    [Authorize(Policy = "EmployeePolicy")]
    [Area("Employee")]
    public class OrdersController : Controller
    {
        private readonly StoreDbContext _context;
        private IOrderRepository _orderRepository;

        public OrdersController(StoreDbContext context,
            IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders
                .Include(p => p.OrderedProducts)
                    .ThenInclude(o => o.Product) 
                .ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(p => p.OrderedProducts)
                    .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        // GET: Orders/Create
        public async Task<IActionResult> Create(int? idToAdd, int? idToRemove)
        {
            try
            {
                var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
                var orderedProducts = TempData.Get<IList<ProductItem>>("OrderedProducts");
                
                if (orderedProducts != null)
                {
                    if (idToRemove != null)
                    {
                        var item = orderedProducts.FirstOrDefault(p => p.Product.ID == idToRemove);
                        if (item != null)
                        {
                            if (item.Quantity > 1)
                                item.Quantity--;
                            else
                                orderedProducts.Remove(item);
                        }
                    }
                }

                if (orderedProducts == null)
                    orderedProducts = new List<ProductItem>();

                if (idToAdd != null)
                {
                    var item = orderedProducts.FirstOrDefault(p => p.Product.ID == idToAdd);
                    if (item == null)
                    {
                        orderedProducts.Add(new ProductItem
                        {
                            Product = products.FirstOrDefault(p => p.ID == idToAdd),
                            Quantity = 1
                        });
                    }
                    else
                    {
                        item.Quantity++;
                    }
                }

                TempData.Put<IList<ProductItem>>("OrderedProducts", orderedProducts);
                var model = new CreateOrderViewModel
                {
                    OrderedProducts = orderedProducts,
                    Products = new SelectList(products, "ID", "Name"),
                    PlacementDate = DateTime.Now.Date,
                };
                return View(model);
            }
            catch(Exception)
            {
                return NotFound();
            }
        }
        
        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(
            [Bind("PlacementDate, RecipientName, RecipientAddress, RecipientPhone, Customer, OrderedProducts")] 
            CreateOrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order order = new Order()
                    {
                        PlacementDate = model.PlacementDate ?? DateTime.MinValue,
                        Customer = model.Customer,
                        RecipientName = model.RecipientName,
                        RecipientAddress = model.RecipientAddress,
                        RecipientPhone = model.RecipientPhone,
                    };

                    var orderedProductsData = TempData.Get<IList<ProductItem>>("OrderedProducts");
                    await _orderRepository.Create(order, orderedProductsData);
                    TempData.Put<IList<ProductItem>>("OrderedProducts", null);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(model);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderToUpdate = await _context.Orders.FindAsync(id);

            if (orderToUpdate == null)
                return NotFound();

            if (await TryUpdateModelAsync(
                    orderToUpdate, 
                    "",
                    o => o.PlacementDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(orderToUpdate);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderedProducts)
                    .ThenInclude(p => p.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ID == id);
            if (order == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ModelState.AddModelError( "",
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderedProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.ID == id);

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }
    }
}

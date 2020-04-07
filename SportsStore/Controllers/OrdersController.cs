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

namespace SportsStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly StoreDbContext _context;

        public OrdersController(StoreDbContext context)
        {
            _context = context;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PlacementDate, RecipientName, RecipientAddress, RecipientPhone")] 
            CreateOrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order order = new Order()
                    {
                        PlacementDate = model.PlacementDate ?? DateTime.MinValue
                    };
                    var orderedProductsData = TempData.Get<IList<ProductItem>>("OrderedProducts");
                    var orderedProducts = new List<OrderedProduct>();
                    var products = await _context.Products
                                            .Include(p => p.OrderedProducts)
                                                .ThenInclude(op => op.Order)
                                            .ToListAsync();

                    foreach (var productItem in orderedProductsData)
                    {
                        var product = products.FirstOrDefault(p => p.ID == productItem.Product.ID);
                        var orderedProduct = new OrderedProduct
                        {
                            Product = product,
                            Order = order,
                            Quantity = productItem.Quantity
                        };
                        product.OrderedProducts.Add(orderedProduct);
                        orderedProducts.Add(orderedProduct);
                        _context.Update(product);
                    }
                    order.OrderedProducts = orderedProducts;
                    _context.Add(order);
                    await _context.SaveChangesAsync();
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

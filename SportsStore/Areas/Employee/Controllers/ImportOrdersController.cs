using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using SportsStore.Helpers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Policy = "AdminPolicy")]
    public class ImportOrdersController : Controller
    {
        private readonly IImportOrderRepository _repo;
        private readonly StoreDbContext _context;

        public ImportOrdersController(IImportOrderRepository repo,
            StoreDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // GET: Employee/ImportOrders
        public async Task<IActionResult> Index()
        {
            return View(await _repo.GetAllOrders());
        }

        // GET: Employee/ImportOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importOrder = await _repo.Read(id);
            if (importOrder == null)
            {
                return NotFound();
            }

            return View(importOrder);
        }

        // GET: Employee/ImportOrders/Create
        public async Task<IActionResult> Create(int? idToAdd, int? idToRemove)
        {
            try
            {
                var productsQuery = _context.Products.OrderBy(p => p.Name);
                var importedProducts = TempData.Get<IList<ProductItem>>("ImportedProducts");

                if (importedProducts != null)
                {
                    if (idToRemove != null)
                    {
                        var item = importedProducts.FirstOrDefault(p => p.Product.ID == idToRemove);
                        if (item != null)
                        {
                            if (item.Quantity > 1)
                                item.Quantity--;
                            else
                                importedProducts.Remove(item);
                        }
                    }
                }

                if (importedProducts == null)
                    importedProducts = new List<ProductItem>();

                if (idToAdd != null)
                {
                    var item = importedProducts.FirstOrDefault(p => p.Product.ID == idToAdd);
                    if (item == null)
                    {
                        importedProducts.Add(new ProductItem
                        {
                            Product = await productsQuery.FirstOrDefaultAsync(p => p.ID == idToAdd),
                            Quantity = 1
                        });
                    }
                    else
                    {
                        item.Quantity++;
                    }
                }

                TempData.Put<IList<ProductItem>>("ImportedProducts", importedProducts);
                var model = new CreateImportOrderViewModel
                {
                    ImportedOrders = importedProducts,
                    Products = new SelectList(await productsQuery.ToListAsync(), "ID", "Name"),
                    PlacementDate = DateTime.Now.Date,
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home",
                    new
                    {
                        ErrorTitle = "Create Import Order Error",
                        ErrorMessage = "Something happened while trying to create the import order:\n" +
                                       ex.Message
                    });
            }
        }

        // POST: Employee/ImportOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateImportOrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ImportOrder order = new ImportOrder()
                    {
                        PlacementDate = model.PlacementDate ?? DateTime.MinValue,
                        WholesalerName = model.WholesalerName,
                        WholesalerAddress = model.WholesalerAddress,
                        WholesalerPhone = model.WholesalerPhone
                    };

                    var importProductsData = TempData.Get<IList<ProductItem>>("ImportedProducts");
                    await _repo.Create(order, importProductsData);
                    TempData.Put<IList<ProductItem>>("ImportedProducts", null);
                    return RedirectToAction(nameof(Index));
                }
                model.ImportedOrders = TempData.Get<IList<ProductItem>>("ImportedProducts");
                return View(model);
            }
            catch (DbUpdateException /* ex */)
            {
                return RedirectToAction("Error", "Home",
                    new
                    {
                        ErrorTitle = "Create Import Order Error",
                        ErrorMessage = "Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator."
                    });
                //Log the error (uncomment ex variable name and write a log.
            }
        }

        // GET: Employee/ImportOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importOrder = await _repo.Read(id);
            if (importOrder == null)
            {
                return NotFound();
            }
            return View(importOrder);
        }

        // POST: Employee/ImportOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ImportOrder importOrder)
        {
            if (id != importOrder.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.Update(importOrder);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await ImportOrderExists(importOrder.ID)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "ImportOrders", new { area = "Employee" });
            }
            return View(importOrder);
        }

        // GET: Employee/ImportOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importOrder = await _repo.Read(id);
            if (importOrder == null)
            {
                return NotFound();
            }

            return View(importOrder);
        }

        // POST: Employee/ImportOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            await _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ImportOrderExists(int? id)
        {
            return (await _repo.GetAllOrders()).Any(o => o.ID == id);
        }
    }
}

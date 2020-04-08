using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Areas.Employee.Models;
using SportsStore.Areas.Employee.Models.ViewModels;
using SportsStore.Data;
using SportsStore.Models;

namespace SportsStore.Areas.Employee.Controllers
{
    [Authorize(Policy = "EmployeePolicy")]
    [Area("Employee")]
    public class EmployeesController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EmployeesController(StoreDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index(
            string sortOrder,
            string role,
            string searchString,
            int? pageNumber)
        {
            // Use LINQ to get list of role.
            IQueryable<string> roleQuery = _roleManager.Roles.Select(r => r.Name).Where(n => n != "Customer");
            var employees = _context.Employees.Select(e => e);
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.FullName.Contains(searchString) ||
                                                 e.Email.Contains(searchString) ||
                                                 e.Id.Contains(searchString));
            }

            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.SalarySortParm = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(s => s.FirstName);
                    break;
                case "price_asc":
                    employees = employees.OrderBy(s => s.Salary);
                    break;
                case "price_desc":
                    employees = employees.OrderByDescending(s => s.Salary);
                    break;
                default:
                    employees = employees.OrderBy(s => s.FirstName);
                    break;
            }

            var employeesInRole = new List<SportsStore.Models.Employee>();
            if (!string.IsNullOrEmpty(role))
            {
                foreach (var employee in employees.ToList())
                {
                    if (await _userManager.IsInRoleAsync(employee, role))
                        employeesInRole.Add(employee);
                }
            }
            else
            {
                employeesInRole = employees.ToList();
            }
            int pageSize = 3;
            var model = new EmployeesIndexViewModel()
            {
                Employees = PaginatedList<SportsStore.Models.Employee>
                                        .Create(employeesInRole,
                                                pageNumber ?? 1,
                                                pageSize),
                Role = !string.IsNullOrEmpty(role) ? role : "All",
                SearchString = searchString,
                SortOrder = sortOrder,
                Roles = await roleQuery.Distinct().ToListAsync()
            };
            return View(model);
        }

        //// GET: Products/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //// GET: Products/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Products/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Name,Brand,Category,Price,ImportPrice")] Product product)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(product);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //    catch (DbUpdateException /* ex */)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        ModelState.AddModelError("", "Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator.");
        //    }
        //    return View(product);
        //}

        //// GET: Products/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Products/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost, ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditPost(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var productToUpdate = await _context.Products.FindAsync(id);
        //    if (await TryUpdateModelAsync<Product>(
        //            productToUpdate,
        //            "",
        //            p => p.Name,
        //            p => p.Brand,
        //            p => p.Category,
        //            p => p.Price,
        //            p => p.ImportPrice
        //        ))
        //    {
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException /* ex */)
        //        {
        //            //Log the error (uncomment ex variable name and write a log.)
        //            ModelState.AddModelError("", "Unable to save changes. " +
        //                "Try again, and if the problem persists, " +
        //                "see your system administrator.");
        //        }
        //    }
        //    return View(productToUpdate);
        //}

        //// GET: Products/Delete/5
        //public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(p => p.ID == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    if (saveChangesError.GetValueOrDefault())
        //    {
        //        ViewData["ErrorMessage"] =
        //            "Delete failed. Try again, and if the problem persists " +
        //            "see your system administrator.";

        //    }

        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _context.Products
        //                        .Include(p => p.OrderedProducts)
        //                            .ThenInclude(op => op.Order)
        //                        .FirstOrDefaultAsync(p => p.ID == id);
        //    if (product == null)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    try
        //    {
        //        _context.Products.Remove(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (InvalidOperationException /* ex */)
        //    {
        //        string title = "Product Deletion Error";
        //        string message = "An error occured while trying to delete the product.\n" +
        //            "It is most likely that the product is still in some orders. " +
        //            "Please try to delete the orders first and then the product.";
        //        return RedirectToAction("Error", "Home", new
        //        {
        //            errorTitle = title,
        //            errorMessage = message
        //        });
        //    }
        //}
    }
}
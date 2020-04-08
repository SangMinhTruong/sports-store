using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsStore.Data;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StoreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, 
            StoreDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index(
            string sortOrder,
            string category,
            string searchString,
            int? pageNumber)
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                    return RedirectToAction("Index", "Home", new { area = "Employee" } );
            }

            // Use LINQ to get list of genres.  
            IQueryable<string> categoryQuery = from p in _context.Products
                                               orderby p.Category
                                               select p.Category;

            var products = from p in _context.Products
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) ||
                                               p.Brand.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(x => x.Category == category);
            }

            //if (searchString != null)
            //{
            //    pageNumber = 1;
            //}

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "price_asc":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            var model = new ProductIndexViewModel()
            {
                Products = await PaginatedList<Product>
                                .CreateAsync(products.AsNoTracking(),
                                             pageNumber ?? 1,
                                             pageSize),
                Category = !string.IsNullOrEmpty(category) ? category : "All",
                SearchString = searchString,
                SortOrder = sortOrder,
                Categories = await categoryQuery.Distinct().ToListAsync()
            };
            ViewBag.returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errorTitle, string errorMessage)
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorTitle = errorTitle,
                ErrorMessage = errorMessage
            });
        }
    }
}

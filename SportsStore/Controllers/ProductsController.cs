using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using SportsStore.Data;
using SportsStore.Helpers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreDbContext _context;

        public ProductsController(StoreDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(
            string sortOrder,
            string category,
            string brand,
            string searchString,
            decimal? curMinPrice,
            decimal? curMaxPrice,
            int? pageNumber,
            int? pageSize)
        {
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

            var filteredProducts = products;

            if (!string.IsNullOrEmpty(brand))
            {
                products = products.Where(p => p.Brand.Equals(brand));
            }

            var brandFilteredProducts = products;
            decimal? minPrice = 0, maxPrice = 0;
            if (brandFilteredProducts.Any())
            {
                minPrice = await brandFilteredProducts.MinAsync(p => p.Price);
                maxPrice = await brandFilteredProducts.MaxAsync(p => p.Price);
            }

            if (curMinPrice != null)
            {
                products = products.Where(p => p.Price >= curMinPrice);
            }
            else
            {
                curMinPrice = await brandFilteredProducts.MinAsync(p => p.Price);
            }

            if (curMaxPrice != null)
            {
                products = products.Where(p => p.Price <= curMaxPrice);
            }
            else
            {
                curMaxPrice = await brandFilteredProducts.MaxAsync(p => p.Price);
            }    


            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            IOrderedQueryable<Product> orderedProductsQuery;
            switch (sortOrder)
            {
                case "name_desc":
                    orderedProductsQuery = products.OrderByDescending(s => s.Name);
                    break;
                case "price_asc":
                    orderedProductsQuery = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    orderedProductsQuery = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    orderedProductsQuery = products.OrderBy(s => s.Name);
                    break;
            }

            var cart = SessionHelper
                .GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");

            var paginatedProducts = await PagingList
                                    .CreateAsync(orderedProductsQuery,
                                                 pageSize ?? 6,
                                                 pageNumber ?? 1);
            paginatedProducts.Action = nameof(Index);
            paginatedProducts.RouteValue = new RouteValueDictionary
            {
                { "sortOrder", sortOrder },
                { "searchString", searchString },
                { "pageSize", pageSize },
            };
            var model = new ProductIndexViewModel()
            {
                ProductsAllFiltered = await filteredProducts.ToListAsync(),
                PaginatedProducts = paginatedProducts,
                Category = category,
                SearchString = searchString,
                SortOrder = sortOrder,
                Brand = brand,
                CurMinPrice = curMinPrice,
                CurMaxPrice = curMaxPrice,
                MinPrice = minPrice ?? 0,
                MaxPrice = maxPrice ?? 0,
                PageSize = pageSize ?? 6,
                Categories = await categoryQuery.Distinct().ToListAsync(),
                Cart = cart
            };

            ViewBag.returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            var errorList = TempData.Get<List<string>>("Error");
            if (errorList != null)
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int? id, string returnUrl, int pageNumber = 1)
        {
            var product = await _context.Products.Include(p => p.ProductReviews)
                                                    .ThenInclude(r => r.Order)
                                                .FirstOrDefaultAsync(p => p.ID == id);

            var productReviews = _context.ProductReviews.Include(r => r.Order)
                                                        .Include(r => r.Product)
                                                        .Where(r => r.ProductID == id)
                                                        .OrderBy(r => r.DateAdded);
            var paginatedReviews = await PagingList.CreateAsync(productReviews, 3, pageNumber);
            paginatedReviews.Action = nameof(Details);
            paginatedReviews.RouteValue = new RouteValueDictionary
            {
                { "id", id },
                { "returnUrl", returnUrl }
            };
            var model = new ProductDetailsViewModel
            {
                Product = product,
                ProductReviews = paginatedReviews,
                Id = product.ID,
                Quantity = 0,
                ReturnUrl = returnUrl
            };
            return View(model);
        }
    }
}
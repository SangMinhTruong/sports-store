using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using SportsStore.Helpers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Controllers
{
    [Authorize(Policy = "CustomerPolicy")]
    public class CustomersController : Controller
    {
        private readonly StoreDbContext _context;

        public CustomersController(StoreDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CartIndex(string returnUrl)
        {
            var cart = SessionHelper
                .GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                cart = new List<ProductItem>();
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            if (cart.Any())
                ViewBag.Total = cart.Sum(i => i.Product.Price * i.Quantity);
            ViewBag.ReturnUrl = returnUrl;
            return View(cart);
        }
        public async Task<IActionResult> AddToCart(int id, string returnUrl)
        {
            var productsQuery = _context.Products;
            if (SessionHelper
                .GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart") == null)
            {
                List<ProductItem> cart = new List<ProductItem>();
                cart.Add(new ProductItem
                {
                    Product = await productsQuery.FindAsync(id),
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<ProductItem> cart = SessionHelper
                    .GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
                if (cart.Any(i => i.Product.ID == id) == true)
                {
                    cart.Find(i => i.Product.ID == id).Quantity++;
                }
                else
                {
                    cart.Add(new ProductItem { Product = await productsQuery.FindAsync(id), Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("CartIndex", "Customers", new { returnUrl });
        }
        public IActionResult RemoveFromCart(int id, string returnUrl)
        {
            var productsQuery = _context.Products;
            var cart = SessionHelper.GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                if (cart.Any(i => i.Product.ID == id) == true)
                {
                    cart.Remove(cart.Find(i => i.Product.ID == id));
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("CartIndex", "Customers", new { returnUrl });
        }
        public IActionResult ContinueShopping(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            var productsQuery = _context.Products; 
            var cart = SessionHelper.GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var model = new CheckoutViewModel
            {
                OrderedProducts = cart
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(
            [Bind("PlacementDate, RecipientName, RecipientAddress, RecipientPhone")]
            CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
    }
}
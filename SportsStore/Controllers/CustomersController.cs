using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
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
        // Identity services
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private IOrderRepository _orderRepository;

        public CustomersController(StoreDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOrderRepository orderRepository)
        {
            this._context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._orderRepository = orderRepository;
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
        public async Task<IActionResult> AddToCart(int id, string returnUrl, int? quantity)
        {
            var productsQuery = _context.Products;
        
            if (SessionHelper
                .GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart") == null)
            {
                List<ProductItem> cart = new List<ProductItem>();
                cart.Add(new ProductItem
                {
                    Product = await productsQuery.FindAsync(id),
                    Quantity = quantity ?? 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<ProductItem> cart = SessionHelper
                    .GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
                if (cart.Any(i => i.Product.ID == id) == true)
                {
                    var productToAdd = await productsQuery.FindAsync(id);
                    var itemToAdd = cart.Find(i => i.Product.ID == id);
                    if (productToAdd.Stock > itemToAdd.Quantity)
                        itemToAdd.Quantity += quantity ?? 1;
                    else
                    {
                        IList<string> errorList = new List<string>();
                        errorList.Add("Maximum number of products.");
                        TempData.Put("Error", errorList);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    cart.Add(new ProductItem { Product = await productsQuery.FindAsync(id), Quantity = quantity ?? 1});
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
        public async Task<IActionResult> Checkout()
        {
            var productsQuery = _context.Products; 
            var cart = SessionHelper.GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Error", "Home", new 
                { 
                    ErrorTitle = "Empty Cart", 
                    ErrorMessage = "You cannot checkout with an empty cart."
                });
            }
            Customer customer = null;
            if (signInManager.IsSignedIn(User))
            {
                customer = await _context.Customers.FindAsync(userManager.GetUserId(User));
            }
            var model = new CheckoutViewModel
            {
                OrderedProducts = cart,
                CustomerId = customer?.Id ?? "",
                RecipientName = customer?.FullName ?? "",
                RecipientAddress = customer?.Address ?? "",
                RecipientPhone = customer?.PhoneNumber ?? "",
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cart = SessionHelper.GetObjectFromJson<List<ProductItem>>(HttpContext.Session, "cart");
              
                    Order order = new Order()
                    {
                        PlacementDate = model.PlacementDate ?? DateTime.MinValue,
                        Customer = await _context.Customers.FindAsync(model.CustomerId),
                        RecipientName = model.RecipientName,
                        RecipientAddress = model.RecipientAddress,
                        RecipientPhone = model.RecipientPhone,
                    };
                    var orderReturn = await _orderRepository.Create(order, cart);
                    if (orderReturn != null)
                    {
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", null);
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch(Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCart(IEnumerable<ProductItem> model, string returnUrl)
        {
            var productQuery = _context.Products;
            List<ProductItem> newCart = new List<ProductItem>();
            foreach (var product in model)
            {
                if (product.Product.ID != null)
                {
                    newCart.Add(new ProductItem
                    {
                        Product = await productQuery.FirstOrDefaultAsync(p => p.ID == product.Product.ID),
                        Quantity = product.Quantity
                    });
                }
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", newCart);

            return RedirectToAction("CartIndex", "Customers", new { returnUrl });
        }
        public async Task<IActionResult> Details()
        {
            if (!signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Error", "Home", new
                {
                    ErrorTitle = "Not signed in",
                    ErrorMessage = "You are not signed in to perform this action."
                });
            }
            else
            {
                var customerID = userManager.GetUserId(User);
                var users = _context.Customers
                                .Include(c => c.Orders)
                                .ThenInclude(o => o.OrderedProducts)
                                    .ThenInclude(op => op.Product);
                var user = await users.FirstOrDefaultAsync(c => c.Id == customerID);
                return View(user);
            }
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var customers = _context.Customers
                                   .Include(c => c.Orders)
                                   .ThenInclude(o => o.OrderedProducts)
                                       .ThenInclude(op => op.Product);
            var customerToUpdate = await customers.FirstOrDefaultAsync(c => c.Id == id);
            if (await TryUpdateModelAsync<Customer>(
                       customerToUpdate,
                       "",
                       c => c.FirstName,
                       c => c.LastName,
                       c => c.Address,
                       c => c.PhoneNumber
                   ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    ViewBag.SuccessMessage = "Saved changes successfully.";
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(customerToUpdate);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home", new 
                { 
                    ErrorTitle = "Products information not found", 
                    ErrorMessage = "Something happened while trying to get products information, please try again."
                });
            }

            if (!signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Error", "Home", new
                {
                    ErrorTitle = "Not signed in",
                    ErrorMessage = "You are not signed in to perform this action."
                });
            }
            else
            {
                var customerID = userManager.GetUserId(User);
                var customer = await _context.Customers
                                        .Include(c => c.Orders)
                                            .ThenInclude(o => o.OrderedProducts)
                                                .ThenInclude(op => op.Product)
                                        .FirstOrDefaultAsync(c => c.Id == customerID);

                var order = customer.Orders.FirstOrDefault(o => o.ID == id);
                if (order == null)
                {
                    return NotFound();
                }
                return View(order);
            }
        }
        [HttpGet]
        public async Task<IActionResult> CreateReview(int? orderID, int? productID)
        {
            if (productID == null)
            {
                return RedirectToAction("Error", "Home", new
                {
                    ErrorTitle = "Product information not found",
                    ErrorMessage = "Something happened while trying to get the product information, please try again."
                });
            }

            if (orderID == null)
            {
                return RedirectToAction("Error", "Home", new
                {
                    ErrorTitle = "Order information not found",
                    ErrorMessage = "Something happened while trying to get the order information, please try again."
                });
            }

            if (!signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Error", "Home", new
                {
                    ErrorTitle = "Not signed in",
                    ErrorMessage = "You are not signed in to perform this action."
                });
            }
            else
            {
                var customerID = userManager.GetUserId(User);
                var customer = await _context.Customers
                                        .Include(c => c.Orders)
                                            .ThenInclude(o => o.OrderedProducts)
                                                .ThenInclude(op => op.Product)
                                        .FirstOrDefaultAsync(c => c.Id == customerID);

                var order = customer.Orders.FirstOrDefault(o => o.ID == orderID);
                var product = order.OrderedProducts.FirstOrDefault(p => p.ProductID == productID).Product;
                CreateReviewViewModel model = new CreateReviewViewModel
                {
                    Product = product,
                    Order = order,
                };
                return View(model);
                
            }
        }
        [HttpPost, ActionName("CreateReview")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReviewPost(CreateReviewViewModel model)
        {
            var order = await _context.Orders.Include(o => o.OrderedProducts).ThenInclude(op => op.Product)
                                             .FirstOrDefaultAsync(o => o.ID == model.OrderID);
            var product = order.OrderedProducts.FirstOrDefault(op => op.ProductID == model.ProductID).Product;
            try
            {
                if (ModelState.IsValid)
                {
                    ProductReview review = new ProductReview()
                    {
                        Order = order,
                        Product = product,
                        Rating = model.Rating,
                        DateAdded = DateTime.Now.Date,
                        Description = model.Description
                    };

                    _context.Add(review);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            model.Order = order;
            model.Product = product;
            return View(model);
        }
    }
}
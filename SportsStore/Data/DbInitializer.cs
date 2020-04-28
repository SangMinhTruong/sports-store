using SportsStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Data
{
    public static class DbInitializer
    {

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            StoreDbContext context = serviceProvider.GetRequiredService<StoreDbContext>();
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            context.Database.EnsureCreated();
            if (!roleManager.Roles.Any())
            {
                var roles = new IdentityRole[]
                {
                new IdentityRole { Name = "Admin"},
                new IdentityRole { Name = "Employee"},
                new IdentityRole { Name = "Customer"}
                };
                List<IdentityResult> results = new List<IdentityResult>();
                foreach (var role in roles)
                {
                    results.Add(await roleManager.CreateAsync(role));
                }
                context.SaveChanges();
                if (results.Any(r => !r.Succeeded))
                    throw new Exception("Error Seeding Database");
            }
            
            if (!context.Employees.Any())
            {
                if (await userManager.FindByEmailAsync("admin@admin") == null)
                {
                    var user = new Employee
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        UserName = "admin@admin",
                        Email = "admin@admin",
                        HireDate = DateTime.MinValue,
                        Salary = 0
                    };
                    var result1 = await userManager.CreateAsync(user, "Secret123$");
                    var result2 = await userManager.AddToRolesAsync
                        (
                            await userManager.FindByEmailAsync("admin@admin"),
                            new string[] { "Admin", "Employee" }
                        );
                    if (!result1.Succeeded || !result2.Succeeded)
                        throw new Exception("Error Seeding Database");
                }
                var employees = new Employee[]
                {
                    new Employee
                    {
                        FirstName = "A",
                        LastName = "Nguyen",
                        UserName = "ANguyen@store",
                        Email = "ANguyen@store",
                        HireDate = DateTime.Parse("04/06/2020"),
                        Salary = 10000000
                    },
                    new Employee
                    {
                        FirstName = "B",
                        LastName = "Tran",
                        UserName = "BTran@store",
                        Email = "BTran@store",
                        HireDate = DateTime.Parse("12/01/2019"),
                        Salary = 7000000
                    },

                };
                foreach (Employee employee in employees)
                {
                    var result1 = await userManager.CreateAsync(employee, "123456");
                    var result2 = await userManager.AddToRoleAsync
                        (
                            employee,
                            "Employee"
                        );
                    if (!result1.Succeeded || !result2.Succeeded)
                        throw new Exception("Error Seeding Database");
                }
            }
            if (!context.Customers.Any())
            {
                var customers = new Customer[]
                {
                    new Customer
                    {
                        FirstName = "C",
                        LastName = "Truong",
                        UserName = "CTruong@customer",
                        Email = "CTruong@customer"
                    },
                    new Customer
                    {
                        FirstName = "D",
                        LastName = "Nguyen",
                        UserName = "DNguyen@customer",
                        Email = "DNguyen@customer"
                    },

                };
                foreach (Customer customer in customers)
                {
                    var result1 = await userManager.CreateAsync(customer, "123456");
                    var result2 = await userManager.AddToRoleAsync
                        (
                            customer,
                            "Customer"
                        );
                    if (!result1.Succeeded || !result2.Succeeded)
                        throw new Exception("Error Seeding Database");
                }
            }

            // Look for any products.
            if (!context.Products.Any())
            {
                var products = new Product[]
                {
                new Product { Name="Máy chạy bộ V9", Brand="Sakura", Category="Máy chạy bộ", Price=14000000, Stock=10},
                new Product { Name="Tạ đơn", Brand="Brosman", Category="Tạ", Price=65000, Stock=100},
                new Product { Name="Máy chạy bộ S6", Brand="Sakura", Category="Máy chạy bộ", Price=10000000, Stock=15},
                new Product { Name="Bánh tạ gang", Brand="", Category="Tạ", Price=22000, Stock=200},
                new Product { Name="Xà đơn", Brand="", Category="Xà", Price=350000, Stock=50},
                new Product { Name="Xà kép", Brand="Thiên Trường", Category="Xà", Price=1000000, Stock=50},
                new Product { Name="Bánh tạ inox", Brand="", Category="Tạ", Price=60000, Stock=150},
                new Product { Name="Ghế tập tạ", Brand="Xuki", Category="Ghế tập gym", Price=3500000, Stock=60}
                };
                foreach (Product p in products)
                {
                    context.Products.Add(p);
                }
                context.SaveChanges();
            }
            if (!context.Orders.Any())
            {
                var products = await context.Products
                                        .Include(p => p.OrderedProducts)
                                            .ThenInclude(op => op.Order)
                                        .ToListAsync();
                var customers = await context.Customers.ToListAsync();
                var order1 = new Order
                {
                    Customer = customers.Find(c => c.Email == "CTruong@customer"),
                    PlacementDate = DateTime.Parse("11/01/2019")
                };
                var order1Products = new List<OrderedProduct>
                {
                    new OrderedProduct
                    {
                        Order = order1,
                        Product = products.Find(p => p.ID == 1),
                        Quantity = 1
                    },
                    new OrderedProduct
                    {
                        Order = order1,
                        Product = products.Find(p => p.ID == 3),
                        Quantity = 3
                    },
                    new OrderedProduct
                    {
                        Order = order1,
                        Product = products.Find(p => p.ID == 6),
                        Quantity = 2
                    },
                };
                order1.OrderedProducts = order1Products;
                var order2 = new Order
                {
                    Customer = customers.Find(c => c.Email == "CTruong@customer"),
                    PlacementDate = DateTime.Parse("09/21/2019")
                };
                var order2Products = new List<OrderedProduct>
                {
                    new OrderedProduct
                    {
                        Order = order1,
                        Product = products.Find(p => p.ID == 2),
                        Quantity = 4
                    },
                    new OrderedProduct
                    {
                        Order = order1,
                        Product = products.Find(p => p.ID == 5),
                        Quantity = 1
                    },
                };
                order2.OrderedProducts = order1Products;

                context.Orders.Add(order1);
                context.Orders.Add(order2);
                context.SaveChanges();
            }
            if (!context.ImportOrders.Any())
            {
                var productsQuery = context.Products
                                    .Include(p => p.OrderedProducts)
                                        .ThenInclude(op => op.Order);
                var order1 = new ImportOrder
                {
                    PlacementDate = DateTime.Parse("11/01/2019"),
                    WholesalerName = "Xuki",
                    WholesalerAddress = "ABC XYZ",
                    WholesalerPhone = "19002222"
                };
                var order1Products = new List<ImportedProduct>
                {
                    new ImportedProduct
                    {
                        ImportOrder = order1,
                        Product = await productsQuery.FirstOrDefaultAsync(p => p.ID == 4),
                        Quantity = 100
                    },
                    new ImportedProduct
                    {
                        ImportOrder = order1,
                        Product = await productsQuery.FirstOrDefaultAsync(p => p.ID == 1),
                        Quantity = 50
                    },
                    new ImportedProduct
                    {
                        ImportOrder = order1,
                        Product = await productsQuery.FirstOrDefaultAsync(p => p.ID == 5),
                        Quantity = 100
                    },
                };
                order1.ImportedProducts = order1Products;

                var order2 = new ImportOrder
                {
                    PlacementDate = DateTime.Parse("11/25/2019"),
                    WholesalerName = "Brooks",
                    WholesalerAddress = "AAAA BBBB",
                    WholesalerPhone = "19006969"
                };
                var order2Products = new List<ImportedProduct>
                {
                    new ImportedProduct
                    {
                        ImportOrder = order1,
                        Product = await productsQuery.FirstOrDefaultAsync(p => p.ID == 2),
                        Quantity = 150
                    },
                    new ImportedProduct
                    {
                        ImportOrder = order1,
                        Product = await productsQuery.FirstOrDefaultAsync(p => p.ID == 3),
                        Quantity = 10
                    }
                };
                order2.ImportedProducts = order2Products;

                context.ImportOrders.Add(order1);
                context.ImportOrders.Add(order2);
                context.SaveChanges();
            }
        }
    }
}
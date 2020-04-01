using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public static class SeedData
    {
        //Create seed data so that Startup.cs setup service and configure
        //public static void EnsurePopulated(IApplicationBuilder app)
        public static void EnsurePopulated(IServiceProvider services)
        {
            //ApplicationDbContext context = app.ApplicationServices
            //    .GetRequiredService<ApplicationDbContext>();
            //seed data by Azure sql db
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            //Ensure that the data migration of entity framework has been applied
            //context.Database.Migrate();
            //Check if the database have any object, if not add object collection to the data
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                new Product
                {
                    Name = "Giày thể thao",
                    Description = "Đôi giày thể thao mới.",
                    Category = "Fashion",
                    Price = 1000000m,
                    ImportPrice = 700000m
                },
                new Product
                {
                    Name = "Quần thể thao",
                    Description = "Quần thể thao thoải mái khi tập thể dục.",
                    Category = "Fashion",
                    Price = 500000m,
                    ImportPrice = 200000m
                },
                new Product
                {
                    Name = "Banh bóng đá",
                    Description = "Trái banh để chơi đá bóng.",
                    Category = "Soccer",
                    Price = 100000m,
                    ImportPrice = 75000m
                },
                new Product
                {
                    Name = "Banh bóng rổ",
                    Description = "Trái banh để chơi bóng rổ",
                    Category = "Basketball",
                    Price = 120000m,
                    ImportPrice = 80000m
                },
                new Product
                {
                    Name = "Tạ 5kg",
                    Description = "Tạ 5kg dùng để tập thể hình.",
                    Category = "Gym",
                    Price = 50000m,
                    ImportPrice = 30000m
                },
                new Product
                {
                    Name = "Găng tay tập gym",
                    Description = "Găng tay chống ma sát khi tập gym",
                    Category = "Gym",
                    Price = 50000m,
                    ImportPrice = 20000m
                },
                new Product
                {
                    Name = "Vành rổ sắt",
                    Description = "Vành rổ bằng sắt để chơi bóng rổ.",
                    Category = "Basketball",
                    Price = 250000m,
                    ImportPrice = 200000m
                },
                new Product
                {
                    Name = "Ghế tập tạ",
                    Description = "Ghế tập tạ chỉnh tư thế được.",
                    Category = "Gym",
                    Price = 1200000m,
                    ImportPrice = 700000m
                },
                new Product
                {
                    Name = "Áo thể thao",
                    Description = "Áo để mặc khi chơi thể thao.",
                    Category = "Fashion",
                    Price = 200000m,
                    ImportPrice = 120000m
                }
                );
                context.SaveChanges();
            }
        }
    }
}

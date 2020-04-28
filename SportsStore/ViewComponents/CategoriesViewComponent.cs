using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using SportsStore.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly StoreDbContext _context;

        public CategoriesViewComponent(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool? isExpanded = false)
        {
            List<string> categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
            Dictionary<string, List<string>> brandsOfCategory = new Dictionary<string, List<string>>();
            foreach (var category in categories)
            {
                var brands = await _context.Products.Where(p => p.Category == category)
                                                    .Select(p => p.Brand).Distinct()
                                                    .Where(s => !string.IsNullOrEmpty(s))
                                                    .ToListAsync();
                brandsOfCategory[category] = brands;
            }
            var model = new CategoriesViewModel
            {
                Categories = categories,
                BrandsOfCategory = brandsOfCategory,
                IsExpanded = isExpanded
            };
            return View(model);
        }
    }
}

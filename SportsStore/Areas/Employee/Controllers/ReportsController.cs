using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Areas.Employee.Models.ViewModels;
using SportsStore.Areas.Employee.Models;
using SportsStore.Data;
using SportsStore.Models;

namespace SportsStore.Areas.Employee.Controllers
{
    [Authorize(Policy = "EmployeePolicy")]
    [Area("Employee")]
    public class ReportsController : Controller
    {
        private readonly StoreDbContext _context;

        public ReportsController(StoreDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> MonthReport(string dateInput)
        {
            DateTime date = DateTime.ParseExact(dateInput, "MM/yyyy", CultureInfo.InvariantCulture);
            List<Order> orders = await _context.Orders
                                        .Include(p => p.OrderedProducts)
                                            .ThenInclude(o => o.Product)
                                        .Where(o => o.PlacementDate.Month == date.Month && o.PlacementDate.Year == date.Year)
                                        .ToListAsync();
            List<ImportOrder> importOrders = await _context.ImportOrders
                                                    .Include(p => p.ImportedProducts)
                                                        .ThenInclude(o => o.Product)
                                                    .Where(io => io.PlacementDate.Month == date.Month && io.PlacementDate.Year == date.Year)
                                                    .ToListAsync();

            decimal revenue = orders.Sum(o => o.OrderedProducts.Sum(op => op.Product.Price * op.Quantity));
            decimal expenses = importOrders.Sum(io => io.ImportedProducts.Sum(ip => ip.Product.ImportPrice * ip.Quantity));

            MonthReportViewModel model = new MonthReportViewModel
            {
                DateInput = date,
                Revenue = revenue,
                Expenses = expenses,
                Income = revenue - expenses,
                OrdersList = orders,
                ImportOrdersList = importOrders,
            };
            return View(model);
        }
        public async Task<IActionResult> YearReport(string dateInput)
        {
            List<MonthReportViewModel> model = new List<MonthReportViewModel>();
            for (int month = 1; month <= 12; month++)
            {
                DateTime curMonth = DateTime.ParseExact(month + "/" + dateInput, "M/yyyy", CultureInfo.InvariantCulture); ;
                List<Order> orders = await _context.Orders
                                           .Include(p => p.OrderedProducts)
                                               .ThenInclude(o => o.Product)
                                           .Where(o => o.PlacementDate.Month == curMonth.Month && o.PlacementDate.Year == curMonth.Year)
                                           .ToListAsync();
                List<ImportOrder> importOrders = await _context.ImportOrders
                                                        .Include(p => p.ImportedProducts)
                                                            .ThenInclude(o => o.Product)
                                                        .Where(io => io.PlacementDate.Month == curMonth.Month && io.PlacementDate.Year == curMonth.Year)
                                                        .ToListAsync();
                decimal revenue = orders.Sum(o => o.OrderedProducts.Sum(op => op.Product.Price * op.Quantity));
                decimal expenses = importOrders.Sum(io => io.ImportedProducts.Sum(ip => ip.Product.ImportPrice * ip.Quantity));

                MonthReportViewModel monthReport = new MonthReportViewModel
                {
                    DateInput = curMonth,
                    Revenue = revenue,
                    Expenses = expenses,
                    Income = revenue - expenses,
                    OrdersList = orders,
                    ImportOrdersList = importOrders,
                };
                model.Add(monthReport);
            }
            
            return View(model);
        }
    }
}

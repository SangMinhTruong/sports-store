using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{   
    [Authorize]
    public class ReportController : Controller
    {
        private IEmployeeRepository employeeRepo;
        private IImportOrderRepository importRepo;
        private IOrderRepository orderRepo;
        private IIncomeReportRepository reportRepo;
        public ReportController(
            IIncomeReportRepository reportRepository,
            IEmployeeRepository employeeRepository,
            IImportOrderRepository importRepository,
            IOrderRepository orderRepository)
        {
            this.reportRepo = reportRepository;
            this.employeeRepo = employeeRepository;
            this.importRepo = importRepository;
            this.orderRepo = orderRepository;
        }
        public IActionResult IncomeReport()
        {
            DateTime reportDate = DateTime.Now;
            IncomeReport incomeReport = new IncomeReport();

            incomeReport.CreateDate = reportDate;

            incomeReport.ImportCost = importRepo.ImportOrders
                .Where(o => o.PlacedDate.Year == reportDate.Year
                && o.PlacedDate.Month == reportDate.Month)
                .Sum(o => o.Sum);

            incomeReport.SaleIncome = orderRepo.Orders
                .Where(o => o.PlacedDate.Year == reportDate.Year
                && o.PlacedDate.Month == reportDate.Month)
                .Sum(o => o.Sum);

            incomeReport.EmployeeSalaries = employeeRepo.Employees
                .Sum(e => e.Salary);

            incomeReport.CalculateProfit();
            reportRepo.SaveReport(incomeReport);
            return View(incomeReport);
        }
        [HttpPost]
        public IActionResult IncomeReport(DateTime datepicker)
        {
            if (datepicker != null)
            {
                DateTime reportDate = datepicker;
                IncomeReport incomeReport = new IncomeReport();

                incomeReport.CreateDate = reportDate;

                incomeReport.ImportCost = importRepo.ImportOrders
                    .Where(o => o.PlacedDate.Year == reportDate.Year
                    && o.PlacedDate.Month == reportDate.Month)
                    .Sum(o => o.Sum);

                incomeReport.SaleIncome = orderRepo.Orders
                    .Where(o => o.PlacedDate.Year == reportDate.Year
                    && o.PlacedDate.Month == reportDate.Month)
                    .Sum(o => o.Sum);

                incomeReport.EmployeeSalaries = employeeRepo.Employees
                    .Sum(e => e.Salary);

                incomeReport.CalculateProfit();
                reportRepo.SaveReport(incomeReport);
                return View(incomeReport);
            }
            else
            {
                return View(null);
            }
        }
    }
}
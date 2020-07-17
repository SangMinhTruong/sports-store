using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Areas.Employee.Models.ViewModels
{
    public class MonthReportViewModel
    {
        public DateTime DateInput { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Income { get; set; }
        public List<Order> OrdersList { get; set; }
        public List<ImportOrder> ImportOrdersList { get; set; }
    }
}

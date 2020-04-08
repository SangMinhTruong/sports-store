using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsStore.Models;
namespace SportsStore.Areas.Employee.Models.ViewModels
{
    public class EmployeesIndexViewModel
    {
        public PaginatedList<SportsStore.Models.Employee> Employees { get; set; }
        public List<string> Roles { get; set; }
        public string Role { get; set; }
        public string SearchString { get; set; }
        public string SortOrder { get; set; }
    }
}

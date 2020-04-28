using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models.ViewModels
{
    public class CategoriesViewModel
    {
        public List<string> Categories { get; set; }
        public Dictionary<string, List<string>> BrandsOfCategory { get; set; }
        public bool? IsExpanded { get; set; }
    }
}

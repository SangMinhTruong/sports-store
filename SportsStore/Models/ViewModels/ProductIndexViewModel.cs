using Microsoft.AspNetCore.Mvc.Rendering;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models.ViewModels
{
    public class ProductIndexViewModel
    {
        public List<Product> ProductsAllFiltered { get; set; }
        public PaginatedList<Product> Products { get; set; }
        public PagingList<Product> PaginatedProducts { get; set; }
        public List<string> Categories { get; set; }
        public string Category { get; set; }
        public string SearchString { get; set; }
        public string SortOrder { get; set; }
        public string Brand { get; set; }
        public int? PageSize { get; set; }
        public decimal? CurMinPrice { get; set; }
        public decimal? CurMaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public IList<ProductItem> Cart { get; set; }
    }
}

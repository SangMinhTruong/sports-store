using System;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models
{
    public class OrderedProduct
    {
        public int? ProductID { get; set; }
        public int? OrderID { get; set; }
        [Range(1, Int32.MaxValue)]
        public int Quantity { get; set; }
        public decimal Sum
        {
            get
            {
                return Product.Price * Quantity;
            }
        }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
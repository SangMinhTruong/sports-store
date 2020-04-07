using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SportsStore.Models
{
    public class Order
    {
        public int? ID { get; set; }
        public string CustomerID { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Placement Date")]
        public DateTime PlacementDate { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
        public decimal Total
        {
            get
            {
                if (OrderedProducts != null)
                    return OrderedProducts.Sum(p => p.Product.Price * p.Quantity);
                return 0;
            }
        }
        public Customer Customer { get; set; }
        public ICollection<OrderedProduct> OrderedProducts { get; set; }
    }
}
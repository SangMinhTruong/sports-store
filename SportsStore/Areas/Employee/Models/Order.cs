using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SportsStore.Models
{
    public class Order
    {
        public int? ID { get; set; }
        [DisplayFormat(NullDisplayText = "Anonymous")]
        [Display(Name = "Customer ID")]
        public string CustomerID { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Placement Date")]
        public DateTime PlacementDate { get; set; }
        [Display(Name = "Recipient's Name")]
        public string RecipientName { get; set; }
        [Display(Name = "Address")]
        public string RecipientAddress { get; set; }
        [Display(Name = "Phone")]
        public string RecipientPhone { get; set; }
        [Range(1000, Double.MaxValue)]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
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
        public ICollection<ProductReview> ProductReviews { get; set; }
    }
}
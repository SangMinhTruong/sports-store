using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? PlacementDate { get; set; }
        [Required]
        public string RecipientName { get; set; }
        [Required]
        public string RecipientAddress { get; set; }
        [Required]
        public string RecipientPhone { get; set; }
        public string CustomerId { get; set; }
        public IList<ProductItem> OrderedProducts { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? PlacementDate { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
        public IList<ProductItem> OrderedProducts { get; set; }
    }
}

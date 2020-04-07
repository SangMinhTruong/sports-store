using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models.ViewModels
{
    public class CreateOrderViewModel
    {
        public int? IdToAdd { get; set; }
        public int? IdToRemove{ get; set; }
        [DataType(DataType.Date)]
        public DateTime? PlacementDate { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
        public IList<ProductItem> OrderedProducts { get; set; }
        public SelectList Products { get; set; }
    }
}

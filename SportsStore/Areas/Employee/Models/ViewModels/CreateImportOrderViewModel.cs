using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ViewModels
{
    public class CreateImportOrderViewModel
    {
        public int? IdToAdd { get; set; }
        public int? IdToRemove { get; set; }
        [DataType(DataType.Date)]
        public DateTime? PlacementDate { get; set; }
        public string WholesalerName { get; set; }
        public string WholesalerAddress { get; set; }
        public string WholesalerPhone { get; set; }
        public IList<ProductItem> ImportedOrders { get; set; }
        public SelectList Products { get; set; }
    }
}

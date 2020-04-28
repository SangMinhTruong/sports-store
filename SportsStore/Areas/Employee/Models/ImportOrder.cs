using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class ImportOrder
    {
        public int? ID { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Placement Date")]
        public DateTime PlacementDate { get; set; }
        [Display(Name = "Wholesaler's Name")]
        public string WholesalerName { get; set; }
        [Display(Name = "Wholesaler's Address")]
        public string WholesalerAddress { get; set; }
        [Display(Name = "Wholesaler's Phone")]
        public string WholesalerPhone { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public decimal Total
        {
            get
            {
                if (ImportedProducts != null)
                    return ImportedProducts.Sum(p => p.Product.Price * p.Quantity);
                return 0;
            }
        }
        public ICollection<ImportedProduct> ImportedProducts { get; set; }
    }
}

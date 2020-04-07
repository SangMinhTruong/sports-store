using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class ImportOrder
    {
        public int ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Placement Date")] 
        public DateTime PlacementDate { get; set; }
        public decimal Total
        {
            get
            {
                return ImportedProducts.Sum(p => p.Product.Price * p.Quantity);
            }
        }

        public ICollection<ImportedProduct> ImportedProducts { get; set; }
    }
}

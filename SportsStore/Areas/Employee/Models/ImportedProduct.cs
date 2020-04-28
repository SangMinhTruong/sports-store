using System;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models
{
    public class ImportedProduct
    {
        public int? ProductID { get; set; }
        public int? ImportOrderID { get; set; }
        [Range(1, Int32.MaxValue)]
        public int Quantity { get; set; }
        [Range(1000, Double.MaxValue)]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal Sum
        {
            get
            {
                return Product.Price * Quantity;
            }
        }

        public Product Product { get; set; }
        public ImportOrder ImportOrder { get; set; }
    }
}

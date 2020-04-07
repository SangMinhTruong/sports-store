using System;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models
{
    public class ImportedProduct
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int ImportOrderID { get; set; }

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
        public ImportOrder Order { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class Product
    {
        public int? ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Category")]
        [StringLength(30)]
        public string Category { get; set; }

        [DataType(DataType.Currency)]
        [Range(1000, Double.MaxValue,
            ErrorMessage = "Value for {0} must be at least {1}.")]
        [Display(Name = "Price")]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Price { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Import Price")]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal ImportPrice { get; set; }

        [Range(0, Double.MaxValue,
            ErrorMessage = "Value for {0} must be at least {1}.")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        public double AverageRating
        {
            get
            {
                if (ProductReviews != null)
                {
                    return Math.Round(this.ProductReviews.Average(r => r.Rating), 2);
                }
                else
                    return 0;
            }
        }

        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<OrderedProduct> OrderedProducts { get; set; }
        public ICollection<ImportedProduct> ImportedProducts { get; set; }

    }
}

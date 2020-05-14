using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class ProductReview
    {
        public int? ProductID { get; set; }
        public int? OrderID { get; set; }
        [Range(0, 5, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}

﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class ImportOrder
    {
        //Prevents model binding from setting a model's property
        [BindNever]
        public int ImportOrderID { get; set; }
        [BindNever]
        public ICollection<ImportItemsLine> Lines { get; set; }
        [BindNever]
        public bool Received { get; set; }
        [BindNever]
        public decimal Sum { get; set; }
        //[Required(ErrorMessage = "Please enter a wholesaler")]
        public WholeSalerInfo SalerInfo { get; set; }
        [Required(ErrorMessage = "Need to specify the date this order is placed")]
        public DateTime PlacedDate { get; set; }
        public ImportOrder() { }
        public ImportOrder(ImportItems importItems)
        {
            Lines = importItems.Lines.ToArray();
            Sum = importItems.ComputeTotalValue();
            PlacedDate = DateTime.Now;
            Received = false;
        }
    }
    public class WholeSalerInfo
    {
        [BindNever]
        public int WholeSalerInfoID { get; set; }
        [Required(ErrorMessage = "Please enter a wholesaler name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter the first address line")]
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        [Required(ErrorMessage = "Please enter a city name")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter a state name")]
        public string State { get; set; }
        public string Zip { get; set; }
        [Required(ErrorMessage = "Please enter a country name")]
        public string Country { get; set; }
        public bool GiftWrap { get; set; }
    }
}

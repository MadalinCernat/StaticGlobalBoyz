using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class OrderViewModel
    {
        public OrderModel Order { get; set; }
        public bool HasOrderedBefore { get; set; }
        public List<CountryModel> Countries { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string CountryCode { get; set; }
    }
}

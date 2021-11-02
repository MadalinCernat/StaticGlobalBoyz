using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; } 
        public string UserId { get; set; }
        public List<ClothingArticleItemModel> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingTax { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public AddressModel Address { get; set; }
        public string PaymentMethod { get; set; }
        [Display(Name = "Use the previously used address")]
        public bool UseLastAddress { get; set; }
        public string CountryCode { get; set; }
    }
}

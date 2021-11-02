using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Models
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
        public bool UseLastAddress { get; set; }
        public string CountryCode { get; set; }
    }
}

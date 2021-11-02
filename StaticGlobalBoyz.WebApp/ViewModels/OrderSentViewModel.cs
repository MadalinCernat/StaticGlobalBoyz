using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class OrderSentViewModel
    {
        public Guid OrderId { get; set; }
        public decimal OrderSubtotal { get; set; }
        public decimal OrderShippingTax { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime Date { get; set; }
    }
}

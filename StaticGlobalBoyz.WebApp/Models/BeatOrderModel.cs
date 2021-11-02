using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class BeatOrderModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<BeatItemModel> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public bool PaymentSuccessful { get; set; }
    }
}

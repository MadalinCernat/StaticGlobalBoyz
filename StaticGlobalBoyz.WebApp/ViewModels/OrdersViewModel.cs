using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class OrdersViewModel
    {
        public List<OrderModel> Orders { get; set; }
        public Guid OrderId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class ClothesOrderModel
    {
        public Guid Id { get; set; }
        public List<ClothingArticleItemModel> Items { get; set; }
        public decimal Price { get; set; }
        public string ClientUserId { get; set; }
        public DateTime Date { get; set; }
    }
}

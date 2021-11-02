using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Models
{
    public class ClothingArticleItemModel
    {
        public Guid Id { get; set; }
        public ClothingArticleModel Product { get; set; }
        public int Quantity { get; set; }
    }
}

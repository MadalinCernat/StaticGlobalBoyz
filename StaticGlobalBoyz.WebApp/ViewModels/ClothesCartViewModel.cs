using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class ClothesCartViewModel
    {
        public List<ClothingArticleItemModel> Clothes { get; set; } = new List<ClothingArticleItemModel>();
        public decimal Total { get; set; }
    }
}

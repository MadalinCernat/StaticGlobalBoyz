using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class EditDeleteClothingArticleViewModel
    {
        public Guid ClothingArticleId { get; set; }
        public List<ClothingArticleModel> Clothes { get; set; }
    }
}

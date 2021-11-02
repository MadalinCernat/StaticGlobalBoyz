using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Helpers;
using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using StaticGlobalBoyz.WebApp.ViewModels;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    public class ClothesCartController : Controller
    {
        private readonly MongoDbDataAccess _db;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClothesCartController(MongoDbDataAccess db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>(_contextAccessor.HttpContext.Session, "clothesCart");
            if (cart==null)
            {
                cart = new List<ClothingArticleItemModel>();
            }
            var model = new ClothesCartViewModel
            {
                Clothes = cart,
                Total = cart.Sum(item => item.Product.Price * item.Quantity)
            };
            return View(model);
        }
        [HttpGet]
        [Route("buyclothingarticle/{title?}/{id?}")]
        public IActionResult BuyClothingArticle()
        {
            if (TempData["ClothingArticle"] is string s)
            {
                var clothingArticle = JsonConvert.DeserializeObject<ClothingArticleModel>(s);
                //Check if the cart is empty,
                //If it's empty
                if (SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>(_contextAccessor.HttpContext.Session, "clothesCart") == null)
                {
                    List<ClothingArticleItemModel> cart = new List<ClothingArticleItemModel>();
                    //Add the item to the cart and update the cart
                    cart.Add(new ClothingArticleItemModel { Product = clothingArticle, Quantity = 1 });
                    SessionHelper.SetObjectAsJson(_contextAccessor.HttpContext.Session, "clothesCart", cart);
                }
                //If it's not empty
                else
                {
                    //Get the cart
                    List<ClothingArticleItemModel> cart = SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>(_contextAccessor.HttpContext.Session, "clothesCart");
                    int index = ClothingArticleExists(clothingArticle.Id, clothingArticle.Size.Id);
                    //If the item is already in the cart
                    if (index != -1)
                    {
                        if (cart[index].Quantity < 10)
                            cart[index].Quantity++;
                    }
                    //If the item is not in the cart
                    else
                    {
                        cart.Add(new ClothingArticleItemModel { Product = clothingArticle, Quantity = 1 });
                    }
                    SessionHelper.SetObjectAsJson(_contextAccessor.HttpContext.Session, "clothesCart", cart);
                }
            }
            return RedirectToAction("index");
        }
        private int ClothingArticleExists(Guid id, Guid sizeId)
        {
            List<ClothingArticleItemModel> cart = SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>(_contextAccessor.HttpContext.Session, "clothesCart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id) && cart[i].Product.Size.Id.Equals(sizeId))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Helpers;
using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    public class BeatsCartController : Controller
    {
        private readonly MongoDbDataAccess _db;

        public BeatsCartController(MongoDbDataAccess db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
            if(cart!=null)
            {
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(item => item.Product.Price * item.Quantity);
            }
            return View();
        }
        [Route("buybeat/{title?}")]
        public IActionResult BuyBeat(BeatModel beat)
        {
            //Check if the cart is empty,
            //If it's empty
            if (SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart") == null)
            {
                List<BeatItemModel> cart = new List<BeatItemModel>();
                //Add the item to the cart and update the cart
                cart.Add(new BeatItemModel { Product = beat, Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "beatsCart", cart);
            }
            //If it's not empty
            else
            {
                //Get the cart
                List<BeatItemModel> cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
                int index = BeatExists(beat.Id);
                //If the item is already in the cart
                if (index != -1)
                {
                    //Remove it and insert it again(in case the user changed the format)
                    cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
                    index = BeatExists(beat.Id);
                    cart.RemoveAt(index);
                    cart.Add(new BeatItemModel { Product = beat, Quantity = 1 });
                }
                //If the item is not in the cart
                else
                {
                    cart.Add(new BeatItemModel { Product = beat, Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "beatsCart", cart);
            }
            return RedirectToAction("Index");
        }

        [Route("removebeatfromcart/{title?}")]
        public IActionResult RemoveBeatFromCart(Guid id)
        {
            List<BeatItemModel> cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
            int index = BeatExists(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "beatsCart", cart);
            return RedirectToAction("Index");
        }

        private int BeatExists(Guid id)
        {
            List<BeatItemModel> cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

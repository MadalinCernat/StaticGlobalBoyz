using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Helpers;
using StaticGlobalBoyz.WebApp.Models;
using StaticGlobalBoyz.WebApp.Services;
using StaticGlobalBoyz.WebApp.ViewModels;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    public class BeatsCheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MailKitSender _mailKitSender;
        private readonly IWebHostEnvironment _environment;
        private readonly MongoDbDataAccess _db;

        public BeatsCheckoutController(UserManager<ApplicationUser> userManager, MailKitSender mailKitSender, IWebHostEnvironment environment,
            MongoDbDataAccess db)
        {
            _userManager = userManager;
            _mailKitSender = mailKitSender;
            _environment = environment;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");

            if (cart == null)
            {
                return RedirectToAction("index", "products");
            }
            var dollarAmount = cart.Sum(item => item.Product.Price * item.Quantity);
            var total = Math.Round(dollarAmount, 2) * 100;
            total = Convert.ToInt64(total);
            BeatsCheckoutIndexViewModel model = new BeatsCheckoutIndexViewModel
            {
                DollarAmount = dollarAmount,
                Total = total
            };
            ViewBag.Cart = cart;
            ViewBag.DollarAmount = cart.Sum(item => item.Product.Price * item.Quantity);
            ViewBag.Total = Math.Round(ViewBag.DollarAmount, 2) * 100;
            ViewBag.Total = Convert.ToInt64(ViewBag.Total);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Processing(string stripeToken, string stripeEmail, BeatsCheckoutIndexViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var optionsCust = new CustomerCreateOptions
            {
                Email = stripeEmail,
            };
            if (user.FirstName != null)
            {
                optionsCust.Name = user.FirstName + " " + user.LastName;
            }
            else if (user.ExternalFirstName != null)
            {
                optionsCust.Name = user.ExternalFirstName + " " + user.ExternalLastName;
            }
            else
            {
                optionsCust.Name = user.UserName;
            }
            var serviceCust = new CustomerService();
            Customer customer = await serviceCust.CreateAsync(optionsCust);
            var optionsCharge = new ChargeCreateOptions
            {
                /*Amount = HttpContext.Session.GetLong("Amount")*/
                Amount = Convert.ToInt64(model.Total),
                Currency = "USD",
                Description = "Beat Purchase",
                Source = stripeToken,
                ReceiptEmail = stripeEmail,
            };
            var service = new ChargeService();
            Charge charge = await service.CreateAsync(optionsCharge);
            if (charge.Status == "succeeded")
            {
                string BalanceTransactionId = charge.BalanceTransactionId;
                ViewBag.AmountPaid = Convert.ToDecimal(charge.Amount) % 100 / 100 + (charge.Amount) / 100;
                ViewBag.BalanceTxId = BalanceTransactionId;
                ViewBag.Customer = customer.Name;

                //Send email
                var cart = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
                BodyBuilder bodyBuilder = new BodyBuilder(); ;
                string extension = "";
                foreach (var beat in cart)
                {
                    if (beat.Product.Format.Equals("Mp3") || beat.Product.Format.Equals("Wav") || beat.Product.Format.Equals("Zip"))
                    {
                        extension = beat.Product.Format.ToLower();
                    }
                    else
                    {
                        extension = "zip";
                    }
                    var filePath = $"{_environment.WebRootPath}/data/{beat.Product.Title.Replace(" ", "-").ToLower()}.{extension}";
                    FileInfo fileInfo = new FileInfo(filePath);
                    //    MimePart attachment = new MimePart();
                    //    attachment.FileName = $"{beat.Product.Title.Replace(" ", "-")}.{extension}";
                    //    attachment.Content = new MimeContent(System.IO.File.OpenRead(filePath));
                    //    attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                    //    attachment.ContentTransferEncoding = ContentEncoding.Base64;

                    //    if (fileInfo.Exists)
                    //    {
                    //        bodyBuilder.Attachments.Add(attachment);
                    //        bodyBuilder.HtmlBody = "Thank you for the purchase. You can find your products in the attachments! - SGB";
                    //    }

                    //}
                    //await _mailKitSender.SendEmailAsync(user.Email, "Here are ya beats", true, bodyBuilder.ToMessageBody());
                }
                var items = SessionHelper.GetObjectFromJson<List<BeatItemModel>>(HttpContext.Session, "beatsCart");
                var beatOrder = new BeatOrderModel
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Items = items,
                    Status = "paid",
                    PaymentSuccessful = false,
                    UserId = user.Id,
                    Total = items.Sum(item => item.Product.Price * item.Quantity)
                };
                _db.InsertRecord<BeatOrderModel>("BeatOrders", beatOrder);
            }
            return View();
        }
    }
}

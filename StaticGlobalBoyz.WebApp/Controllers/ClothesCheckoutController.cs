using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Data;
using StaticGlobalBoyz.WebApp.Helpers;
using StaticGlobalBoyz.WebApp.Services;
using StaticGlobalBoyz.WebApp.Models;
using StaticGlobalBoyz.WebApp.ViewModels;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StaticGlobalBoyz.WebApp.Components;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Helpers.Mail;
using System.Text;
using MimeKit;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    public class ClothesCheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MongoDbDataAccess _db;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly MailKitSender _mailKitSender;
        private readonly JsonFileCountriesService _jsonFileCountriesService;
        private readonly DataService _dataService;

        public ClothesCheckoutController(UserManager<ApplicationUser> userManager,  MongoDbDataAccess db, 
            ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, MailKitSender mailKitSender,
            JsonFileCountriesService jsonFileCountriesService, DataService dataService)
        {
            _userManager = userManager;
            _db = db;
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _mailKitSender = mailKitSender;
            _jsonFileCountriesService = jsonFileCountriesService;
            _dataService = dataService;
        }
        [TempData]
        public string TotalAmount { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>
                (_contextAccessor.HttpContext.Session, "clothesCart");
            if (user != null)
            {
                if (cart != null)
                {
                    List<CountryModel> countries = _jsonFileCountriesService.GetCountries();
                    OrderModel order = new OrderModel
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Items = cart,
                        Subtotal = cart.Sum(item => item.Product.Price * item.Quantity),
                    };
                    OrderViewModel model = new OrderViewModel
                    {
                        Order = order,
                        HasOrderedBefore = user.HasOrdered,
                        Countries = countries
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("index", "clothescheckout");
                }
            }
            else
            {
                return LocalRedirect("/Identity/Account/Login/?returnUrl=/clothescheckout");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(OrderViewModel model)
        {
            model.Order.Items = SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>
                (_contextAccessor.HttpContext.Session, "clothesCart");
            if (!ModelState.IsValid || model.Order.Items == null) 
            {
                return RedirectToAction("index", "products");
            }

            var user = await _userManager.GetUserAsync(User);

            var order = new OrderModel();
            order.Id = model.Order.Id;
            order.UserId = model.Order.UserId;
            order.UseLastAddress = model.Order.UseLastAddress;
            order.PaymentMethod = model.Order.PaymentMethod;
            order.Items = model.Order.Items;
            order.Date = DateTime.Now;
            order.Subtotal = model.Order.Items.Sum(item => item.Product.Price * item.Quantity);
            order.ShippingTax = model.CountryCode == "RO" ? 0 : 50;
            order.Total = order.Subtotal + order.ShippingTax;
            order.CountryCode = model.CountryCode;
            order.Address = new AddressModel();

            if (model.Order.UseLastAddress == false)
            {
                using(_dbContext)
                {
                    user.FirstName = model.Order.Address.FirstName;
                    user.LastName = model.Order.Address.LastName;
                    user.StreetAddress = model.Order.Address.StreetAddress;
                    user.City = model.Order.Address.City;
                    user.County = model.Order.Address.County;
                    user.ZipCode = model.Order.Address.ZipCode;
                    user.PhoneNumber = model.Order.Address.PhoneNumber;
                    user.HasOrdered = true;
                    if (model.Order.Address.EmailAddress == null)
                    {
                        order.Address.EmailAddress = user.Email;
                    }
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                order.Address = new AddressModel();
                order.Address.FirstName = user.FirstName;
                order.Address.LastName = user.LastName;
                order.Address.EmailAddress = user.Email;
                order.Address.StreetAddress = user.StreetAddress;
                order.Address.County = user.County;
                order.Address.City = user.City;
                order.Address.ZipCode = user.ZipCode;
                order.Address.PhoneNumber = user.PhoneNumber;
            }
            var orderSentViewModel = new OrderSentViewModel();
            

            if (model.Order.PaymentMethod.Equals("stripe"))
            {
                order.Status = "unpaid";
                _db.InsertRecord("Orders", order);
            }
            else
            {
                order.Status = "sent";
                _db.InsertRecord("Orders", order);
                orderSentViewModel = new OrderSentViewModel()
                {
                    OrderId = order.Id,
                    OrderSubtotal = order.Subtotal,
                    OrderShippingTax = order.ShippingTax,
                    OrderTotal = order.Total,
                    Date = order.Date
                };
                var estimatedArrival = order.Date.AddDays(4);
                if(estimatedArrival.DayOfWeek == DayOfWeek.Saturday)
                {
                    estimatedArrival = estimatedArrival.AddDays(1);
                }
                if(estimatedArrival.DayOfWeek == DayOfWeek.Sunday)
                {
                    estimatedArrival = estimatedArrival.AddDays(1);
                }
                ViewBag.EstimatedArrival = estimatedArrival;
            }

            BodyBuilder body = new BodyBuilder();
            body.HtmlBody = BuildEmail(order);

            var to = order.Address.EmailAddress;
            var subject = "We received your order!";
            var isBodyHtml = true;
            var emailBody = $"<h3>Hi, {order.Address.FirstName}! Your order is being processed. We will contact you soon!</h3>" + body;


            await _mailKitSender.SendEmailAsync(order.Address.EmailAddress, subject, isBodyHtml, body.ToMessageBody());
            if(order.PaymentMethod.Equals("stripe"))
            {
                return RedirectToAction("Pay", new { orderId = model.Order.Id });
            }
            return View("OrderSent", orderSentViewModel);
        }
        private string BuildEmail(OrderModel order)
        {
            StringBuilder body = new StringBuilder();
            body.Append($"<strong>Order id:</strong> {order.Id}");
            body.Append("<br />");
            body.Append("<strong>Custromer details:</strong>");
            body.Append("<br />");

            body.Append($"<strong>Name:</strong> {order.Address.FirstName} {order.Address.LastName}. <br /> <strong>Phone:</strong> {order.Address.PhoneNumber}.");
            body.Append("<br />");

            body.Append($"<strong>County:</strong> {order.Address.County}. <strong>City:</strong> {order.Address.City}. <strong>Street:</strong> {order.Address.StreetAddress}. <strong>Zip:</strong> {order.Address.ZipCode}");
            body.Append("<br />");

            body.Append("<strong>Products:</strong>");
            body.Append("<br />");

            body.Append("<table style=\"border:1px solid black; font-family: Verdana; border-collapse: collapse; width:50%;\">");
            body.Append("<tr><th style=\"border:1px solid black;\">Name</th><th style=\"border:1px solid black;\">Photo</th><th style=\"border:1px solid black;\">Price</th><th style=\"border:1px solid black;\">Quantity</th><th style=\"border:1px solid black;\">Size</th></tr>");

            foreach (var item in order.Items)
            {
                body.Append("<tr>");
                body.Append($"<td style=\"border:1px solid black;\">{item.Product.Title}</td>");
                body.Append($"<td style=\"height:5%; width:5%; border:1px solid black;\" align=\"center\"><img src=\"{item.Product.PhotosUrl[0].Url}\" style=\"display:block; width:100%;\"/></td>");
                body.Append($"<td align=\"center\" style=\"border:1px solid black;\">{item.Product.Price:C2}</td>");
                body.Append($"<td align=\"center\" style=\"border:1px solid black;\">{item.Quantity}</td>");
                body.Append($"<td align=\"center\" style=\"border:1px solid black;\">{item.Product.Size.Name}</td>");
                body.Append("</tr>");
            }
            body.Append("</table>");
            body.Append("<br />");
            body.Append($"Subtotal: {order.Subtotal:C2}");
            body.Append("<br />");
            body.Append($"Shipping Tax: {order.ShippingTax:C2}");
            body.Append("<br />");
            body.Append($"<strong>Total:</strong> {order.Total:C2}");
            body.Append("<br />");

            body.Append($"<strong>Payment method:</strong> {order.PaymentMethod}");
            body.Append("<br />");

            body.Append($"<strong>Date:</strong> {order.Date}");
            body.Append("<br />");

            body.Append("Thanks for support - SGB");

            return body.ToString();
        }

        [HttpGet]
        public async Task<IActionResult> Pay(Guid orderId)
        {
            var cart = SessionHelper.GetObjectFromJson<List<ClothingArticleItemModel>>
                (_contextAccessor.HttpContext.Session, "clothesCart");
            ViewBag.Cart = cart;
            var order = await _dataService.Get<OrderModel>("orders", orderId);
            if (cart != null)
            {
                ViewBag.DollarAmount = cart.Sum(item => item.Product.Price * item.Quantity) + order.ShippingTax;
                ViewBag.Total = Math.Round(ViewBag.DollarAmount, 2) * 100;
                ViewBag.Total = Convert.ToInt64(ViewBag.Total);
                long total = ViewBag.Total;
                TotalAmount = total.ToString();
                return View(orderId);
            }
            return RedirectToAction("index", "products");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Processing(string stripeToken, string stripeEmail, Guid orderId)
        {
            var optionsCust = new CustomerCreateOptions
            {
                Email = stripeEmail,
                Name = (await _userManager.GetUserAsync(User)).UserName
            };
            var serviceCust = new CustomerService();
            Customer customer = await serviceCust.CreateAsync(optionsCust);
            var optionsCharge = new ChargeCreateOptions
            {
                //Amount = HttpContext.Session.GetLong("Amount")
                Amount = Convert.ToInt64(TempData["TotalAmount"]),
                Currency = "USD",
                Description = "Clothes Purchase",
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
                var order = await _dataService.Get<OrderModel>("orders", orderId);
                order.Status = "paid";
                _db.UpsertRecord("Orders", orderId, order);
            }
            return View();
        }
    }
}

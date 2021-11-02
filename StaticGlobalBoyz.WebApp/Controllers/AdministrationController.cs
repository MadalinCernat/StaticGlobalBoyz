using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Models;
using StaticGlobalBoyz.WebApp.Services;
using StaticGlobalBoyz.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MongoDbDataAccess _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MailKitSender _mailKitSender;
        private readonly IWebHostEnvironment _environment;
        private readonly DataService _dataService;
        private readonly EmailSender _emailSender;

        public AdministrationController(RoleManager<IdentityRole> roleManager, 
            MongoDbDataAccess db, UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            MailKitSender mailKitSender, IWebHostEnvironment environment, DataService dataService)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
            _mailKitSender = mailKitSender;
            _environment = environment;
            _dataService = dataService;
            _emailSender = emailSender as EmailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region Roles Administration
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "products");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListOfRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach(var user in _userManager.Users)
            {
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
            }
            var result = await _roleManager.UpdateAsync(role);
            if(result.Succeeded)
            {
                return RedirectToAction("listofroles");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            
            var role = await _roleManager.FindByIdAsync(roleId);

            ViewBag.roleName = role.Name;
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            foreach(var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var roleName = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            for(int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result;
                if(model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if(result.Succeeded)
                {
                    if(i < (model.Count - 1))
                    {
                        continue;
                    }
                    return RedirectToAction("editrole", new { Id = roleId, title = roleName });
                }
            }
            return RedirectToAction("editrole", new { id = roleId, title = roleName });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("listofroles");
        }
        #endregion
        #region Tags Administration
        [HttpGet]
        public IActionResult CreateTag()
        {
            return View(new TagModel { Id = Guid.NewGuid()});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTag(TagModel tag)
        {
            if(ModelState.IsValid)
            {
                tag.Id = Guid.NewGuid();
                _db.InsertRecord("Tags", tag);
            }
            return RedirectToAction(nameof(CreateTag));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteTag()
        {
            var tags = await _dataService.GetAll<TagModel>("tags");
            return View(tags);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTag(TagModel tag)
        {
            _db.DeleteRecord<TagModel>("Tags", tag.Id);
            var beats = await _dataService.GetAll<BeatModel>("beats");
            foreach (var beat in beats)
            {
                if (beat.Tags.Any(x => x.Id == tag.Id))
                {
                    beat.Tags = beat.Tags.Where(x => x.Id != tag.Id).ToList();
                    _db.UpsertRecord("Beats", beat.Id, beat);
                }
            }
            return RedirectToAction(nameof(DeleteTag));
        }
        #endregion
        #region Products Administration
        
        [HttpGet]
        public async Task<IActionResult> EditDeleteClothingArticle()
        {
            var clothes = await _dataService.GetAll<ClothingArticleModel>("clothes");
            var model = new EditDeleteClothingArticleViewModel
            {
                Clothes = clothes
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeleteClothingArticle(EditDeleteClothingArticleViewModel model)
        {
            _db.DeleteRecord<ClothingArticleModel>("Clothes", model.ClothingArticleId);
            model.Clothes = await _dataService.GetAll<ClothingArticleModel>("clothes");
            return View(model);
        }

        [HttpGet("editclothingarticle/{title}/{id}")]
        public async Task<IActionResult> EditClothingArticle(Guid id, string title)
        {
            var clothingArticle = await _dataService.Get<ClothingArticleModel>("clothes", id);
            if (clothingArticle == null)
            {
                ViewBag.ErrorMessage = "Clothing Article not found";
                return View("NotFound");
            }
            return View(clothingArticle);
        }
        [HttpPost("editclothingarticle/{title}/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditClothingArticle(ClothingArticleModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.AvailableSizes.Any(x => x.Selected == true))
                {
                    model.InStock = false;
                }
                _db.UpsertRecord("Clothes", model.Id, model);
            };
            return RedirectToAction("clothingarticledetails", "products", new { id = model.Id, title = model.Title.Replace(" ", "-").ToLower() });
        }
        [HttpGet]
        public async Task<IActionResult> EditDeleteBeat()
        {
            var beats = await _dataService.GetAll<BeatModel>("beats");
            var model = new EditDeleteBeatViewModel
            {
                Beats = beats
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeleteBeat(EditDeleteBeatViewModel model)
        {
            _db.DeleteRecord<BeatModel>("Beats", model.BeatId);
            model.Beats = await _dataService.GetAll<BeatModel>("beats");
            return View(model);
        }
        [HttpGet("editbeat/{title}/{id}")]
        public async Task<IActionResult> EditBeat(Guid id, string title)
        {
            var beat = await _dataService.Get<BeatModel>("beats", id);
            ViewBag.Tags = await _dataService.GetAll<TagModel>("tags");

            if(beat==null)
            { 
                ViewBag.ErrorMessage = $"Beat cannot be found";
                return View("NotFound");
            }
            return View(beat);
        }
        [HttpPost("editbeat/{title}/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditBeat(BeatModel beat)
        {
            if(ModelState.IsValid)
            {
                beat.Tags = beat.Tags.Where(x => x.Selected == true).ToList();
                _db.UpsertRecord("Beats", beat.Id, beat);
            }
            return RedirectToAction("beatdetails", "products", new { id = beat.Id, title = beat.Title.Replace(" ", "-").ToLower() });
        }
        #endregion
        #region Orders Administration

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _dataService.GetAll<OrderModel>("orders");
            var model = new OrdersViewModel
            {
                Orders = orders,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Orders(OrdersViewModel model)
        {
            model.Orders = await _dataService.GetAll<OrderModel>("orders");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsHandedOver(OrdersViewModel model)
        {
            var order = await _dataService.Get<OrderModel>("orders", model.OrderId);
            order.Status = "handed-over";
            _db.UpsertRecord("Orders", model.OrderId, order);
            model.Orders = await _dataService.GetAll<OrderModel>("orders");
            BodyBuilder body = new BodyBuilder();
            body.HtmlBody = "Your order has been marked as handed over. You will be contacted by the courier company for the delivery. <br /> Thanks for the shopping - SGB";
            await _mailKitSender.SendEmailAsync(order.Address.EmailAddress, "Your order is on its way!", true, body.ToMessageBody());
            return View("Orders", model);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsDelivered(OrdersViewModel model)
        {
            var order = await _dataService.Get<OrderModel>("orders", model.OrderId);
            order.Status = "delivered";
            _db.UpsertRecord("Orders", model.OrderId, order);
            model.Orders = await _dataService.GetAll<OrderModel>("orders");
            return View("Orders", model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(OrdersViewModel model)
        {
            var order = await _dataService.Get<OrderModel>("orders", model.OrderId);
            order.Status = "hidden";
            _db.UpsertRecord("Orders", model.OrderId, order);
            model.Orders = await _dataService.GetAll<OrderModel>("orders");
            return View("Orders", model);
        }
        public async Task<IActionResult> DeleteForever(OrdersViewModel model)
        {
            _db.DeleteRecord<OrderModel>("Orders", model.OrderId);
            model.Orders = await _dataService.GetAll<OrderModel>("orders");
            return View("Orders", model);
        }
        public async Task<IActionResult> HiddenOrders()
        {
            var orders = await _dataService.GetAll<OrderModel>("orders");
            var hiddenOrders = orders.Where(x => x.Status.Equals("hidden")).ToList();
            return View(hiddenOrders);
        }
        public async Task<IActionResult> BeatOrders()
        {
            var orders = await _dataService.GetAll<BeatOrderModel>("beatorders");
            var model = new BeatOrdersViewModel
            {
                Orders = orders,
            };
            return View(model);
        }
        public async Task<IActionResult> MarkBeatAsDelivered(BeatOrdersViewModel model)
        {
            var order = await _dataService.Get<BeatOrderModel>("beatorders", model.OrderId);
            order.Status = "delivered";
            _db.UpsertRecord("BeatOrders", model.OrderId, order);
            model.Orders = await _dataService.GetAll<BeatOrderModel>("beatorders");

            var user = await _userManager.GetUserAsync(User);
            BeatsPurchasedModel beats;
            try
            {
                var allBeats = await _dataService.GetAll<BeatsPurchasedModel>("beatspurchased");
                beats = allBeats.First();
                for (int i = 0; i < order.Items.Count; i++)
                {
                    if (!beats.PurchasedBeatsIds.Contains(order.Items[i].Id))
                    {
                        beats.PurchasedBeatsIds.Add(order.Items[i].Id);
                    }
                }
                _db.UpsertRecord("BeatsPurchased", beats.Id, beats);
            }
            catch
            {
                beats = new BeatsPurchasedModel
                {
                    Id = Guid.NewGuid(),
                    PurchasedBeatsIds = new List<Guid>(),
                    UserId = user.Id
                };
                for(int i = 0; i < order.Items.Count; i++)
                {
                    beats.PurchasedBeatsIds.Add(order.Items[i].Id);
                }
                _db.InsertRecord("BeatsPurchased", beats);
            }
            return View("BeatOrders", model);
        }
        public async Task<IActionResult> DeleteBeatForever(BeatOrdersViewModel model)
        {
            _db.DeleteRecord<BeatOrderModel>("BeatOrders", model.OrderId);
            model.Orders = await _dataService.GetAll<BeatOrderModel>("beatorders");
            return View("BeatOrders", model);
        }
        public IActionResult UploadFile()
        {
            return View(new UploadFileViewModel());
        }
        [HttpPost]
        public IActionResult UploadFile(UploadFileViewModel model)
        {
            if (model.File != null)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "data", model.File.FileName);
                model.File.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            return View(new UploadFileViewModel());
        }
        #endregion
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

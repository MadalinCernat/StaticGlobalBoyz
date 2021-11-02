using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Models;
using StaticGlobalBoyz.WebApp.Services;
using StaticGlobalBoyz.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MongoDbDataAccess _db;
        private readonly IWebHostEnvironment _environment;
        private readonly DataService _dataService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProductsController(ILogger<ProductsController> logger, UserManager<ApplicationUser> userManager, MongoDbDataAccess db,
            IWebHostEnvironment environment, DataService dataService, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
            _environment = environment;
            _dataService = dataService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Beats
        [HttpGet("beats")]
        public async Task<IActionResult> Beats()
        {
            var beats = await _dataService.GetAll<BeatModel>("beats");
            return View(beats);
        }
        [HttpGet("beats/{title}/{id}")]
        public async Task<IActionResult> BeatDetails(Guid id)
        {
            var beat = await _dataService.Get<BeatModel>("beats", id);

            if(beat == null)
            {
                ViewBag.ErrorMessage = $"Beat cannot be found";
                return View("NotFound");
            }
            return View(beat);
        }
        [HttpPost("beats/{title}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BeatDetails(BeatModel beatModel)
        {
            var beat = await _dataService.Get<BeatModel>("beats", beatModel.Id);
            beat.Price = beatModel.Price;
            return RedirectToAction("buybeat", "beatscart", beat);
        }

        #endregion

        #region Create Beats (Admins Only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBeat()
        {
            var beat = new BeatModel();
            beat.Id = Guid.NewGuid();
            var tags = await _dataService.GetAll<TagModel>("tags");
            beat.Tags = tags;
            return View(beat);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateBeat(BeatModel beat)
        {
            if(ModelState.IsValid == false)
            {
                _logger.LogWarning("Invalid beat submitted!");
                return View(beat);
            }
            try
            {
                beat.Tags = beat.Tags.Where(x => x.Selected == true).ToList();
                _db.InsertRecord("Beats", beat);
                _logger.LogInformation($"Beat Created. : {beat.Title} prod by {beat.ProducerName}");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(beat);
            }     
        }
        #endregion

        #region Clothes
        [HttpGet("clothes")]
        public async Task<IActionResult> Clothes()
        {
            var clothes = await _dataService.GetAll<ClothingArticleModel>("clothes");
            return View(clothes);
        }
        [HttpGet("clothes/{title}/{id}")]
        public async Task<IActionResult> ClothingArticleDetails(Guid id)
        {
            var clothingArticle = await _dataService.Get<ClothingArticleModel>("clothes", id);
            
            if(clothingArticle == null)
            {
                ViewBag.ErrorMessage = $"Clothing Article cannot be found";
                return View("NotFound");
            }
            return View(clothingArticle);
        }
        [HttpPost("clothes/{title}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClothingArticleDetails(ClothingArticleModel clothingArticleModel)
        {
            var clothingArticle = await _dataService.Get<ClothingArticleModel>("clothes", clothingArticleModel.Id);
            if(clothingArticle.ClothingType.Name.Equals("Headwear") ||  clothingArticle.ClothingType.Name.Equals("Accessories"))
            {
                clothingArticle.Size = clothingArticle.AvailableSizes[0];
            }
            else
            {
                clothingArticle.Size = await _dataService.Get<SizeModel>("sizes", clothingArticleModel.Size.Id);
            }
            var clothingArticleSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(clothingArticle);
            TempData["ClothingArticle"] = clothingArticleSerialized;
            return RedirectToAction("buyclothingarticle", "clothescart");
        }
        #endregion

        #region Create Clothing Articles (Admins only)

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClothingArticle()
        {
            var clothingArticle = new ClothingArticleModel();
            clothingArticle.Id = Guid.NewGuid();
            clothingArticle.ClothingTypes = await _dataService.GetAll<ClothingTypeModel>("clothingtypes");
            return View(clothingArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClothingArticle(ClothingArticleModel clothingArticle)
        {
            if (ModelState.IsValid == false)
            {
                _logger.LogWarning("Invalid clothing article submitted!");
                return View();
            }
            try
            {
                if(clothingArticle.ClothingType.Name.Equals("Headwear") || clothingArticle.ClothingType.Name.Equals("Accessories"))
                {
                    clothingArticle.AvailableSizes.Add(new SizeModel { Name = "Universal" });
                }
                else
                {
                    clothingArticle.AvailableSizes = await _dataService.GetAll<SizeModel>("sizes");
                }
                _db.InsertRecord("Clothes", clothingArticle);
                _logger.LogInformation($"Clothing Created: {clothingArticle.Title}");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(clothingArticle);
            }
        }
        #endregion

        [HttpGet("yourbeats")]
        public async Task<IActionResult> YourBeats()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.GetUserAsync(User);
            var orders = await _dataService.GetAll<BeatOrderModel>("beatorders");
            orders = orders.Where(x => x.UserId == user.Id).ToList();

            var model = new YourBeatsViewModel();
            for(int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Status == "delivered")
                {
                    for (int j = 0; j < orders[i].Items.Count; j++)
                    {
                        model.Beats.Add(orders[i].Items[j]);
                    }
                }
            }
            return View(model);
        }
        public IActionResult DownloadBeat(Guid id, string title, string format)
        {
            string fullName = $"{title}.{format.ToLower()}";
            string filePath = Path.Combine(_environment.WebRootPath, "data", fullName);

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "application/force-download", fullName);
        }
        [HttpGet]
        public IActionResult Privacy()
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

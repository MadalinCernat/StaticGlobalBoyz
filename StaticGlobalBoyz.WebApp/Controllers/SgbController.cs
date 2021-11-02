using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Controllers
{
    public class SgbController : Controller
    {
        // GET: SgbController
        public ActionResult Index()
        {
            return View();
        }
        public IActionResult SupportUs()
        {
            return View();
        }
        public IActionResult TermsOfService()
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

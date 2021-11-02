using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Services
{
    public class JsonFileCountriesService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JsonFileCountriesService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        private string JsonFileName
        {
            get { return Path.Combine(_webHostEnvironment.WebRootPath, "data", "countries.json"); }
        }
        public List<CountryModel> GetCountries()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonConvert.DeserializeObject<List<CountryModel>>(File.ReadAllText(JsonFileName));
            }
        }
    }
}

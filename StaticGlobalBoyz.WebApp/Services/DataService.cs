using Newtonsoft.Json;
using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Services
{
    public class DataService
    {
        private readonly IHttpClientFactory _clientFactory;

        public DataService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<List<T>> GetAll<T>(string type)
        {
            var output = new List<T>();
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:44394/api/SgbData/{type}/all");
            
            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                output = JsonConvert.DeserializeObject<List<T>>(responseText);
            }
            return output;
        }
        public async Task<T> Get<T>(string type, Guid id) where T: new()
        {
            var output = new T();
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:44394/api/SgbData/{type}/{id}");

            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                output = JsonConvert.DeserializeObject<T>(responseText);
            }
            return output;
        }
    }
}

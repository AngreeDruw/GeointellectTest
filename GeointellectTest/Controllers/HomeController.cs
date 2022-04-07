using GeointellectTest.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http;
using GeointellectTest.Classes;
using System.Text.Json;
using Newtonsoft.Json;
using GeointellectTest.Data;

namespace GeointellectTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GeointellectTestContext _context;

        public HomeController(ILogger<HomeController> logger, GeointellectTestContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("URL")] QueryToApiModal queryToApiModal)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync(queryToApiModal.URL, null);
                    response.EnsureSuccessStatusCode();
                    ViewData["ApiUrl"] = queryToApiModal.URL;

                    var responseContent = await response.Content.ReadAsStringAsync();
                    ViewData["ResponseMsg"] = responseContent;
                    ViewData["SavedProperty"] = await ParsJSON(responseContent);

                }
                catch (HttpRequestException e)
                {
                    ViewData["ResponseMsg"] = e.Message;
                }
            }
            return  View();
        }

        private async Task< Dictionary<string, string> >ParsJSON(string JSON)
        {
            var ObjectJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(JSON);
            var ItemList = new Dictionary<string, string>();
            if (ObjectJson is not null)
            {
                using (var urlConfig = new UrlConfig())
                {
                    foreach (var item in ObjectJson)
                    {
                        if (urlConfig.CheckProperty(item.Key))
                        {
                            ItemList.Add(item.Key, item.Value);
                        }
                    }
                };
            }
            await SavePropertyToDb(ItemList);
            return ItemList;
        }

        private async Task SavePropertyToDb(IDictionary<string, string> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
            {
                var property = new PropertyModel { Name = item.Key, Value = item.Value };
                await _context.Property.AddAsync(property);
                await _context.SaveChangesAsync();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
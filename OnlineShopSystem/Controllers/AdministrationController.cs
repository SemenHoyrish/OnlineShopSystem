using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem.Controllers
{
    public class AdministrationController : Controller
    {
        public IActionResult Index()
        {
            AuthController.CheckUserLoggedIn(this);
            return View();
        }

        public IActionResult EditShopSettings()
        {
            AuthController.CheckUserLoggedIn(this);
            return View();
        }

        public IActionResult EditCategories()
        {
            return View();
        }

        public IActionResult EditProduct()
        {
            return View();
        }

        [HttpPost]
        public string ChangeShopSettings([FromBody] ShopSettingsDeserialization settings)
        {
            string json = JsonSerializer.Serialize(settings);
            System.IO.File.WriteAllText($"ShopSettingsBackup/ShopSettings_{DateTime.Now.ToString().Replace(":",".")}.json", System.IO.File.ReadAllText("ShopSettings.json"));
            System.IO.File.WriteAllText("ShopSettings.json", json);
            return JsonResponse.Success();
        }
    }
}

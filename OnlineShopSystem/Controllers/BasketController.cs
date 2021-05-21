using Microsoft.AspNetCore.Mvc;
using OnlineShopSystem.DB;
using OnlineShopSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            AuthController.CheckUserLoggedIn(this);

            List<PurchaseViewModel> purchases = new List<PurchaseViewModel>();

            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Purchases.Where(a => a.Email == Request.Cookies["Email"] && a.IsDone == false).ToList();
                if (temp.Count != 0)
                {
                    foreach(var p in temp)
                    {
                        purchases.Add(new PurchaseViewModel(p));
                    }
                }
            }

            ViewBag.Purchases = purchases;

            return View();
        }

        public IActionResult BuyAll()
        {
            AuthController.CheckUserLoggedIn(this);
            var email = Request.Cookies["Email"];

            using (ApplicationContext db = new ApplicationContext())
            {
                var purchases = db.Purchases.Where(a => a.Email == email && a.IsDone == false).ToList();
                if (purchases.Count == 0)
                {
                    ViewBag.Status = "error";
                }
                else
                {
                    purchases[0].IsDone = true;
                    db.SaveChanges();
                }
            }

            return View();
        }


        [HttpGet]
        public string AddProduct(int id)
        {
            //AuthController.CheckUserLoggedIn(this);
            if (AuthController.IsUserLoggedIn(this) == false)
            {
                return JsonResponse.Error("user are not logged in!");
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Products.Where(a => a.ID == id).ToList();
                if(temp.Count != 1)
                {
                    return JsonResponse.Error("No such product!");
                }
                var product = temp[0];

                var email = Request.Cookies["Email"];


                var userPurchase = db.Purchases.Where(a => a.Email == email && a.IsDone == false).ToList();
                if (userPurchase.Count == 0)
                {
                    List<ProductViewModel> products = new List<ProductViewModel>();
                    products.Add(new ProductViewModel(product));
                    Purchase purchase = new Purchase() { Products = JsonSerializer.Serialize(products), IsDone = false, Email = email };
                    db.Purchases.Add(purchase);
                }
                else
                {
                    Purchase purchase = userPurchase[0];
                    var purchaseProducts = JsonSerializer.Deserialize<List<ProductViewModel>>(purchase.Products);
                    purchaseProducts.Add(new ProductViewModel(product));
                    purchase.Products = JsonSerializer.Serialize(purchaseProducts);
                }
                db.SaveChanges();
            }
            return JsonResponse.Success();
        }

        [HttpGet]
        public string RemoveProduct(int id)
        {
            if (AuthController.IsUserLoggedIn(this) == false)
            {
                return JsonResponse.Error("user are not logged in!");
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var email = Request.Cookies["Email"];
                var temp = db.Purchases.Where(a => a.Email == email && a.IsDone == false).ToList();
                if (temp.Count == 0)
                {
                    return JsonResponse.Error("no such product in the basket!");
                }
                var purchase = temp[0];
                var purchaseProducts = JsonSerializer.Deserialize<List<ProductViewModel>>(purchase.Products);
                purchaseProducts.RemoveAt(0);
                purchase.Products = JsonSerializer.Serialize(purchaseProducts);
                db.SaveChanges();
            }
            return JsonResponse.Success();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using OnlineShopSystem.DB;
using OnlineShopSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem.Controllers
{
    public class AdministrationController : Controller
    {
        public IActionResult Index()
        {
            AuthController.CheckUserLoggedIn(this, true);
            return View();
        }

        public IActionResult EditShopSettings()
        {
            AuthController.CheckUserLoggedIn(this, true);
            return View();
        }

        public IActionResult EditCategories()
        {
            AuthController.CheckUserLoggedIn(this, true);
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Categories.ToList();
                foreach(var cat in temp)
                {
                    categories.Add(new CategoryViewModel(cat));
                }
            }
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult EditProducts(int? id)
        {
            AuthController.CheckUserLoggedIn(this, true);
            if (id == null)
            {
                ViewBag.Mode = "products";
                List<ProductViewModel> products = new List<ProductViewModel>();
                using (ApplicationContext db = new ApplicationContext())
                {
                    var temp = db.Products.ToList();
                    foreach(var prod in temp)
                    {
                        products.Add(new ProductViewModel(prod));
                    }
                }

                ViewBag.Products = products;
            }
            else if (id == 0)
            {
                ViewBag.Mode = "product";
                ProductViewModel product = new ProductViewModel();
                List<CategoryViewModel> categories = new List<CategoryViewModel>();
                using (ApplicationContext db = new ApplicationContext())
                {
                    var cats = db.Categories.ToList();
                    foreach (var cat in cats)
                    {
                        categories.Add(new CategoryViewModel(cat));
                    }
                }
                ViewBag.Product = product;
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Mode = "product";
                ProductViewModel product = null;
                List<CategoryViewModel> categories = new List<CategoryViewModel>();
                using (ApplicationContext db = new ApplicationContext())
                {
                    var temp = db.Products.Where(a => a.ID == id).ToList();
                    if (temp.Count != 0)
                    {
                        product = new ProductViewModel(temp[0]);
                    }
                    var cats = db.Categories.ToList();
                    foreach(var cat in cats)
                    {
                        categories.Add(new CategoryViewModel(cat));
                    }
                }
                ViewBag.Product = product;
                ViewBag.Categories = categories;
            }
            return View();
        }

        [HttpPost]
        public string EditShopSettingsR([FromBody] ShopSettingsDeserialization settings)
        {
            string json = JsonSerializer.Serialize(settings);
            System.IO.File.WriteAllText($"ShopSettingsBackup/ShopSettings_{DateTime.Now.ToString().Replace(":",".")}.json", System.IO.File.ReadAllText("ShopSettings.json"));
            System.IO.File.WriteAllText("ShopSettings.json", json);

            ShopSettings.Name = settings.Name;
            ShopSettings.Description = settings.Description;
            ShopSettings.PhoneNumber = settings.PhoneNumber;
            ShopSettings.Address = settings.Address;
            ShopSettings.Vk = settings.Vk;
            ShopSettings.Instagram = settings.Instagram;

            return JsonResponse.Success();
        }

        [HttpPost]
        public string EditCategoriesR([FromBody] List<CategoryViewModel> categories)
        {
            int errorsCount = 0;
            int changesCount = 0;
            int createdCount = 0;
            int removedCount = 0;
            using (ApplicationContext db = new ApplicationContext())
            {
                foreach (var category in categories)
                {
                    if (category.ID == 0)
                    {
                        db.Categories.Add(new Category() { Name = category.Name, Description = category.Description });
                        createdCount++;
                    }
                    else
                    {
                        var temp = db.Categories.Where(a => a.ID == category.ID).ToList();
                        if (temp.Count == 0)
                        {
                            errorsCount++;
                            continue;
                        }
                        if (category.Name == "" && category.Description == "")
                        {
                            db.Categories.Remove(temp[0]);
                            removedCount++;
                        }
                        else
                        {
                            if (temp[0].Name != category.Name)
                            {
                                temp[0].Name = category.Name;
                                changesCount++;
                            }
                            if (temp[0].Description != category.Description)
                            {
                                temp[0].Description = category.Description;
                                changesCount++;
                            }
                        }
                        
                    }
                }
                db.SaveChanges();
            }
            return JsonResponse.Success($"Errors:{errorsCount}; Fields changed:{changesCount}; Categories created:{createdCount}; Categories removed:{removedCount}");
        }

        [HttpPost]
        public string EditProductsR([FromBody] ProductViewModel product)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (product.ID == 0)
                {
                    db.Products.Add(new Product(product));
                }
                else
                {
                    var temp = db.Products.Where(a => a.ID == product.ID).ToList();
                    if (temp.Count == 0)
                    {
                        return JsonResponse.Error("No such product!");
                    }
                    else
                    {
                        //List<int> indexes = new List<int>();
                        //for (int i = 0; i < product.Images.Count; i++)
                        //{
                        //    if (product.Images[i] == "")
                        //    {
                        //        indexes.Add(i);
                        //    }
                        //}
                        //indexes.Reverse();
                        //foreach(var index in indexes)
                        //{
                        //    product.Images.RemoveAt(index);
                        //}
                        Product obj = new Product(product);
                        temp[0].Name = obj.Name;
                        temp[0].Description = obj.Description;
                        temp[0].Cost = obj.Cost;
                        temp[0].GoodsRemain = obj.GoodsRemain;
                        temp[0].Images = obj.Images;
                        temp[0].Category = obj.Category;
                    }
                }
                db.SaveChanges();
            }
            return JsonResponse.Success();
        }

        [HttpPost]
        public string DeleteProductR([FromBody] int id)
        {
            
            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Products.Where(a => a.ID == id).ToList();
                if (temp.Count != 0)
                {
                    db.Products.Remove(temp[0]);
                    db.SaveChanges();
                    return JsonResponse.Success();
                }
                return JsonResponse.Error("no such product!");
            }
        }


        public class UploadedFile
        {
            public string Name { get; set; }
            public string Content { get; set; }
        }

        [HttpPost]
        public string UploadImage([FromBody] UploadedFile file)
        {
            string filename = AuthController.CreateMD5(file.Name + DateTime.Now.ToString())+"." + file.Name.Split(".")[file.Name.Split(".").Length - 1];
            //System.IO.File.WriteAllText($"wwwroot/Data/Uploads/Images/{filename}", file.Content);
            var bytes = Convert.FromBase64String(file.Content.Split(",")[1]);
            System.IO.File.WriteAllBytes($"wwwroot/Data/Uploads/Images/{filename}", bytes);


            //using (var stream = new FileStream($"wwwroot/Data/Uploads/Images/{filename}", FileMode.Create))
            //{
            //    stream.Write(bytes, 0, bytes.Length);
            //    stream.Flush();
            //}

            return JsonResponse.Success(filename);
        }

        //[HttpPost]
        //public string UploadSiteIco([FromBody] UploadedFile file)
        //{
        //    string filename = "site_ico" + "." + file.Name.Split(".")[file.Name.Split(".").Length - 1];
        //    //System.IO.File.WriteAllText($"wwwroot/Data/Uploads/Images/{filename}", file.Content);
        //    var bytes = Convert.FromBase64String(file.Content.Split(",")[1]);
        //    System.IO.File.WriteAllBytes($"wwwroot/Data/{filename}", bytes);

        //    return JsonResponse.Success(filename);
        //}

        //[HttpGet]
        //public string CheckSiteIco()
        //{
        //    if (System.IO.File.Exists("wwwroot/Data/site_ico.png") || System.IO.File.Exists("wwwroot/Data/site_ico.jpg") || System.IO.File.Exists("wwwroot/Data/site_ico.jpeg"))
        //    { }
        //    else
        //    {
        //        System.IO.File.WriteAllBytes("wwwroot/Data/site_ico.png", System.IO.File.ReadAllBytes("wwwroot/Data/site_ico_default.png"));
        //    }
        //    return JsonResponse.Success();
        //}

    }
}

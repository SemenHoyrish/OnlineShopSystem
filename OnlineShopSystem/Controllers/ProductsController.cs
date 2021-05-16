using Microsoft.AspNetCore.Mvc;
using OnlineShopSystem.DB;
using OnlineShopSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopSystem.Controllers
{
    public class ProductsController : Controller
    {

        public IActionResult Index()
        {
            //AuthController.CheckUserLoggedIn(this);
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            using (ApplicationContext db = new ApplicationContext())
            {
                var cats = db.Categories.ToList();
                foreach(var cat in cats)
                {
                    categories.Add(new CategoryViewModel(cat));
                }
            }
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult Category(int id)
        {
            CategoryViewModel category = null;
            List<ProductViewModel> products = new List<ProductViewModel>();
            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Categories.Where(a => a.ID == id).ToList();
                if (temp.Count == 0)
                {
                    
                }
                else
                {
                    category = new CategoryViewModel(temp[0]);
                    var prods = db.Products.Where(a => a.Category == temp[0].Name).ToList();
                    foreach(var prod in prods)
                    {
                        products.Add(new ProductViewModel(prod));
                    }
                }
            }
            ViewBag.Category = category;
            ViewBag.Products = products;
            return View();
        }

        public IActionResult Product(int id)
        {
            ProductViewModel product = null;
            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Products.Where(a => a.ID == id).ToList();
                if (temp.Count == 0)
                {

                }
                else
                {
                    product = new ProductViewModel(temp[0]);
                }
            }
            ViewBag.Product = product;
            return View();
        }
    }
}

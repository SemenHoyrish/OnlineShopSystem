using OnlineShopSystem.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem.Models
{
    public class PurchaseViewModel
    {
        public int ID { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public bool IsDone { get; set; }
        public string Email { get; set; }

        public PurchaseViewModel() { }
        public PurchaseViewModel(Purchase purchase)
        {
            ID = purchase.ID;
            Products = JsonSerializer.Deserialize<List<ProductViewModel>>(purchase.Products);
            IsDone = purchase.IsDone;
            Email = purchase.Email;
        }
    }
}

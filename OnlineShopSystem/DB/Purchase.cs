using OnlineShopSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem.DB
{
    public class Purchase
    {
        public int ID { get; set; }
        public string Products { get; set; }
        public bool IsDone { get; set; }
        public string Email { get; set; }

        public Purchase() { }
        public Purchase(PurchaseViewModel purchase)
        {
            ID = purchase.ID;
            Products = JsonSerializer.Serialize(purchase.Products);
            IsDone = purchase.IsDone;
            Email = purchase.Email;
        }
    }
}

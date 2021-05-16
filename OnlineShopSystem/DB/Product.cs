using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem.DB
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int GoodsRemain { get; set; }
        public string Images { get; set; }

        public Product() { }
        public Product(Models.ProductViewModel product)
        {
            Name = product.Name;
            Description = product.Description;
            Cost = product.Cost;
            GoodsRemain = product.GoodsRemain;
            Images = JsonSerializer.Serialize(product.Images);
        }
    }
}

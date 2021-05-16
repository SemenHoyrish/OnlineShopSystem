using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace OnlineShopSystem.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int GoodsRemain { get; set; }
        public List<string> Images { get; set; }

        public ProductViewModel() { }
        public ProductViewModel(DB.Product product)
        {
            Name = product.Name;
            Description = product.Description;
            Cost = product.Cost;
            GoodsRemain = product.GoodsRemain;
            Images = JsonSerializer.Deserialize<List<string>>(product.Images);
        }
    }
}

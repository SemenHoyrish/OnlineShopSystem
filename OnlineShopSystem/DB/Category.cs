using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopSystem.DB
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Category() { }
        public Category(Models.CategoryViewModel cat)
        {
            ID = cat.ID;
            Name = cat.Name;
            Description = cat.Description;
        }
    }
}

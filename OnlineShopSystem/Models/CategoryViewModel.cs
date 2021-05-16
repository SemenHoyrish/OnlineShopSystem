using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopSystem.Models
{
    public class CategoryViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CategoryViewModel() { }
        public CategoryViewModel(DB.Category cat)
        {
            ID = cat.ID;
            Name = cat.Name;
            Description = cat.Description;
        }
    }
}

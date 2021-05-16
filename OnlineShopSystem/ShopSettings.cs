using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopSystem
{
    public static class ShopSettings
    {
        public static string Name { get; set; }
        public static string Description { get; set; }
        public static string PhoneNumber { get; set; }
        public static string Address { get; set; }
        public static string Vk { get; set; }
        public static string Instagram { get; set; }
    }

    public class ShopSettingsDeserialization
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Vk { get; set; }
        public string Instagram { get; set; }
    }
}

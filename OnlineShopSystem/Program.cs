using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // Load shop settings
            string json = File.ReadAllText("ShopSettings.json");
            var obj = JsonSerializer.Deserialize<ShopSettingsDeserialization>(json);
            ShopSettings.Name = obj.Name;
            ShopSettings.Description = obj.Description;
            ShopSettings.PhoneNumber = obj.PhoneNumber;
            ShopSettings.Address = obj.Address;
            ShopSettings.Vk = obj.Vk;
            ShopSettings.Instagram = obj.Instagram;
            
            CreateHostBuilder(args).Build().Run();

            

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

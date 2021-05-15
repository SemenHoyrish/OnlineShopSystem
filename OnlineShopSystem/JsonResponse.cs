using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace OnlineShopSystem
{
    public class JsonResponse
    {
        public static string Success()
        {
            return JsonSerializer.Serialize(new { status = "success" });
        }

        public static string Success(string result)
        {
            return JsonSerializer.Serialize(new { status = "success", result = $"{result}" });
        }

        public static string Error(string message)
        {
            return JsonSerializer.Serialize(new { status = "error", error = $"{message}" });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopSystem.DB
{
    public class Session
    {
        public int ID { get; set; }
        public string SessionName { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Email { get; set; }
    }
}

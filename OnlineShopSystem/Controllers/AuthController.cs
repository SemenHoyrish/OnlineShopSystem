using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using OnlineShopSystem.DB;
using OnlineShopSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("Email");
            Response.Cookies.Delete("Session");
            Response.Cookies.Delete("IsAdmin");
            return View();
        }

        [HttpPost]
        public string DoLogin([FromBody] AuthLoginViewModel model)
        {
            List<DB.User> user;

            using (ApplicationContext db = new ApplicationContext())
            {
                user = db.Users.Where(a => a.Email == model.Email && a.Password == HashPassword(model.Password)).ToList();
            }

            if (user.Count == 0)
            {
                return JsonResponse.Error("incorrect login or password");
            }
            else
            {
                Response.Cookies.Append("Email", model.Email, new Microsoft.AspNetCore.Http.CookieOptions() {Path = "/", Expires = DateTimeOffset.Now.AddDays(7) });
                Response.Cookies.Append("Session", GetNewSession(model.Email), new Microsoft.AspNetCore.Http.CookieOptions() {Path = "/", Expires = DateTimeOffset.Now.AddDays(7) });
                if (user[0].Role == "admin")
                {
                    Response.Cookies.Append("IsAdmin", "it`s a secret!", new Microsoft.AspNetCore.Http.CookieOptions() { Path = "/", Expires = DateTimeOffset.Now.AddDays(7) });
                }
                return JsonResponse.Success();
            }
        }
        [HttpPost]
        public string DoRegister([FromBody] AuthLoginViewModel model)
        {
            //if (model.Password != model.RePassword)
            //{
            //    return JsonResponse.Error("passwords differ");
            //}
            using (ApplicationContext db = new ApplicationContext())
            {
                var res = db.Users.Where(a => a.Email == model.Email).ToList();
                if (res.Count != 0)
                {
                    return JsonResponse.Error("user already exists");
                }
                else
                {
                    db.Users.Add(new DB.User() { Email = model.Email, Password = HashPassword(model.Password), Role = "user" });
                    db.SaveChanges();
                    return JsonResponse.Success();
                }
            }
        }

        public static bool IsUserLoggedIn(ControllerBase controller)
        {
            string email = "";
            string session = "";
            controller.Request.Cookies.TryGetValue("email", out email);
            controller.Request.Cookies.TryGetValue("session", out session);
            if (email == "" || session == "")
            {
                return false;
            }
            if (CheckSession(session, email))
            {
                return true;
            }
            return false;
        }

        public static void CheckUserLoggedIn(ControllerBase controller, bool mustbeadmin=false)
        {
            if (!IsUserLoggedIn(controller))
            {
                controller.Response.Redirect("/Auth/Login");
            }
            if (mustbeadmin == true && GetCurrentUserRole(controller) != "admin")
            {
                controller.Response.Redirect("/Auth/Login");
            }
        }

        // ! Maybe not work
        public static string GetCurrentUserRole(ControllerBase controller)
        {
            if(!IsUserLoggedIn(controller))
            {
                return "";
            }
            string email = "";
            controller.Request.Cookies.TryGetValue("email", out email);
            using (ApplicationContext db = new ApplicationContext())
            {
                var temp = db.Users.Where(a => a.Email == email).ToList();
                if (temp.Count == 0)
                {
                    return "";
                }
                return temp[0].Role;
            }

        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8] { 54, 201, 77, 61, 222, 184, 12, 107, 10, 72, 239, 32, 165, 167, 93, 12 };
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));


            return hashed;
        }

        private static string GetNewSession(string email)
        {
            List<char> chars = new List<char> {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                        '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
            //'!', '@', '#', '$', '%', '&', '?', '-', '+', '=', '~' };

            Random rand = new Random();
            string tmp = "";
            for (int i = 0; i < 100; i++)
            {
                tmp += chars[rand.Next(0, chars.Count)];
            }
            string md5hash = CreateMD5(tmp);

            using (ApplicationContext db = new ApplicationContext())
            {
                var res = db.Sessions.Where(a => a.SessionName == md5hash).ToList();
                if (res.Count != 0)
                {
                    return GetNewSession(email);
                }
                else
                {
                    db.Sessions.Add(new DB.Session() { SessionName = md5hash, ExpiresOn = DateTime.Now.AddDays(7), Email = email });
                    db.SaveChanges();
                    return md5hash;
                }
            }
        }

        public static bool CheckSession(string sessionName, string email)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var res = db.Sessions.Where(a => a.SessionName == sessionName).ToList();
                if (res.Count == 0)
                {
                    return false;
                }
                else
                {
                    if (DateTime.Now >= res[0].ExpiresOn)
                    {
                        return false;
                    }
                    else
                    {
                        if (res[0].Email == email)
                            return true;
                        else
                            return false;
                    }
                }
            }
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}

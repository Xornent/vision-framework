using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vision.Data;
using Vision.Models;

namespace Vision.Controllers {

    public class AccountController : Controller {
        private readonly UserContext _ctx_user;

        public AccountController(UserContext ctxUser) {
            this._ctx_user = ctxUser;
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection form) {
            if (HttpContext.User.Identity.IsAuthenticated) {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            var currentName = Utilities.Query.GetUserByName(_ctx_user, form["username"]);
            if (currentName == null) return ManagedUserNameNotFoundError(form["username"]);
            if (!(currentName.Password == form["password"])) {
                return ManagedPasswordError();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("display", form["username"]));
            identity.AddClaim(new Claim("level", currentName.Level.ToString()));
            identity.AddClaim(new Claim("evaluation", currentName.Evaluation.ToString()));
            if (currentName.Banned == 0)
                identity.AddClaim(new Claim("banned", "false"));
            else identity.AddClaim(new Claim("banned", "true"));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddSeconds(600)
                });

            ViewData["AuthenticatedUser"] = true;
            ViewData["AuthenticatedAdministrator"] = currentName.Level >= 4; 
            ViewData["AuthenticatedUserName"] = currentName.Display;
            return View("../Home/Index");
        }

        public IActionResult Logout() {
            if (HttpContext.User.Identity.IsAuthenticated) {
                HttpContext.AuthenticateAsync();
                var userName = HttpContext.User.Claims.First().Value;
                if (!string.IsNullOrEmpty(userName)) {
                    string loc = "Local IP Address: " + Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ": " + Request.HttpContext.Connection.LocalPort;
                    string rem = "Remote IP Address: " + Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + ": " + Request.HttpContext.Connection.RemotePort;
                    ViewData["Authenticated"] = loc + "<br/>" + rem + "<br/>" + "User: " + userName;
                    return View();
                }
            }
            ViewData["Authenticated"] = "Not Logged In";
            ViewData["AuthenticatedUser"] = false;
            ViewData["AuthenticatedAdministrator"] = false;
            ViewData["AuthenticatedUserName"] = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(IFormCollection form) {
            try {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            } catch { }
            ViewData["AuthenticatedUser"] = false;
            ViewData["AuthenticatedAdministrator"] = false;
            ViewData["AuthenticatedUserName"] = "";
            return View("../Home/Index");
        }

        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(IFormCollection form) {
            var currentName = Utilities.Query.GetUserByName(_ctx_user, form["username"]);
            if (currentName == null) {
                Models.User user = new User();
                user.Banned = 0;
                user.Contact = form["contact"];
                user.Password = form["password"];
                user.Level = int.Parse(form["level"]);
                user.Display = form["username"];
                user.Edit = "";
                user.Evaluation = 60;

                _ctx_user.Add(user);
            } else return ManagedUserNameExistError(form["username"]);
            await _ctx_user.SaveChangesAsync();
            return View("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ManagedError(string title, string description) {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View("../Home/Error", new ManagedError()
            {
                Title = title,
                Details = description
            });
        }

        public IActionResult ManagedUserNameNotFoundError(string name) => ManagedError(
            "用户名不存在",

            "你输入的用户名 <code>"+ name +"</code> 不存在，请检查用户名的拼写错误和大小写");

        public IActionResult ManagedUserNameExistError(string name) => ManagedError(
            "用户名已存在",

            "你输入的用户名 <code>" + name + "</code> 已存在，请更换用户名");

        public IActionResult ManagedPasswordError() => ManagedError(
            "密码错误",

            "请检查密码的拼写错误和大小写");
    }
}

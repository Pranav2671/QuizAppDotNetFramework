using System;
using System.Data;
using System.Web;
using System.Web.Mvc;
using QuizAppDotNetFramework.Helpers;
using QuizAppDotNetFramework.Repository;

namespace QuizAppDotNetFramework.Helpers
{

    public class AuthController : Controller
    {

        private readonly AuthRepository authRepo = new AuthRepository();

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string role)
        {
            try
            {
                // Remove existing JWT cookie to prevent loops
                if (Request.Cookies["QuizAppJWT"] != null)
                {
                    var oldCookie = new HttpCookie("QuizAppJWT") { Expires = DateTime.Now.AddDays(-1), Path = "/" };
                    Response.Cookies.Add(oldCookie);
                }

                var dt = authRepo.GetUserByUsername(username);
                if (dt.Rows.Count == 0)
                {
                    ViewBag.Error = "Invalid username, password, or role.";
                    return View();
                }

                string storedPassword = dt.Rows[0]["Password"].ToString();
                string dbRole = dt.Rows[0]["Role"].ToString();
                string userId = dt.Rows[0]["UserId"].ToString();

                if (!PasswordHelper.HashPassword(password).Equals(storedPassword, StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Error = "Invalid username, password, or role.";
                    return View();
                }

                if (!role.Equals(dbRole, StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Error = "Role mismatch.";
                    return View();
                }

                // Generate new JWT
                string token = JwtHelper.GenerateToken(userId, username, dbRole);

                var cookie = new HttpCookie("QuizAppJWT", token)
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    //Expires = DateTime.Now.AddHours(1),
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                };
                Response.Cookies.Add(cookie);

                // Redirect based on role
                return dbRole.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction("Index", "Admin")
                    : RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Login failed: " + ex.Message;
                return View();
            }
        }


        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string password)
        {
            try
            {
                string passwordHash = PasswordHelper.HashPassword(password);
                string role = "User";
                authRepo.RegisterUser(username, passwordHash, role);
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Registration failed: " + ex.Message;
                return View();
            }
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["QuizAppJWT"] != null)
            {
                var cookie = new HttpCookie("QuizAppJWT") { Expires = DateTime.Now.AddDays(-1), Path = "/" };
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Login");
        }
    }
}

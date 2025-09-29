using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Repository;

namespace QuizAppDotNetFramework.Controllers
{
    public class AuthController : Controller
    {
        private AuthRepository authRepo = new AuthRepository();

        // GET: Login page
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password, string role)
        {
            try
            {
                string passwordHash = password; // Hash if needed

                var dt = authRepo.LoginUser(username, passwordHash);

                if (dt.Rows.Count > 0)
                {
                    string dbRole = dt.Rows[0]["Role"].ToString();

                    if (!role.Equals(dbRole, StringComparison.OrdinalIgnoreCase))
                    {
                        ViewBag.Error = "Selected role does not match your account.";
                        return View("Login");
                    }

                    // Set session
                    Session["UserId"] = dt.Rows[0]["UserId"];
                    Session["Username"] = dt.Rows[0]["Username"];
                    Session["Role"] = dbRole;

                    // Role-based redirect
                    if (dbRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Index", "Admin");
                    else if (dbRole.Equals("User", StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Index", "User");
                }

                ViewBag.Error = "Invalid username, password, or role selection.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Login failed: " + ex.Message;
                return View("Login");
            }
        }

        // GET: Registration page
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Registration
        [HttpPost]
        public ActionResult Register(string username, string password)
        {
            try
            {
                string passwordHash = password; // Hash if needed
                string role = "User";           // Force all new users to be "User"

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

        // POST: Logout
        //[HttpPost]
        public ActionResult Logout()
        {
            Session.Clear(); // Clear session data
            return RedirectToAction("Index", "Auth"); // Redirect to login page
        }
    }
}

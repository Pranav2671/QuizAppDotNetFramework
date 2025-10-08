using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Repository;
using QuizAppDotNetFramework.Helpers; // ✅ For password hashing
using System.Data; // ✅ Needed for DataTable

namespace QuizAppDotNetFramework.Controllers
{
    public class AuthController : Controller
    {
        private AuthRepository authRepo = new AuthRepository();

        // GET: Login page
        [HttpGet]
        public ActionResult Login()
        {
            var s = Session["UserId"];
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password, string role)
        {
            try
            {
                // ✅ Step 1: Get user by username
                DataTable dt = authRepo.GetUserByUsername(username);

                if (dt.Rows.Count == 0)
                {
                    ViewBag.Error = "Invalid username, password, or role selection.";
                    return View("Login");
                }

                string storedPassword = dt.Rows[0]["Password"].ToString();
                string dbRole = dt.Rows[0]["Role"].ToString();
                string userId = dt.Rows[0]["UserId"].ToString();
                string dbUsername = dt.Rows[0]["Username"].ToString();

                bool loginSuccess = false;

                // ✅ Step 2: Check if stored password looks like a hash (64 hex chars)
                if (storedPassword.Length == 64)
                {
                    // Compare hashed input with stored hash
                    string inputHash = PasswordHelper.HashPassword(password);
                    if (storedPassword.Equals(inputHash, StringComparison.OrdinalIgnoreCase))
                        loginSuccess = true;
                }
                else
                {
                    // Old plain-text password: check directly
                    if (storedPassword == password)
                    {
                        loginSuccess = true;

                        // ✅ Step 3: Upgrade password to hashed version
                        string newHashed = PasswordHelper.HashPassword(password);
                        authRepo.UpdateUserPassword(username, newHashed);
                    }
                }

                if (!loginSuccess)
                {
                    ViewBag.Error = "Invalid username, password, or role selection.";
                    return View("Login");
                }

                // ✅ Step 4: Role check
                if (!role.Equals(dbRole, StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Error = "Selected role does not match your account.";
                    return View("Login");
                }

                // ✅ Step 5: Set session values
                Session["UserId"] = userId;
                Session["Username"] = dbUsername;
                Session["Role"] = dbRole;

                // ✅ Step 6: Redirect based on role
                if (dbRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Index", "Admin");
                else if (dbRole.Equals("User", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Index", "User");

                ViewBag.Error = "Invalid role selection.";
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
                // ✅ Hash the password before saving
                string passwordHash = PasswordHelper.HashPassword(password);
                string role = "User"; // Default role stays the same

                // ✅ Save hashed password
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
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}

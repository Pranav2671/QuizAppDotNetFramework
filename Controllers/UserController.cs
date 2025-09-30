using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QuizAppDotNetFramework.Controllers
{
    public class UserController : Controller
    {
        // Check if the logged-in user is a regular User
    public ActionResult Index()
        {
            if (Session["Role"]?.ToString() != "User")
                return RedirectToAction("Login", "Auth");
            return View();
        }
    }
}
using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Repository;
using QuizAppDotNetFramework.Models;

namespace QuizAppDotNetFramework.Controllers
{
    public class AdminController : Controller
    {
        private QuizRepository _quizRepository = new QuizRepository();
        private UserRepository _userRepository = new UserRepository();

        // -------------------- Admin Dashboard --------------------
        public ActionResult Index()
        {
            return View(); // Shows Admin Dashboard
        }

        // -------------------- Quiz Management --------------------
        public ActionResult ManageQuizzes()
        {
            var quizzes = _quizRepository.GetAllQuizzes();
            return View(quizzes);
        }

        public ActionResult DeleteQuiz(Guid id)
        {
            _quizRepository.DeleteQuiz(id);
            return RedirectToAction("ManageQuizzes");
        }

        // -------------------- Question Management --------------------
        public ActionResult ManageQuestions(Guid quizId)
        {
            var questions = _quizRepository.GetQuestionsByQuizId(quizId);
            ViewBag.QuizId = quizId;
            return View(questions);
        }

        public ActionResult DeleteQuestion(Guid id, Guid quizId)
        {
            _quizRepository.DeleteQuestion(id);
            return RedirectToAction("ManageQuestions", new { quizId = quizId });
        }

        // -------------------- User Management --------------------
        public ActionResult ManageUsers()
        {
            var users = _userRepository.GetAllUsers();
            return View(users);
        }

        public ActionResult DeleteUser(Guid id)
        {
            _userRepository.DeleteUser(id);
            return RedirectToAction("ManageUsers");
        }
        //public ActionResult Logout()
        //{
        //    Session.Clear(); // Clear all session data
        //    return RedirectToAction("Login", "Auth"); // Redirect back to login page
        //}
    }
}

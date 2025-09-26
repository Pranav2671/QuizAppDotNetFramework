using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Models;
using QuizAppDotNetFramework.Repository;

namespace QuizAppDotNetFramework.Controllers
{
    public class AdminController : Controller
    {
        private QuizRepository quizRepo = new QuizRepository();

        // GET: Display all quizzes
        public ActionResult Index()
        {
            var quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        // GET: Load Update Quiz form
        [HttpGet]
        public ActionResult UpdateQuiz(Guid quizId)
        {
            var quiz = quizRepo.GetQuizById(quizId);
            return View(quiz);
        }

        // POST: Update Quiz
        [HttpPost]
        public ActionResult UpdateQuiz(QuizModel quiz)
        {
            try
            {
                quizRepo.UpdateQuiz(quiz);
                TempData["Success"] = "Quiz updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Update failed: " + ex.Message;
                return View(quiz);
            }
        }

        // GET: Add new quiz
        [HttpGet]
        public ActionResult AddQuiz()
        {
            return View(new QuizModel());
        }

        // POST: Add new quiz
        [HttpPost]
        public ActionResult AddQuiz(QuizModel quiz)
        {
            try
            {
                quizRepo.AddQuiz(quiz);
                TempData["Success"] = "Quiz added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Add failed: " + ex.Message;
                return View(quiz);
            }
        }

        // Delete Quiz
        public ActionResult DeleteQuiz(Guid quizId)
        {
            try
            {
                quizRepo.DeleteQuiz(quizId);
                TempData["Success"] = "Quiz deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Delete failed: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}

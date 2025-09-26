using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Models;
using QuizAppDotNetFramework.Repository;

namespace QuizAppDotNetFramework.Controllers
{
    public class AdminController : Controller
    {
        private QuizRepository _quizRepository = new QuizRepository();

        // GET: load quiz to edit
        public ActionResult UpdateQuiz(Guid id)
        {
            var quiz = _quizRepository.GetQuizById(id);
            return View(quiz);
        }

        // POST: save edited quiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuiz(QuizModel model)
        {
            if (ModelState.IsValid)
            {
                _quizRepository.UpdateQuiz(model);
                TempData["Success"] = "Quiz updated successfully!";
                return RedirectToAction("Index"); // Quiz list page
            }
            return View(model);
        }
    }
}

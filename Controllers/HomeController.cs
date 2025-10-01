using QuizAppDotNetFramework.Models;
using QuizAppDotNetFramework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizAppDotNetFramework.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuizRepository quizRepo;

        public HomeController()
        {
            quizRepo = new QuizRepository();
        }

        // GET: Home/Index
        public ActionResult Index()
        {
            var quizzes = quizRepo.GetAllQuizzes(); // gets all quizzes
            return View(quizzes);
        }

        // GET: Home/TakeQuiz
        public ActionResult TakeQuiz(Guid quizId)
        {
            QuestionRepository questionRepo = new QuestionRepository();
            var questions = questionRepo.GetQuestionsByQuizId(quizId);

            if (questions.Count == 0)
                return Content("No questions available for this quiz.");

            ViewBag.QuizId = quizId;
            return View(questions);
        }

        // POST: Home/SubmitQuiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(Guid QuizID, FormCollection answers)
        {
            var userId = (Guid)Session["UserId"]; // Current logged in User
            QuestionRepository questionRepo = new QuestionRepository();
            var allQuestions = questionRepo.GetQuestionsByQuizId(QuizID); // ✅ fixed typo

            UserResponseRepository responseRepo = new UserResponseRepository();

            foreach (var question in allQuestions) // ✅ now matches
            {
                string selectedOption = answers["answers[" + question.QuestionId + "]"];
                responseRepo.AddResponse(new UserResponseModel
                {
                    ResponseId = Guid.NewGuid(),
                    UserId = userId,
                    QuizId = QuizID,
                    QuestionId = question.QuestionId,
                    SelectedOption = selectedOption,
                    IsCorrect = selectedOption == question.CorrectOption,
                    ResponseDate = DateTime.Now
                });
            }

            return RedirectToAction("QuizResult", new { quizId = QuizID });
        }

        // GET: Home/QuizResult
        public ActionResult QuizResult(Guid quizId)
        {
            var userId = (Guid)Session["UserId"];
            UserResponseRepository responseRepo = new UserResponseRepository();

            var responses = responseRepo.GetUserResponsesForQuiz(userId, quizId);

            int totalQuestions = responses.Count;
            int correctAnswers = responses.Count(r => r.IsCorrect);

            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.CorrectAnswers = correctAnswers;

            return View(responses);
        }
    }
}

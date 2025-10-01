using QuizAppDotNetFramework.Models;
using QuizAppDotNetFramework.Repository;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuizAppDotNetFramework.Controllers
{
    public class UserController : Controller
    {
        private readonly QuizRepository quizRepo = new QuizRepository();
        private readonly QuestionRepository questionRepo = new QuestionRepository();

        // User Dashboard - shows quizzes
        public ActionResult Index()
        {
            // Ensure user is logged in
            if (Session["Role"] == null || Session["Role"].ToString() != "User")
                return RedirectToAction("Login", "Auth");

            var quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        // Show quiz questions
        public ActionResult TakeQuiz(Guid quizId)
        {
            var questions = questionRepo.GetQuestionsByQuizId(quizId);

            if (questions.Count == 0)
                return Content("No questions available for this quiz.");

            ViewBag.QuizId = quizId;
            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(Guid quizId, FormCollection answers)
        {
            var userId = (Guid)Session["UserId"];
            var allQuestions = questionRepo.GetQuestionsByQuizId(quizId);

            UserResponseRepository responseRepo = new UserResponseRepository();

            foreach (var question in allQuestions)
            {
                string selectedOption = answers["answers[" + question.QuestionId + "]"];
                responseRepo.AddResponse(new UserResponseModel
                {
                    ResponseId = Guid.NewGuid(),
                    UserId = userId,
                    QuizId = quizId,
                    QuestionId = question.QuestionId,
                    SelectedOption = selectedOption,
                    IsCorrect = selectedOption == question.CorrectOption,
                    ResponseDate = DateTime.Now
                });
            }

            return RedirectToAction("QuizResult", new { quizId = quizId });
        }

        // Show quiz result
        public ActionResult QuizResult(Guid quizId)
        {
            var userId = (Guid)Session["UserId"];
            UserResponseRepository responseRepo = new UserResponseRepository();

            var responses = responseRepo.GetUserResponsesForQuiz(userId, quizId);

            ViewBag.TotalQuestions = responses.Count;
            ViewBag.CorrectAnswers = responses.Count(r => r.IsCorrect);

            return View(responses);
        }
    }
}

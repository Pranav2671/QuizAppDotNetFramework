using QuizAppDotNetFramework.Models;
using QuizAppDotNetFramework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuizAppDotNetFramework.Controllers
{
    public class UserController : Controller
    {
        private readonly QuizRepository quizRepo = new QuizRepository();
        private readonly QuestionRepository questionRepo = new QuestionRepository();
        private readonly UserResponseRepository responseRepo = new UserResponseRepository();

        // User Dashboard - list quizzes
        public ActionResult Index()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "User")
                return RedirectToAction("Login", "Auth");

            var quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        // Take Quiz
        public ActionResult TakeQuiz(Guid quizId)
        {
            var questions = questionRepo.GetQuestionsByQuizId(quizId);

            if (questions.Count == 0)
                return Content("No questions available for this quiz.");

            ViewBag.QuizId = quizId;

            // Optional: Clear any previous session answers if you store them
            Session["CurrentQuizAnswers"] = null;

            return View(questions);
        }

        // Submit Quiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(Guid QuizID, FormCollection answers)
        {
            var questions = questionRepo.GetQuestionsByQuizId(QuizID);
            var userId = (Guid)Session["UserId"];

            // ✅ One AttemptId per quiz attempt
            Guid attemptId = Guid.NewGuid();

            foreach (var q in questions)
            {
                string userAnswer = answers["answers[" + q.QuestionId + "]"];
                bool isCorrect = q.CorrectOption.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);

                responseRepo.AddResponse(new UserResponseModel
                {
                    ResponseId = Guid.NewGuid(),
                    UserId = userId,
                    QuizId = QuizID,
                    QuestionId = q.QuestionId,
                    SelectedOption = userAnswer,
                    IsCorrect = isCorrect,
                    ResponseDate = DateTime.Now,
                    AttemptId = attemptId
                });
            }

            // Prepare result to display immediately
            var resultDetails = questions.Select(q => new QuizResultDetail
            {
                QuestionText = q.QuestionText,
                CorrectAnswer = q.CorrectOption,
                YourAnswer = answers["answers[" + q.QuestionId + "]"],
                IsCorrect = q.CorrectOption.Equals(answers["answers[" + q.QuestionId + "]"], StringComparison.OrdinalIgnoreCase)
            }).ToList();

            int totalQuestions = questions.Count;
            int correctAnswers = resultDetails.Count(r => r.IsCorrect);

            var resultModel = new QuizResultModel
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                Score = (totalQuestions > 0) ? (correctAnswers * 100 / totalQuestions) : 0,
                ResultDetails = resultDetails
            };

            return View("QuizResult", resultModel);
        }

        // View Quiz History
        public ActionResult QuizHistory()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            var userId = (Guid)Session["UserId"];
            var history = responseRepo.GetQuizHistoryForUser(userId);
            return View(history);
        }

        // View Quiz Result by Attempt
        public ActionResult QuizResultByAttempt(Guid attemptId)
        {
            var responses = responseRepo.GetUserResponsesByAttempt(attemptId);
            if (responses.Count == 0)
                return Content("No data found for this attempt.");

            var resultDetails = responses.Select(r => new QuizResultDetail
            {
                QuestionText = r.QuestionText,
                CorrectAnswer = r.CorrectOption,
                YourAnswer = r.SelectedOption,
                IsCorrect = r.IsCorrect
            }).ToList();

            int totalQuestions = responses.Count;
            int correctAnswers = responses.Count(r => r.IsCorrect);

            var resultModel = new QuizResultModel
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                Score = (totalQuestions > 0) ? (correctAnswers * 100 / totalQuestions) : 0,
                ResultDetails = resultDetails
            };

            return View("QuizResult", resultModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttempt(Guid attemptId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            var userId = (Guid)Session["UserId"];
            responseRepo.DeleteAttempt(attemptId, userId);

            TempData["SuccessMessage"] = "Quiz attempt deleted successfully!";
            return RedirectToAction("QuizHistory");
        }

    }
}

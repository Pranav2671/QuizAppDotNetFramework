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

        // Dashboard - list quizzes
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
            ViewBag.TimeLimit = questions.Count; // 1 min per question
            return View(questions);
        }

        // Submit Quiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(Guid QuizID, FormCollection answers, int ElapsedSeconds = 0)
        {
            var questions = questionRepo.GetQuestionsByQuizId(QuizID);
            var userId = (Guid)Session["UserId"];

            Guid attemptId = Guid.NewGuid();

            foreach (var q in questions)
            {
                string userAnswer = answers["answers[" + q.QuestionId + "]"];
                if (string.IsNullOrEmpty(userAnswer) || userAnswer == "NA")
                    userAnswer = "Not Attempted";

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

            return RedirectToAction("QuizHistory");
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

        // Delete attempt
        [HttpPost]
        public ActionResult DeleteAttempt(Guid attemptId)
        {
            responseRepo.DeleteAttempt(attemptId);
            return RedirectToAction("QuizHistory");
        }

        // View result for a specific attempt (without correct answers)
        public ActionResult QuizHistoryResult(Guid attemptId)
        {
            var userId = (Guid)Session["UserId"];
            var responses = responseRepo.GetUserResponsesByAttempt(attemptId);

            if (responses == null || responses.Count == 0)
                return Content("No data found for this attempt.");

            int totalQuestions = responses.Count;
            int correctAnswers = responses.Count(r => r.IsCorrect);
            int scorePercent = (totalQuestions > 0) ? (correctAnswers * 100 / totalQuestions) : 0;

            var resultDetails = responses.Select(r => new QuizHistoryResultDetail
            {
                QuestionText = r.QuestionText,
                YourAnswer = r.SelectedOption
            }).ToList();

            var model = new QuizHistoryResultModel
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                ScorePercentage = scorePercent,
                ResultDetails = resultDetails
            };

            return View(model);
        }
    }
}

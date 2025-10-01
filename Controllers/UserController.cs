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

        // Handle quiz submission and calculate results
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(Guid QuizID, FormCollection answers)
        {
            var questions = questionRepo.GetQuestionsByQuizId(QuizID);

            int totalQuestions = questions.Count;
            int correctAnswers = 0;
            var resultDetails = new List<QuizResultDetail>();

            foreach (var q in questions)
            {
                // Get user's selected answer for this question
                string userAnswer = answers["answers[" + q.QuestionId + "]"];

                // Compare letters (A/B/C/D)
                bool isCorrect = q.CorrectOption.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);

                if (isCorrect)
                    correctAnswers++;

                resultDetails.Add(new QuizResultDetail
                {
                    QuestionText = q.QuestionText,
                    CorrectAnswer = q.CorrectOption + ": " + GetOptionText(q, q.CorrectOption),
                    YourAnswer = (userAnswer != null ? userAnswer + ": " + GetOptionText(q, userAnswer) : "Not Answered"),
                    IsCorrect = isCorrect
                });
            }

            var resultModel = new QuizResultModel
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                Score = (totalQuestions > 0) ? (correctAnswers * 100 / totalQuestions) : 0,
                ResultDetails = resultDetails
            };

            return View("QuizResult", resultModel);
        }

        // Optional: Show quiz result from repository (if needed)
        public ActionResult QuizResult(Guid quizId)
        {
            var userId = (Guid)Session["UserId"];
            UserResponseRepository responseRepo = new UserResponseRepository();

            var responses = responseRepo.GetUserResponsesForQuiz(userId, quizId);

            ViewBag.TotalQuestions = responses.Count;
            ViewBag.CorrectAnswers = responses.Count(r => r.IsCorrect);

            return View(responses);
        }

        // Helper method to get option text from letter
        private string GetOptionText(QuestionModel q, string option)
        {
            switch (option)
            {
                case "A": return q.OptionA;
                case "B": return q.OptionB;
                case "C": return q.OptionC;
                case "D": return q.OptionD;
                default: return "";
            }
        }
    }
}

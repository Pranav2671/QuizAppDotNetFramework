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
        private readonly AssignedQuizRepository assignedQuizRepo = new AssignedQuizRepository();


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

            // 1️⃣ Save each answer to the database
            foreach (var q in questions)
            {
                string userAnswer = answers["answers[" + q.QuestionId + "]"];
                if (string.IsNullOrEmpty(userAnswer))
                    userAnswer = "Not Attempted"; // handle unanswered

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

            // 2️⃣ Calculate total score
            int totalQuestions = questions.Count;
            int correctAnswers = questions.Count(q =>
            {
                string ans = answers["answers[" + q.QuestionId + "]"];
                if (string.IsNullOrEmpty(ans) || ans == "NA") ans = "Not Attempted";
                return q.CorrectOption.Equals(ans, StringComparison.OrdinalIgnoreCase);
            });
            int score = (int)((double)correctAnswers / totalQuestions * 100);

            // 3️⃣ Update assignment if this quiz is assigned
            assignedQuizRepo.UpdateAssignmentAfterAttempt(userId, QuizID, attemptId, score);

            // 4️⃣ Prepare question-wise details
            var resultDetails = questions.Select(q =>
            {
                string userAnswer = answers["answers[" + q.QuestionId + "]"];
                if (string.IsNullOrEmpty(userAnswer) || userAnswer == "NA") userAnswer = "Not Attempted";

                return new QuizResultDetail
                {
                    QuestionText = q.QuestionText,
                    YourAnswer = userAnswer,
                    CorrectAnswer = q.CorrectOption,
                    IsCorrect = q.CorrectOption.Equals(userAnswer, StringComparison.OrdinalIgnoreCase)
                };
            }).ToList();

            // 5️⃣ Create QuizResultModel for the view
            var resultModel = new QuizResultModel
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                Score = score,
                ResultDetails = resultDetails
            };

            // 6️⃣ Pass QuizId for Retake button
            ViewBag.QuizId = QuizID;

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

        // Delete attempt
        [HttpPost]
        public ActionResult DeleteAttempt(Guid attemptId)
        {
            var userId = (Guid)Session["UserId"];
            responseRepo.DeleteAttempt(attemptId, userId);

            TempData["SuccessMessage"] = "Quiz attempt deleted successfully!";
            return RedirectToAction("QuizHistory");
        }

        public ActionResult QuizAssignments()
        {
            var userId = (Guid)Session["UserId"];
            var assignments = assignedQuizRepo.GetAssignedQuizzesByUser(userId);
            return View(assignments);
        }

        //View Assignment
        [HttpGet]
        public ActionResult ViewAssignments()
        {
            // Ensure user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Guid userId = Guid.Parse(Session["UserId"].ToString());
            var assignments = assignedQuizRepo.GetAssignedQuizzesByUser(userId);


            // Sort by AssignedOn descending (latest first)
            var sortedAssignments = assignments
                .OrderByDescending(a => a.AssignedOn)
                .ToList();

            return View("AssignedQuizzes", sortedAssignments);

        }

        // View result for a specific attempt (without correct answers)
        // View result for a specific attempt (without correct answers)
        //public ActionResult QuizHistoryResult(Guid attemptId)
        //{
        //    // ✅ Use responseRepo instead of non-existent userRepo
        //    var historyResult = responseRepo.GetQuizHistoryResultByAttemptId(attemptId);
        //    if (historyResult == null)
        //        return HttpNotFound();

        //    // Map to QuizResultModel
        //    var resultModel = new QuizResultModel
        //    {
        //        TotalQuestions = historyResult.TotalQuestions,
        //        CorrectAnswers = historyResult.CorrectAnswers,
        //        Score = historyResult.Score,
        //        ResultDetails = null // we don’t want to show Q&A in this styled view
        //    };

        //    // Store QuizId in ViewBag for Retake button
        //    ViewBag.QuizId = historyResult.QuizId;

        //    // Return the styled QuizResult view
        //    return View("QuizResult", resultModel);
        //}


    }
}

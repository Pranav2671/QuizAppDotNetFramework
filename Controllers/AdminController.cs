using QuizAppDotNetFramework.Models;
using QuizAppDotNetFramework.Repository;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuizAppDotNetFramework.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuizRepository quizRepo;
        private readonly QuestionRepository questionRepo;
        private readonly UserRepository userRepo;
        private readonly AssignedQuizRepository assignedQuizRepo;

        public AdminController()
        {
            quizRepo = new QuizRepository();
            questionRepo = new QuestionRepository();
            userRepo = new UserRepository();
            assignedQuizRepo = new AssignedQuizRepository();
        }

        // ----------------- ADMIN DASHBOARD -----------------
        public ActionResult Index()
        {
            return View();
        }

        // ----------------- USER MANAGEMENT -----------------
        [HttpGet]
        public ActionResult ManageUsers()
        {
            var users = userRepo.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public ActionResult DeleteUser(Guid id)
        {
            try
            {
                userRepo.DeleteUser(id);
                TempData["Success"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete user: " + ex.Message;
            }
            return RedirectToAction("ManageUsers");
        }

        // ----------------- QUIZ MANAGEMENT -----------------
        public ActionResult ManageQuizzes()
        {
            var quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        public ActionResult AddQuiz() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuiz(QuizModel quiz)
        {
            if (ModelState.IsValid)
            {
                quiz.QuizId = Guid.NewGuid();
                quiz.CreatedDate = DateTime.Now;

                var userId = (Guid)Session["UserId"];
                var username = (string)Session["Username"];

                quiz.CreatedBy = userId;
                quiz.CreatedByUsername = username;

                quizRepo.AddQuiz(quiz);
                return RedirectToAction("ManageQuizzes");
            }
            return View(quiz);
        }

        [HttpGet]
        public ActionResult DeleteQuiz(Guid id)
        {
            try
            {
                quizRepo.DeleteQuiz(id);
                TempData["Success"] = "Quiz deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete quiz: " + ex.Message;
            }
            return RedirectToAction("ManageQuizzes");
        }

        // ----------------- QUESTION MANAGEMENT -----------------
        public ActionResult ManageQuestions(Guid quizId)
        {
            var questions = questionRepo.GetQuestionsByQuizId(quizId);
            ViewBag.QuizId = quizId;
            return View(questions);
        }

        [HttpGet]
        public ActionResult AddQuestion(Guid quizId) => View(new QuestionModel { QuizId = quizId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                questionRepo.AddQuestion(model);
                return RedirectToAction("ManageQuestions", new { quizId = model.QuizId });
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult EditQuestion(Guid id, Guid quizId)
        {
            var question = questionRepo.GetQuestionById(id);
            if (question == null) return HttpNotFound();
            question.QuizId = quizId;
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestion(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                questionRepo.UpdateQuestion(model);
                return RedirectToAction("ManageQuestions", new { quizId = model.QuizId });
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteQuestion(Guid id, Guid quizId)
        {
            questionRepo.DeleteQuestion(id);
            return RedirectToAction("ManageQuestions", new { quizId });
        }

        // ----------------- ASSIGN QUIZZES -----------------
        [HttpGet]
        public ActionResult AssignQuiz()
        {
            ViewBag.Quizzes = new SelectList(quizRepo.GetAllQuizzes(), "QuizId", "Title");
            ViewBag.Users = new SelectList(userRepo.GetAllUsers(), "UserId", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignQuiz(Guid quizId, Guid userId, DateTime dueDate)
        {
            assignedQuizRepo.AssignQuiz(quizId, userId, dueDate);
            TempData["SuccessMessage"] = "Quiz assigned successfully!";
            return RedirectToAction("AssignQuiz");
        }

        [HttpGet]
        public ActionResult ViewAssignedQuizzes()
        {
            var assignments = assignedQuizRepo.GetAllAssignedQuizzesWithScore()
                                .OrderByDescending(a => a.AssignedOn) // latest first
                                .ToList();
            return View("AssignedQuizzes", assignments);
        }

        // Edit Assignment
        // GET: Edit Assignment
        public ActionResult EditAssignment(Guid id)
        {
            var assignment = assignedQuizRepo.GetAssignmentById(id);
            if (assignment == null) return HttpNotFound();
            return View(assignment);
        }

        // POST: Edit Assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssignment(AssignedQuizModel model)
        {
            if (ModelState.IsValid)
            {
                bool updated = assignedQuizRepo.UpdateAssignment(model);
                if (updated)
                {
                    TempData["SuccessMessage"] = "Assignment updated successfully!";
                    return RedirectToAction("ViewAssignedQuizzes");
                }
                ModelState.AddModelError("", "Failed to update assignment. Please try again.");
            }
            return View(model);
        }
        // Delete Assignment
        public ActionResult DeleteAssignment(Guid id)
        {
            var assignment = assignedQuizRepo.GetAssignmentById(id);
            if (assignment == null) return HttpNotFound();

            bool deleted = assignedQuizRepo.DeleteAssignment(id);
            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] =
                deleted ? "Assignment deleted successfully!" : "Failed to delete assignment.";

            return RedirectToAction("ViewAssignedQuizzes");
        } 

    }
}

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
            // Single user dropdown
            ViewBag.Users = new SelectList(userRepo.GetAllUsers(), "UserId", "Username");

            // Build ViewModel for multi-quiz selection
            var model = new AssignQuizViewModel
            {
                Users = userRepo.GetAllUsers(),
                Quizzes = quizRepo.GetAllQuizzes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignQuiz(AssignQuizViewModel model)
        {
            if (model.SelectedUserId == Guid.Empty || model.SelectedQuizIds == null || !model.SelectedQuizIds.Any())
            {
                TempData["Error"] = "Please select a user and at least one quiz.";
                return RedirectToAction("AssignQuiz");
            }

            try
            {
                foreach (var quizId in model.SelectedQuizIds)
                {
                    // Avoid duplicate assignments
                    if (!assignedQuizRepo.AssignmentExists(model.SelectedUserId, quizId))
                    {
                        assignedQuizRepo.AddAssignment(new AssignedQuizModel
                        {
                            AssignmentId = Guid.NewGuid(),
                            UserId = model.SelectedUserId,
                            QuizId = quizId,
                            AssignedOn = DateTime.Now,
                            DueDate = model.DueDate,
                            IsCompleted = false
                        });
                    }
                }

                TempData["Success"] = "Quizzes assigned successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error assigning quizzes: " + ex.Message;
            }

            return RedirectToAction("AssignQuiz");
        }




        [HttpGet]
        public ActionResult ViewAssignedQuizzes()
        {
            var assignments = assignedQuizRepo.GetAllAssignedQuizzesWithScore()
                                .OrderByDescending(a => a.AssignedOn)
                                .ToList();
            return View("AssignedQuizzes", assignments);
        }

        [HttpGet]
        public ActionResult EditAssignment(Guid id)
        {
            var assignment = assignedQuizRepo.GetAssignmentById(id);
            if (assignment == null)
                return HttpNotFound();

            // Load dropdown data using actual model properties
            var users = userRepo.GetAllUsers();
            var quizzes = quizRepo.GetAllQuizzes();

            ViewBag.Users = new SelectList(users, "UserId", "Username", assignment.UserId);
            ViewBag.Quizzes = new SelectList(quizzes, "QuizId", "Title", assignment.QuizId);

            return View(assignment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssignment(AssignedQuizModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = assignedQuizRepo.UpdateAssignment(model);
                if (success)
                {
                    TempData["Success"] = "Assignment updated successfully!";
                    return RedirectToAction("ViewAssignedQuizzes");
                }
                else
                {
                    TempData["Error"] = "Failed to update assignment.";
                }
            }

            // Reload dropdowns in case of validation error
            var users = userRepo.GetAllUsers();
            var quizzes = quizRepo.GetAllQuizzes();

            ViewBag.Users = new SelectList(users, "UserId", "Username", model.UserId);
            ViewBag.Quizzes = new SelectList(quizzes, "QuizId", "Title", model.QuizId);

            return View(model);
        }


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

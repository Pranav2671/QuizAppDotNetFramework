using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Repository;
using QuizAppDotNetFramework.Models;

namespace QuizAppDotNetFramework.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuizRepository quizRepo;
        private readonly QuestionRepository questionRepo;
        private readonly UserRepository userRepo;
        private readonly AssignedQuizRepository assignedQuizRepo = new AssignedQuizRepository();

        public AdminController()
        {
            quizRepo = new QuizRepository();
            questionRepo = new QuestionRepository();
            userRepo = new UserRepository(); // <-- fixed null reference
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
            var users = userRepo.GetAllUsers(); // now properly initialized
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

        // GET: Admin/AddQuiz
        public ActionResult AddQuiz()
        {
            return View();
        }

        // POST: Admin/AddQuiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuiz(QuizModel quiz)
        {
            if (ModelState.IsValid)
            {
                quiz.QuizId = Guid.NewGuid();
                quiz.CreatedDate = DateTime.Now;

                // Get current logged-in admin details from session
                var userId = (Guid)Session["UserId"];
                var username = (string)Session["Username"];

                quiz.CreatedBy = userId;
                quiz.CreatedByUsername = username;

                quizRepo.AddQuiz(quiz); // use already initialized repo

                return RedirectToAction("ManageQuizzes");
            }

            return View(quiz);
        }

        [HttpGet]
        public ActionResult DeleteQuiz(Guid id)
        {
            try
            {
                quizRepo.DeleteQuiz(id); // use already initialized repo
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
        public ActionResult AddQuestion(Guid quizId)
        {
            var model = new QuestionModel { QuizId = quizId };
            return View(model);
        }

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

        // GET: Admin/EditQuestion
        [HttpGet]
        public ActionResult EditQuestion(Guid id, Guid quizId)
        {
            var question = questionRepo.GetQuestionById(id); // we will create this method in repo
            if (question == null)
                return HttpNotFound();

            question.QuizId = quizId; // keep the quizId for redirect
            return View(question);
        }

        // POST: Admin/EditQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestion(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                questionRepo.UpdateQuestion(model); // we will create this method in repo
                return RedirectToAction("ManageQuestions", new { quizId = model.QuizId });
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteQuestion(Guid id, Guid quizId)
        {
            questionRepo.DeleteQuestion(id);
            return RedirectToAction("ManageQuestions", new { quizId = quizId });
        }

        //Assingn Quiz
        // GET: Admin/AssignQuiz
        // GET: Admin/AssignQuiz
        public ActionResult AssignQuiz()
        {
            var quizzes = quizRepo.GetAllQuizzes();
            var users = userRepo.GetAllUsers();

            ViewBag.Quizzes = new SelectList(quizzes, "QuizId", "Title");
            ViewBag.Users = new SelectList(users, "UserId", "Username");

            return View();
        }

        // POST: Handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignQuiz(Guid quizId, Guid userId, DateTime dueDate)
        {
            assignedQuizRepo.AssignQuiz(quizId, userId, dueDate);
            TempData["SuccessMessage"] = "Quiz assigned successfully!";
            return RedirectToAction("AssignQuiz");
        }

        public ActionResult ViewAssignedQuizzes()
        {
            var assignments = assignedQuizRepo.GetAllAssignedQuizzes();
            return View("AssignedQuizzes", assignments);
        }







    }

}


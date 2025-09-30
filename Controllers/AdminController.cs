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

        public AdminController()
        {
            quizRepo = new QuizRepository();
            questionRepo = new QuestionRepository();
        }

        public ActionResult Index()
        {
            return View();
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

                // Get current logged-in admin details
                var userId = (Guid)Session["UserId"];
                var username = (string)Session["Username"];

                quiz.CreatedBy = userId;
                quiz.CreatedByUsername = username;

                QuizRepository quizRepo = new QuizRepository();
                quizRepo.AddQuiz(quiz);

                return RedirectToAction("ManageQuizzes");
            }

            return View(quiz);
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

        [HttpGet]
        public ActionResult DeleteQuestion(Guid id, Guid quizId)
        {
            questionRepo.DeleteQuestion(id);
            return RedirectToAction("ManageQuestions", new { quizId = quizId });
        }
    }
}

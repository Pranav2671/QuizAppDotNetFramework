using System;
using System.Web.Mvc;
using QuizAppDotNetFramework.Repository;
using QuizAppDotNetFramework.Models;

namespace QuizAppDotNetFramework.Controllers
{
    public class AdminController : Controller
    {
        private QuizRepository _quizRepository = new QuizRepository();
        private UserRepository _userRepository = new UserRepository();

        // -------------------- Admin Dashboard --------------------
        public ActionResult Index()
        {
            return View(); // Shows Admin Dashboard
        }





        // -------------------- Quiz Management --------------------
        public ActionResult ManageQuizzes()
        {
            var quizzes = _quizRepository.GetAllQuizzes();
            return View(quizzes);
        }

        // GET: Show Add Quiz form
        public ActionResult AddQuiz()
        {
            QuizModel model = new QuizModel();
            return View(model);
        }

        // POST: Save new quiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuiz(QuizModel model)
        {
            if (ModelState.IsValid)
            {
                model.QuizId = Guid.NewGuid();              // Generate new quiz ID
                model.CreatedDate = DateTime.Now;           // Set current date

                // Set CreatedBy from logged-in admin
                if (Session["UserId"] != null)
                {
                    model.CreatedBy = new Guid(Session["UserId"].ToString());
                }
                else
                {
                    // Optional: fallback to a default admin ID if session is null
                    return RedirectToAction("ManageQuizzes"); // or show error
                }

                _quizRepository.AddQuiz(model);             // Insert into database
                return RedirectToAction("ManageQuizzes");   // Go back to quiz list
            }

            return View(model); // Return view if validation fails
        }



        public ActionResult DeleteQuiz(Guid id)
        {
            // Get all questions for this quiz
            var questions = _quizRepository.GetQuestionsByQuizId(id);

            // Delete each question
            foreach (var q in questions)
            {
                _quizRepository.DeleteQuestion(q.QuestionId);
            }

            // Now delete the quiz itself
            _quizRepository.DeleteQuiz(id);

            // Redirect back to Manage Quizzes page
            return RedirectToAction("ManageQuizzes");
        }


        // GET: Show quiz data in edit form
        public ActionResult EditQuiz(Guid id)
        {
            var quiz = _quizRepository.GetQuizById(id);
            if (quiz == null)
                return HttpNotFound();

            return View(quiz); // Pass quiz data to view
        }

        // POST: Save edited quiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuiz(QuizModel model)
        {
            if (ModelState.IsValid)
            {
                _quizRepository.UpdateQuiz(model);
                return RedirectToAction("ManageQuizzes");
            }
            return View(model); // Return view with errors if validation fails
        }






        // -------------------- Question Management --------------------
        public ActionResult ManageQuestions(Guid quizId)
        {
            var questions = _quizRepository.GetQuestionsByQuizId(quizId);
            ViewBag.QuizId = quizId;
            return View(questions);
        }

        public ActionResult DeleteQuestion(Guid id, Guid quizId)
        {
            _quizRepository.DeleteQuestion(id);
            return RedirectToAction("ManageQuestions", new { quizId = quizId });
        }




        // -------------------- User Management --------------------
        public ActionResult ManageUsers()
        {
            var users = _userRepository.GetAllUsers();
            return View(users);
        }

        public ActionResult DeleteUser(Guid id)
        {
            _userRepository.DeleteUser(id);
            return RedirectToAction("ManageUsers");
        }
        //public ActionResult Logout()
        //{
        //    Session.Clear(); // Clear all session data
        //    return RedirectToAction("Login", "Auth"); // Redirect back to login page
        //}
    }
}

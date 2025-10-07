using System;
using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class AssignQuizViewModel
    {
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<QuizModel> Quizzes { get; set; } = new List<QuizModel>();

        // For multi-selection binding in form
        public Guid SelectedUserId { get; set; }  // single user
        public List<Guid> SelectedQuizIds { get; set; } = new List<Guid>();

        public DateTime DueDate { get; set; }
    }
}

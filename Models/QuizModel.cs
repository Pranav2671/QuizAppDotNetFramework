using System;
using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class QuizModel
    {
        public Guid QuizId { get; set; }
        public string Title { get; set; }
        public Guid CreatedBy { get; set; }          // UserId (FK)
        public string CreatedByUsername { get; set; } // Comes from JOIN
        public DateTime CreatedDate { get; set; }



        // List of questions in this quiz
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}

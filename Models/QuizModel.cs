using System;
using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class QuizModel
    {
        public Guid QuizId { get; set; }             // Unique ID of the quiz
        public string Title { get; set; }            // Quiz title
        public Guid CreatedBy { get; set; }          // Foreign key: UserId of creator
        public DateTime CreatedDate { get; set; }    // Date when quiz was created

        public string CreatedByUsername { get; set; } // NEW: Store creator's username for display

        // List of questions in this quiz
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}

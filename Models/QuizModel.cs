using System;
using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class QuizModel
    {
        public Guid QuizId { get; set; }
        public string Title { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Questions for this quiz
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}

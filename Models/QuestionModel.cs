using System;
using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class QuestionModel
    {
        public Guid QuestionId { get; set; }
        public Guid QuizId { get; set; }
        public string QuestionText { get; set; }

        // THIS MUST BE ADDED
        public List<OptionModel> Options { get; set; } = new List<OptionModel>();
    }
}

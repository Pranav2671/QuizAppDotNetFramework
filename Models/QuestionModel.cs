using System;

namespace QuizAppDotNetFramework.Models
{
    public class QuestionModel
    {
        public Guid QuestionId { get; set; }
        public Guid QuizId { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
    }
}

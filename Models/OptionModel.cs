using System;

namespace QuizAppDotNetFramework.Models
{
    public class OptionModel
    {
        public Guid OptionId { get; set; }
        public Guid QuestionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}

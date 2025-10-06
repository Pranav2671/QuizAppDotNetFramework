using System;

namespace QuizAppDotNetFramework.Models
{
    public class UserQuizHistoryModel
    {
        public Guid QuizId { get; set; }
        public Guid AttemptId { get; set; } // track unique attempt
        public string QuizTitle { get; set; }
        public DateTime AttemptedOn { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int Score { get; set; } // percentage
    }
}

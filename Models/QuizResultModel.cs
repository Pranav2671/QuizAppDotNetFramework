using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class QuizResultModel
    {
        public int TotalQuestions { get; set; }                  // Total questions in the quiz
        public int CorrectAnswers { get; set; }                  // Number of correct answers
        public int Score { get; set; }                           // Percentage score
        public List<QuizResultDetail> ResultDetails { get; set; } = new List<QuizResultDetail>(); // Details for each question
    }
}

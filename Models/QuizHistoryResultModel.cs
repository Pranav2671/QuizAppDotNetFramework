using System.Collections.Generic;

namespace QuizAppDotNetFramework.Models
{
    public class QuizHistoryResultModel
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int ScorePercentage { get; set; }
        public List<QuizHistoryResultDetail> ResultDetails { get; set; } = new List<QuizHistoryResultDetail>();
    }

    public class QuizHistoryResultDetail
    {
        public string QuestionText { get; set; }
        public string YourAnswer { get; set; }
    }
}

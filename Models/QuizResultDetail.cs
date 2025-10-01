using System;
namespace QuizAppDotNetFramework.Models
{
    public class QuizResultDetail
    {
        public string QuestionText { get; set; }   // The question itself
        public string CorrectAnswer { get; set; }  // Correct answer
        public string YourAnswer { get; set; }     // Answer chosen by the user
        public bool IsCorrect { get; set; }        // True if the user was correct
    }
}

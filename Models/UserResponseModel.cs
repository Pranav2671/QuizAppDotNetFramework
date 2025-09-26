using System;

namespace QuizApp.Models
{
    public class UserResponseModel
    {
        public Guid ResponseId { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid SelectedOptionId { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}

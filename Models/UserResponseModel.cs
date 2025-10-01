using System;

public class UserResponseModel
{
    public Guid ResponseId { get; set; }
    public Guid UserId { get; set; }
    public Guid QuizId { get; set; }
    public Guid QuestionId { get; set; }
    public string SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime ResponseDate { get; set; }

    // Add this property
    public Guid AttemptId { get; set; }

    // Optional for display
    public string QuestionText { get; set; }
    public string CorrectOption { get; set; }
}

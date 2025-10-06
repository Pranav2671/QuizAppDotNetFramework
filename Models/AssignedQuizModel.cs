using System;

namespace QuizAppDotNetFramework.Models
{
	public class AssignedQuizModel
	{
		public Guid AssignmentId { get; set; }
		public Guid QuizId { get; set; }
		public string QuizTitle { get; set; }
		public Guid UserId { get; set; }
		public string Username { get; set; }
		public DateTime AssignedOn { get; set; }
		public DateTime DueDate { get; set; }
		public bool IsCompleted { get; set; }
		public int? Score { get; set; }  // nullable for incomplete quizzes
		public Guid? AttemptId { get; set; } // optional
	}
}

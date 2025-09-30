using System;

namespace QuizAppDotNetFramework.Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

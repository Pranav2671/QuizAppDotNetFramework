using QuizAppDotNetFramework.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuizAppDotNetFramework.Repository
{
    public class AssignedQuizRepository
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // Assign quiz to a user
        public void AssignQuiz(Guid quizId, Guid userId, DateTime dueDate)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AssignQuizToUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get all assigned quizzes (for admin)
        public List<AssignedQuizModel> GetAllAssignedQuizzes()
        {
            var list = new List<AssignedQuizModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetAllAssignedQuizzes", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new AssignedQuizModel
                        {
                            AssignmentId = (Guid)dr["AssignmentId"],
                            QuizId = (Guid)dr["QuizId"],
                            Username = dr["Username"].ToString(),
                            QuizTitle = dr["QuizTitle"].ToString(),
                            AssignedOn = (DateTime)dr["AssignedOn"],
                            DueDate = (DateTime)dr["DueDate"],
                            IsCompleted = (bool)dr["IsCompleted"],
                            Score = dr["Score"] != DBNull.Value ? (int)dr["Score"] : 0,
                            AttemptId = dr["AttemptId"] != DBNull.Value ? (Guid?)dr["AttemptId"] : null
                        });
                    }
                }
            }

            return list;
        }


        // Get assigned quizzes by user
        public List<AssignedQuizModel> GetAssignedQuizzesByUser(Guid userId)
        {
            var list = new List<AssignedQuizModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAssignedQuizzesByUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new AssignedQuizModel
                    {
                        AssignmentId = (Guid)dr["AssignmentId"],
                        QuizId = (Guid)dr["QuizId"],
                        QuizTitle = dr["QuizTitle"].ToString(),
                        AssignedOn = (DateTime)dr["AssignedOn"],
                        DueDate = (DateTime)dr["DueDate"],
                        IsCompleted = (bool)dr["IsCompleted"],
                        Score = dr["Score"] != DBNull.Value ? (int)dr["Score"] : 0
                    });

                }
            }
            return list;
        }

        // Mark assigned quiz as completed
        public void MarkAsCompleted(Guid assignmentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_MarkAssignedQuizCompleted", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarkAsCompletedWithScore(Guid userId, Guid quizId, int score)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_MarkAssignedQuizCompletedWithScore", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                cmd.Parameters.AddWithValue("@Score", score);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public List<AssignedQuizModel> GetAllAssignedQuizzesWithScore()
        {
            var list = new List<AssignedQuizModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllAssignedQuizzesWithScore", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new AssignedQuizModel
                    {
                        AssignmentId = (Guid)dr["AssignmentId"],
                        QuizId = (Guid)dr["QuizId"],
                        QuizTitle = dr["QuizTitle"].ToString(),
                        UserId = (Guid)dr["UserId"],
                        Username = dr["Username"].ToString(),
                        AssignedOn = (DateTime)dr["AssignedOn"],
                        DueDate = (DateTime)dr["DueDate"],
                        IsCompleted = (bool)dr["IsCompleted"],
                        Score = dr["Score"] != DBNull.Value ? (int)dr["Score"] : 0,
                        AttemptId = dr["AttemptId"] != DBNull.Value ? (Guid?)dr["AttemptId"] : null
                    });

                }
                return list;
            }
        }

            public void UpdateAssignmentAfterAttempt(Guid userId, Guid quizId, Guid attemptId, int score)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateAssignedQuizAfterAttempt", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@QuizId", quizId);
                    cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                    cmd.Parameters.AddWithValue("@Score", score);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}


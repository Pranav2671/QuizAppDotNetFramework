using QuizAppDotNetFramework.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace QuizAppDotNetFramework.Repository
{
    public class UserResponseRepository
    {
        // Only one connection string, private
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // Add a single response
        public void AddResponse(UserResponseModel response)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddUserResponse", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ResponseId", response.ResponseId);
                cmd.Parameters.AddWithValue("@UserId", response.UserId);
                cmd.Parameters.AddWithValue("@QuizId", response.QuizId);
                cmd.Parameters.AddWithValue("@QuestionId", response.QuestionId);
                // If user did not answer, store "Not Attempted"
                cmd.Parameters.AddWithValue("@SelectedOption", string.IsNullOrEmpty(response.SelectedOption) ? "Not Attempted" : response.SelectedOption);
                cmd.Parameters.AddWithValue("@IsCorrect", response.IsCorrect);
                cmd.Parameters.AddWithValue("@ResponseDate", response.ResponseDate);
                cmd.Parameters.AddWithValue("@AttemptId", response.AttemptId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get quiz history summary for a user
        public List<UserQuizHistoryModel> GetQuizHistoryForUser(Guid userId)
        {
            var history = new List<UserQuizHistoryModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetQuizHistoryForUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    history.Add(new UserQuizHistoryModel
                    {
                        QuizId = (Guid)reader["QuizId"],
                        AttemptId = (Guid)reader["AttemptId"],
                        QuizTitle = reader["QuizTitle"].ToString(),
                        AttemptedOn = Convert.ToDateTime(reader["AttemptedOn"]),
                        TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                        CorrectAnswers = Convert.ToInt32(reader["CorrectAnswers"]),
                        Score = Convert.ToInt32(reader["Score"])
                    });
                }
            }

            return history;
        }

        // Get responses for a specific attempt
        public List<UserResponseModel> GetUserResponsesByAttempt(Guid attemptId)
        {
            var responses = new List<UserResponseModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetUserResponsesByAttempt", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new UserResponseModel
                    {
                        QuestionText = reader["QuestionText"].ToString(),
                        SelectedOption = reader["SelectedOption"].ToString(),
                        CorrectOption = reader["CorrectOption"].ToString(),
                        IsCorrect = Convert.ToBoolean(reader["IsCorrect"])
                    });
                }
            }

            return responses;
        }

        // Delete an attempt
        public void DeleteAttempt(Guid attemptId, Guid userId)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_DeleteUserResponsesByAttempt", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                cmd.Parameters.AddWithValue("@UserId", userId); // Pass the required UserId
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }




    }
}

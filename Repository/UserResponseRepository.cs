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
        private string connectionString = ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // Add a response
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
                cmd.Parameters.AddWithValue("@SelectedOption", response.SelectedOption);
                cmd.Parameters.AddWithValue("@IsCorrect", response.IsCorrect);
                cmd.Parameters.AddWithValue("@ResponseDate", response.ResponseDate);
                cmd.Parameters.AddWithValue("@AttemptId", response.AttemptId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get all responses by attempt
        public List<UserResponseModel> GetUserResponsesByAttempt(Guid attemptId)
        {
            List<UserResponseModel> responses = new List<UserResponseModel>();
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
                        ResponseId = (Guid)reader["ResponseId"],
                        UserId = (Guid)reader["UserId"],
                        QuizId = (Guid)reader["QuizId"],
                        QuestionId = (Guid)reader["QuestionId"],
                        SelectedOption = reader["SelectedOption"].ToString(),
                        IsCorrect = (bool)reader["IsCorrect"],
                        ResponseDate = Convert.ToDateTime(reader["ResponseDate"]),
                        QuestionText = reader["QuestionText"].ToString(),
                        CorrectOption = reader["CorrectOption"].ToString(),
                        AttemptId = (Guid)reader["AttemptId"]
                    });
                }
            }
            return responses;
        }

        // Get quiz history grouped by AttemptId
        public List<UserQuizHistoryModel> GetQuizHistoryForUser(Guid userId)
        {
            List<UserQuizHistoryModel> history = new List<UserQuizHistoryModel>();
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
                        AttemptId = (Guid)reader["AttemptId"],
                        QuizId = (Guid)reader["QuizId"],
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

        public List<UserResponseModel> GetUserResponsesForQuiz(Guid userId, Guid quizId)
        {
            List<UserResponseModel> responses = new List<UserResponseModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetUserResponsesForQuiz", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    responses.Add(new UserResponseModel
                    {
                        ResponseId = (Guid)reader["ResponseId"],
                        UserId = (Guid)reader["UserId"],
                        QuizId = (Guid)reader["QuizId"],
                        QuestionId = (Guid)reader["QuestionId"],
                        SelectedOption = reader["SelectedOption"].ToString(),
                        IsCorrect = (bool)reader["IsCorrect"],
                        ResponseDate = Convert.ToDateTime(reader["ResponseDate"]),
                        QuestionText = reader["QuestionText"].ToString(),
                        CorrectOption = reader["CorrectOption"].ToString()
                    });
                }
            }
            return responses;
        }

        public void DeleteAttempt(Guid attemptId, Guid userId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteUserResponsesByAttempt", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using QuizAppDotNetFramework.Models;

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

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get all responses of a user for a quiz
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
    }
}

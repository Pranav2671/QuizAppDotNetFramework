using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QuizAppDotNetFramework.Models;

namespace QuizAppDotNetFramework.Repository
{
    public class QuestionRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // Get all questions for a specific quiz
        public List<QuestionModel> GetQuestionsByQuizId(Guid quizId)
        {
            List<QuestionModel> questions = new List<QuestionModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetQuestionsByQuizId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quizId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    questions.Add(new QuestionModel
                    {
                        QuestionId = (Guid)reader["QuestionId"],
                        QuizId = (Guid)reader["QuizId"],
                        QuestionText = reader["QuestionText"].ToString(),
                        OptionA = reader["OptionA"].ToString(),
                        OptionB = reader["OptionB"].ToString(),
                        OptionC = reader["OptionC"].ToString(),
                        OptionD = reader["OptionD"].ToString(),
                        CorrectOption = reader["CorrectOption"].ToString()
                    });
                }
            }

            return questions;
        }

        // Add a new question
        public void AddQuestion(QuestionModel question)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@QuestionId", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@QuizId", question.QuizId);
                cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                cmd.Parameters.AddWithValue("@OptionA", question.OptionA);
                cmd.Parameters.AddWithValue("@OptionB", question.OptionB);
                cmd.Parameters.AddWithValue("@OptionC", question.OptionC);
                cmd.Parameters.AddWithValue("@OptionD", question.OptionD);
                cmd.Parameters.AddWithValue("@CorrectOption", question.CorrectOption);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        // Get single question by ID
        public QuestionModel GetQuestionById(Guid questionId)
        {
            QuestionModel question = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetQuestionById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionId", questionId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    question = new QuestionModel
                    {
                        QuestionId = (Guid)reader["QuestionId"],
                        QuizId = (Guid)reader["QuizId"],
                        QuestionText = reader["QuestionText"].ToString(),
                        OptionA = reader["OptionA"].ToString(),
                        OptionB = reader["OptionB"].ToString(),
                        OptionC = reader["OptionC"].ToString(),
                        OptionD = reader["OptionD"].ToString(),
                        CorrectOption = reader["CorrectOption"].ToString()
                    };
                }
            }
            return question;
        }

        // Update question
        public void UpdateQuestion(QuestionModel question)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                cmd.Parameters.AddWithValue("@OptionA", question.OptionA);
                cmd.Parameters.AddWithValue("@OptionB", question.OptionB);
                cmd.Parameters.AddWithValue("@OptionC", question.OptionC);
                cmd.Parameters.AddWithValue("@OptionD", question.OptionD);
                cmd.Parameters.AddWithValue("@CorrectOption", question.CorrectOption);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a question by QuestionId
        public void DeleteQuestion(Guid questionId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionId", questionId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

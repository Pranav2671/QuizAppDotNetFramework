using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QuizAppDotNetFramework.Models;

namespace QuizAppDotNetFramework.Repository
{
    public class QuizRepository
    {
        // Database connection string
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // ===================== QUIZ METHODS =====================

        // Get all quizzes with creator username
        public List<QuizModel> GetAllQuizzes()
        {
            List<QuizModel> quizzes = new List<QuizModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllQuizzes", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    quizzes.Add(new QuizModel
                    {
                        QuizId = (Guid)reader["QuizId"],
                        Title = reader["Title"].ToString(),
                        CreatedBy = (Guid)reader["CreatedBy"],
                        CreatedByUsername = reader["CreatedByUsername"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }

            return quizzes;
        }


        // Get quiz by ID
        public QuizModel GetQuizById(Guid quizId)
        {
            QuizModel quiz = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetQuizById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quizId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    quiz = new QuizModel
                    {
                        QuizId = reader.GetGuid(reader.GetOrdinal("QuizId")),
                        Title = reader["Title"].ToString(),
                        CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    };
                }
            }

            return quiz; // returns null if not found
        }

        // Add a new quiz
        public void AddQuiz(QuizModel model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddQuiz", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", model.QuizId);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDate", model.CreatedDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Update an existing quiz
        public void UpdateQuiz(QuizModel model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateQuiz", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", model.QuizId);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDate", model.CreatedDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a quiz
        public void DeleteQuiz(Guid quizId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteQuiz", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quizId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // ===================== QUESTION METHODS =====================

        // Get all questions for a quiz
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
                        QuestionId = reader.GetGuid(reader.GetOrdinal("QuestionId")),
                        QuizId = reader.GetGuid(reader.GetOrdinal("QuizId")),
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

        // Add a question
        public void AddQuestion(QuestionModel model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionId", model.QuestionId);
                cmd.Parameters.AddWithValue("@QuizId", model.QuizId);
                cmd.Parameters.AddWithValue("@QuestionText", model.QuestionText);
                cmd.Parameters.AddWithValue("@OptionA", model.OptionA);
                cmd.Parameters.AddWithValue("@OptionB", model.OptionB);
                cmd.Parameters.AddWithValue("@OptionC", model.OptionC);
                cmd.Parameters.AddWithValue("@OptionD", model.OptionD);
                cmd.Parameters.AddWithValue("@CorrectOption", model.CorrectOption);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Update a question
        public void UpdateQuestion(QuestionModel model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionId", model.QuestionId);
                cmd.Parameters.AddWithValue("@QuestionText", model.QuestionText);
                cmd.Parameters.AddWithValue("@OptionA", model.OptionA);
                cmd.Parameters.AddWithValue("@OptionB", model.OptionB);
                cmd.Parameters.AddWithValue("@OptionC", model.OptionC);
                cmd.Parameters.AddWithValue("@OptionD", model.OptionD);
                cmd.Parameters.AddWithValue("@CorrectOption", model.CorrectOption);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a question
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

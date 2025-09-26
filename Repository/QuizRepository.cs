using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using QuizAppDotNetFramework.Models;

namespace QuizAppDotNetFramework.Repository
{
    public class QuizRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // Get all quizzes
        public List<QuizModel> GetAllQuizzes()
        {
            List<QuizModel> quizzes = new List<QuizModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Quizzes", con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quizzes.Add(new QuizModel
                            {
                                QuizId = (Guid)reader["QuizId"],
                                Title = reader["Title"].ToString(),
                                CreatedBy = (Guid)reader["CreatedBy"],
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                Questions = new List<QuestionModel>() // initialize empty
                            });
                        }
                    }
                }
            }

            return quizzes;
        }

        // Get a single quiz with questions and options
        public QuizModel GetQuizById(Guid quizId)
        {
            QuizModel quiz = new QuizModel();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Quiz details
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Quizzes WHERE QuizId=@QuizId", con))
                {
                    cmd.Parameters.AddWithValue("@QuizId", quizId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quiz.QuizId = (Guid)reader["QuizId"];
                            quiz.Title = reader["Title"].ToString();
                            quiz.CreatedBy = (Guid)reader["CreatedBy"];
                            quiz.CreatedDate = (DateTime)reader["CreatedDate"];
                        }
                    }
                }

                // Questions
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Questions WHERE QuizId=@QuizId", con))
                {
                    cmd.Parameters.AddWithValue("@QuizId", quizId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        quiz.Questions = new List<QuestionModel>();
                        while (reader.Read())
                        {
                            quiz.Questions.Add(new QuestionModel
                            {
                                QuestionId = (Guid)reader["QuestionId"],
                                QuizId = (Guid)reader["QuizId"],
                                QuestionText = reader["QuestionText"].ToString(),
                                Options = new List<OptionModel>() // initialize empty
                            });
                        }
                    }
                }

                // Options for each question
                foreach (var question in quiz.Questions)
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Options WHERE QuestionId=@QuestionId", con))
                    {
                        cmd.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                question.Options.Add(new OptionModel
                                {
                                    OptionId = (Guid)reader["OptionId"],
                                    QuestionId = (Guid)reader["QuestionId"],
                                    OptionText = reader["OptionText"].ToString(),
                                    IsCorrect = (bool)reader["IsCorrect"]
                                });
                            }
                        }
                    }
                }
            }

            return quiz;
        }

        // Add quiz using JSON stored procedure
        public void AddQuiz(QuizModel quiz)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddQuiz", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    string jsonData = JsonConvert.SerializeObject(quiz);
                    cmd.Parameters.AddWithValue("@JsonData", jsonData);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Update quiz using JSON stored procedure
        public void UpdateQuiz(QuizModel quiz)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateQuiz", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    string jsonData = JsonConvert.SerializeObject(quiz);
                    cmd.Parameters.AddWithValue("@JsonData", jsonData);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete quiz
        public void DeleteQuiz(Guid quizId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteQuiz", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QuizId", quizId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

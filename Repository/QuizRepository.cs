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

                quiz = new QuizModel();
                quiz.Questions = new List<QuestionModel>();

                while (reader.Read())
                {
                    if (quiz.QuizId == Guid.Empty)
                    {
                        quiz.QuizId = reader.GetGuid(reader.GetOrdinal("QuizId"));
                        quiz.Title = reader.GetString(reader.GetOrdinal("Title"));
                        quiz.CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy"));
                        quiz.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                    }

                    // Read questions
                    Guid questionId = reader.GetGuid(reader.GetOrdinal("QuestionId"));
                    QuestionModel question = quiz.Questions.Find(q => q.QuestionId == questionId);
                    if (question == null)
                    {
                        question = new QuestionModel
                        {
                            QuestionId = questionId,
                            QuizId = quiz.QuizId,
                            QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                            Options = new List<OptionModel>()
                        };
                        quiz.Questions.Add(question);
                    }

                    // Read options
                    if (!reader.IsDBNull(reader.GetOrdinal("OptionId")))
                    {
                        OptionModel option = new OptionModel
                        {
                            OptionId = reader.GetGuid(reader.GetOrdinal("OptionId")),
                            QuestionId = questionId,
                            OptionText = reader.GetString(reader.GetOrdinal("OptionText")),
                            IsCorrect = reader.GetBoolean(reader.GetOrdinal("IsCorrect"))
                        };
                        question.Options.Add(option);
                    }
                }
                con.Close();
            }
            return quiz;
        }

        // Update quiz using JSON
        public void UpdateQuiz(QuizModel quiz)
        {
            string jsonData = JsonConvert.SerializeObject(quiz);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateQuiz", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JsonData", jsonData);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}

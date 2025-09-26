using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Configuration;

namespace QuizAppDotNetFramework.Repository
{
    public class AuthRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QuizDbConnection"].ConnectionString;

        // Login method (already exists)
        public DataTable LoginUser(string username, string passwordHash)
        {
            var jsonData = JsonConvert.SerializeObject(new { username, passwordHash });

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_LoginUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = jsonData;

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // Register new user
        public void RegisterUser(string username, string passwordHash, string role)
        {
            var jsonData = JsonConvert.SerializeObject(new { username, passwordHash, role });

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = jsonData;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

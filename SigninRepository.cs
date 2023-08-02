using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.Services.Description;
using HR_Managemennt.Models;
using WebGrease.Activities;

namespace HR_Managemennt.Repositories
{
    namespace HR_Managemennt.Repositories
    {
        public class SigninRepository
        {
            private SqlConnection connect;

           /// <summary>
           /// Database connection
           /// </summary>           
            private void connection()
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ToString();
                connect = new SqlConnection(connectionString);
            }

            /// <summary>
            /// To signin a user
            /// </summary>
            /// <param name="signin"></param>
            /// <param name="errorMessage"></param>
            /// <returns></returns>
            public bool Signin_User(Signin signin, out string errorMessage)
            {
                try
                {
                    connection();

                    using (SqlCommand command = new SqlCommand("SP_Signin", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Username", signin.Username);
                        command.Parameters.AddWithValue("@Password", signin.Password);
                        command.Parameters.AddWithValue("@Role", signin.Role);

                        connect.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                      
                            HttpContext.Current.Session["username"] = signin.Username;

                            errorMessage = null;
                            return true;
                        }
                        else
                        {
                            errorMessage = "Invalid username or password";
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "An error occurred during login: " + ex.Message;
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }

        }
    }
}






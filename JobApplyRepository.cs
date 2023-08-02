using HR_Managemennt.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HR_Managemennt.Repository
{
    public class JobApplyRepository
    {
        private SqlConnection connect;

        /// <summary>
        /// Database Connection
        /// </summary>
        private void connection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ToString();
            connect = new SqlConnection(connectionString);
        }

        /// <summary>
        /// To apply for the job
        /// </summary>
        /// <param name="jobapply"></param>
        /// <returns></returns>
        public bool Insert(JobApply jobapply)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                try
                {
                    string insertProcedure = "SPI_AppliedCandidates";
                    SqlCommand command = new SqlCommand(insertProcedure, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@JobName", jobapply.JobName);
                    command.Parameters.AddWithValue("@Username", jobapply.Username);
                    command.Parameters.AddWithValue("@FullName", jobapply.FullName);
                    command.Parameters.AddWithValue("@Mobile", jobapply.Mobile);
                    command.Parameters.AddWithValue("@YearOfGraduation", jobapply.YearOfGraduation);
                    command.Parameters.AddWithValue("@Experience", jobapply.Experience);
                    command.Parameters.AddWithValue("@CurrentStatus", jobapply.CurrentStatus);
                    command.Parameters.AddWithValue("@Skills", jobapply.Skills);
                    command.Parameters.AddWithValue("@CurrentLocation", jobapply.CurrentLocation);
                    command.Parameters.AddWithValue("@PreferredLocation", jobapply.PreferredLocation);
                    command.Parameters.AddWithValue("@PassportSizePhoto", jobapply.PassportSizePhoto);
                    command.Parameters.AddWithValue("@ResumePDF", jobapply.ResumePDF);
                    connection.Open();
                    int i = command.ExecuteNonQuery();
                    return i >= 1;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


    }
}
using HR_Managemennt.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HR_Managemennt.Repository
{
    public class HR_Repository
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
        /// To post a job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool JobPost(Jobs job)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string insertProcedure = "SPI_Jobs";
                SqlCommand command = new SqlCommand(insertProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@jobName", job.JobName);
                command.Parameters.AddWithValue("@jobDescription", job.JobDescription);
                command.Parameters.AddWithValue("@JobType", job.JobType);
                command.Parameters.AddWithValue("@Shift", job.Shift);
                command.Parameters.AddWithValue("@SalaryPerMonth", job.SalaryPerMonth);
                command.Parameters.AddWithValue("@jobLocation", job.JobLocation);
                command.Parameters.AddWithValue("@startDate", job.StartDate);
                command.Parameters.AddWithValue("@endDate", job.EndDate);

                try
                {
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


        /// <summary>
        /// To view posted jobs details
        /// </summary>
        /// <returns></returns>
        public List<Jobs> JobsDetails()
        {
            List<Jobs> jobsList = new List<Jobs>();

            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string selectProcedure = "SPS_Jobs";
                SqlCommand command = new SqlCommand(selectProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                DataTable datatable = new DataTable();
                SqlDataAdapter dataadapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    dataadapter.Fill(datatable);

                    foreach (DataRow datarow in datatable.Rows)
                    {
                        Jobs job = new Jobs
                        {
                            ID = Convert.ToInt32(datarow["ID"]),
                            JobName = Convert.ToString(datarow["JobName"]),
                            JobDescription = Convert.ToString(datarow["JobDescription"]),
                            JobType = Convert.ToString(datarow["JobType"]),
                            Shift = Convert.ToString(datarow["Shift"]),
                            SalaryPerMonth = Convert.ToSingle(datarow["SalaryPerMonth"]),
                            JobLocation = Convert.ToString(datarow["JobLocation"]),
                            StartDate = Convert.ToDateTime(datarow["StartDate"]),
                            EndDate = Convert.ToDateTime(datarow["EndDate"]),
                        };

                        jobsList.Add(job);
                    }

                    return jobsList;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// To view all applicant details
        /// </summary>
        /// <returns></returns>
        public List<JobApply> ApplicandDetail()
        {
            List<JobApply> jobApplyList = new List<JobApply>();

            connection(); 

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string selectProcedure = "SPS_AppliedCandidates";
                SqlCommand command = new SqlCommand(selectProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                DataTable datatable = new DataTable();
                SqlDataAdapter dataadapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    dataadapter.Fill(datatable);

                    foreach (DataRow datarow in datatable.Rows)
                    {
                        JobApply jobApply = new JobApply
                        {
                            ID = Convert.ToInt32(datarow["ID"]),
                            JobName = Convert.ToString(datarow["JobName"]),
                            Username = Convert.ToString(datarow["Username"]),
                            FullName = Convert.ToString(datarow["FullName"]),
                            Mobile = Convert.ToString(datarow["Mobile"]),
                            YearOfGraduation = datarow["YearOfGraduation"] != DBNull.Value ? Convert.ToInt32(datarow["YearOfGraduation"]) : (int?)null,
                            Experience = Convert.ToString(datarow["Experience"]),
                            CurrentStatus = Convert.ToString(datarow["CurrentStatus"]),
                            Skills = Convert.ToString(datarow["Skills"]),
                            CurrentLocation = Convert.ToString(datarow["CurrentLocation"]),
                            PreferredLocation = Convert.ToString(datarow["PreferredLocation"]),
                            PassportSizePhoto = datarow["PassportSizePhoto"] != DBNull.Value ? (byte[])datarow["PassportSizePhoto"] : null,
                            ResumePDF = datarow["ResumePDF"] != DBNull.Value ? (byte[])datarow["ResumePDF"] : null
                        };

                        jobApplyList.Add(jobApply);
                    }

                    return jobApplyList;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// To view only Approved candidates
        /// </summary>
        /// <returns></returns>
        public List<JobApply> ApprovedCandidates()
        {
            List<JobApply> jobApplyList = new List<JobApply>();

            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string query = "SELECT * FROM AppliedCandidates WHERE Status = 'Approved'";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = System.Data.CommandType.Text;

                DataTable datatable = new DataTable();
                SqlDataAdapter dataadapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    dataadapter.Fill(datatable);

                    foreach (DataRow datarow in datatable.Rows)
                    {
                        JobApply jobApply = new JobApply
                        {
                            ID = Convert.ToInt32(datarow["ID"]),
                            JobName = Convert.ToString(datarow["JobName"]),
                            Username = Convert.ToString(datarow["Username"]),
                            FullName = Convert.ToString(datarow["FullName"]),
                            Mobile = Convert.ToString(datarow["Mobile"]),
                            YearOfGraduation = datarow["YearOfGraduation"] != DBNull.Value ? Convert.ToInt32(datarow["YearOfGraduation"]) : (int?)null,
                            Experience = Convert.ToString(datarow["Experience"]),
                            CurrentStatus = Convert.ToString(datarow["CurrentStatus"]),
                            Skills = Convert.ToString(datarow["Skills"]),
                            CurrentLocation = Convert.ToString(datarow["CurrentLocation"]),
                            PreferredLocation = Convert.ToString(datarow["PreferredLocation"]),
                            InterviewDate = Convert.ToString(datarow["InterviewDate"]),
                            PassportSizePhoto = datarow["PassportSizePhoto"] != DBNull.Value ? (byte[])datarow["PassportSizePhoto"] : null,
                            ResumePDF = datarow["ResumePDF"] != DBNull.Value ? (byte[])datarow["ResumePDF"] : null
                        };

                        jobApplyList.Add(jobApply);
                    }

                    return jobApplyList;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// To view only rejected candidates
        /// </summary>
        /// <returns></returns>
        public List<JobApply> RejectedCandidates()
        {
            List<JobApply> jobApplyList = new List<JobApply>();

            connection(); // Initialize the connect object

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string query = "SELECT * FROM AppliedCandidates WHERE Status = 'Rejected'";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = System.Data.CommandType.Text;

                DataTable datatable = new DataTable();
                SqlDataAdapter dataadapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    dataadapter.Fill(datatable);

                    foreach (DataRow datarow in datatable.Rows)
                    {
                        JobApply jobApply = new JobApply
                        {
                            ID = Convert.ToInt32(datarow["ID"]),
                            JobName = Convert.ToString(datarow["JobName"]),
                            Username = Convert.ToString(datarow["Username"]),
                            FullName = Convert.ToString(datarow["FullName"]),
                            Mobile = Convert.ToString(datarow["Mobile"]),
                            YearOfGraduation = datarow["YearOfGraduation"] != DBNull.Value ? Convert.ToInt32(datarow["YearOfGraduation"]) : (int?)null,
                            Experience = Convert.ToString(datarow["Experience"]),
                            CurrentStatus = Convert.ToString(datarow["CurrentStatus"]),
                            Skills = Convert.ToString(datarow["Skills"]),
                            CurrentLocation = Convert.ToString(datarow["CurrentLocation"]),
                            PreferredLocation = Convert.ToString(datarow["PreferredLocation"]),
                            PassportSizePhoto = datarow["PassportSizePhoto"] != DBNull.Value ? (byte[])datarow["PassportSizePhoto"] : null,
                            ResumePDF = datarow["ResumePDF"] != DBNull.Value ? (byte[])datarow["ResumePDF"] : null
                        };

                        jobApplyList.Add(jobApply);
                    }

                    return jobApplyList;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// To get user details
        /// </summary>
        /// <returns></returns>
        public List<Signup> GetDetails()
        {
            List<Signup> signupList = new List<Signup>();

            connection(); 

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string selectProcedure = "SPS_Registered_Candidates";
                SqlCommand command = new SqlCommand(selectProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                DataTable datatable = new DataTable();
                SqlDataAdapter dataadapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    dataadapter.Fill(datatable);

                    foreach (DataRow datarow in datatable.Rows)
                    {
                        Signup signup = new Signup
                        {
                            ID = Convert.ToInt32(datarow["ID"]),
                            firstName = Convert.ToString(datarow["firstName"]),
                            lastName = Convert.ToString(datarow["lastName"]),
                            dateofBirth = Convert.ToDateTime(datarow["dateofBirth"]),
                            Gender = Convert.ToString(datarow["Gender"]),
                            Mobile = Convert.ToString(datarow["Mobile"]),
                            emailAddress = Convert.ToString(datarow["emailAddress"]),
                            Address = Convert.ToString(datarow["Address"]),
                            State = Convert.ToString(datarow["State"]),
                            City = Convert.ToString(datarow["City"]),
                            Username = Convert.ToString(datarow["Username"]),
                            Password = Convert.ToString(datarow["Password"]),
                            Role = Convert.ToString(datarow["Role"]),
                        };

                        signupList.Add(signup);
                    }

                    return signupList;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// To delete a user
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool Delete(string Username)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string deleteProcedure = "SPD_Registered_Candidates";
                SqlCommand command = new SqlCommand(deleteProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", Username);

                try
                {
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


        /// <summary>
        /// To make a contactus form
        /// </summary>
        /// <param name="contactus"></param>
        /// <returns></returns>
        public bool Contact(Contactus contactus)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string insertProcedure = "SPI_contactUs";
                SqlCommand command = new SqlCommand(insertProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FullName", contactus.FullName);
                command.Parameters.AddWithValue("@Email", contactus.Email);
                command.Parameters.AddWithValue("@Mobile", contactus.Mobile);
                command.Parameters.AddWithValue("@Message", contactus.Message);

                try
                {
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


        /// <summary>
        /// Get the paricular user 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public string GetUsername(string role)
        {
            if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;
                var userRoles = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (userRoles.Contains(role))
                {
                    return username;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// To add a new HR
        /// </summary>
        /// <param name="signup"></param>
        /// <returns></returns>
        public bool AddHR(Signup signup)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string insertProcedure = "SPI_Registered_Candidates";
                SqlCommand command = new SqlCommand(insertProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@firstName", signup.firstName);
                command.Parameters.AddWithValue("@lastName", signup.lastName);
                command.Parameters.AddWithValue("@dateofBirth", signup.dateofBirth);
                command.Parameters.AddWithValue("@Gender", signup.Gender);
                command.Parameters.AddWithValue("@Mobile", signup.Mobile);
                command.Parameters.AddWithValue("@emailAddress", signup.emailAddress);
                command.Parameters.AddWithValue("@Address", signup.Address);
                command.Parameters.AddWithValue("@State", signup.State);
                command.Parameters.AddWithValue("@City", signup.City);
                command.Parameters.AddWithValue("@Username", signup.Username);
                string encryptedPassword = Encrypt(signup.Password);
                command.Parameters.AddWithValue("@Password", encryptedPassword);
                command.Parameters.AddWithValue("@Role", signup.Role);

                try
                {
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


        /// <summary>
        /// To update the Candidate details
        /// </summary>
        /// <param name="signup"></param>
        /// <returns></returns>
        public bool Update(Signup signup)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string updateProcedure = "SPU_Registered_Candidates";
                SqlCommand command = new SqlCommand(updateProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", signup.ID);
                command.Parameters.AddWithValue("@emailAddress", signup.emailAddress);
                command.Parameters.AddWithValue("@firstName", signup.firstName);
                command.Parameters.AddWithValue("@lastName", signup.lastName);
                command.Parameters.AddWithValue("@dateofBirth", signup.dateofBirth);
                command.Parameters.AddWithValue("@Gender", signup.Gender);
                command.Parameters.AddWithValue("@Mobile", signup.Mobile);
                command.Parameters.AddWithValue("@Address", signup.Address);
                command.Parameters.AddWithValue("@State", signup.State);
                command.Parameters.AddWithValue("@City", signup.City);
                command.Parameters.AddWithValue("@Username", signup.Username);
          
                command.Parameters.AddWithValue("@Role", signup.Role);

                try
                {
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

        /// <summary>
        /// Encrypt the Password
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        private string Encrypt(string clearText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        /// <summary>
        /// Decrypt the Password
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        private string Decrypt(string cipherText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
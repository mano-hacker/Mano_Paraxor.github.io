using HR_Managemennt.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace HR_Managemennt.Repository
{
    public class Candidate_Repository
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

        private bool CheckUsernameExists(string Username, string emailAddress)
        {
            connection();
            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string checkQuery = "SELECT COUNT(*) FROM Registered_Candidates WHERE Username = @Username and emailAddress = @emailAddress";
                SqlCommand command = new SqlCommand(checkQuery, connection);
                command.Parameters.AddWithValue("@Username", Username);
                command.Parameters.AddWithValue("@emailAddress", emailAddress);
                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }


        /// <summary>
        /// Insert userdata 
        /// </summary>
        /// <param name="signup"></param>
        /// <returns></returns>
        public bool Insert(Signup signup)
        {
            if (CheckUsernameExists(signup.Username, signup.emailAddress))
            {
               
                throw new Exception("User already exists. Please choose a different username or emailaddress");
            }
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

                    if (i >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Get user Details
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

        /// <summary>
        /// UPDATE operation
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
        /// DELETE operation
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool Delete(int ID)
        {
            connection();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string deleteProcedure = "SPD_Registered_Candidates";
                SqlCommand command = new SqlCommand(deleteProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", ID);

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
        /// Contact us method
        /// </summary>
        /// <param name="contactus"></param>
        /// <returns></returns>
        public bool ContactUs(Contactus contactus)
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
        /// Signin user method
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
                    string encryptedPassword = Encrypt(signin.Password); 
                    command.Parameters.AddWithValue("@Password", encryptedPassword);
                    command.Parameters.AddWithValue("@Role", signin.Role);

                    connect.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        string Username = reader.GetString(reader.GetOrdinal("Username"));
                        HttpContext.Current.Session["ID"] = ID;
                        HttpContext.Current.Session["Username"] = Username;

                        errorMessage = null;
                        return true;
                    }
                    else
                    {
                        errorMessage = "Invalid Username or Password";
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

        /// <summary>
        /// Get user details by id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<Signup> GetUserByID(int ID)
        {
            connection();
            List<Signup> signupList = new List<Signup>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string query = "SELECT * FROM Registered_Candidates WHERE ID = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);

                    DataTable datatable = new DataTable();
                    SqlDataAdapter dataadapter = new SqlDataAdapter(command);

                    try
                    {
                        connection.Open();
                        dataadapter.Fill(datatable);

                        foreach (DataRow datarow in datatable.Rows)
                        {
                            Signup SignUp = new Signup
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

                            signupList.Add(SignUp);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return signupList;
        }


        /// <summary>
        /// get job status by username
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public List<JobApply> GetStatusByUsername(string Username)
        {
            connection();
            List<JobApply> statusList = new List<JobApply>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string query = "SELECT * FROM AppliedCandidates WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);

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
                                Status = Convert.ToString(datarow["Status"]),
                                InterviewDate = Convert.ToString(datarow["InterviewDate"]),
                                PassportSizePhoto = datarow["PassportSizePhoto"] != DBNull.Value ? (byte[])datarow["PassportSizePhoto"] : null,
                                ResumePDF = datarow["ResumePDF"] != DBNull.Value ? (byte[])datarow["ResumePDF"] : null
                            };

                            statusList.Add(jobApply);
                        }

                        return statusList;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

    }
}



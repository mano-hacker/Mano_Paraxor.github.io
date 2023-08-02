using HR_Managemennt.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HR_Managemennt.Repository
{
    public class HomeRepository
    {
        
            private SqlConnection connect;

        /// <summary>
        /// Database connection method
        /// </summary>
            private void connection()
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ToString();
                connect = new SqlConnection(connectionString);
            }

        /// <summary>
        /// Contactus method
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
        /// To display the posted jobs method
        /// </summary>
        /// <returns></returns>
        public List<Jobs> Jobs()
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
        /// Change Password
        /// </summary>
        /// <param name="signup"></param>
        /// <returns>True if at least one row is affected, otherwise false (bool)</returns>
        public bool ChangePassword(Signup signup)
        {
            connection();

            string query = "UPDATE Registered_Candidates SET Password = @Password WHERE Username = @Username AND emailAddress = @emailAddress";

            using (SqlCommand command = new SqlCommand(query, connect)) 
            {
                command.Parameters.AddWithValue("@emailAddress", signup.emailAddress);
                command.Parameters.AddWithValue("@Username", signup.Username);
                string encryptedPassword = Encrypt(signup.Password);
                command.Parameters.AddWithValue("@Password", encryptedPassword);

                try
                {
                    connect.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected >= 1;
                }
                finally
                {
                    connect.Close();
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

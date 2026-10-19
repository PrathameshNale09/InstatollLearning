using InstaTollLearningApp.Data;
using InstaTollLearningApp.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaTollLearningApp.Services
{
    public class AuthenticationService
    {
        //public bool Authenticate(
        //   string username, string password)
        //{
        //    const string sql = @"
        //        SELECT PasswordHash
        //        FROM dbo.Users
        //        WHERE Username = @Username
        //          AND IsActive = 1";

        //    using (SqlConnection connection =
        //        DatabaseHelper.GetConnection())
        //    using (SqlCommand command =
        //        new SqlCommand(sql, connection))
        //    {
        //        command.Parameters.AddWithValue(
        //            "@Username", username);

        //        connection.Open();

        //        object result = command.ExecuteScalar();

        //        if (result == null || result == DBNull.Value)
        //            return false;

        //        string storedHash = result.ToString();

        //        return PasswordHasher.VerifyPassword(
        //            password, storedHash);
        //    }
        //}

        private static readonly Logger Logger =
                 LogManager.GetCurrentClassLogger();

        public bool Authenticate(string username, string password)
        {
            const string sql = @"
        SELECT PasswordHash
        FROM dbo.Users
        WHERE Username = @Username
          AND IsActive = 1";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Username", SqlDbType.NVarChar, 50).Value = username;

                connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    Logger.Warn("Login failed: user not found or inactive.");
                    return false;
                }
                   

                string storedHash = result.ToString();

                if (string.IsNullOrWhiteSpace(storedHash))
                    return false;

                bool isValid =
               PasswordHasher.VerifyPassword(password, storedHash);

                //return PasswordHasher.VerifyPassword(password, storedHash);
                if (isValid)
                    Logger.Info("Login successful for user: {0}", username);
                else
                    Logger.Warn("Login failed: invalid credentials for user: {0}", username);

                return isValid;
            }
        }


        public int GetUserId(string username)
        {
            const string sql = @"
        SELECT UserId
        FROM dbo.Users
        WHERE Username = @Username
          AND IsActive = 1";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value =
                    username;

                connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException(
                        "Active user was not found.");
                }

                return Convert.ToInt32(result);
            }
        }

    }
}

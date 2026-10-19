using System;
using System.Data;
using System.Data.SqlClient;
using InstaTollLearningApp.Security;

namespace InstaTollLearningApp.Data
{
    internal class DatabaseInitializer
    {
        public static void Initialize()
        {
            EnsureUsersTableExists();
            SeedAdminUser();
        }

        private static void EnsureUsersTableExists()
        {
            const string sql = @"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users')
                BEGIN
                    CREATE TABLE dbo.Users (
                        UserId     INT IDENTITY(1,1) PRIMARY KEY,
                        Username   NVARCHAR(50)  NOT NULL UNIQUE,
                        PasswordHash NVARCHAR(500) NOT NULL,
                        IsActive   BIT NOT NULL DEFAULT 1
                    );
                END";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void SeedAdminUser()
        {
            const string checkSql = @"
                SELECT COUNT(1) FROM dbo.Users
                WHERE Username = @Username";

            const string insertSql = @"
                INSERT INTO dbo.Users (Username, PasswordHash, IsActive)
                VALUES (@Username, @PasswordHash, 1)";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
                {
                    checkCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = "admin";
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                        return;
                }

                string hash = PasswordHasher.HashPassword("Demo@123");

                using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                {
                    insertCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = "admin";
                    insertCmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = hash;
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}

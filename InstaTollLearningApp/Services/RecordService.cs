using InstaTollLearningApp.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaTollLearningApp.Services;

namespace InstaTollLearningApp.Services
{
    internal class RecordService
    {
        private static readonly Logger Logger =
    LogManager.GetCurrentClassLogger();
        public DataTable GetAll(int userId)
        {
            const string sql = @"
                SELECT RecordId, Title, Description,
                       CreatedAt, UpdatedAt
                FROM dbo.Records
                WHERE CreatedBy = @UserId
                ORDER BY RecordId DESC";

            DataTable table = new DataTable();

            using (SqlConnection connection =
                DatabaseHelper.GetConnection())
            using (SqlCommand command =
                new SqlCommand(sql, connection))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(command))
            {
                command.Parameters.Add(
                    "@UserId", SqlDbType.Int).Value = userId;

                adapter.Fill(table);
            }

            return table;
        }

        public void Create(string title, string description, int userId)
        {
            const string sql = @"
                INSERT INTO dbo.Records
                    (Title, Description, CreatedBy)
                VALUES
                    (@Title, @Description, @UserId)";

            using (SqlConnection connection =
                DatabaseHelper.GetConnection())
            using (SqlCommand command =
                new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Title", SqlDbType.NVarChar, 100).Value =
                    title;

                command.Parameters.Add(
                    "@Description", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(description)
                    ? (object)DBNull.Value
                    : description;

                command.Parameters.Add(
                    "@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                command.ExecuteNonQuery();
                ActivitySummaryService.RecordCreated();
            }
            Logger.Info(
            "Record created. UserId: {0}, Title: {1}",
            userId,
            title);
        }

        public void Update(int recordId, string title, string description, int userId)
        {
            const string sql = @"
                UPDATE dbo.Records
                SET Title = @Title,
                    Description = @Description,
                    UpdatedAt = GETDATE()
                WHERE RecordId = @RecordId
                  AND CreatedBy = @UserId";

            using (SqlConnection connection =
                DatabaseHelper.GetConnection())
            using (SqlCommand command =
                new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Title", SqlDbType.NVarChar, 100).Value =
                    title;

                command.Parameters.Add(
                    "@Description", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(description)
                    ? (object)DBNull.Value
                    : description;

                command.Parameters.Add(
                    "@RecordId", SqlDbType.Int).Value = recordId;

                command.Parameters.Add(
                    "@UserId", SqlDbType.Int).Value = userId;

                connection.Open();

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException(
                        "Record not found or access denied.");

                ActivitySummaryService.RecordUpdated();
                Logger.Info(
                    "Record updated. RecordId: {0}, UserId: {1}",
                    recordId,
                    userId);
            }
        }

        public void Delete(int recordId, int userId)
        {
            const string sql = @"
                DELETE FROM dbo.Records
                WHERE RecordId = @RecordId
                  AND CreatedBy = @UserId";

            using (SqlConnection connection =
                DatabaseHelper.GetConnection())
            using (SqlCommand command =
                new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@RecordId", SqlDbType.Int).Value = recordId;

                command.Parameters.Add(
                    "@UserId", SqlDbType.Int).Value = userId;

                connection.Open();

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException(
                        "Record not found or access denied.");

                ActivitySummaryService.RecordDeleted();
                Logger.Info("Record Deleted Record :{0} UserId : {1}",
                    recordId, userId);   
            }
        }
    }
}

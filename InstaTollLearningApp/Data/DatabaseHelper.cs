using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaTollLearningApp.Data
{
    internal class DatabaseHelper
    {
        public static SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[
                    "InstaTollDb"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}

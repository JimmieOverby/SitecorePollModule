namespace DMSPollData.Analytics.Data.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using System.Data.SqlClient;
    using Sitecore.Diagnostics;
    
    public static class SqlHelper
    {
        // Methods
        public static string Escape(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            return value.Replace("'", "''");
        }

        public static int ExecuteCommand(string sql, params object[] parameters)
        {
            Assert.ArgumentNotNull(sql, "sql");
            Assert.ArgumentNotNull(parameters, "parameters");
            using (SqlConnection connection = new SqlConnection(AnalyticsDataContext.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                for (int i = 0; i < (parameters.Length - 1); i += 2)
                {
                    string parameterName = parameters[i] as string;
                    object obj2 = parameters[i + 1];
                    SqlParameter parameter = new SqlParameter(parameterName, obj2);
                    command.Parameters.Add(parameter);
                }

                return command.ExecuteNonQuery();
            }
        }

        public static void Read(string sql, AnalyticsProvider.ReadDelegate readDelegate)
        {
            Assert.ArgumentNotNull(sql, "sql");
            Assert.ArgumentNotNull(readDelegate, "readDelegate");
            using (SqlConnection connection = new SqlConnection(AnalyticsDataContext.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        readDelegate(reader);
                    }

                    reader.Close();
                }
            }
        }
    }
}

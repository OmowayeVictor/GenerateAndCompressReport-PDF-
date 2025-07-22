

using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace GenerateAndCompressPDF
{
    public class Database
    {
        private readonly string? environment;
        private readonly string? connectionString;
        private readonly string query;

        public Database(string query)
        {
            environment = Environment.GetEnvironmentVariable("Environment");
            connectionString = environment == "Live"
                ? Environment.GetEnvironmentVariable("LiveConnectionString")
                : Environment.GetEnvironmentVariable("DevConnectionString");
            this.query = query;
        }

        public List<Dictionary<string, object>> GetDetailsFromDB()
        {
            var detailsFromDb = new List<Dictionary<string, object>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }
                                detailsFromDb.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
            return detailsFromDb;
        }
    }
}

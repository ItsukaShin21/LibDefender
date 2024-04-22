using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;

namespace LibDefender
{
    public static class Authentication
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function for User Authentication
        public static async Task<bool> AuthenticateUser(string rfiduid, string password)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("SELECT username FROM users WHERE userRfid = @rfiduid AND password = @password", connection);

            command.Parameters.AddWithValue("@rfiduid", rfiduid);
            command.Parameters.AddWithValue("@password", password);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string username = reader.GetString(0);

                // Update the UI directly
                AdminWindow.Instance.Username.Content = username;

                return true; // User exists and UI is updated
            }
            else
            {
                // User does not exist or credentials are incorrect
                return false;
            }
        }

    }
}

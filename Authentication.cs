using MySql.Data.MySqlClient;

namespace LibDefender
{
    public static class Authentication
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function for User Authentication
        public static async Task<int> AuthenticateUser(string rfiduid, string password)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE userRfid = @rfiduid AND password = @password", connection);

            command.Parameters.AddWithValue("@rfiduid", rfiduid);
            command.Parameters.AddWithValue("@password", password);

            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
    }
}

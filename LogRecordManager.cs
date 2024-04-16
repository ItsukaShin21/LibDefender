using System;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public static class LogRecordManager
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function for inserting and updating the student in the logrecords table
        public static void HandleStudentRfid(string rfid)
        {
            string LogStudentQuery = "SELECT COUNT(*) FROM logrecords WHERE studentRfid = @rfid AND timeout IS NULL";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(LogStudentQuery, connection);

            command.Parameters.AddWithValue("@rfid", rfid);

            connection.Open();

            long count = (Int64)command.ExecuteScalar();

            if (count > 0)
            {
                command.CommandText = "UPDATE logrecords SET timeout = NOW() WHERE studentRfid = @rfid AND timeout IS NULL";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = "INSERT INTO logrecords (studentRfid, timein) VALUES (@rfid, NOW())";
                command.ExecuteNonQuery();
            }
        }
    }
}

using System;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public static class TableQueries
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function for checking the rfid if it is a student or a book
        public static void CheckRfid(string rfid)
        {
            string CheckQuery = @"SELECT 'student' AS type, studentRfid AS rfid FROM students WHERE studentRfid = @rfid
                                UNION ALL SELECT 'book' AS type, bookRfid AS rfid FROM books WHERE bookRfid = @rfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(CheckQuery, connection);

            command.Parameters.AddWithValue("@rfid", rfid);

            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string type = reader.GetString("type");

                    if (type == "student")
                    {
                        LogRecordManager.HandleStudentRfid(rfid);
                        StudentLogPage.CurrentInstance.RfidTxtBox.Clear();
                        DataFetcher.FetchLogRecordsData();
                    }
                    else if (type == "book")
                    {
                        BookManager.HandleBookRfid(rfid);
                        StudentLogPage.CurrentInstance.RfidTxtBox.Clear();
                        DataFetcher.FetchLogRecordsData();
                    }
                }
            }
        }
    }
}

using System.Media;
using System.Numerics;
using System.Windows;
using MySql.Data.MySqlClient;
using ToastNotifications.Messages;

namespace LibDefender
{
    public static class BookManager
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function for alerting if a book is stolen
        public static void HandleBookRfid(string rfid)
        {
            string CheckBookQuery = "SELECT bookName,status FROM books WHERE bookRfid = @rfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(CheckBookQuery, connection);

            command.Parameters.AddWithValue("@rfid", rfid);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                string bookName = reader.GetString("bookName");
                string status = reader.GetString("status");
                var currentDateTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

                if (status == "Available")
                {
                    SoundPlayer player = new(@"C:\\Users\\jeflo\\Documents\\Visual Studio 2022\\Projects\\LibDefender\\Resources\\alarm_audio.wav");

                    NotificationManager.Notifier?.ShowError($"The {bookName} book has been stolen...\n" +
                                                            $"\n" +
                                                            $"BOOK INFORMATION:\n" +
                                                            $"\n" +
                                                            $"Book Name : {bookName}\n" +
                                                            $"Book Scanned: {currentDateTime}");

                    Task.Run(() =>
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            player.PlaySync(); // PlaySync blocks until the sound has finished playing
                        }
                    });
                }
            }

        }

        //Function for deleting a book data
        public static void DeleteBook(string bookRfid) 
        {
            string DeleteBookQuery = "DELETE FROM books WHERE bookRfid = @bookRfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(DeleteBookQuery, connection);
            command.Parameters.AddWithValue("@bookRfid", bookRfid);

            connection.Open();
            command.ExecuteNonQuery();
        }

        //Function for inserting a data in books table
        public static int InsertBook(BigInteger rfidUID, string bookName, string authorName)
        {
            string InsertBookQuery = "INSERT INTO books (bookRfid, bookName, bookAuthor, dateAdded, status)" +
                           "VALUES (@rfidUID, @bookName, @authorName, NOW(), 'Available')";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(InsertBookQuery, connection);

            command.Parameters.AddWithValue("@rfidUID", rfidUID);
            command.Parameters.AddWithValue("@bookName", bookName);
            command.Parameters.AddWithValue("@authorName", authorName);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            return result;
        }

        //Function for updating the status of a book in books table
        public static void UpdateBook(string bookRfid, string studentRfid)
        {
            string checkStatusQuery = "SELECT status FROM books WHERE bookRfid = @bookRfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(checkStatusQuery, connection);

            command.Parameters.AddWithValue("@bookRfid", bookRfid);

            connection.Open();

            string status = (string)command.ExecuteScalar();

            if (status == "Available")
            {
                string updateBookQuery = "UPDATE books SET status = 'Borrowed' WHERE bookRfid = @bookRfid";
                using var updateCommand = new MySqlCommand(updateBookQuery, connection);

                updateCommand.Parameters.AddWithValue("@bookRfid", bookRfid);
                updateCommand.ExecuteNonQuery();

                InsertBorrowedBook(studentRfid, bookRfid);
            }
            else if (status == "Borrowed")
            {
                MessageBox.Show("The book is already borrowed.");
            }
        }

        //Function for inserting the book that the student borrowed in the borrowedbooks table
        public static void InsertBorrowedBook(string studentRfid, string bookRfid)
        {
            DateTime borrowDate = DateTime.Now;
            DateTime returnDate = borrowDate.AddDays(1);

            string insertQuery = $"INSERT INTO borrowedbooks (bookRfid, studentRfid, borrowDate, returnDate) " +
                                 $"VALUES (@bookRfid, @studentRfid, @borrowDate, @returnDate);";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(insertQuery, connection);

            command.Parameters.AddWithValue("@bookRfid", bookRfid);
            command.Parameters.AddWithValue("@studentRfid", studentRfid);
            command.Parameters.AddWithValue("@borrowDate", borrowDate);
            command.Parameters.AddWithValue("@returnDate", returnDate);

            connection.Open();
            command.ExecuteNonQuery();
            BorrowBookModal.CurrentInstance.BookRfidTxtBox.Clear();
        }

        //Function to refresh the status of the book
        public static void RefreshBookStatus()
        {
            string refreshStatusQuery = "UPDATE books JOIN borrowedbooks ON books.bookRfid = borrowedBooks.bookRfid " +
                                        "SET books.status = 'Not Returned' WHERE borrowedbooks.returnDate <= CURDATE() " +
                                        "AND books.status <> 'Not Returned'";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(refreshStatusQuery, connection);

            connection.Open();
            command.ExecuteNonQuery();
        }

    }
}

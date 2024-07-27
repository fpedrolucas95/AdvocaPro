using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class HolidayService
    {
        private readonly string _databasePath;

        public HolidayService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddHoliday(Holiday holiday)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Holidays (Date, Description) VALUES (@Date, @Description)";
                command.Parameters.AddWithValue("@Date", holiday.Date);
                command.Parameters.AddWithValue("@Description", holiday.Description);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateHoliday(Holiday holiday)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE Holidays SET Date = @Date, Description = @Description WHERE Id = @Id";
                command.Parameters.AddWithValue("@Date", holiday.Date);
                command.Parameters.AddWithValue("@Description", holiday.Description);
                command.Parameters.AddWithValue("@Id", holiday.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteHoliday(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Holidays WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<Holiday> GetHolidays()
        {
            var holidays = new List<Holiday>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Holidays";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        holidays.Add(new Holiday
                        {
                            Id = reader.GetInt32(0),
                            Date = reader.GetString(1),
                            Description = reader.GetString(2)
                        });
                    }
                }
            }
            return holidays;
        }
    }
}

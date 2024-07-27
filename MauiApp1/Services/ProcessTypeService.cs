using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class ProcessTypeService
    {
        private readonly string _databasePath;

        public ProcessTypeService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddProcessType(ProcessType processType)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO ProcessTypes (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", processType.Name);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateProcessType(ProcessType processType)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE ProcessTypes SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", processType.Name);
                command.Parameters.AddWithValue("@Id", processType.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteProcessType(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM ProcessTypes WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<ProcessType> GetProcessTypes()
        {
            var processTypes = new List<ProcessType>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM ProcessTypes";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        processTypes.Add(new ProcessType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return processTypes;
        }
    }
}

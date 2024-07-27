using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class CategoryEntryService
    {
        private readonly string _databasePath;

        public CategoryEntryService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddCategoryEntry(CategoryEntry category)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO CategoryEntry (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", category.Name);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCategoryEntry(CategoryEntry category)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE CategoryEntry SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Id", category.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteCategoryEntry(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM CategoryEntry WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<CategoryEntry> GetCategoryEntries()
        {
            var categories = new List<CategoryEntry>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM CategoryEntry";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategoryEntry
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return categories;
        }
    }
}

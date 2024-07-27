using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class CategoryExitService
    {
        private readonly string _databasePath;

        public CategoryExitService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddCategoryExit(CategoryExit category)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO CategoryExit (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", category.Name);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCategoryExit(CategoryExit category)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE CategoryExit SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Id", category.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteCategoryExit(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM CategoryExit WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<CategoryExit> GetCategoryExits()
        {
            var categories = new List<CategoryExit>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM CategoryExit";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategoryExit
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

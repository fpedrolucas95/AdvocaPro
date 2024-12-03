using AdvocaPro.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace AdvocaPro.Services
{
    public class UserService
    {
        private readonly DatabaseService _databaseService;

        public UserService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddUser(User user)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                    INSERT INTO User (
                        username, password, first_name, last_name, phone, cell_phone, user_type, created_at, created_by, 
                        updated_at, updated_by, last_edit_computer, birthday_date, age, email
                    ) VALUES (
                        @UserName, @Password, @FirstName, @LastName, @Phone, @CellPhone, @UserType, @CreatedAt, @CreatedBy, 
                        @UpdatedAt, @UpdatedBy, @LastEditComputer, @BirthdayDate, @Age, @Email
                    )";

                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName ?? string.Empty);
                    command.Parameters.AddWithValue("@LastName", user.LastName ?? string.Empty);
                    command.Parameters.AddWithValue("@Phone", user.Phone ?? string.Empty);
                    command.Parameters.AddWithValue("@CellPhone", user.CellPhone ?? string.Empty);
                    command.Parameters.AddWithValue("@UserType", user.UserType);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", user.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@LastEditComputer", user.LastEditComputer ?? string.Empty);
                    command.Parameters.AddWithValue("@BirthdayDate", user.BirthdayDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Age", user.Age ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", user.Email ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<User> GetUsers()
        {
            var users = new List<User>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM User";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int age = reader.IsDBNull(13) ? 0 : reader.GetInt32(13); // Default to 0 if null or handle differently
                        age = (age >= 0 && age <= 150) ? age : 0; // Clamp values

                        users.Add(new User
                        {
                            UserName = reader.GetString(1),
                            Password = reader.GetString(2),
                            FirstName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            LastName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Phone = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            CellPhone = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            UserType = reader.GetInt32(7),
                            CreatedAt = reader.GetDateTime(8),
                            CreatedBy = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            UpdatedAt = reader.IsDBNull(10) ? null : (DateTime?)reader.GetDateTime(10),
                            UpdatedBy = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                            LastEditComputer = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            BirthdayDate = reader.IsDBNull(13) ? null : (DateTime?)reader.GetDateTime(13),
                            Age = age, // Use the clamped age value
                            Email = reader.IsDBNull(14) ? string.Empty : reader.GetString(14)
                        });
                    }
                }
            }

            return users;
        }
    }
}

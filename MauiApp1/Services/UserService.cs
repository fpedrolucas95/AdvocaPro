using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class UserService
    {
        private readonly string _databasePath;

        public UserService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddUser(User user)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO Users (
                            UserName, Password, FirstName, LastName, Phone, CellPhone, Registry, UserType, CreatedAt,
                            CreatedBy, UpdatedAt, UpdatedBy, LastEditComputer, BirthdayDate, Age, Email
                          ) VALUES (
                            @UserName, @Password, @FirstName, @LastName, @Phone, @CellPhone, @Registry, @UserType, @CreatedAt,
                            @CreatedBy, @UpdatedAt, @UpdatedBy, @LastEditComputer, @BirthdayDate, @Age, @Email
                          )";

                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName ?? string.Empty);
                    command.Parameters.AddWithValue("@LastName", user.LastName ?? string.Empty);
                    command.Parameters.AddWithValue("@Phone", user.Phone ?? string.Empty);
                    command.Parameters.AddWithValue("@CellPhone", user.CellPhone ?? string.Empty);
                    command.Parameters.AddWithValue("@Registry", user.Registry ?? string.Empty);
                    command.Parameters.AddWithValue("@UserType", user.UserType);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", user.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@LastEditComputer", user.LastEditComputer ?? string.Empty);
                    command.Parameters.AddWithValue("@BirthdayDate", user.BirthdayDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@Email", user.Email ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE Users SET
                            Password = @Password, FirstName = @FirstName, LastName = @LastName, Phone = @Phone,
                            CellPhone = @CellPhone, Registry = @Registry, UserType = @UserType, CreatedAt = @CreatedAt,
                            CreatedBy = @CreatedBy, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy, LastEditComputer = @LastEditComputer,
                            BirthdayDate = @BirthdayDate, Age = @Age, Email = @Email WHERE UserName = @UserName";

                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName ?? string.Empty);
                    command.Parameters.AddWithValue("@LastName", user.LastName ?? string.Empty);
                    command.Parameters.AddWithValue("@Phone", user.Phone ?? string.Empty);
                    command.Parameters.AddWithValue("@CellPhone", user.CellPhone ?? string.Empty);
                    command.Parameters.AddWithValue("@Registry", user.Registry ?? string.Empty);
                    command.Parameters.AddWithValue("@UserType", user.UserType);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", user.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@LastEditComputer", user.LastEditComputer ?? string.Empty);
                    command.Parameters.AddWithValue("@BirthdayDate", user.BirthdayDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@Email", user.Email ?? string.Empty);
                    command.Parameters.AddWithValue("@UserName", user.UserName);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteUser(string userName)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Users WHERE UserName = @UserName";
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<User> GetUsers()
        {
            var users = new List<User>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            UserName = reader.GetString(0),
                            Password = reader.GetString(1),
                            FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Phone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            CellPhone = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            Registry = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            UserType = reader.GetInt32(7),
                            CreatedAt = reader.GetDateTime(8),
                            CreatedBy = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            UpdatedAt = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                            UpdatedBy = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                            LastEditComputer = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            BirthdayDate = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13),
                            Age = reader.GetInt32(14),
                            Email = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                        };

                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public IEnumerable<User> GetLawyers()
        {
            var lawyers = new List<User>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users WHERE UserType = 1";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            UserName = reader.GetString(0),
                            Password = reader.GetString(1),
                            FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Phone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            CellPhone = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            Registry = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            UserType = reader.GetInt32(7),
                            CreatedAt = reader.GetDateTime(8),
                            CreatedBy = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            UpdatedAt = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                            UpdatedBy = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                            LastEditComputer = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            BirthdayDate = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13),
                            Age = reader.GetInt32(14),
                            Email = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                        };

                        lawyers.Add(user);
                    }
                }
            }

            return lawyers;
        }
    }
}

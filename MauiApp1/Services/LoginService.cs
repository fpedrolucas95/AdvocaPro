using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class LoginService
    {
        private readonly string _databasePath;
        private User? _loggedInUser;

        public LoginService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public async Task<User?> GetUserAsync(string username, string password)
        {
            try
            {
                using (var connection = new SqliteConnection($"Filename={_databasePath}"))
                {
                    await connection.OpenAsync();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Users WHERE UserName = @UserName AND Password = @Password";
                    command.Parameters.AddWithValue("@UserName", username);
                    command.Parameters.AddWithValue("@Password", password);

                    string debugCommandText = command.CommandText;
                    foreach (SqliteParameter param in command.Parameters)
                    {
                        debugCommandText = debugCommandText.Replace(param.ParameterName, $"'{param.Value}'");
                    }

                    System.Diagnostics.Debug.WriteLine($"Executing Command: {debugCommandText}");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
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
                                Age = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14),
                                Email = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            };

                            _loggedInUser = user;
                            return user;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in GetUserAsync: {ex.Message}");
            }

            return null;
        }

        public User? GetLoggedInUser()
        {
            return _loggedInUser;
        }
    }
}

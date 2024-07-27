using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class SettingsService
    {
        private readonly string _databasePath;

        public SettingsService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddSetting(Settings setting)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO Settings (
                            CompanyName, Address, CNPJ, Phone, Logo, Website, Email, Instagram, Facebook,
                            Theme, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                          ) VALUES (
                            @CompanyName, @Address, @CNPJ, @Phone, @Logo, @Website, @Email, @Instagram, @Facebook,
                            @Theme, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                          )";

                    command.Parameters.AddWithValue("@CompanyName", setting.CompanyName);
                    command.Parameters.AddWithValue("@Address", setting.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CNPJ", setting.CNPJ ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", setting.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Logo", setting.Logo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Website", setting.Website ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", setting.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Instagram", setting.Instagram ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Facebook", setting.Facebook ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Theme", setting.Theme);
                    command.Parameters.AddWithValue("@CreatedAt", setting.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", setting.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", setting.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", setting.UpdatedBy ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public void UpdateSetting(Settings setting)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE Settings SET
                            Address = @Address, CNPJ = @CNPJ, Phone = @Phone, Logo = @Logo, Website = @Website,
                            Email = @Email, Instagram = @Instagram, Facebook = @Facebook, Theme = @Theme,
                            CreatedAt = @CreatedAt, CreatedBy = @CreatedBy, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                            WHERE CompanyName = @CompanyName";

                    command.Parameters.AddWithValue("@CompanyName", setting.CompanyName);
                    command.Parameters.AddWithValue("@Address", setting.Address);
                    command.Parameters.AddWithValue("@CNPJ", setting.CNPJ);
                    command.Parameters.AddWithValue("@Phone", setting.Phone);
                    command.Parameters.AddWithValue("@Logo", setting.Logo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Website", setting.Website);
                    command.Parameters.AddWithValue("@Email", setting.Email);
                    command.Parameters.AddWithValue("@Instagram", setting.Instagram);
                    command.Parameters.AddWithValue("@Facebook", setting.Facebook);
                    command.Parameters.AddWithValue("@Theme", setting.Theme);
                    command.Parameters.AddWithValue("@CreatedAt", setting.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", setting.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedAt", setting.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", setting.UpdatedBy);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteSetting(string companyName)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Settings WHERE CompanyName = @CompanyName";
                    command.Parameters.AddWithValue("@CompanyName", companyName);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<Settings> GetSettings()
        {
            var settings = new List<Settings>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Settings";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var setting = new Settings
                        {
                            CompanyName = reader.GetString(0),
                            Address = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            CNPJ = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Logo = reader.IsDBNull(4) ? null : reader.GetFieldValue<byte[]>(4),
                            Website = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            Email = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            Instagram = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            Facebook = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            Theme = reader.GetInt32(9),
                            CreatedAt = reader.GetDateTime(10),
                            CreatedBy = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                            UpdatedAt = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            UpdatedBy = reader.IsDBNull(13) ? string.Empty : reader.GetString(13)
                        };

                        settings.Add(setting);
                    }
                }
            }

            return settings;
        }
    }
}

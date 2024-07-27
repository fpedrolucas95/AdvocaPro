using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class HearingService
    {
        private readonly string _databasePath;

        public HearingService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddHearing(Hearing hearing)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO Hearings (
                            ClientId, HearingDetails, Location, Date, Time, Completed, Situation, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                          ) VALUES (
                            @ClientId, @HearingDetails, @Location, @Date, @Time, @Completed, @Situation, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                          )";

                    command.Parameters.AddWithValue("@ClientId", hearing.ClientId);
                    command.Parameters.AddWithValue("@HearingDetails", hearing.HearingDetails ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Location", hearing.Location ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Date", hearing.Date);
                    command.Parameters.AddWithValue("@Time", hearing.Time ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Completed", hearing.Completed ? 1 : 0);
                    command.Parameters.AddWithValue("@Situation", hearing.Situation ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", hearing.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", hearing.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", hearing.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", hearing.UpdatedBy ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public void UpdateHearing(Hearing hearing)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE Hearings SET
                            HearingDetails = @HearingDetails, Location = @Location, Date = @Date, Time = @Time,
                            Completed = @Completed, Situation = @Situation, CreatedAt = @CreatedAt,
                            CreatedBy = @CreatedBy, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                            WHERE ClientId = @ClientId";

                    command.Parameters.AddWithValue("@ClientId", hearing.ClientId);
                    command.Parameters.AddWithValue("@HearingDetails", hearing.HearingDetails);
                    command.Parameters.AddWithValue("@Location", hearing.Location);
                    command.Parameters.AddWithValue("@Date", hearing.Date);
                    command.Parameters.AddWithValue("@Time", hearing.Time);
                    command.Parameters.AddWithValue("@Completed", hearing.Completed ? 1 : 0);
                    command.Parameters.AddWithValue("@Situation", hearing.Situation);
                    command.Parameters.AddWithValue("@CreatedAt", hearing.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", hearing.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedAt", hearing.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", hearing.UpdatedBy);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteHearing(int clientId)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Hearings WHERE ClientId = @ClientId";
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<Hearing> GetHearings()
        {
            var hearings = new List<Hearing>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Hearings";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var hearing = new Hearing
                        {
                            ClientId = reader.GetInt32(0),
                            HearingDetails = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Location = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Date = reader.GetDateTime(3),
                            Time = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Completed = reader.GetInt32(5) == 1,
                            Situation = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            CreatedAt = reader.GetDateTime(7),
                            CreatedBy = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                            UpdatedBy = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                        };

                        hearings.Add(hearing);
                    }
                }
            }

            return hearings;
        }
    }
}

using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class ExpertiseService
    {
        private readonly string _databasePath;

        public ExpertiseService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddExpertise(Expertise expertise)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO Expertise (
                            ClientId, ExpertiseDetails, Location, Date, Time, Completed, Situation, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                          ) VALUES (
                            @ClientId, @ExpertiseDetails, @Location, @Date, @Time, @Completed, @Situation, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                          )";

                    command.Parameters.AddWithValue("@ClientId", expertise.ClientId);
                    command.Parameters.AddWithValue("@ExpertiseDetails", expertise.ExpertiseDetails ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Location", expertise.Location ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Date", expertise.Date);
                    command.Parameters.AddWithValue("@Time", expertise.Time ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Completed", expertise.Completed ? 1 : 0);
                    command.Parameters.AddWithValue("@Situation", expertise.Situation ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", expertise.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", expertise.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", expertise.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", expertise.UpdatedBy ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public void UpdateExpertise(Expertise expertise)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE Expertise SET
                            ExpertiseDetails = @ExpertiseDetails, Location = @Location, Date = @Date, Time = @Time,
                            Completed = @Completed, Situation = @Situation, CreatedAt = @CreatedAt,
                            CreatedBy = @CreatedBy, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                            WHERE ClientId = @ClientId";

                    command.Parameters.AddWithValue("@ClientId", expertise.ClientId);
                    command.Parameters.AddWithValue("@ExpertiseDetails", expertise.ExpertiseDetails);
                    command.Parameters.AddWithValue("@Location", expertise.Location);
                    command.Parameters.AddWithValue("@Date", expertise.Date);
                    command.Parameters.AddWithValue("@Time", expertise.Time);
                    command.Parameters.AddWithValue("@Completed", expertise.Completed ? 1 : 0);
                    command.Parameters.AddWithValue("@Situation", expertise.Situation);
                    command.Parameters.AddWithValue("@CreatedAt", expertise.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", expertise.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedAt", expertise.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", expertise.UpdatedBy);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteExpertise(int clientId)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Expertise WHERE ClientId = @ClientId";
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<Expertise> GetExpertises()
        {
            var expertises = new List<Expertise>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Expertise";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var expertise = new Expertise
                        {
                            ClientId = reader.GetInt32(0),
                            ExpertiseDetails = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
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

                        expertises.Add(expertise);
                    }
                }
            }

            return expertises;
        }
    }
}

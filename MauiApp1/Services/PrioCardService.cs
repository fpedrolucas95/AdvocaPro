using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class PrioCardService
    {
        private readonly DatabaseService _databaseService;

        public PrioCardService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddPrioCard(PrioCardModel prioCard)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO PrioCard (
                            card_number, driver_id, available, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @CardNumber, @DriverId, @Available, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                    command.Parameters.AddWithValue("@CardNumber", prioCard.CardNumber ?? string.Empty);
                    command.Parameters.AddWithValue("@DriverId", prioCard.DriverId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Available", prioCard.Available);
                    command.Parameters.AddWithValue("@CreatedAt", prioCard.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", prioCard.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", prioCard.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", prioCard.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<PrioCardModel> GetPrioCards()
        {
            var prioCardsList = new List<PrioCardModel>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM PrioCard";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prioCardsList.Add(new PrioCardModel
                        {
                            Id = reader.GetInt32(0),
                            CardNumber = reader.GetString(1),
                            DriverId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                            Available = reader.GetBoolean(3),
                            CreatedAt = reader.GetDateTime(4),
                            CreatedBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            UpdatedAt = reader.IsDBNull(6) ? null : (DateTime?)reader.GetDateTime(6),
                            UpdatedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                        });
                    }
                }
            }

            return prioCardsList;
        }

        public void UpdatePrioCard(PrioCardModel prioCard)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        UPDATE PrioCard SET
                            card_number = @CardNumber,
                            driver_id = @DriverId,
                            available = @Available,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE id = @Id";

                    command.Parameters.AddWithValue("@Id", prioCard.Id);
                    command.Parameters.AddWithValue("@CardNumber", prioCard.CardNumber ?? string.Empty);
                    command.Parameters.AddWithValue("@DriverId", prioCard.DriverId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Available", prioCard.Available);
                    command.Parameters.AddWithValue("@UpdatedAt", prioCard.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", prioCard.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<PrioCardModel> GetAvailablePrioCards()
        {
            var prioCardsList = new List<PrioCardModel>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, card_number FROM PrioCard WHERE available = 1";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prioCardsList.Add(new PrioCardModel
                        {
                            Id = reader.GetInt32(0),
                            CardNumber = reader.GetString(1),
                            DriverId = null,
                            Available = true
                        });
                    }
                }
            }

            return prioCardsList;
        }

        public void DeletePrioCard(int prioCardId)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM PrioCard WHERE id = @Id";
                    command.Parameters.AddWithValue("@Id", prioCardId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
    }
}

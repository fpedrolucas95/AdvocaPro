using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class BankInformationService
    {
        private readonly DatabaseService _databaseService;

        public BankInformationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddBankInformation(BankInformationModel bankInformation)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO BankInformation (
                            entity_id, entity_type, bank_name, account_number, iban, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @EntityId, @EntityType, @BankName, @AccountNumber, @Iban, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                    command.Parameters.AddWithValue("@EntityId", bankInformation.EntityId);
                    command.Parameters.AddWithValue("@EntityType", bankInformation.EntityType);
                    command.Parameters.AddWithValue("@BankName", bankInformation.BankName ?? string.Empty);
                    command.Parameters.AddWithValue("@AccountNumber", bankInformation.AccountNumber ?? string.Empty);
                    command.Parameters.AddWithValue("@Iban", bankInformation.Iban ?? string.Empty);
                    command.Parameters.AddWithValue("@CreatedAt", bankInformation.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", bankInformation.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", bankInformation.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", bankInformation.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<BankInformationModel> GetBankInformation()
        {
            var bankInformationList = new List<BankInformationModel>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM BankInformation";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bankInformationList.Add(new BankInformationModel
                        {
                            Id = reader.GetInt32(0),
                            EntityId = reader.GetInt32(1),
                            EntityType = (EntityType)reader.GetInt32(2),
                            BankName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            AccountNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Iban = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6),
                            CreatedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            UpdatedAt = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                            UpdatedBy = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
                        });
                    }
                }
            }

            return bankInformationList;
        }

        public void UpdateBankInformation(BankInformationModel bankInformation)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        UPDATE BankInformation SET
                            entity_id = @EntityId,
                            entity_type = @EntityType,
                            bank_name = @BankName,
                            account_number = @AccountNumber,
                            iban = @Iban,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE id = @Id";

                    command.Parameters.AddWithValue("@Id", bankInformation.Id);
                    command.Parameters.AddWithValue("@EntityId", bankInformation.EntityId);
                    command.Parameters.AddWithValue("@EntityType", bankInformation.EntityType);
                    command.Parameters.AddWithValue("@BankName", bankInformation.BankName ?? string.Empty);
                    command.Parameters.AddWithValue("@AccountNumber", bankInformation.AccountNumber ?? string.Empty);
                    command.Parameters.AddWithValue("@Iban", bankInformation.Iban ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", bankInformation.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", bankInformation.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteBankInformation(int bankInformationId)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM BankInformation WHERE id = @Id";
                    command.Parameters.AddWithValue("@Id", bankInformationId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
    }
}

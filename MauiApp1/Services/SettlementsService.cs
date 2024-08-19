using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services;

public class SettlementsService
{
    private readonly DatabaseService _databaseService;

    public SettlementsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void AddSettlement(SettlementsModel settlement)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                        INSERT INTO Settlements (
                            driver_id, amount_paid, deductions, tax, date, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @DriverId, @AmountPaid, @Deductions, @Tax, @Date, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                command.Parameters.AddWithValue("@DriverId", settlement.DriverId);
                command.Parameters.AddWithValue("@AmountPaid", settlement.AmountPaid);
                command.Parameters.AddWithValue("@Deductions", settlement.Deductions);
                command.Parameters.AddWithValue("@Tax", settlement.Tax);
                command.Parameters.AddWithValue("@Date", settlement.Date);
                command.Parameters.AddWithValue("@CreatedAt", settlement.CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", settlement.CreatedBy ?? string.Empty);
                command.Parameters.AddWithValue("@UpdatedAt", settlement.UpdatedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", settlement.UpdatedBy ?? string.Empty);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }

    public IEnumerable<SettlementsModel> GetSettlements()
    {
        var settlementsList = new List<SettlementsModel>();

        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Settlements";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    settlementsList.Add(new SettlementsModel
                    {
                        Id = reader.GetInt32(0),
                        DriverId = reader.GetInt32(1),
                        AmountPaid = reader.GetDecimal(2),
                        Deductions = reader.GetDecimal(3),
                        Tax = reader.GetDecimal(4),
                        Date = reader.GetDateTime(5),
                        CreatedAt = reader.GetDateTime(6),
                        CreatedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                        UpdatedAt = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                        UpdatedBy = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
                    });
                }
            }
        }

        return settlementsList;
    }

    public void UpdateSettlement(SettlementsModel settlement)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                        UPDATE Settlements SET
                            amount_paid = @AmountPaid,
                            deductions = @Deductions,
                            tax = @Tax,
                            date = @Date,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE id = @Id";

                command.Parameters.AddWithValue("@Id", settlement.Id);
                command.Parameters.AddWithValue("@AmountPaid", settlement.AmountPaid);
                command.Parameters.AddWithValue("@Deductions", settlement.Deductions);
                command.Parameters.AddWithValue("@Tax", settlement.Tax);
                command.Parameters.AddWithValue("@Date", settlement.Date);
                command.Parameters.AddWithValue("@UpdatedAt", settlement.UpdatedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", settlement.UpdatedBy ?? string.Empty);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }

    public void DeleteSettlement(int settlementId)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Settlements WHERE id = @Id";
                command.Parameters.AddWithValue("@Id", settlementId);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}

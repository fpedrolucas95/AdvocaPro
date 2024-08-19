using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services;

public class TransactionsService
{
    private readonly DatabaseService _databaseService;

    public TransactionsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void AddTransaction(TransactionsModel transaction)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transactionDb = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                        INSERT INTO Transactions (
                            driver_id, type, amount, date, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @DriverId, @Type, @Amount, @Date, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                command.Parameters.AddWithValue("@DriverId", transaction.DriverId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Type", transaction.Type ?? string.Empty);
                command.Parameters.AddWithValue("@Amount", transaction.Amount);
                command.Parameters.AddWithValue("@Date", transaction.Date);
                command.Parameters.AddWithValue("@CreatedAt", transaction.CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", transaction.CreatedBy ?? string.Empty);
                command.Parameters.AddWithValue("@UpdatedAt", transaction.UpdatedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", transaction.UpdatedBy ?? string.Empty);

                command.ExecuteNonQuery();
                transactionDb.Commit();
            }
        }
    }

    public IEnumerable<TransactionsModel> GetTransactions()
    {
        var transactionsList = new List<TransactionsModel>();

        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Transactions";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactionsList.Add(new TransactionsModel
                    {
                        Id = reader.GetInt32(0),
                        DriverId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                        Type = reader.GetString(2),
                        Amount = reader.GetDouble(3),
                        Date = reader.GetDateTime(4),
                        CreatedAt = reader.GetDateTime(5),
                        CreatedBy = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                        UpdatedAt = reader.IsDBNull(7) ? null : (DateTime?)reader.GetDateTime(7),
                        UpdatedBy = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                    });
                }
            }
        }

        return transactionsList;
    }

    public void UpdateTransaction(TransactionsModel transaction)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transactionDb = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                        UPDATE Transactions SET
                            driver_id = @DriverId,
                            type = @Type,
                            amount = @Amount,
                            date = @Date,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE id = @Id";

                command.Parameters.AddWithValue("@Id", transaction.Id);
                command.Parameters.AddWithValue("@DriverId", transaction.DriverId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Type", transaction.Type ?? string.Empty);
                command.Parameters.AddWithValue("@Amount", transaction.Amount);
                command.Parameters.AddWithValue("@Date", transaction.Date);
                command.Parameters.AddWithValue("@UpdatedAt", transaction.UpdatedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", transaction.UpdatedBy ?? string.Empty);

                command.ExecuteNonQuery();
                transactionDb.Commit();
            }
        }
    }

    public void DeleteTransaction(int transactionId)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transactionDb = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Transactions WHERE id = @Id";
                command.Parameters.AddWithValue("@Id", transactionId);

                command.ExecuteNonQuery();
                transactionDb.Commit();
            }
        }
    }
}

using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class ExpensesService
    {
        private readonly DatabaseService _databaseService;

        public ExpensesService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddExpense(ExpensesModel expense)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO Expenses (
                            description, amount, month, year, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @Description, @Amount, @Month, @Year, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                    command.Parameters.AddWithValue("@Description", expense.Description ?? string.Empty);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Month", expense.Month);
                    command.Parameters.AddWithValue("@Year", expense.Year);
                    command.Parameters.AddWithValue("@CreatedAt", expense.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", expense.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", expense.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", expense.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<ExpensesModel> GetExpenses()
        {
            var expensesList = new List<ExpensesModel>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Expenses";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        expensesList.Add(new ExpensesModel
                        {
                            Id = reader.GetInt32(0),
                            Description = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Amount = reader.GetDouble(2),
                            Month = reader.GetInt32(3),
                            Year = reader.GetInt32(4),
                            CreatedAt = reader.GetDateTime(5),
                            CreatedBy = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            UpdatedAt = reader.IsDBNull(7) ? null : (DateTime?)reader.GetDateTime(7),
                            UpdatedBy = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                        });
                    }
                }
            }

            return expensesList;
        }

        public void UpdateExpense(ExpensesModel expense)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        UPDATE Expenses SET
                            description = @Description,
                            amount = @Amount,
                            month = @Month,
                            year = @Year,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE id = @Id";

                    command.Parameters.AddWithValue("@Id", expense.Id);
                    command.Parameters.AddWithValue("@Description", expense.Description ?? string.Empty);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Month", expense.Month);
                    command.Parameters.AddWithValue("@Year", expense.Year);
                    command.Parameters.AddWithValue("@UpdatedAt", expense.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", expense.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteExpense(int expenseId)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Expenses WHERE id = @Id";
                    command.Parameters.AddWithValue("@Id", expenseId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
    }
}

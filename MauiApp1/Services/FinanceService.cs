using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class FinanceService
    {
        private readonly string _databasePath;

        public FinanceService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddFinance(Finance finance)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO Finance (
                            Date, Type, Category, Description, PaymentMethod, Amount, Received, ClientName, ClientCellPhone,
                            Reference, TotalAgreed, Installments, InstallmentValue, DueDate, ReceivedAmount, PaymentDate, Status,
                            CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                          ) VALUES (
                            @Date, @Type, @Category, @Description, @PaymentMethod, @Amount, @Received, @ClientName, @ClientCellPhone,
                            @Reference, @TotalAgreed, @Installments, @InstallmentValue, @DueDate, @ReceivedAmount, @PaymentDate, @Status,
                            @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                          )";

                    command.Parameters.AddWithValue("@Date", finance.Date);
                    command.Parameters.AddWithValue("@Type", finance.Type);
                    command.Parameters.AddWithValue("@Category", finance.Category);
                    command.Parameters.AddWithValue("@Description", finance.Description);
                    command.Parameters.AddWithValue("@PaymentMethod", finance.PaymentMethod);
                    command.Parameters.AddWithValue("@Amount", finance.Amount);
                    command.Parameters.AddWithValue("@Received", finance.Received ? 1 : 0);
                    command.Parameters.AddWithValue("@ClientName", finance.ClientName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ClientCellPhone", finance.ClientCellPhone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Reference", finance.Reference ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TotalAgreed", finance.TotalAgreed);
                    command.Parameters.AddWithValue("@Installments", finance.Installments);
                    command.Parameters.AddWithValue("@InstallmentValue", finance.InstallmentValue);
                    command.Parameters.AddWithValue("@DueDate", finance.DueDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ReceivedAmount", finance.ReceivedAmount);
                    command.Parameters.AddWithValue("@PaymentDate", finance.PaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", finance.Status);
                    command.Parameters.AddWithValue("@CreatedAt", finance.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", finance.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", finance.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", finance.UpdatedBy ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public void UpdateFinance(Finance finance)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE Finance SET
                            Type = @Type, Category = @Category, Description = @Description, PaymentMethod = @PaymentMethod,
                            Amount = @Amount, Received = @Received, ClientName = @ClientName, ClientCellPhone = @ClientCellPhone,
                            Reference = @Reference, TotalAgreed = @TotalAgreed, Installments = @Installments, InstallmentValue = @InstallmentValue,
                            DueDate = @DueDate, ReceivedAmount = @ReceivedAmount, PaymentDate = @PaymentDate, Status = @Status,
                            CreatedAt = @CreatedAt, CreatedBy = @CreatedBy, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                            WHERE Date = @Date";

                    command.Parameters.AddWithValue("@Date", finance.Date);
                    command.Parameters.AddWithValue("@Type", finance.Type);
                    command.Parameters.AddWithValue("@Category", finance.Category);
                    command.Parameters.AddWithValue("@Description", finance.Description);
                    command.Parameters.AddWithValue("@PaymentMethod", finance.PaymentMethod);
                    command.Parameters.AddWithValue("@Amount", finance.Amount);
                    command.Parameters.AddWithValue("@Received", finance.Received ? 1 : 0);
                    command.Parameters.AddWithValue("@ClientName", finance.ClientName);
                    command.Parameters.AddWithValue("@ClientCellPhone", finance.ClientCellPhone);
                    command.Parameters.AddWithValue("@Reference", finance.Reference);
                    command.Parameters.AddWithValue("@TotalAgreed", finance.TotalAgreed);
                    command.Parameters.AddWithValue("@Installments", finance.Installments);
                    command.Parameters.AddWithValue("@InstallmentValue", finance.InstallmentValue);
                    command.Parameters.AddWithValue("@DueDate", finance.DueDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ReceivedAmount", finance.ReceivedAmount);
                    command.Parameters.AddWithValue("@PaymentDate", finance.PaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", finance.Status);
                    command.Parameters.AddWithValue("@CreatedAt", finance.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", finance.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedAt", finance.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", finance.UpdatedBy);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteFinance(DateTime date)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Finance WHERE Date = @Date";
                    command.Parameters.AddWithValue("@Date", date);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<Finance> GetFinances()
        {
            var finances = new List<Finance>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Finance";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var finance = new Finance
                        {
                            Date = reader.GetDateTime(0),
                            Type = reader.GetString(1),
                            Category = reader.GetString(2),
                            Description = reader.GetString(3),
                            PaymentMethod = reader.GetString(4),
                            Amount = reader.GetDecimal(5),
                            Received = reader.GetInt32(6) == 1,
                            ClientName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            ClientCellPhone = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            Reference = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            TotalAgreed = reader.GetDecimal(10),
                            Installments = reader.GetInt32(11),
                            InstallmentValue = reader.GetDecimal(12),
                            DueDate = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
                            ReceivedAmount = reader.GetDecimal(14),
                            PaymentDate = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                            Status = reader.GetString(16),
                            CreatedAt = reader.GetDateTime(17),
                            CreatedBy = reader.IsDBNull(18) ? string.Empty : reader.GetString(18),
                            UpdatedAt = reader.IsDBNull(19) ? null : reader.GetDateTime(19),
                            UpdatedBy = reader.IsDBNull(20) ? string.Empty : reader.GetString(20)
                        };

                        finances.Add(finance);
                    }
                }
            }

            return finances;
        }
    }
}

using AdvocaPro.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace AdvocaPro.Services
{
    public class PaymentsService
    {
        private readonly DatabaseService _databaseService;

        public PaymentsService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddPayment(PaymentsModel payment)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO Payments (
                            driver_id, amount_paid, payment_date, description, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @DriverId, @AmountPaid, @PaymentDate, @Description, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                    command.Parameters.AddWithValue("@DriverId", payment.DriverId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AmountPaid", payment.AmountPaid);
                    command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                    command.Parameters.AddWithValue("@Description", payment.Description ?? string.Empty);
                    command.Parameters.AddWithValue("@CreatedAt", payment.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", payment.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", payment.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", payment.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<PaymentsModel> GetPayments()
        {
            var paymentsList = new List<PaymentsModel>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Payments";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        paymentsList.Add(new PaymentsModel
                        {
                            Id = reader.GetInt32(0),
                            DriverId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                            AmountPaid = reader.GetDouble(2),
                            PaymentDate = reader.GetDateTime(3),
                            Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5),
                            CreatedBy = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            UpdatedAt = reader.IsDBNull(7) ? null : (DateTime?)reader.GetDateTime(7),
                            UpdatedBy = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                        });
                    }
                }
            }

            return paymentsList;
        }

        public void UpdatePayment(PaymentsModel payment)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        UPDATE Payments SET
                            driver_id = @DriverId,
                            amount_paid = @AmountPaid,
                            payment_date = @PaymentDate,
                            description = @Description,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE id = @Id";

                    command.Parameters.AddWithValue("@Id", payment.Id);
                    command.Parameters.AddWithValue("@DriverId", payment.DriverId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AmountPaid", payment.AmountPaid);
                    command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                    command.Parameters.AddWithValue("@Description", payment.Description ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", payment.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", payment.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeletePayment(int paymentId)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Payments WHERE id = @Id";
                    command.Parameters.AddWithValue("@Id", paymentId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
    }
}

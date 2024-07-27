using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class PaymentMethodService
    {
        private readonly string _databasePath;

        public PaymentMethodService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddPaymentMethod(PaymentMethod paymentMethod)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO PaymentMethods (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", paymentMethod.Name);
                command.ExecuteNonQuery();
            }
        }

        public void UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE PaymentMethods SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", paymentMethod.Name);
                command.Parameters.AddWithValue("@Id", paymentMethod.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeletePaymentMethod(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM PaymentMethods WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<PaymentMethod> GetPaymentMethods()
        {
            var paymentMethods = new List<PaymentMethod>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM PaymentMethods";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        paymentMethods.Add(new PaymentMethod
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return paymentMethods;
        }
    }
}

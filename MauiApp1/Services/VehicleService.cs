using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services;

public class VehicleService
{
    private readonly DatabaseService _databaseService;

    public VehicleService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void AddVehicle(VehicleModel vehicle)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                        INSERT INTO Vehicle (
                            registration, owner_id, insurance, remarks, available, owner_bank_name, 
                            owner_account_number, owner_iban, owner_phone, owner_email, owner_nif, 
                            owner_driving_license, created_at, created_by, updated_at, updated_by
                        ) VALUES (
                            @Registration, @OwnerId, @Insurance, @Remarks, @Available, @OwnerBankName, 
                            @OwnerAccountNumber, @OwnerIban, @OwnerPhone, @OwnerEmail, @OwnerNif, 
                            @OwnerDrivingLicense, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        )";

                command.Parameters.AddWithValue("@Registration", vehicle.Registration);
                command.Parameters.AddWithValue("@OwnerId", vehicle.OwnerId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Insurance", vehicle.Insurance ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Remarks", vehicle.Remarks ?? string.Empty);
                command.Parameters.AddWithValue("@Available", vehicle.Available);
                command.Parameters.AddWithValue("@OwnerBankName", vehicle.OwnerBankName ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerAccountNumber", vehicle.OwnerAccountNumber ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerIban", vehicle.OwnerIban ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerPhone", vehicle.OwnerPhone ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerEmail", vehicle.OwnerEmail ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerNif", vehicle.OwnerNif ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerDrivingLicense", vehicle.OwnerDrivingLicense ?? string.Empty);
                command.Parameters.AddWithValue("@CreatedAt", vehicle.CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", vehicle.CreatedBy ?? string.Empty);
                command.Parameters.AddWithValue("@UpdatedAt", vehicle.UpdatedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", vehicle.UpdatedBy ?? string.Empty);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }

    public IEnumerable<VehicleModel> GetVehicles()
    {
        var vehiclesList = new List<VehicleModel>();

        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Vehicle";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehiclesList.Add(new VehicleModel
                    {
                        Registration = reader.GetString(0),
                        OwnerId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                        Insurance = reader.IsDBNull(2) ? (double?)null : reader.GetDouble(2),
                        Remarks = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Available = reader.GetBoolean(4),
                        OwnerBankName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        OwnerAccountNumber = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                        OwnerIban = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                        OwnerPhone = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                        OwnerEmail = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                        OwnerNif = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                        OwnerDrivingLicense = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                        CreatedAt = reader.GetDateTime(12),
                        CreatedBy = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                        UpdatedAt = reader.IsDBNull(14) ? null : (DateTime?)reader.GetDateTime(14),
                        UpdatedBy = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                    });
                }
            }
        }

        return vehiclesList;
    }

    public void UpdateVehicle(VehicleModel vehicle)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                        UPDATE Vehicle SET
                            owner_id = @OwnerId,
                            insurance = @Insurance,
                            remarks = @Remarks,
                            available = @Available,
                            owner_bank_name = @OwnerBankName,
                            owner_account_number = @OwnerAccountNumber,
                            owner_iban = @OwnerIban,
                            owner_phone = @OwnerPhone,
                            owner_email = @OwnerEmail,
                            owner_nif = @OwnerNif,
                            owner_driving_license = @OwnerDrivingLicense,
                            updated_at = @UpdatedAt,
                            updated_by = @UpdatedBy
                        WHERE registration = @Registration";

                command.Parameters.AddWithValue("@Registration", vehicle.Registration);
                command.Parameters.AddWithValue("@OwnerId", vehicle.OwnerId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Insurance", vehicle.Insurance ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Remarks", vehicle.Remarks ?? string.Empty);
                command.Parameters.AddWithValue("@Available", vehicle.Available);
                command.Parameters.AddWithValue("@OwnerBankName", vehicle.OwnerBankName ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerAccountNumber", vehicle.OwnerAccountNumber ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerIban", vehicle.OwnerIban ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerPhone", vehicle.OwnerPhone ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerEmail", vehicle.OwnerEmail ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerNif", vehicle.OwnerNif ?? string.Empty);
                command.Parameters.AddWithValue("@OwnerDrivingLicense", vehicle.OwnerDrivingLicense ?? string.Empty);
                command.Parameters.AddWithValue("@UpdatedAt", vehicle.UpdatedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", vehicle.UpdatedBy ?? string.Empty);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }

    public IEnumerable<VehicleModel> GetAvailableVehicles()
    {
        var vehiclesList = new List<VehicleModel>();

        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT registration, available FROM Vehicle WHERE available = 1";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehiclesList.Add(new VehicleModel
                    {
                        Registration = reader.GetString(0),
                        Available = reader.GetBoolean(1),
                    });
                }
            }
        }

        return vehiclesList;
    }

    public void DeleteVehicle(string registration)
    {
        using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Vehicle WHERE registration = @Registration";
                command.Parameters.AddWithValue("@Registration", registration);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}

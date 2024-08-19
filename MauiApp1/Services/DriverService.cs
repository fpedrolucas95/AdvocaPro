using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class DriverService
    {
        private readonly DatabaseService _databaseService;

        public DriverService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddDriver(DriverModel driver)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                INSERT INTO Driver (
                    name, contract_percentage, vacation_start, vacation_days, remarks, prio_card, 
                    vehicle_registration, start_date, end_date, phone, email, nif, driving_license, 
                    bank_name, account_number, iban, bolt_uid, uber_uid, created_at, created_by, updated_at, updated_by
                ) VALUES (
                    @Name, @ContractPercentage, @VacationStart, @VacationDays, @Remarks, @PrioCard, 
                    @VehicleRegistration, @StartDate, @EndDate, @Phone, @Email, @Nif, @DrivingLicense, 
                    @BankName, @AccountNumber, @Iban, @BoltUid, @UberUid, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                )";

                    command.Parameters.AddWithValue("@Name", driver.Name);
                    command.Parameters.AddWithValue("@ContractPercentage", driver.ContractPercentage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VacationStart", driver.VacationStart ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VacationDays", driver.VacationDays ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Remarks", driver.Remarks ?? string.Empty);
                    command.Parameters.AddWithValue("@PrioCard", driver.PrioCard ?? string.Empty);
                    command.Parameters.AddWithValue("@VehicleRegistration", driver.VehicleRegistration ?? string.Empty);
                    command.Parameters.AddWithValue("@StartDate", driver.StartDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", driver.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", driver.Phone ?? string.Empty);
                    command.Parameters.AddWithValue("@Email", driver.Email ?? string.Empty);
                    command.Parameters.AddWithValue("@Nif", driver.Nif ?? string.Empty);
                    command.Parameters.AddWithValue("@DrivingLicense", driver.DrivingLicense ?? string.Empty);
                    command.Parameters.AddWithValue("@BankName", driver.BankName ?? string.Empty);
                    command.Parameters.AddWithValue("@AccountNumber", driver.AccountNumber ?? string.Empty);
                    command.Parameters.AddWithValue("@Iban", driver.Iban ?? string.Empty);
                    command.Parameters.AddWithValue("@BoltUid", driver.BoltUid ?? string.Empty);
                    command.Parameters.AddWithValue("@UberUid", driver.UberUid ?? string.Empty);
                    command.Parameters.AddWithValue("@CreatedAt", driver.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", driver.CreatedBy ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", driver.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", driver.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<DriverModel> GetDrivers()
        {
            var drivers = new List<DriverModel>();

            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Driver";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drivers.Add(new DriverModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ContractPercentage = reader.IsDBNull(2) ? null : (double?)reader.GetDouble(2),
                            VacationStart = reader.IsDBNull(3) ? null : (DateTime?)reader.GetDateTime(3),
                            VacationDays = reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4),
                            Remarks = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            PrioCard = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            VehicleRegistration = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            StartDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                            EndDate = reader.IsDBNull(9) ? null : (DateTime?)reader.GetDateTime(9),
                            Phone = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                            Email = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                            Nif = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            DrivingLicense = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            BankName = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            AccountNumber = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            Iban = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                            BoltUid = reader.IsDBNull(17) ? string.Empty : reader.GetString(17),
                            UberUid = reader.IsDBNull(18) ? string.Empty : reader.GetString(18),
                            CreatedAt = reader.GetDateTime(19),
                            CreatedBy = reader.IsDBNull(20) ? string.Empty : reader.GetString(20),
                            UpdatedAt = reader.IsDBNull(21) ? null : (DateTime?)reader.GetDateTime(21),
                            UpdatedBy = reader.IsDBNull(22) ? string.Empty : reader.GetString(22)
                        });
                    }
                }
            }

            return drivers;
        }

        public void UpdateDriver(DriverModel driver)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                UPDATE Driver SET
                    name = @Name,
                    contract_percentage = @ContractPercentage,
                    vacation_start = @VacationStart,
                    vacation_days = @VacationDays,
                    remarks = @Remarks,
                    prio_card = @PrioCard,
                    vehicle_registration = @VehicleRegistration,
                    start_date = @StartDate,
                    end_date = @EndDate,
                    phone = @Phone,
                    email = @Email,
                    nif = @Nif,
                    driving_license = @DrivingLicense,
                    bank_name = @BankName,
                    account_number = @AccountNumber,
                    iban = @Iban,
                    bolt_uid = @BoltUid,
                    uber_uid = @UberUid,
                    updated_at = @UpdatedAt,
                    updated_by = @UpdatedBy
                WHERE id = @Id";

                    command.Parameters.AddWithValue("@Id", driver.Id);
                    command.Parameters.AddWithValue("@Name", driver.Name);
                    command.Parameters.AddWithValue("@ContractPercentage", driver.ContractPercentage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VacationStart", driver.VacationStart ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VacationDays", driver.VacationDays ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Remarks", driver.Remarks ?? string.Empty);
                    command.Parameters.AddWithValue("@PrioCard", driver.PrioCard ?? string.Empty);
                    command.Parameters.AddWithValue("@VehicleRegistration", driver.VehicleRegistration ?? string.Empty);
                    command.Parameters.AddWithValue("@StartDate", driver.StartDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", driver.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", driver.Phone ?? string.Empty);
                    command.Parameters.AddWithValue("@Email", driver.Email ?? string.Empty);
                    command.Parameters.AddWithValue("@Nif", driver.Nif ?? string.Empty);
                    command.Parameters.AddWithValue("@DrivingLicense", driver.DrivingLicense ?? string.Empty);
                    command.Parameters.AddWithValue("@BankName", driver.BankName ?? string.Empty);
                    command.Parameters.AddWithValue("@AccountNumber", driver.AccountNumber ?? string.Empty);
                    command.Parameters.AddWithValue("@Iban", driver.Iban ?? string.Empty);
                    command.Parameters.AddWithValue("@BoltUid", driver.BoltUid ?? string.Empty);
                    command.Parameters.AddWithValue("@UberUid", driver.UberUid ?? string.Empty);
                    command.Parameters.AddWithValue("@UpdatedAt", driver.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", driver.UpdatedBy ?? string.Empty);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteDriver(int driverId)
        {
            using (var connection = new SqliteConnection($"Filename={_databaseService.GetDatabasePath()}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Driver WHERE id = @Id";
                    command.Parameters.AddWithValue("@Id", driverId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
    }
}

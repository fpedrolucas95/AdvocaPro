using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class ClientService
    {
        private readonly string _databasePath;

        public ClientService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddClient(Client client)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO Clients (
                Name, ProcessNumber, ProcessType, IsActive, CPF, Phone, CellPhone, Email, Address,
                City, State, ZipCode, Detail, Reminder, Action, Date, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
                LeadObs, StartPeriod, Days, Count, Completed, BirthdayDate, Age, MaritalStatus, Company, Profession,
                Spouse, ChildrenJson, Nationality, ParentsJson, RG, Registry, LawyerId
            ) VALUES (
                @Name, @ProcessNumber, @ProcessType, @IsActive, @CPF, @Phone, @CellPhone, @Email, @Address,
                @City, @State, @ZipCode, @Detail, @Reminder, @Action, @Date, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy,
                @LeadObs, @StartPeriod, @Days, @Count, @Completed, @BirthdayDate, @Age, @MaritalStatus, @Company, @Profession,
                @Spouse, @ChildrenJson, @Nationality, @ParentsJson, @RG, @Registry, @LawyerId
            )";

                    command.Parameters.AddWithValue("@Name", client.Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProcessNumber", client.ProcessNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProcessType", client.ProcessType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", client.IsActive ? 1 : 0);
                    command.Parameters.AddWithValue("@CPF", client.CPF ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", client.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CellPhone", client.CellPhone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", client.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", client.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@City", client.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@State", client.State ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ZipCode", client.ZipCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Detail", client.Detail ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Reminder", client.Reminder ? 1 : 0);
                    command.Parameters.AddWithValue("@Action", client.Action ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Date", client.Date ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", client.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", client.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", client.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", client.UpdatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LeadObs", client.LeadObs ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StartPeriod", client.StartPeriod ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Days", client.Days);
                    command.Parameters.AddWithValue("@Count", client.Count);
                    command.Parameters.AddWithValue("@Completed", client.Completed ? 1 : 0);
                    command.Parameters.AddWithValue("@BirthdayDate", client.BirthdayDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Age", client.Age);
                    command.Parameters.AddWithValue("@MaritalStatus", client.MaritalStatus ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Company", client.Company ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Profession", client.Profession ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Spouse", client.Spouse ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ChildrenJson", client.ChildrenJson ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Nationality", client.Nationality ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ParentsJson", client.ParentsJson ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RG", client.RG ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Registry", client.Registry ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LawyerId", client.LawyerId ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    var getIdCommand = connection.CreateCommand();
                    getIdCommand.CommandText = "SELECT last_insert_rowid()";
                    client.Id = Convert.ToInt32(getIdCommand.ExecuteScalar() ?? 0);

                    transaction.Commit();
                }
            }
        }

        public void UpdateClient(Client client)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE Clients SET
                    Name = @Name, ProcessNumber = @ProcessNumber, ProcessType = @ProcessType, IsActive = @IsActive, CPF = @CPF,
                    Phone = @Phone, CellPhone = @CellPhone, Email = @Email, Address = @Address, City = @City, State = @State, ZipCode = @ZipCode,
                    Detail = @Detail, Reminder = @Reminder, Action = @Action, Date = @Date, CreatedAt = @CreatedAt, CreatedBy = @CreatedBy,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy, LeadObs = @LeadObs, StartPeriod = @StartPeriod, Days = @Days, Count = @Count,
                    Completed = @Completed, BirthdayDate = @BirthdayDate, Age = @Age, MaritalStatus = @MaritalStatus, Company = @Company,
                    Profession = @Profession, Spouse = @Spouse, ChildrenJson = @ChildrenJson, Nationality = @Nationality, ParentsJson = @ParentsJson,
                    RG = @RG, Registry = @Registry, LawyerId = @LawyerId WHERE Id = @Id";

                    command.Parameters.AddWithValue("@Name", client.Name);
                    command.Parameters.AddWithValue("@ProcessNumber", client.ProcessNumber);
                    command.Parameters.AddWithValue("@ProcessType", client.ProcessType);
                    command.Parameters.AddWithValue("@IsActive", client.IsActive ? 1 : 0);
                    command.Parameters.AddWithValue("@CPF", client.CPF);
                    command.Parameters.AddWithValue("@Phone", client.Phone);
                    command.Parameters.AddWithValue("@CellPhone", client.CellPhone);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    command.Parameters.AddWithValue("@Address", client.Address);
                    command.Parameters.AddWithValue("@City", client.City);
                    command.Parameters.AddWithValue("@State", client.State);
                    command.Parameters.AddWithValue("@ZipCode", client.ZipCode);
                    command.Parameters.AddWithValue("@Detail", client.Detail);
                    command.Parameters.AddWithValue("@Reminder", client.Reminder ? 1 : 0);
                    command.Parameters.AddWithValue("@Action", client.Action);
                    command.Parameters.AddWithValue("@Date", client.Date ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", client.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", client.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedAt", client.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", client.UpdatedBy);
                    command.Parameters.AddWithValue("@LeadObs", client.LeadObs);
                    command.Parameters.AddWithValue("@StartPeriod", client.StartPeriod ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Days", client.Days);
                    command.Parameters.AddWithValue("@Count", client.Count);
                    command.Parameters.AddWithValue("@Completed", client.Completed ? 1 : 0);
                    command.Parameters.AddWithValue("@BirthdayDate", client.BirthdayDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Age", client.Age);
                    command.Parameters.AddWithValue("@MaritalStatus", client.MaritalStatus);
                    command.Parameters.AddWithValue("@Company", client.Company);
                    command.Parameters.AddWithValue("@Profession", client.Profession);
                    command.Parameters.AddWithValue("@Spouse", client.Spouse);
                    command.Parameters.AddWithValue("@ChildrenJson", client.ChildrenJson);
                    command.Parameters.AddWithValue("@Nationality", client.Nationality);
                    command.Parameters.AddWithValue("@ParentsJson", client.ParentsJson);
                    command.Parameters.AddWithValue("@RG", client.RG);
                    command.Parameters.AddWithValue("@Registry", client.Registry);
                    command.Parameters.AddWithValue("@LawyerId", client.LawyerId);
                    command.Parameters.AddWithValue("@Id", client.Id);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteClient(int clientId)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var deleteClientCommand = connection.CreateCommand();
                        deleteClientCommand.CommandText = "DELETE FROM Clients WHERE Id = @Id";
                        deleteClientCommand.Parameters.AddWithValue("@Id", clientId);

                        int rowsAffected = deleteClientCommand.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Client not found or could not be deleted.");
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting client: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<Client> GetClients()
        {
            var clients = new List<Client>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Clients";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var client = new Client
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ProcessNumber = reader.GetString(2),
                            ProcessType = reader.GetString(3),
                            IsActive = reader.GetInt32(4) == 1,
                            CPF = reader.GetString(5),
                            Phone = reader.GetString(6),
                            CellPhone = reader.GetString(7),
                            Email = reader.GetString(8),
                            Address = reader.GetString(9),
                            City = reader.GetString(10),
                            State = reader.GetString(11),
                            ZipCode = reader.GetString(12),
                            Detail = reader.GetString(13),
                            Reminder = reader.GetInt32(14) == 1,
                            Action = reader.GetString(15),
                            Date = reader.IsDBNull(16) ? (DateTime?)null : reader.GetDateTime(16),
                            CreatedAt = reader.GetDateTime(17),
                            CreatedBy = reader.GetString(18),
                            UpdatedAt = reader.IsDBNull(19) ? (DateTime?)null : reader.GetDateTime(19),
                            UpdatedBy = reader.GetString(20),
                            LeadObs = reader.GetString(21),
                            StartPeriod = reader.IsDBNull(22) ? (DateTime?)null : reader.GetDateTime(22),
                            Days = reader.GetInt32(23),
                            Count = reader.GetInt32(24),
                            Completed = reader.GetInt32(25) == 1,
                            BirthdayDate = reader.IsDBNull(26) ? (DateTime?)null : reader.GetDateTime(26),
                            Age = reader.GetInt32(27),
                            MaritalStatus = reader.GetString(28),
                            Company = reader.GetString(29),
                            Profession = reader.GetString(30),
                            Spouse = reader.GetString(31),
                            ChildrenJson = reader.GetString(32),
                            Nationality = reader.GetString(33),
                            ParentsJson = reader.GetString(34),
                            RG = reader.GetString(35),
                            Registry = reader.GetString(36)
                        };

                        clients.Add(client);
                    }
                }
            }

            return clients;
        }
    }
}

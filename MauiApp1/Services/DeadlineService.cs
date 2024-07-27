using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class DeadlineService
    {
        private readonly string _databasePath;

        public DeadlineService(string databasePath)
        {
            _databasePath = databasePath;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();

            var tableCommand = connection.CreateCommand();
            tableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT,
                    ProcessNumber TEXT,
                    ProcessType TEXT,
                    IsActive INTEGER,
                    CPF TEXT,
                    Phone TEXT,
                    CellPhone TEXT,
                    Email TEXT,
                    Street TEXT,
                    Number TEXT,
                    Complement TEXT,
                    City TEXT,
                    State TEXT,
                    ZipCode TEXT,
                    Detail TEXT,
                    Reminder INTEGER,
                    Action TEXT,
                    Date TEXT,
                    CreatedAt TEXT,
                    CreatedBy TEXT,
                    UpdatedAt TEXT,
                    UpdatedBy TEXT,
                    LeadObs TEXT,
                    StartPeriod TEXT,
                    Days INTEGER,
                    Count INTEGER,
                    Completed INTEGER,
                    Court TEXT,
                    Opponent TEXT,
                    CaseDetails TEXT,
                    CaseObservation TEXT,
                    CaseCompleted INTEGER,
                    BirthdayDate TEXT,
                    Age INTEGER,
                    MaritalStatus TEXT,
                    Company TEXT,
                    Profession TEXT,
                    Spouse TEXT,
                    ChildrenJson TEXT,
                    Nationality TEXT,
                    ParentsJson TEXT,
                    RG TEXT,
                    Registry TEXT,
                    LawyerId TEXT
                );

                CREATE TABLE IF NOT EXISTS Deadline (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    StartDate TEXT,
                    DurationDays INTEGER,
                    IsBusinessDays INTEGER,
                    EndDate TEXT,
                    IsCompleted INTEGER,
                    Status TEXT,
                    Details TEXT,
                    CreatedAt TEXT,
                    CreatedBy TEXT,
                    UpdatedAt TEXT,
                    UpdatedBy TEXT,
                    FOREIGN KEY(ClientId) REFERENCES Clients(Id)
                );
            ";
            tableCommand.ExecuteNonQuery();
        }

        public void AddDeadline(Deadline deadline)
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();

            if (deadline.ClientId <= 0)
            {
                throw new ArgumentException("ClientId must be a positive integer.", nameof(deadline.ClientId));
            }

            var clientCheckCommand = connection.CreateCommand();
            clientCheckCommand.CommandText = "SELECT COUNT(*) FROM Clients WHERE Id = @ClientId";
            clientCheckCommand.Parameters.AddWithValue("@ClientId", deadline.ClientId);

            var clientCount = clientCheckCommand.ExecuteScalar();
            if (clientCount is null || Convert.ToInt64(clientCount) <= 0)
            {
                throw new Exception($"Client with Id {deadline.ClientId} does not exist.");
            }

            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Deadline (
                    ClientId, StartDate, DurationDays, IsBusinessDays, EndDate, 
                    IsCompleted, Status, Details, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                ) VALUES (
                    @ClientId, @StartDate, @DurationDays, @IsBusinessDays, @EndDate, 
                    @IsCompleted, @Status, @Details, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                )";

            AddParametersToCommand(command, deadline);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void UpdateDeadline(Deadline deadline)
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Deadline SET
                    ClientId = @ClientId, StartDate = @StartDate, DurationDays = @DurationDays, 
                    IsBusinessDays = @IsBusinessDays, EndDate = @EndDate, IsCompleted = @IsCompleted, 
                    Status = @Status, Details = @Details, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            AddParametersToCommand(command, deadline);
            command.Parameters.AddWithValue("@Id", deadline.Id);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void DeleteDeadline(int id)
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Deadline WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public IEnumerable<Deadline> GetDeadlines()
        {
            var deadlines = new List<Deadline>();

            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT d.*, c.Name as ClientName, c.ProcessNumber 
                FROM Deadline d
                LEFT JOIN Clients c ON d.ClientId = c.Id";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var deadline = ReadDeadlineFromReader(reader);
                deadline.Client = new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ClientId")),
                    Name = reader.GetString(reader.GetOrdinal("ClientName")),
                    ProcessNumber = reader.GetString(reader.GetOrdinal("ProcessNumber"))
                };
                deadlines.Add(deadline);
            }

            return deadlines;
        }

        private void AddParametersToCommand(SqliteCommand command, Deadline deadline)
        {
            command.Parameters.AddWithValue("@ClientId", deadline.ClientId);
            command.Parameters.AddWithValue("@StartDate", deadline.StartDate);
            command.Parameters.AddWithValue("@DurationDays", deadline.DurationDays);
            command.Parameters.AddWithValue("@IsBusinessDays", deadline.IsBusinessDays ? 1 : 0);
            command.Parameters.AddWithValue("@EndDate", deadline.EndDate);
            command.Parameters.AddWithValue("@IsCompleted", deadline.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@Status", deadline.Status);
            command.Parameters.AddWithValue("@Details", deadline.Details ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", deadline.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", deadline.CreatedBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", deadline.UpdatedAt ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", deadline.UpdatedBy ?? (object)DBNull.Value);
        }

        private Deadline ReadDeadlineFromReader(SqliteDataReader reader)
        {
            return new Deadline
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ClientId = reader.GetInt32(reader.GetOrdinal("ClientId")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                DurationDays = reader.GetInt32(reader.GetOrdinal("DurationDays")),
                IsBusinessDays = reader.GetInt32(reader.GetOrdinal("IsBusinessDays")) == 1,
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                IsCompleted = reader.GetInt32(reader.GetOrdinal("IsCompleted")) == 1,
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Details = reader.IsDBNull(reader.GetOrdinal("Details")) ? string.Empty : reader.GetString(reader.GetOrdinal("Details")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}

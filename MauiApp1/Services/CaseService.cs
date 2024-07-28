using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class CaseService
    {
        private readonly string _databasePath;

        public CaseService(string databasePath)
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
                CREATE TABLE IF NOT EXISTS Cases (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    Court TEXT,
                    Opponent TEXT,
                    CaseDetails TEXT,
                    CaseObservation TEXT,
                    CaseCompleted INTEGER,
                    CreatedAt TEXT,
                    CreatedBy TEXT,
                    UpdatedAt TEXT,
                    UpdatedBy TEXT,
                    FOREIGN KEY(ClientId) REFERENCES Clients(Id)
                );
            ";
            tableCommand.ExecuteNonQuery();
        }

        public void AddCase(Case caseRecord)
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();

            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO Cases (
            ClientId, Court, Opponent, CaseDetails, CaseObservation, 
            CaseCompleted, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
        ) VALUES (
            @ClientId, @Court, @Opponent, @CaseDetails, @CaseObservation, 
            @CaseCompleted, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
        )";

            AddParametersToCommand(command, caseRecord);
            command.ExecuteNonQuery();

            var clientCommand = connection.CreateCommand();
            clientCommand.CommandText = @"
        UPDATE Clients
        SET ProcessNumber = @ProcessNumber, ProcessType = @ProcessType
        WHERE Id = @ClientId";

            clientCommand.Parameters.AddWithValue("@ProcessNumber", caseRecord.Client.ProcessNumber);
            clientCommand.Parameters.AddWithValue("@ProcessType", caseRecord.Client.ProcessType);
            clientCommand.Parameters.AddWithValue("@ClientId", caseRecord.ClientId);
            clientCommand.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdateCase(Case caseRecord)
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();

            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = @"
        UPDATE Cases SET
            ClientId = @ClientId, Court = @Court, Opponent = @Opponent, 
            CaseDetails = @CaseDetails, CaseObservation = @CaseObservation, 
            CaseCompleted = @CaseCompleted, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
        WHERE Id = @Id";

            AddParametersToCommand(command, caseRecord);
            command.Parameters.AddWithValue("@Id", caseRecord.Id);
            command.ExecuteNonQuery();

            var clientCommand = connection.CreateCommand();
            clientCommand.CommandText = @"
        UPDATE Clients
        SET ProcessNumber = @ProcessNumber, ProcessType = @ProcessType
        WHERE Id = @ClientId";

            clientCommand.Parameters.AddWithValue("@ProcessNumber", caseRecord.Client.ProcessNumber);
            clientCommand.Parameters.AddWithValue("@ProcessType", caseRecord.Client.ProcessType);
            clientCommand.Parameters.AddWithValue("@ClientId", caseRecord.ClientId);
            clientCommand.ExecuteNonQuery();

            transaction.Commit();
        }

        public void DeleteCase(int id)
        {
            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();

            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Cases WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public IEnumerable<Case> GetCases()
        {
            var cases = new List<Case>();

            using var connection = new SqliteConnection($"Filename={_databasePath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.*, cl.Name as ClientName, cl.ProcessNumber, cl.ProcessType
                FROM Cases c
                LEFT JOIN Clients cl ON c.ClientId = cl.Id";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var caseRecord = new Case
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ClientId = reader.GetInt32(reader.GetOrdinal("ClientId")),
                    Client = new Client
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ClientId")),
                        Name = reader.GetString(reader.GetOrdinal("ClientName")),
                        ProcessNumber = reader.GetString(reader.GetOrdinal("ProcessNumber")),
                        ProcessType = reader.GetString(reader.GetOrdinal("ProcessType"))
                    },
                    Court = reader.GetString(reader.GetOrdinal("Court")),
                    Opponent = reader.GetString(reader.GetOrdinal("Opponent")),
                    CaseDetails = reader.IsDBNull(reader.GetOrdinal("CaseDetails")) ? string.Empty : reader.GetString(reader.GetOrdinal("CaseDetails")),
                    CaseObservation = reader.IsDBNull(reader.GetOrdinal("CaseObservation")) ? string.Empty : reader.GetString(reader.GetOrdinal("CaseObservation")),
                    CaseCompleted = reader.GetInt32(reader.GetOrdinal("CaseCompleted")) == 1,
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("UpdatedBy"))
                };

                cases.Add(caseRecord);
            }

            return cases;
        }

        private void AddParametersToCommand(SqliteCommand command, Case caseRecord)
        {
            command.Parameters.AddWithValue("@ClientId", caseRecord.ClientId);
            command.Parameters.AddWithValue("@Court", caseRecord.Court);
            command.Parameters.AddWithValue("@Opponent", caseRecord.Opponent);
            command.Parameters.AddWithValue("@CaseDetails", caseRecord.CaseDetails ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CaseObservation", caseRecord.CaseObservation ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CaseCompleted", caseRecord.CaseCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@CreatedAt", caseRecord.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", caseRecord.CreatedBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", caseRecord.UpdatedAt ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", caseRecord.UpdatedBy ?? (object)DBNull.Value);
        }
    }
}

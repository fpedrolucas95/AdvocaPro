using System.IO;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace AdvocaPro.Services
{
    public class DatabaseService
    {
        private readonly string _databaseDirectory;
        private readonly string _databasePath;

        public DatabaseService()
        {
            // Define the directory and path for the SQLite database file
            _databaseDirectory = Path.Combine(ApplicationData.Current.LocalFolder.Path, "FrotaApp", "database");
            _databasePath = Path.Combine(_databaseDirectory, "sqlite.db");
            EnsureDatabaseDirectoryExists(); // Ensure the database directory is present
            EnsureDatabaseFileExists();      // Ensure the database file is present
            EnsureTablesExist();             // Ensure all required tables are created
        }

        private void EnsureDatabaseDirectoryExists()
        {
            // Create the database directory if it does not exist
            if (!Directory.Exists(_databaseDirectory))
            {
                Directory.CreateDirectory(_databaseDirectory);
            }
        }

        private void EnsureDatabaseFileExists()
        {
            // Create an empty database file if it does not exist
            if (!File.Exists(_databasePath))
            {
                using (var connection = new SqliteConnection($"Filename={_databasePath}"))
                {
                    connection.Open();
                }
            }
        }

        private void EnsureTablesExist()
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();

                // Check if the 'User' table exists; if not, create all tables
                var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='User';";
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    // Table 'User' does not exist, so create all tables
                    CreateTables(connection);
                }
            }
        }

        private void CreateTables(SqliteConnection connection)
        {
            var createUserTable = @"
                CREATE TABLE IF NOT EXISTS User (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL,
                    first_name TEXT,
                    last_name TEXT,
                    phone TEXT,
                    cell_phone TEXT,
                    user_type INTEGER,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT,
                    last_edit_computer TEXT,
                    birthday_date DATE,
                    age INTEGER,
                    email TEXT
                );";

            var createDriverTable = @"
                CREATE TABLE IF NOT EXISTS Driver (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    contract_percentage REAL,
                    vacation_start DATE,
                    vacation_days INTEGER,
                    remarks TEXT,
                    prio_card TEXT,
                    vehicle_registration TEXT,
                    start_date DATE,
                    end_date DATE,
                    phone TEXT,
                    email TEXT,
                    nif TEXT,
                    driving_license TEXT,
                    bank_name TEXT,
                    account_number TEXT,
                    iban TEXT,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT
                );";

            var createVehicleTable = @"
                CREATE TABLE IF NOT EXISTS Vehicle (
                    registration TEXT PRIMARY KEY,
                    owner_id INTEGER,
                    insurance REAL,
                    remarks TEXT,
                    available BOOLEAN DEFAULT 1,
                    owner_bank_name TEXT,
                    owner_account_number TEXT,
                    owner_iban TEXT,
                    owner_phone TEXT,
                    owner_email TEXT,
                    owner_nif TEXT,
                    owner_driving_license TEXT,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT,
                    FOREIGN KEY (owner_id) REFERENCES Driver(id)
                );";

            var createPrioCardTable = @"
                CREATE TABLE IF NOT EXISTS PrioCard (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    card_number TEXT UNIQUE NOT NULL,
                    driver_id INTEGER,
                    available BOOLEAN DEFAULT 1,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT,
                    FOREIGN KEY (driver_id) REFERENCES Driver(id)
                );";

            var createTransactionsTable = @"
                CREATE TABLE IF NOT EXISTS Transactions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    driver_id INTEGER,
                    type TEXT,
                    amount REAL,
                    date DATE,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT,
                    FOREIGN KEY (driver_id) REFERENCES Driver(id)
                );";

            var createExpensesTable = @"
                CREATE TABLE IF NOT EXISTS Expenses (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    description TEXT,
                    amount REAL,
                    month INTEGER,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT
                );";

            var createPaymentsTable = @"
                CREATE TABLE IF NOT EXISTS Payments (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    driver_id INTEGER,
                    amount_paid REAL,
                    payment_date DATE,
                    description TEXT,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT,
                    FOREIGN KEY (driver_id) REFERENCES Driver(id)
                );";

            var createSettlementsTable = @"
                CREATE TABLE IF NOT EXISTS Settlements (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    driver_id INTEGER,
                    amount_paid REAL,
                    deductions REAL,
                    tax REAL,
                    date DATE,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT,
                    FOREIGN KEY (driver_id) REFERENCES Driver(id)
                );";

            var createBankInformationTable = @"
                CREATE TABLE IF NOT EXISTS BankInformation (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    entity_id INTEGER,
                    entity_type INTEGER,
                    bank_name TEXT,
                    account_number TEXT,
                    iban TEXT,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    created_by TEXT,
                    updated_at DATETIME,
                    updated_by TEXT
                );";

            using (var transaction = connection.BeginTransaction())
            {
                var commands = new[]
                {
                    createUserTable, createDriverTable, createVehicleTable,
                    createPrioCardTable, createTransactionsTable, createExpensesTable,
                    createPaymentsTable, createSettlementsTable, createBankInformationTable
                };

                foreach (var sql in commands)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public string GetDatabasePath() => _databasePath;
    }
}

namespace AdvocaPro.Services
{
    public class DatabaseService
    {
        private readonly string _databasePath;

        public DatabaseService()
        {
            _databasePath = Path.Combine(AppContext.BaseDirectory, "Database", "sqlite.db");
        }

        public string GetDatabasePath() => _databasePath;
    }
}

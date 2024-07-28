using Windows.Storage;

namespace AdvocaPro.Services
{
    public class DatabaseService
    {
        private readonly string _databasePath;

        public DatabaseService()
        {
            _databasePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqlite.db");
            EnsureDatabaseFileExists();
        }

        private void EnsureDatabaseFileExists()
        {
            if (!File.Exists(_databasePath))
            {
                var installDbPath = Path.Combine(AppContext.BaseDirectory, "Database", "sqlite.db");
                File.Copy(installDbPath, _databasePath);
            }
        }

        public string GetDatabasePath() => _databasePath;
    }
}

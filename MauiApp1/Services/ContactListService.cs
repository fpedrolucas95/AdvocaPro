using AdvocaPro.Models;
using Microsoft.Data.Sqlite;

namespace AdvocaPro.Services
{
    public class ContactListService
    {
        private readonly string _databasePath;

        public ContactListService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void AddContact(ContactList contact)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"INSERT INTO ContactList (
                            Name, Phone, Email, Type, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                          ) VALUES (
                            @Name, @Phone, @Email, @Type, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                          )";

                    command.Parameters.AddWithValue("@Name", contact.Name);
                    command.Parameters.AddWithValue("@Phone", contact.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", contact.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Type", contact.Type);
                    command.Parameters.AddWithValue("@CreatedAt", contact.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", contact.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", contact.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", contact.UpdatedBy ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public void UpdateContact(ContactList contact)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE ContactList SET
                            Phone = @Phone, Email = @Email, Type = @Type, CreatedAt = @CreatedAt,
                            CreatedBy = @CreatedBy, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                            WHERE Name = @Name";

                    command.Parameters.AddWithValue("@Name", contact.Name);
                    command.Parameters.AddWithValue("@Phone", contact.Phone);
                    command.Parameters.AddWithValue("@Email", contact.Email);
                    command.Parameters.AddWithValue("@Type", contact.Type);
                    command.Parameters.AddWithValue("@CreatedAt", contact.CreatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", contact.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedAt", contact.UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", contact.UpdatedBy);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public void DeleteContact(string name)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM ContactList WHERE Name = @Name";
                    command.Parameters.AddWithValue("@Name", name);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<ContactList> GetContacts()
        {
            var contacts = new List<ContactList>();

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM ContactList";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var contact = new ContactList
                        {
                            Name = reader.GetString(0),
                            Phone = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Type = reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4),
                            CreatedBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                            UpdatedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                        };

                        contacts.Add(contact);
                    }
                }
            }

            return contacts;
        }
    }
}

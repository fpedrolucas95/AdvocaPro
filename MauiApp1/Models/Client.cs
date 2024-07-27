using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdvocaPro.Models
{
    public class Client
    {
        public class Person
        {
            public string Name { get; set; } = string.Empty;
            public DateTime? Birthday { get; set; }
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProcessNumber { get; set; } = string.Empty;
        public string ProcessType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CPF { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CellPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Complement { get; set; } = string.Empty;

        private string _address = string.Empty;
        public string Address
        {
            get
            {
                if (!string.IsNullOrEmpty(_address))
                {
                    return _address;
                }

                var fullAddress = Street;
                if (!string.IsNullOrWhiteSpace(Number))
                    fullAddress += $", {Number}";
                if (!string.IsNullOrWhiteSpace(Complement))
                    fullAddress += $" - {Complement}";
                return fullAddress;
            }
            set
            {
                _address = value;
            }
        }

        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public bool Reminder { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string LeadObs { get; set; } = string.Empty;
        public DateTime? StartPeriod { get; set; }
        public int Days { get; set; }
        public int Count { get; set; }
        public bool Completed { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public int Age { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Spouse { get; set; } = string.Empty;
        public string ChildrenJson { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string ParentsJson { get; set; } = string.Empty;
        public string RG { get; set; } = string.Empty;
        public string Registry { get; set; } = string.Empty;
        public string LawyerId { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Person> Children
        {
            get => string.IsNullOrEmpty(ChildrenJson) ? new List<Person>() : JsonSerializer.Deserialize<List<Person>>(ChildrenJson) ?? new List<Person>();
            set => ChildrenJson = JsonSerializer.Serialize(value);
        }

        [JsonIgnore]
        public List<Person> Parents
        {
            get => string.IsNullOrEmpty(ParentsJson) ? new List<Person>() : JsonSerializer.Deserialize<List<Person>>(ParentsJson) ?? new List<Person>();
            set => ParentsJson = JsonSerializer.Serialize(value);
        }
    }
}

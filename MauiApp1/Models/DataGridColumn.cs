using System.Linq.Expressions;

namespace AdvocaPro.Models
{
    public class DataGridColumn<T>
    {
        public int Column { get; set; }

        public string Header { get; set; } = string.Empty;

        public Expression<Func<T, object>> ContentBinding { get; set; } = x => default!;

        public double Width { get; set; } = 100;
    }
}

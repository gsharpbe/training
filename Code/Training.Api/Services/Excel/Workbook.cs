using System.Collections.Generic;

namespace Training.Api.Services.Excel
{
    public class Workbook
    {
        public Workbook()
        {
            Worksheets = new List<Worksheet>();
        }

        public ICollection<Worksheet> Worksheets { get; }
    }
}
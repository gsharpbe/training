using System.Collections.Generic;

namespace Training.Api.Services.Excel
{
    public class Worksheet
    {
        public List<Dictionary<string, object>> Rows { get; set; }
    }
}
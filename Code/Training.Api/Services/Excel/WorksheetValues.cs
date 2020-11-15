using System.Collections.Generic;

namespace Training.Api.Services.Excel
{
    public class WorksheetValues
    {
        public WorksheetValues()
        {
            Headers = new List<string>();
            MissingHeaders = new List<string>();

            Content = new List<Dictionary<string, WorksheetValue>>();
        }

        public List<string> Headers { get; set; }

        public List<string> MissingHeaders { get; set; }
        public string MissingHeaderComment { get; set; }

        public List<Dictionary<string, WorksheetValue>> Content { get; set; }
    }
}
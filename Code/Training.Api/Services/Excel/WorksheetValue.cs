namespace Training.Api.Services.Excel
{
    public class WorksheetValue
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public object Value { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }
    }
}
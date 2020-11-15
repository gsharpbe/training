namespace Metanous.Model.Core.Filter
{
    public class FilterDescriptor
    {
        public FilterOperator Operator { get; set; }
        public string Value { get; set; }
        public string Field { get; set; }

        public FilterDescriptor() { }

        public FilterDescriptor(string field, FilterOperator filterOperator, string value)
        {
            Field = field;
            Operator = filterOperator;
            Value = value;
        }

        public FilterDescriptor(string field, FilterOperator filterOperator)
        {
            Field = field;
            Operator = filterOperator;
            Value = null;
        }
    }
}
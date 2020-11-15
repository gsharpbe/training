using System;
using System.Globalization;
using System.Linq;
using Metanous.Model.Core.Extensions;
using Metanous.Model.Core.Filter;
using Metanous.Model.Core.Sort;

namespace Metanous.Model.Core.Search
{
    public class SearchParameters
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public SortDescriptor[] Sort { get; set; }
        public FilterDescriptor[] Filter { get; set; }
        public string Search { get; set; }

        public bool SkipCount { get; set; }
        public bool SkipData { get; set; }

        public DateTimeOffset? IfModifiedSince { get; set; }

        public void AddFilter(string field, string value, FilterOperator filterOperator = FilterOperator.Eq)
        {
            var newFilter = new FilterDescriptor(field, filterOperator, value);

            if (Filter == null)
            {
                Filter = new FilterDescriptor[0];
            }
            Filter = Filter.Concat(newFilter).ToArray();
        }

        public void AddFilter(string field, DateTime value, FilterOperator filterOperator = FilterOperator.Eq)
        {
            AddFilter(field, value.ToString("s", CultureInfo.InvariantCulture), filterOperator);
        }

        
    }
}
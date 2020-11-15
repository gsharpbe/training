using System.Collections.Generic;

namespace Metanous.Model.Core.Search
{
    public class SearchResult<T>
    {
        public int? Total { get; set; }
        public int StartIndex { get; set; }
        public int ItemsCount { get; set; }
        public IEnumerable<T> Data { get; set; }

        public SearchResult()
        {
            Data = new List<T>();
        }
    }
}
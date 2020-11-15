using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Metanous.Model.Core.Extensions;

namespace Metanous.Model.Core.Search
{
    public class Includes
    {
        public static readonly string ExcludePrefix = "exclude-";
        public static readonly string IncludePrefix = "include-";
        public static readonly string IncludeAllPrefix = "include-all";

        private const string ModelSuffix = "Model";

        private readonly IDictionary<string, IReadOnlyCollection<string>> _includes = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);

        public bool IncludeAll { get; set; }

        public bool ApplyExplicitLoading { get; set; }

        public Includes()
        {
            ApplyExplicitLoading = true;
        }

        public Includes Add(string key, IReadOnlyCollection<string> value)
        {
            IncludeAll = false;

            key = key.ToLower();
            if (_includes.ContainsKey(key))
            {
                _includes[key] = _includes[key].Union(value).ToList();
            }
            else
            {
                _includes.Add(key, value);
            }

            return this;
        }

        public Includes Add<T>(params string[] value)
        {
            var key = typeof(T).Name.RemoveSuffix(ModelSuffix);
            return Add(key, value);
        }

        public Includes Add<T>(params Expression<Func<T, object>>[] items)
        {
            var key = typeof(T).Name.RemoveSuffix(ModelSuffix);
            var selectedItems = items.Select(item => item.GetPropertyName());
            return Add(key, selectedItems.ToList());
        }

        public Includes Add(string key, string value)
        {
            return Add(key, new List<string> { value });
        }

        public bool Has(string entityType, string propertyName)
        {
            if (IncludeAll) return true;

            entityType = entityType.ToLowerInvariant();
            propertyName = propertyName.ToLowerInvariant();

            return _includes.Any(i => i.Key.Equals(entityType, StringComparison.OrdinalIgnoreCase) && i.Value.Any(select => propertyName == select.ToLowerInvariant().Trim()));
        }

        public bool Has<T>(params Expression<Func<T, object>>[] items)
        {
            var entityType = typeof(T).Name.RemoveSuffix(ModelSuffix);
            var propertyName = items.Select(_ => _.GetPropertyName()).First().Trim();
            return Has(entityType, propertyName);
        }

        public bool Has<T>(string propertyName)
        {
            var entityType = typeof(T).Name.RemoveSuffix(ModelSuffix);
            propertyName = propertyName.Trim();
            return Has(entityType, propertyName);
        }

        public IDictionary<string, IReadOnlyCollection<string>> PrepareHeaders()
        {
            var headers = new Dictionary<string, IReadOnlyCollection<string>>();

            _includes.ForEach(include => headers.Add($"{IncludePrefix}{include.Key}", include.Value));
            headers.Add(IncludeAllPrefix, new[] { IncludeAll.ToString().ToLower() });

            return headers;
        }

        public void Clear()
        {
            _includes.Clear();
            IncludeAll = true;
        }
    }
}
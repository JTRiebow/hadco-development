using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Hadco.Common;
using Hadco.Data.Entities;
using Hadco.Data.Attribute;

namespace Hadco.Data
{
    public static class IQueryableExtensions
    {
        public const string UPDATED_AFTER_FILTER = "UpdatedAfter";
        public const string SEARCH = "search";
        public const string SEARCH_FIELDS = "searchfields";

        public static IQueryable<T> Pagination<T>(this IQueryable<T> query, Pagination pagination) where T : IModel
        {
            return query.Ordering<T>(pagination.OrderBy ?? new OrderBy[] { new OrderBy() { Field = "ID", Direction = OrderDirection.Asc } }).
                    Skip<T>(pagination.Skip).Take<T>(pagination.Take);
        }

        public static IQueryable<T> Ordering<T>(this IQueryable<T> query, IEnumerable<OrderBy> orderBy) where T : IModel
        {
            if (orderBy != null)
            {
                return query.OrderBy(string.Join(",", orderBy));
            }
            return query;
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, NameValueCollection filter) //where T : IModel
        {
            Type entityType = typeof(T);

            var searchProperties = GetSearchProperties(filter);

            if (searchProperties != null)
            {
                query = Search<T>(entityType, searchProperties, query);
                filter.Remove(SEARCH);
                filter.Remove(SEARCH_FIELDS);
            }

            foreach (var key in filter.AllKeys)
            {
                PropertyInfo pi = GetFilterPropertyInfo(entityType, key);
                string value = filter.Get(key);
                if (pi != null && value != null)
                {
                    if (pi.PropertyType.Name == typeof(Guid).Name)
                    {
                        query = query.Where(string.Format("{0}==Guid(@0)", pi.Name), GetValue(entityType, pi.Name, value));
                    }
                    else
                        query = query.Where(string.Format("{0}==@0", pi.Name), GetValue(entityType, pi.Name, value));
                }
            }
            return query;
        }

        private static object GetValue(Type type, string key, string value)
        {
            var propertyType = type.GetProperty(key).PropertyType;
            if (propertyType.Name == typeof(Guid).Name)
            {
                return Guid.Parse(value);
            }
            if (propertyType.GetInterfaces().Contains(typeof(IConvertible)))
            {
                if (propertyType.IsEnum)
                {
                    return Enum.Parse(propertyType, value);
                }
                return Convert.ChangeType(value, propertyType);
            }

            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            if (underlyingType.Name == typeof(Guid).Name)
            {
                return value == "null" ? null : new Guid?(Guid.Parse(value));
            }
            if (underlyingType.GetInterfaces().Contains(typeof(IConvertible)))
            {
                if (underlyingType.IsEnum)
                {
                    return Enum.Parse(underlyingType, value);
                }
                return Convert.ChangeType(value, underlyingType);
            }
            throw new InvalidCastException(string.Format("Cannot convert from a System.String to {0}, or the conversion code does not yet exist.", propertyType.Name));
        }

        private static PropertyInfo GetFilterPropertyInfo(Type type, string key)
        {
            PropertyInfo pi = type.GetProperty(key);
            if (pi == null)
            {
                // check column attributes...
                pi = type.GetProperties().Where(x =>
                {
                    var colAttribute = (ColumnAttribute)x.GetCustomAttribute(typeof(ColumnAttribute));
                    return colAttribute != null && colAttribute.Name == key;
                }).FirstOrDefault();

            }
            return pi;
        }
        public static SearchProperties GetSearchProperties(NameValueCollection filter)
        {
            SearchProperties searchProperties = null;
            var search = filter.AllKeys.Where(x => x.ToLower() == SEARCH).FirstOrDefault();
            if (search != null)
            {
                var searchFields = filter.AllKeys.Where(x => x.ToLower() == SEARCH_FIELDS).FirstOrDefault();
                searchProperties = new SearchProperties()
                {
                    SearchTerms = filter.Get(search).ToLower().Split(' '),
                    SearchFields = searchFields != null ? filter.Get(searchFields).ToLower().Split(',') : null

                    // These lines were added/edited to help allow filtering by a navigation property.
                    // SearchFields = searchFields != null ? filter.Get(searchFields).ToLower().Split(',').Where(x => !x.Contains(".")).ToArray() : null,
                    // NavigationSearchFields = searchFields != null ? filter.Get(searchFields).ToLower().Split(',').Where(x => x.Contains(".")).ToArray() : null
                };
            }

            return searchProperties;
        }

        private static IQueryable<T> Search<T>(Type entityType, SearchProperties searchProperties, IQueryable<T> query)
        {
            IEnumerable<PropertyInfo> properties = null;

            if (searchProperties.SearchFields != null)
                properties = entityType.GetProperties().Where(x => searchProperties.SearchFields.Contains(x.Name.ToLower()));
            else
                properties = entityType.GetProperties().Where(x => x.PropertyType.Equals(typeof(string)) && x.GetCustomAttribute(typeof(NonSearchableAttribute)) == null);

            if (properties.Any(x => x.GetCustomAttribute(typeof(NonSearchableAttribute)) != null))
            {
                var nonSearchableProperties = properties.Where(x => x.GetCustomAttribute(typeof(NonSearchableAttribute)) != null);

                if (nonSearchableProperties.Count() == 1)
                    throw new ArgumentException(string.Format("{0} is not a searchable property.", nonSearchableProperties.Single().Name));
                else
                    throw new ArgumentException(string.Format("{0} are not searchable properties.", string.Join(",", nonSearchableProperties.Select(x => x.Name))));
            }

            List<string> wheres = new List<string>();

            int i = 0;
            foreach (var t in searchProperties.SearchTerms)
            {
                List<string> whereStrings = new List<string>();
                foreach (var p in properties)
                {
                    whereStrings.Add(string.Format("{0}.Contains(@{1})", p.Name, i));
                }
                wheres.Add(string.Format("({0})", string.Join(" || ", whereStrings)));
                i++;
            }

            string where = string.Join(" && ", wheres);

            if (string.IsNullOrEmpty(where))
                return query;

            return query.Where(where, searchProperties.SearchTerms);
        }

        public class SearchProperties
        {
            public string[] SearchTerms { get; set; }
            public string[] SearchFields { get; set; }

            // This field was added to allow filtering by a navigation property.
            // public string[] NavigationSearchFields { get; set; }
        }
    }
}

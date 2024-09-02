using System.Linq.Expressions;

namespace web_api_hospital.Helpers
{
    public static class SortHelper
    {
        /// <summary>
        /// Определяем параметр для сортировки через лямбда выражение
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortField"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetSortExpression<T>(string sortField)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, sortField[0].ToString().ToUpper() + sortField.Substring(1).ToLower());

            // Приведение к object
            var converted = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(converted, parameter);
        }
    }
}

using System;

namespace Metanous.WebApi.Core.Extensions
{
    public static class DateExtensions
    {
        public static int? GetAge(this DateTimeOffset? birthDate)
        {
            if (birthDate == null)
                return null;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age;
        }
    }
}

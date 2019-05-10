using System;
using System.Runtime.Serialization;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers
{
    internal static class EnumExtensions
    {
        public static string GetMemberDescription(this Enum enumerationValue)
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((EnumMemberAttribute)attrs[0]).Value;
                }
            }

            return enumerationValue.ToString();
        }
    }
}

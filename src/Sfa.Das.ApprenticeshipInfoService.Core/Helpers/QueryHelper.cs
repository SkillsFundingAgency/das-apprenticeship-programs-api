using System.Text.RegularExpressions;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public static class QueryHelper
    {
        public static string FormatQuery(string query, bool toLower = true)
        {
            return string.IsNullOrWhiteSpace(query) ? "*" : ReplaceUnacceptableCharacters(query, toLower);
        }

        public static string FormatQueryReturningEmptyStringIfEmptyOrNull(string query, bool toLower = true)
        {
            return string.IsNullOrEmpty(query) ? string.Empty : ReplaceUnacceptableCharacters(query, toLower);
        }

        private static string ReplaceUnacceptableCharacters(string query, bool toLower)
        {
            var queryformatted = Regex.Replace(query, @"[+\-&|!(){}\[\]^""~?:\\/]", @" ");

            return toLower ? queryformatted.ToLowerInvariant() : queryformatted;
        }
    }
}

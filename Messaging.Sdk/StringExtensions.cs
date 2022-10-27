using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Messaging.Sdk
{
    internal static class StringExtensions
    {
        internal static string ToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
    }
}

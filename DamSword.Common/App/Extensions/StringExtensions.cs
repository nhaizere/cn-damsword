using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DamSword.Common
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static bool NonNullOrEmpty(this string self)
        {
            return !string.IsNullOrEmpty(self);
        }

        public static bool EqualsIgnoreCase(this string self, string other)
        {
            return self.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string self, string other)
        {
            return self.ToLower().Contains(other.ToLower());
        }

        public static string ReplaceIgnoreCase(this string self, string oldValue, string newValue)
        {
            return Regex.Replace(self, oldValue, newValue, RegexOptions.IgnoreCase);
        }

        public static string FormatString(this string self, params object[] args)
        {
            return string.Format(self, args);
        }

        public static string FirstLetterToUpperCase(this string self)
        {
            if (self.IsNullOrEmpty())
                throw new ArgumentException("Must be non-empty string.", nameof(self));

            return self.First().ToString().ToUpper() + self.Substring(1);
        }

        public static string FirstLetterToLowerCase(this string self)
        {
            if (self.IsNullOrEmpty())
                throw new ArgumentException("Must be non-empty string.", nameof(self));

            return self.First().ToString().ToLower() + self.Substring(1);
        }

        public static IEnumerable<string> SplitUpperCase(this string self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (self.IsEmpty())
                return Enumerable.Empty<string>();

            var value = self.FirstLetterToUpperCase();
            var parts = Regex.Split(value, @"(?=[A-Z])").Where(p => !p.IsNullOrEmpty()).ToArray();
            return parts;
        }

        public static string SplitUpperCaseBySpace(this string self)
        {
            return self.SplitUpperCase().Join(" ");
        }

        public static string ToCamelCase(this string self)
        {
            var parts = self.SplitUpperCase();
            var pascalCaseParts = parts.Skip(1).Select(p => p.FirstLetterToUpperCase()).ToArray();
            var firstCamelCasePart = parts.First().FirstLetterToLowerCase();
            var result = pascalCaseParts.Prepend(firstCamelCasePart);

            return result.Join();
        }

        public static string ToPascalCase(this string self)
        {
            return self.SplitUpperCase().Select(p => p.FirstLetterToUpperCase()).Join();
        }

        public static string ToSpinalCase(this string self)
        {
            return self.SplitUpperCase().Select(p => p.ToLower()).Join("-");
        }

        public static string ToTrainCase(this string self)
        {
            return self.SplitUpperCase().Select(p => p.FirstLetterToUpperCase()).Join("-");
        }
    }
}
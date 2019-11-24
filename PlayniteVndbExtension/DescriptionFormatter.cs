using System;
using System.Text.RegularExpressions;

namespace VndbSharp
{
    public class DescriptionFormatter
    {
        private readonly Regex _urlMatcher;

        public DescriptionFormatter()
        {
            _urlMatcher = new Regex(@"\[url=((?:[^\[\]])+)\]((?:[^\[\]])+)\[\/url\]", RegexOptions.Compiled);
        }

        public string Format(string description)
        {
            var formatted = description.Replace("\n", "<br>" + Environment.NewLine);
            formatted = _urlMatcher.Replace(formatted, "<a href=\"$1\">$2</a>");
            return formatted;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VndbMetadata
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

        public string RemoveTags(string description)
        {
            return description == null ? "" : _urlMatcher.Replace(description, "$2");
        }
    }
}

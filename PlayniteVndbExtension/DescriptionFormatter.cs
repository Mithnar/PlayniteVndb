using System;
using Playnite.SDK;

namespace VndbSharp
{
    public class DescriptionFormatter
    {
        public string Format(string description)
        {
            var formatted = description.Replace("\n", "<br>" + System.Environment.NewLine);
            return formatted;
        }
    }
}
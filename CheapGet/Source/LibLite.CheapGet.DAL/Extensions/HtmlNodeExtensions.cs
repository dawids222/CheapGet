using HtmlAgilityPack;
using LibLite.CheapGet.Core.Extensions;
using System.ComponentModel;

namespace LibLite.CheapGet.DAL.Extensions
{
    public static class HtmlNodeExtensions
    {
        public static HtmlNode GetFirstChildWithClass(this HtmlNode node, string @class)
        {
            return node.ChildNodes.FirstOrDefault(x => x.GetClasses().Contains(@class));
        }

        public static HtmlNode GetLastChildWithClass(this HtmlNode node, string @class)
        {
            return node.ChildNodes.LastOrDefault(x => x.GetClasses().Contains(@class));
        }

        public static HtmlNode GetFirstChildWithName(this HtmlNode node, string name)
        {
            return node.ChildNodes.FirstOrDefault(x => x.Name == name);
        }

        public static HtmlNode GetLastChildWithName(this HtmlNode node, string name)
        {
            return node.ChildNodes.LastOrDefault(x => x.Name == name);
        }

        public static T GetValue<T>(this HtmlNode node)
        {
            var value = node.InnerHtml;
            if (typeof(T).IsNumericType())
            {
                var characters = value.Where(IsValidDigitCharacter).ToArray();
                value = new string(characters).Replace(",", ".");
            }
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromInvariantString(value);
        }

        private static bool IsValidDigitCharacter(char value) => char.IsDigit(value) || value == '.' || value == ',';
    }
}

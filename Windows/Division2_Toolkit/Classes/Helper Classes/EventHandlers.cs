using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Division2Toolkit.Classes.Helper_Classes
{
    public static class EventHandlers
    {
        public static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        public static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }
    }
}

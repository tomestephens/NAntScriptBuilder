using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NAntScriptBuilder
{
    /// <summary>
    /// Defaults for building an nant script
    /// </summary>
    public static class Defaults
    {
        public static readonly ReadOnlyCollection<string> Includes;
        public static readonly Dictionary<string,string> SystemReferences;

        static Defaults()
        {
            var defIncludes = new List<string>();
            // default includes per nant docs:
            // http://nant.sourceforge.net/release/latest/help/tasks/script.html
            defIncludes.Add("System");
            defIncludes.Add("System.Collections");
            defIncludes.Add("System.IO");
            defIncludes.Add("System.Text");
            defIncludes.Add("NAnt.Core");
            defIncludes.Add("NAnt.Core.Attributes");
            Includes = new ReadOnlyCollection<string>(defIncludes);

            // just a list of known System imports that need a reference to the dll
            // this list is definitely not complete, but for what I need right now it works
            SystemReferences = new Dictionary<string, string>();
            SystemReferences.Add("System.Xml", "System.Xml.dll");
            SystemReferences.Add("System.Configuration", "System.configuration.dll");
        }
    }

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Ext
    {
        /// <summary>
        /// simple util function to determine if a string has any data
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s.Trim());
        }
    }
}

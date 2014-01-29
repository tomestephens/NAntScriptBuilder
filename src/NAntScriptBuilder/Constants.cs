using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NAntScriptBuilder
{
    public class Constants
    {
        public static readonly ReadOnlyCollection<string> DefaultIncludes;
        public static readonly Dictionary<string,string> SystemReferences;

        static Constants()
        {
            var defIncludes = new List<string>();
            defIncludes.Add("NAnt.Core");
            defIncludes.Add("NAnt.Core.Attributes");
            defIncludes.Add("System");
            defIncludes.Add("System.IO");
            DefaultIncludes = new ReadOnlyCollection<string>(defIncludes);

            // just a list of known System imports that need a reference to the dll
            SystemReferences = new Dictionary<string, string>();
            SystemReferences.Add("System.Xml", "System.Xml.dll");
            SystemReferences.Add("System.Configuration", "System.configuration.dll");
        }
    }
}

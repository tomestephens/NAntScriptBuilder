using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace NAntScriptBuilder
{
    public class CodeFileData
    {
        public FileInfo CodeFile { get; private set; }
        public string CodeBlock { get; private set; }
        public ReadOnlyCollection<string> Imports { get; private set; }
        public ReadOnlyCollection<string> References { get; private set; }

        public string Language
        {
            get
            {
                switch (CodeFile.Extension)
                {   
                    case ".cs":
                        return "c#";
                    default:
                        // chop off the .
                        return CodeFile.Extension.Remove(0, 1);
                }
                throw new NotSupportedException("Unsupported file type");
            }
        }

        public CodeFileData(string codeFile)
        {
            CodeFile = new FileInfo(codeFile);
            ParseCode();
        }

        private void ParseCode()
        {
            var lines = File.ReadAllLines(CodeFile.FullName);

            FindImports(lines);
            FindCodeBlock(lines);
            FindReferences();
        }

        // assuming the imports were already found
        private void FindReferences()
        {
            var refs = new List<string>();
            // always need this
            refs.Add("System.dll");

            // really fuzzy - will probably always need to be checked manually after the fact
            foreach (var import in Imports)
            {
                if (Constants.SystemReferences.ContainsKey(import))
                    refs.Add(Constants.SystemReferences[import]);
            }

            References = new ReadOnlyCollection<string>(refs);
        }

        private void FindCodeBlock(string[] lines)
        {
            StringBuilder code = new StringBuilder();
            bool foundNamespace = false;

            for (int i = 0; i < lines.Length - 1; i++)
            {
                string line = lines[i];
                if (!IsImport(line))
                {
                    if (!foundNamespace && line.Contains("namespace"))
                    {
                        // skip the next line with the open brace
                        if (!line.Contains("{"))
                            i += 1;

                        foundNamespace = true;
                    }
                    else
                    {
                        // should be valid code
                        code.AppendLine(line);
                    }
                }
            }

            // no namespace declaration
            // the last line is probably important
            if (!foundNamespace)
                code.AppendLine(lines[lines.Length - 1]);

            CodeBlock = code.ToString();
        }

        private void FindImports(string[] lines)
        {
            var imports = new List<string>();

            foreach (var line in lines)
            {
                if (IsImport(line))
                {
                    string import = GetImportValue(line);
                    if(!Constants.DefaultIncludes.Contains(import))
                        imports.Add(import);
                }
            }

            Imports = new ReadOnlyCollection<string>(imports);
        }

        private bool IsImport(string line)
        {
            return line.Trim().StartsWith("using") && !line.Contains("(");
        }

        private string GetImportValue(string line)
        {
            return line.Replace("using", "").Replace(";", "").Trim();
        }
    }
}

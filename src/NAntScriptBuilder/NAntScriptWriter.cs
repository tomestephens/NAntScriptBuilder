using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NAntScriptBuilder
{
    internal class NAntScriptWriter
    {
        public static void OutputScript(CodeFileData codeFile, FileInfo outputFile)
        {
            OutputScript(new List<CodeFileData>(new[] { codeFile }), outputFile);
        }

        public static void OutputScript(List<CodeFileData> codeFiles, FileInfo outputFile)
        {
            XmlDocument xml = new XmlDocument();
            var root = xml.CreateElement("project");

            Dictionary<string, List<CodeFileData>> languageGroups = new Dictionary<string, List<CodeFileData>>();

            foreach (var codeFile in codeFiles)
            {
                if (!languageGroups.ContainsKey(codeFile.Language))
                    languageGroups.Add(codeFile.Language, new List<CodeFileData>());

                languageGroups[codeFile.Language].Add(codeFile);
            }

            foreach (var group in languageGroups)
            {
                var script = xml.CreateElement("script");
                var language = xml.CreateAttribute("language");
                language.Value = group.Key;
                script.Attributes.Append(language);

                // build references
                var refs = FindDistinctReferences(group.Value);
                var refTag = xml.CreateElement("references");
                foreach (var reference in refs)
                {
                    var include = xml.CreateElement("include");
                    var name = xml.CreateAttribute("name");
                    name.Value = reference;
                    include.Attributes.Append(name);
                    refTag.AppendChild(include);
                }
                script.AppendChild(refTag);

                // build imports
                var imports = FindDistinctImports(group.Value);
                var importsTag = xml.CreateElement("imports");
                foreach (var import in imports)
                {
                    var importTag = xml.CreateElement("import");
                    var namespaceAttr = xml.CreateAttribute("namespace");
                    namespaceAttr.Value = import;
                    importTag.Attributes.Append(namespaceAttr);
                    importsTag.AppendChild(importTag);
                }
                script.AppendChild(importsTag);

                // build code block                
                StringBuilder allCode = new StringBuilder();
                foreach (var codeFile in group.Value)
                {
                    allCode.AppendLine(codeFile.CodeBlock);
                }

                var code = xml.CreateElement("code");
                var cdata = xml.CreateCDataSection(allCode.ToString());
                code.AppendChild(cdata);

                script.AppendChild(code);
                root.AppendChild(script);
            }

            xml.AppendChild(root);
            xml.Save(outputFile.FullName);
        }

        private static List<string> FindDistinctImports(IEnumerable<CodeFileData> codeFiles)
        {
            var imports = new List<string>();
            foreach (var codeFile in codeFiles)
            {
                foreach (var import in codeFile.Imports)
                {
                    if (!imports.Contains(import))
                        imports.Add(import);
                }
            }

            return imports;
        }

        private static List<string> FindDistinctReferences(IEnumerable<CodeFileData> codeFiles)
        {
            var refs = new List<string>();
            foreach (var codeFile in codeFiles)
            {
                foreach (var reference in codeFile.References)
                {
                    if (!refs.Contains(reference))
                        refs.Add(reference);
                }
            }

            return refs;
        }
    }
}

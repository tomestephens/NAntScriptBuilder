using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntScriptBuilder
{
    [TaskName("proj_to_script")]
    public class CompileProject : Task
    {
        private FileInfo projectFile;
        [TaskAttribute("project", Required = true)]
        public FileInfo ProjectFile
        {
            get { return projectFile; }
            set { projectFile = value; }
        }

        private FileInfo outputFile;
        [TaskAttribute("output", Required = true)]
        public FileInfo OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        protected override void ExecuteTask()
        {
            if (!ProjectFile.Exists)
            {
                string error = string.Format("Unable to find project file {0}.", ProjectFile.FullName);
                if (FailOnError)
                {
                    throw new Exception(error);
                }
                Log(Level.Error, error);
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.Load(ProjectFile.FullName);

            XmlNodeList nodes = xml.GetElementsByTagName("Compile");
            var files = new List<CodeFileData>();

            foreach (XmlNode node in nodes)
            {
                string file = node.Attributes["Include"].Value;
                if(!file.Contains("AssemblyInfo"))
                    files.Add(new CodeFileData(Path.Combine(ProjectFile.Directory.FullName, file)));
            }

            NAntScriptWriter.OutputScript(files, OutputFile);
        }
    }
}

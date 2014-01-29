using System;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntScriptBuilder
{
    [TaskName("file_to_script")]
    public class CompileFile : Task
    {
        private FileInfo codeFile;
        [TaskAttribute("fileName", Required = true)]
        public FileInfo CodeFile
        {
            get { return codeFile; }
            set { codeFile = value; }
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
            if (!CodeFile.Exists)
            {
                string error = string.Format("Unable to find code file {0}.", CodeFile.FullName);
                if (FailOnError)
                {
                    throw new Exception(error);
                }
                Log(Level.Error, error);
                return;
            }

            var file = new CodeFileData(codeFile.FullName);
            NAntScriptWriter.OutputScript(file, outputFile);
        }
    }
}

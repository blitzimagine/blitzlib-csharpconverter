using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpConverter
{
    public class CppFile
    {
        public string FilePath { get; private set; }

        private Logger _logger;

        public List<Function> Functions { get; private set; }

        public CppFile(string filePath)
        {
            FilePath = filePath;
            _logger = new Logger("CppFile:" + Path.GetFileName(FilePath));
            Functions = new List<Function>();

            Process();
        }

        private void Process()
        {
            try
            {
                Stream s = File.Open(FilePath, FileMode.Open);
                StreamReader sr = new StreamReader(s);

                string data = sr.ReadToEnd();

                ParseData(data);

                sr.Close();
                s.Close();
            }
            catch (FileNotFoundException ex)
            {
                _logger.Error("File not found!");
            }
        }

        private void ParseData(string data)
        {
            string keyword = "PUBLIC_METHOD";

            List<string> methodList = new List<string>();

            string[] lines = data.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (!line.StartsWith(keyword))
                    continue;

                string method = line.Substring(keyword.Length).Trim();
                methodList.Add(method);
            }

            _logger.Info("Found " + methodList.Count + " Methods");

            ParseMethod(methodList.ToArray());
        }

        private void ParseMethod(string[] methods)
        {
            foreach (string method in methods)
            {
                _logger.Info("Processing Method: " + method);
                int spaceIndex = method.IndexOf(' ');
                int openBracketIndex = method.IndexOf('(');
                int openBracketIndexPlusOne = openBracketIndex + 1;
                int closeBracketIndex = method.IndexOf(')');
                bool retPointer = false;
                string retType = method.Substring(0, spaceIndex).Trim();
                if (retType.EndsWith("*"))
                {
                    retType = retType.Substring(0, retType.Length - 1);
                    if (retType == "MonoString")
                        retType = "string";
                    else
                        retPointer = true;
                }
                string name = method.Substring(spaceIndex, openBracketIndex - spaceIndex).Trim();
                if (name.StartsWith("*"))
                {
                    name = name.Substring(1);
                    if (retType == "MonoString")
                        retType = "string";
                    else
                        retPointer = true;
                }
                string argString = method.Substring(openBracketIndexPlusOne, closeBracketIndex - openBracketIndexPlusOne).Trim();
                List<Argument> argList = new List<Argument>();
                if (argString.Length > 0)
                {
                    string[] args = argString.Split(',');
                    foreach (string argVal in args)
                    {
                        string arg = argVal.Trim();

                        string[] argSplit = arg.Split(' ');
                        string argType = argSplit[0].Trim();
                        string argName = argSplit[1].Trim();
                        bool argPointer = false;

                        if (argType.EndsWith("*"))
                        {
                            argType = argType.Substring(0, argType.Length - 1);

                            if (argType == "MonoString")
                                argType = "string";
                            else
                                argPointer = true;

                        }
                        else if (argName.StartsWith("*"))
                        {
                            argName = argName.Substring(1);
                            if (argType == "MonoString")
                                argType = "string";
                            else
                                argPointer = true;
                        }

                        Argument argument = new Argument(new CType(argType), argName);
                        argList.Add(argument);
                    }
                }

                Function function = Function.Build(name, new CType(retType), argList.ToArray());

                Functions.Add(function);
                
                _logger.Info("Processed Method: " + function);
            }
        }
    }
}

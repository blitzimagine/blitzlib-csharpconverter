using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CSharpConverter
{
    public class CSharpFile
    {
        private Logger _logger;

        public string Name;
        public List<Function> Functions { get; private set; }
        private List<string> _typeNames;

        public CSharpFile(string name, params Function[] funcs)
        {
            _logger = new Logger("CSharpFile:" + Name + ".cs");
            Name = name;
            Functions = new List<Function>(funcs);
            _typeNames = new List<string>();

            Process();
        }

        private void _addType(string type)
        {
            if (!_typeNames.Contains(type))
                _typeNames.Add(type);
        }

        private void Process()
        {
            foreach (Function func in Functions)
            {
                if (func.HasPointers())
                {
                    if (func.ReturnType.Type == EType.POINTER)
                    {
                        _addType(func.ReturnType.CSharpPointerName);
                    }

                    foreach (Argument arg in func.Arguments)
                    {
                        if (arg.Type.Type == EType.POINTER)
                        {
                            _addType(arg.Type.CSharpPointerName);
                        }
                    }
                }
            }
        }

        public void Save(string file)
        {
            _logger.Info("Writing CSharp File");

            Stream s = File.Open(file, FileMode.Create);
            StreamWriter sw = new StreamWriter(s);

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Runtime.CompilerServices;");
            sw.WriteLine();

            sw.WriteLine("namespace BlitzEngine");
            sw.WriteLine("{");

            sw.WriteLine("\tpublic static class " + Name);
            sw.WriteLine("\t{");

            _logger.Info("Writing Classes");
            foreach (string type in _typeNames)
            {
                sw.WriteLine("\t\tpublic class " + type);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tpublic IntPtr Pointer { get; private set; }");
                sw.WriteLine("\t\t\t");
                sw.WriteLine("\t\t\tpublic " + type + "(IntPtr pointer)");
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tPointer = pointer;");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t\t");
            }

            _logger.Info("Writing Functions");
            foreach (Function func in Functions)
            {
                _logger.Info("Writing Function: " + func);
                string retType = func.ReturnType.GetString();
                if (func.ReturnType.IsPointer())
                    retType = "IntPtr";
                string protection = "public";
                if (func.HasPointers())
                {
                    protection = "private";
                }
                string args = "";
                for (int i = 0; i < func.Arguments.Count; i++)
                {
                    Argument arg = func.Arguments[i];
                    string typeName = arg.Type.GetString();
                    if (arg.IsPointer())
                        typeName = "IntPtr";
                    args += typeName + " " + arg.Name;
                    if (i < func.Arguments.Count - 1)
                        args += ", ";
                }

                sw.WriteLine("\t\t[MethodImpl(MethodImplOptions.InternalCall)]");
                string funcName = func.Name;
                if (func.HasPointers())
                    funcName += "_internal";
                sw.WriteLine("\t\t" + protection + " static extern " + retType + " " + funcName + "(" + args + ");");
                sw.WriteLine("\t\t");

                if (func.HasPointers())
                {
                    if (func.ReturnType.IsPointer())
                    {
                        retType = func.ReturnType.GetString();
                    }

                    args = "";
                    for (int i = 0; i < func.Arguments.Count; i++)
                    {
                        Argument arg = func.Arguments[i];
                        string typeName = arg.Type.GetString();
                        args += typeName + " " + arg.Name;
                        if (i < func.Arguments.Count - 1)
                            args += ", ";
                    }

                    sw.WriteLine("\t\tpublic static " + retType + " " + func.Name + "(" + args + ")");
                    sw.WriteLine("\t\t{");

                    string argVal = "";
                    for (int i = 0; i < func.Arguments.Count; i++)
                    {
                        Argument arg = func.Arguments[i];
                        string val = arg.Name;
                        if (arg.IsPointer())
                            val += ".Pointer";
                        argVal += val;
                        if (i < func.Arguments.Count - 1)
                            argVal += ", ";
                    }

                    string retVal = func.Name + "_internal(" + argVal + ")";
                    if (func.ReturnType.IsPointer())
                    {
                        sw.WriteLine("\t\t\t" + retType + " ret = new " + retType + "(" + retVal + ");");
                        sw.WriteLine("\t\t\treturn ret;");
                    }
                    else if (func.ReturnType.Type == EType.VOID)
                    {
                        sw.WriteLine("\t\t\t" + retVal + ";");
                    }
                    else
                    {
                        sw.WriteLine("\t\t\treturn " + retVal + ";");
                    }
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t\t");
                }
            }

            sw.WriteLine("\t}");
            sw.WriteLine("}");

            sw.Flush();
            sw.Close();
            s.Close();
        }
    }
}

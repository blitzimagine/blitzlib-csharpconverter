using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace CSharpConverter
{
    public enum EType
    {
        VOID,
        BOOL,
        CHAR,
        SHORT,
        INT,
        FLOAT,
        STRING,
        POINTER
    }

    public struct CType
    {
        public EType Type { get; set; }
        public string PointerName { get; set; }

        public string CSharpPointerName
        {
            get
            {
                string nType = PointerName;
                if (nType.ToLower().StartsWith("gx") || nType.ToLower().StartsWith("bb"))
                    nType = nType.Substring(2);

                nType = nType[0].ToString().ToUpper() + nType.Substring(1);

                return nType;
            }
        }

        public CType(EType type, string pointerName = "")
        {
            Type = type;
            PointerName = pointerName;
        }

        public CType(string type)
        {
            PointerName = "";
            switch (type)
            {
                case "void":
                    Type = EType.VOID;
                    break;
                case "char":
                    Type = EType.CHAR;
                    break;
                case "short":
                    Type = EType.SHORT;
                    break;
                case "bool":
                    Type = EType.BOOL;
                    break;
                case "int":
                    Type = EType.INT;
                    break;
                case "float":
                    Type = EType.FLOAT;
                    break;
                case "string":
                    Type = EType.STRING;
                    break;
                default:
                    Type = EType.POINTER;
                    PointerName = type;
                    break;
            }
        }

        public bool IsPointer()
        {
            return Type == EType.POINTER;
        }

        public string GetString()
        {
            if (IsPointer())
            {
                return CSharpPointerName;
            } else
            {
                return Util.GetTypeName(Type);
            }
        }
    }

    public struct Argument
    {
        public CType Type { get; set; }
        public string Name { get; set; }

        public Argument(CType type, string name)
        {
            Type = type;
            Name = name;
            if (Name == "value")
                Name = "val";
        }

        public override string ToString()
        {
            return Type + " " + Name;
        }

        public bool IsPointer()
        {
            if (Type.IsPointer())
                return true;

            return false;
        }
    }

    public class Function
    {
        public string Name { get; set; }
        public CType ReturnType { get; set; }
        public List<Argument> Arguments { get; private set; }

        public Function(string name, CType returnType)
        {
            Arguments = new List<Argument>();
            Name = name;
            if (Name.EndsWith("Func"))
                Name = Name.Substring(0, Name.Length - "Func".Length);
            Name = Name[0].ToString().ToUpper() + Name.Substring(1);
            ReturnType = returnType;
        }

        public bool HasPointers()
        {
            if (ReturnType.IsPointer())
                return true;

            foreach (Argument arg in Arguments)
            {
                if (arg.IsPointer())
                    return true;
            }

            return false;
        }

        public static Function Build(string name, CType returnType, params Argument[] args)
        {
            Function ret = new Function(name, returnType);
            
            ret.AddArguments(args);

            return ret;
        }

        public void AddArgument(Argument argument)
        {
            Arguments.Add(argument);
        }

        public void AddArguments(params Argument[] args)
        {
            foreach (Argument arg in args)
            {
                AddArgument(arg);
            }
        }

        public void RemoveArgument(string name)
        {
            foreach (Argument arg in Arguments)
            {
                if (arg.Name == name)
                {
                    Arguments.Remove(arg);
                    break;
                }
            }
        }

        public void ClearArguments()
        {
            Arguments.Clear();
        }

        public int ArgumentCount()
        {
            return Arguments.Count;
        }

        public override string ToString()
        {
            string args = "";
            for (int i = 0; i < Arguments.Count; i++)
            {
                Argument arg = Arguments[i];
                args += arg.Type.GetString() + " " + arg.Name;
                if (i < Arguments.Count - 1)
                    args += ", ";
            }
            return ReturnType.GetString() + " " + Name + "(" + args + ")";
        }
    }
}

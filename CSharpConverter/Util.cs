using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpConverter
{
    public static class Util
    {
        public static void WriteHeader()
        {
            WriteChars('-', 79);
        }

        public static void WriteChars(char c, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Console.Write(c);
            }
            Console.WriteLine();
        }

        public static string GetTypeName(this EType type)
        {
            switch (type)
            {
                case EType.VOID:
                    return "void";
                case EType.BOOL:
                    return "bool";
                case EType.CHAR:
                    return "char";
                case EType.SHORT:
                    return "short";
                case EType.INT:
                    return "int";
                case EType.FLOAT:
                    return "float";
                case EType.STRING:
                    return "string";
                case EType.POINTER:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}

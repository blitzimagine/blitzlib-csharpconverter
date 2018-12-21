using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpConverter
{
    public class Program
    {
        private static Logger logger;

        private static void PreQuit()
        {
#if DEBUG
            Console.ReadKey();
#endif
        }

        public static void Main(string[] args)
        {
            Util.WriteHeader();
            Console.WriteLine("CSharpConverter v0.01");
            Console.WriteLine("Matthieu Parizeau");
            Util.WriteHeader();

            logger = new Logger("Main");

            if (args.Length == 0)
            {
                logger.Error("Usage: CSharpConverter <file1.cpp> [file2.cpp] [file3.cpp] ...");
                PreQuit();
                return;
            }

            foreach (string file in args)
            {
                logger.Info("Processing " + file);

                string outputFile = Path.ChangeExtension(file, ".cs");

                CppFile cppFile = new CppFile(file);
                CSharpFile csFile = new CSharpFile(Path.GetFileNameWithoutExtension(file), cppFile.Functions.ToArray());
                csFile.Save(outputFile);

                logger.Info("Completed " + file);
            }

            logger.Info("Done Processing Files");

            PreQuit();
        }
    }
}

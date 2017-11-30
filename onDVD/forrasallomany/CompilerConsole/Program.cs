using System;
using System.Collections.Generic;
using System.IO;

namespace CompilerConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CheckArgs(args);

            string code = File.ReadAllText(args[0]);
            List<string> output = new Compiler.Compiler().Compile(code);

            foreach (string line in output)
            {
                Console.WriteLine(line);
            }
        }

        private static void CheckArgs(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide the source file's path as first parameter.");
                Environment.Exit(0);
            }

            string path = args[0];
            if (!File.Exists(path))
            {
                Console.WriteLine("Invalid path: the file not exists.");
                Environment.Exit(0);
            }
        }
    }
}
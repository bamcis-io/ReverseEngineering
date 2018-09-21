using BAMCIS.Disassembler.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BAMCIS.Disassembler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("No Disassemble!");
                    string Johnny5 = @"
                        _//)_//)
                       / /)=/ /)
                      ((O)=(O))
                         \||/
                 ________====____[o]_ 
             )/)|___._==      ==_.___|(\(
            (( \ || '-.________.-' || / ))
             \=/ ||     ..''..     || \=/
              \\_//    / [||] \    \\_//
               \V/    / ...... \    \V/
                      \::::::::/
                _____.---'  '---._____
               |_-_-_|__------__|_-_-_|
               |_-_-_|=        =|_-_-_|
               |_-_-_|          |_-_-_|
";
            Console.WriteLine(Johnny5);
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("");

            if (args.Length != 1)
            {
                Console.WriteLine("You must specifiy a path to the machine code file. That is the only accepted parameter.");
                Environment.Exit(1);
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"The specified path {args[0]} does not exist.");
                Environment.Exit(1);
            }

            using (FileStream FStream = File.OpenRead(args[0]))
            {
                byte[] Content = new byte[FStream.Length];
                FStream.Read(Content, 0, Content.Length);

                IEnumerable<string[]> Rows = new List<string[]>();

                try
                {
                    Rows = Instruction.LinearSweepToColumns(Content);
                }
                catch
                {

                }

                int[] Widths = new int[Math.Max(1, Rows.Select(x => x.Length).Max())];
                Widths[0] = -4;

                for (int i = 1; i < Widths.Length; i++)
                {
                    Widths[i] = (Rows.Select(x => i < x.Length ? x[i] : "").Select(x => x.Length).Max() * -1) + -4;
                }

                foreach (string[] Columns in Rows)
                {
                    string Row = "";
                    int Counter = 0;
                    
                    foreach (string Item in Columns)
                    {
                        Row += $"{{{Counter},{Widths[Counter]}}}";
                        Counter++;
                    }

                    string Line = String.Format(Row, Columns);

                    Console.WriteLine(Line);
                }

                Environment.Exit(0);
            }
        }
    }
}

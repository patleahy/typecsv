using System;
using System.IO;
using TypeCsv.Splitters;

namespace TypeCsv
{
    // Displays a CSV with each column in a different color
    //
    // See Help()
    class Program
    {

        static int Main(string[] args)
        {
            if ((args.Length > 0) && (args[0] == "/?"))
                return Help();

            // Remember the foreground color when the program started.
            _consoleColor = new ConsoleColor();

            // Intercept key presses so that we reset the color before the exiting if the user presses Ctrl+C.
            _break = false;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPress);

            try
            {
                // read from file or standard in.
                if (args.Length > 0)
                    TypeFile(args[0]);
                else
                    TypeConsole();
            }
            finally
            {
                // reset the color
                _consoleColor.ResetForegroundColor();
            }
            return 1;
        }

        static ConsoleColor _consoleColor;
        static bool _break;

        // Read a file and output it with colors.
        static void TypeFile(string filepath)
        {
            using (var reader = File.OpenText(filepath))
            {
                Type(reader);
            }
        }

        // Read a file and output it with colors.
        static void TypeConsole()
        {
            Type(Console.In);
        }

        // Read input and output it with colors.
        static void Type(TextReader reader)
        {
            // The input line is split on every comma.
            // Sometines a comma is inside a cell if the cell begins with a double quote.
            // If we find a cel with that starts with a double quote then don't change the color until we find
            // a cell that ends with a double quote.

            ISplitter splitter = new CsvSplitter(_consoleColor);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                splitter.Type(splitter.Split(line));

                if (_break)
                    return;
            }
        }

        // If the user presses Ctrl+C then exit stop processing input and exit cleanly so that the console color is reset.
        static void CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            if (args.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                _break = true;
                args.Cancel = true;
            }
        }

        static int Help()
        {
            Console.WriteLine("Displays a CSV with each column in a different color.");
            Console.WriteLine();
            Console.WriteLine("    typecsv <filepath>");
            Console.WriteLine();
            Console.WriteLine("Displays the file specified in filepath. If no file is specified then input is read from the console. This allows you to pipe the outout of other commands into this command.");
            Console.WriteLine();
            Console.WriteLine("Example");
            Console.WriteLine();
            Console.WriteLine("    type log.csv | typecsv");
            Console.WriteLine();
            return 1;
        }
    }
}

using System;
using System.IO;
using TypeCsv.Splitters;

namespace TypeCsv
{
    // Displays a delimited text files with each column in a different color
    //
    // See Help()
    class Program
    {
        static int Main(string[] args)
        {
            _options = new Options(args);

            if (_options.Help)
                return Help();

            if (_options.Version)
                return Version();

            // Remember the foreground color when the program started.
            _consoleColor = new ConsoleColor();

            // Intercept key presses so that we reset the color before the exiting if the user presses Ctrl+C.
            _break = false;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPress);

            try
            {
                // read from file or standard in.
                if (_options.HasFilePath)
                    TypeFile(_options.FilePath);
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

        static Options _options;
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
            ISplitter splitter = CreateSplitter();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                splitter.Type(splitter.Split(line));

                if (_break)
                    return;
            }
        }

        static ISplitter CreateSplitter()
        {
            switch (_options.FileType.Name)
            {
                case "csv": return new CsvSplitter(_consoleColor);
                case "token": return new TokenSplitter(_options.FileType.Token, _consoleColor);
            }
            throw new InvalidOperationException();
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
            Console.WriteLine();
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

        static int Version()
        {
            var name = typeof(Program).Assembly.GetName();
            Console.WriteLine($"{name.Name} {name.Version}");
            return 1;
        }
    }
}

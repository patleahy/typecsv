using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
            _foregroundColor = Console.ForegroundColor;

            // Intercept key presses so that we reset the color before the exiting if the user presses Ctrl+C.
            _break = false;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPress);

            SetupTerminalForColor();

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
                ResetForegroundColor();
            }
            return 1;
        }

        static ConsoleColor _foregroundColor;
        static bool _break;

        static void ResetForegroundColor()
        {
            Console.Out.Flush();
            Console.ForegroundColor = _foregroundColor;
        }

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

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                int i = 0;
                bool inQuotes = false;
                string join = "";
                foreach (var part in line.Split(",".ToCharArray()))
                {
                    Console.Write(join);

                    if (!inQuotes)
                    {
                        ConsoleColor(Colors[i++ % Colors.Count]);
                        inQuotes = part.StartsWith("\"");
                    }
                    inQuotes &= !part.EndsWith("\"");

                    Console.Write(part);
                    join = ",";
                }
                Console.WriteLine();

                if (_break)
                    return;
            }
        }

        // The colors are based on the Rainbow CSV VSCode extension.
        // These are selected to work when the console background is dark.
        static List<string> Colors = new List<string>
        {
            "212;212;212",
            "86;156;214",
            "220;205;121",
            "106;153;85",
            "206;145;120",
            "156;220;254",
            "181;206;168",
            "86;156;214",
            "244;71;71",
        };

        // Change the color by writing a color code to the console.
        static void ConsoleColor(string color)
        {
            Console.Write($"\x1b[38;2;{color}m");
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

        // Change the mode of the console so that it will handle color codes.
        static void SetupTerminalForColor()
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleMode(handle, out int mode);
            SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x4;
    }
}

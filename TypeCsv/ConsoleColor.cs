using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TypeCsv
{
    public class ConsoleColor
    {
        public ConsoleColor()
        {
            _foregroundColor = Console.ForegroundColor;
            SetupTerminalForColor();
        }

        private System.ConsoleColor _foregroundColor;
        
        public void ResetForegroundColor()
        {
            Console.Out.Flush();
            Console.ForegroundColor = _foregroundColor;
        }

        // The colors are based on the Rainbow CSV VSCode extension.
        // These are selected to work when the console background is dark.
        public static List<string> Colors = new List<string>
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

        public void SetColor(string color)
        {
            Console.Write($"\x1b[38;2;{color}m");
        }

        public void SetColor(int i)
        {
            SetColor(Colors[i % Colors.Count]);
        }

        // Change the mode of the console so that it will handle color codes.
        private void SetupTerminalForColor()
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

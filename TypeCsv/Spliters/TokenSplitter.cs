using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeCsv.Splitters
{
    class TokenSplitter : ISplitter
    {
        public TokenSplitter(string token, ConsoleColor consoleColor)
        {
            _token = token;
            _tokens = new string[] { token };
            _consoleColor = consoleColor;
        }

        private string _token;
        private string[] _tokens;
        private ConsoleColor _consoleColor;

        public List<string> Split(string line)
        {
            return line.Split(_tokens, StringSplitOptions.None).ToList();
        }

        public void Type(List<string> parts)
        {
            string join = "";
            int i = 0;
            foreach (var part in parts)
            {
                Console.Write(join);
                _consoleColor.SetColor(i++);
                Console.Write(part);
                join = _token;
            }
            Console.WriteLine();
        }
    }
}
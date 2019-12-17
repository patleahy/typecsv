using System;
using System.IO;

namespace TypeCsv
{
    // Parse the command line options.
    public class Options
    {
        public Options(string[] args)
        {
            if (args == null)
                return;

            // Set the file type use the options the user specified.
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg.ToLower().Trim())
                {
                    case "/help":
                    case "/?": Help = true; break;

                    case "/version":
                    case "/v": Version = true; break;
                    
                    case "/csv": FileType = CSV; break;
                    
                    case "/tab": 
                    case "/tsv": FileType = TAB; break;
                    
                    case "/pipe": FileType = PIPE; break;
                    
                    // The argument after the /token is the token string.
                    case "/token": 
                    case "/t": 
                        if (i == args.Length - 1)
                            throw new ArgumentException("A token string is required.");

                        FileType = new FileType { Name = "token" , Token = args[++i] }; 
                        break;
                    
                    default: FilePath = arg; break;
                }
            }
            
            // If the user didn't specify a file type the use the file extension.
            if (FileType == null && HasFilePath)
            {
                switch (Path.GetExtension(FilePath).ToLower())
                {
                    case ".csv": FileType = CSV; break;

                    case ".tab": 
                    case ".tsv": FileType = TAB; break;
                }
            }

            // If we still don't know the file type then default to CSV.
            if (FileType == null)
                FileType = CSV;
        }
    
        public string FilePath { get; private set; }
        public bool HasFilePath { get => !string.IsNullOrEmpty(FilePath); }
        public bool Help { get; private set; }
        public bool Version { get; protected set; }
        public FileType FileType { get; private set; }

        private static FileType CSV = new FileType { Name = "csv" };
        private static FileType TAB = new FileType { Name = "token", Token = "\t" };
        private static FileType PIPE = new FileType { Name = "token", Token = "|" };
    }

    public class FileType 
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}
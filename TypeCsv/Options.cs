using System.IO;

namespace TypeCsv
{
    public class Options
    {
        public Options(string[] args)
        {
            if (args == null)
                return;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg.ToLower().Trim())
                {
                    case "/help":
                    case "/?": Help = true; break;
                    
                    case "/csv": FileType = CSV; break;
                    
                    case "/tab": 
                    case "/tsv": FileType = TAB; break;
                    
                    case "/pipe": FileType = PIPE; break;
                    
                    case "/token": 
                    case "/t": FileType = new FileType { Name = "token" , Token = args[++i] }; break;
                    
                    default: FilePath = arg; break;
                }
            }
            
            if (FileType == null && HasFilePath)
            {
                switch (Path.GetExtension(FilePath).ToLower())
                {
                    case ".csv": FileType = CSV; break;

                    case ".tab": 
                    case ".tsv": FileType = TAB; break;
                }
            }

            if (FileType == null)
                FileType = CSV;
        }
    
        public string FilePath { get; private set; }
        public bool HasFilePath { get => !string.IsNullOrEmpty(FilePath); }
        public bool Help { get; private set; }
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
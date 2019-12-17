using System.Collections.Generic;

namespace TypeCsv.Splitters
{
    interface ISplitter
    {
        List<string> Split(string line);
        void Type(List<string> parts);
    }
}
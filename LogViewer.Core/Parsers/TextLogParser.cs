using System;
using LogViewer.Core.Interfaces;

namespace LogViewer.Core.Parsers;

public class TextLogParser : ILogParser
{
    public List<List<KeyValuePair<string, string>>> Parse(string text)
    {
        throw new NotImplementedException();
    }
}

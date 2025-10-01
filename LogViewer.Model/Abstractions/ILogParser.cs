using LogViewer.Model.Types;

namespace LogViewer.Model.Abstractions;

public interface ILogParser
{
    public ParseResult Parse(string logText);
}

namespace LogViewer.Core.Interfaces;

public interface ILogParser
{
    public List<List<KeyValuePair<string, string>>> Parse(string text);
}

using System.Text.Json.Nodes;

namespace LogViewer.Parsers;

public interface ILogParser
{
    Task<List<JsonObject>> Parse(string text);
}
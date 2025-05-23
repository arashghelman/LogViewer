using System.Text.Json;
using System.Text.RegularExpressions;
using LogViewer.Core.Interfaces;

namespace LogViewer.Core.Parsers;

public class JsonLogParser : ILogParser
{
    public List<List<KeyValuePair<string, string>>> Parse(string text)
    {
        var parsedLogs = new List<List<KeyValuePair<string, string>>>();

        var pattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\}";
        var matches = Regex.Matches(text, pattern);

        foreach (Match match in matches)
        {
            var fields = new List<KeyValuePair<string, string>>();

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(match.Value);

            FlattenJson(jsonElement, fields);

            parsedLogs.Add(fields);
        }

        return parsedLogs;
    }

    private void FlattenJson(JsonElement element, List<KeyValuePair<string, string>> fields)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind is JsonValueKind.Object)
            {
                FlattenJson(property.Value, fields);
            }
            else
            {
                fields.Add(new KeyValuePair<string, string>(property.Name, property.Value.ToString()));
            }
        }
    }
}

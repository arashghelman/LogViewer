using System.Text.RegularExpressions;
using System.Text.Json;

var path = @"D:\Logs\logs.log";
var pattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\}";

using var reader = new StreamReader(path);
var fileText = await reader.ReadToEndAsync();

var parsedLogs = new List<List<KeyValuePair<string, string>>>();

var matches = Regex.Matches(fileText, pattern);

foreach (Match match in matches)
{
    var fields = new List<KeyValuePair<string, string>>();

    var jsonElement = JsonSerializer.Deserialize<JsonElement>(match.Value);

    FlattenJson(jsonElement, fields);

    parsedLogs.Add(fields);
}

void FlattenJson(JsonElement element, List<KeyValuePair<string, string>> fields)
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
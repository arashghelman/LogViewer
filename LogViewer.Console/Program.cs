using System.Text.RegularExpressions;
using System.Text.Json.Nodes;

var path = @"D:\project\LogViewer\tart-logs-app_20240821.log";
var pattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\}";

using var reader = new StreamReader(path);
var logsString = await reader.ReadToEndAsync();

var jsonObjects = new List<JsonObject>();

var matches = Regex.Matches(logsString, pattern);

foreach (Match match in matches)
{
    var jsonObject = JsonNode.Parse(match.Value)?.AsObject();

    if (jsonObject is null) continue;

    var flattenedObject = new JsonObject();

    foreach (var property in jsonObject)
    {
        if (property.Value is JsonObject nestedObject)
        {
            foreach (var nestedProp in nestedObject)
            {
                flattenedObject.Add(nestedProp.Key, nestedProp.Value?.DeepClone());
            }
        }
        else
        {
            flattenedObject.Add(property.Key, property.Value?.DeepClone());
        }
    }

    jsonObjects.Add(flattenedObject);
}

var b = 20;
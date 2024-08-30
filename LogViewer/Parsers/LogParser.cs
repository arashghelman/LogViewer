using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace LogViewer.Parsers;

public class LogParser : ILogParser
{
    public async Task<List<JsonObject>> Parse(string text)
    {
        var jsonObjects = new List<JsonObject>();

        var pattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\}";
        var matches = Regex.Matches(text, pattern);

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

        return jsonObjects;
    }
}

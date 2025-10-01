// using System.Text.Json;
// using System.Text.RegularExpressions;
// using LogViewer.Model.Abstractions;
// using LogViewer.Model.Types;
//
// namespace LogViewer.Model.Parsers;
//
// public class JsonLogParser : ILogParser
// {
//     public ParseResult Parse(string logText)
//     {
//         var jsonObjectPattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\}";
//         var jsonMatches = Regex.Matches(logText, jsonObjectPattern);
//         var jsonEntries = new List<List<KeyValuePair<string, string>>>();
//         var fieldNames = new List<string>();
//         
//         foreach (Match match in jsonMatches)
//         {
//             var jsonElement = JsonSerializer.Deserialize<JsonElement>(match.Value);
//             var jsonEntry = new List<KeyValuePair<string, string>>();
//
//             ParseFlatJsonEntries(jsonElement, jsonEntry, fieldNames);
//             
//             jsonEntries.Add(jsonEntry);
//         }
//
//         var logEntries = new List<Dictionary<string, string>>();
//         
//         foreach (var element in jsonEntries)
//         {
//             var elementKeys = element.Select(x => x.Key);
//             var missingFieldNames = fieldNames.Except(elementKeys);
//             
//             var entry = new Dictionary<string, string>();
//             
//             if (missingFieldNames.Any())
//                 foreach (var fieldName in missingFieldNames) entry.Add(fieldName, "-");
//
//             foreach (var (key, value) in element) entry.Add(key, value);
//             
//             logEntries.Add(entry);
//         }
//
//         var result = new ParseResult
//         {
//             LogEntries = logEntries,
//             FieldNames = fieldNames
//         };
//         
//         return result;
//     }
//     
//     private void ParseFlatJsonEntries(
//         JsonElement jsonElement, 
//         List<KeyValuePair<string, string>> properties,
//         List<string> fieldNames)
//     {
//         var nameCounts = new Dictionary<string, int>();
//         
//         foreach (var property in jsonElement.EnumerateObject())
//         {
//             if (property.Value.ValueKind is JsonValueKind.Object) 
//                 ParseFlatJsonEntries(property.Value, properties, fieldNames);
//             else
//             {
//                 if (!fieldNames.Contains(property.Name)) nameCounts[property.Name] = 1;
//                 else nameCounts[property.Name]++;
//                 
//                 var fieldName = nameCounts[property.Name] == 1 
//                     ? property.Name
//                     : $"{property.Name}_{nameCounts[property.Name]}";
//                 
//                 fieldNames.Add(fieldName);
//                 properties.Add(new KeyValuePair<string, string>(fieldName, property.Value.ToString()));
//             }
//         }
//     }
// }

using System.Text.Json;
using System.Text.RegularExpressions;
using LogViewer.Model.Abstractions;
using LogViewer.Model.Types;

namespace LogViewer.Model.Parsers;

public class JsonLogParser : ILogParser
{
    public ParseResult Parse(string logText)
    {
        var jsonObjectPattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\}";
        var jsonMatches = Regex.Matches(logText, jsonObjectPattern);
        var jsonEntries = new List<Dictionary<string, string>>();
        var fieldNames = new HashSet<string>();
        
        foreach (Match match in jsonMatches)
        {
            var properties = new Dictionary<string, string>();
            var nameCounts = new Dictionary<string, int>();
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(match.Value);
            
            FlattenJsonElement(jsonElement, "", properties, nameCounts);
            
            jsonEntries.Add(properties);
            
            foreach (var key in properties.Keys)
            {
                fieldNames.Add(key);
            }
        }
        
        var normalizedEntries = new List<Dictionary<string, string>>();
        foreach (var entry in jsonEntries)
        {
            var normalizedEntry = new Dictionary<string, string>();
            
            foreach (var fieldName in fieldNames)
            {
                normalizedEntry[fieldName] = entry.GetValueOrDefault(fieldName, "-");
            }
            
            normalizedEntries.Add(normalizedEntry);
        }

        var result = new ParseResult
        {
            LogEntries = normalizedEntries,
            FieldNames = fieldNames.ToList()
        };

        return result;
    }
    
    private void FlattenJsonElement(JsonElement element, string currentPath, 
        Dictionary<string, string> properties, Dictionary<string, int> nameCounts)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var newPath = string.IsNullOrEmpty(currentPath) 
                        ? property.Name 
                        : $"{currentPath}.{property.Name}";
                    
                    FlattenJsonElement(property.Value, newPath, properties, nameCounts);
                }
                break;
                
            case JsonValueKind.Array:
                var index = 0;
                foreach (var arrayElement in element.EnumerateArray())
                {
                    var newPath = $"{currentPath}[{index}]";
                    FlattenJsonElement(arrayElement, newPath, properties, nameCounts);
                    index++;
                }
                break;
                
            default:
                AddUniqueProperty(currentPath, element.ToString(), properties, nameCounts);
                break;
        }
    }
    
    private void AddUniqueProperty(string path, string value, 
        Dictionary<string, string> properties, Dictionary<string, int> nameCounts)
    {
        var lastDotIndex = path.LastIndexOf('.');
        var propertyName = lastDotIndex >= 0 ? path[(lastDotIndex + 1)..] : path;
        
        if (!nameCounts.ContainsKey(propertyName)) nameCounts[propertyName] = 0;
        
        nameCounts[propertyName]++;
        
        var finalPropertyName = nameCounts[propertyName] == 1 
            ? path
            : $"{path}_{nameCounts[propertyName]}";
        
        properties[finalPropertyName] = value;
    }
}

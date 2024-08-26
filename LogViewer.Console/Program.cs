using System.Text.RegularExpressions;

var path = @"D:\project\LogViewer\tart-logs-app_20240821.log";
var pattern = @"\{[^}]+\}";

using var reader = new StreamReader(path);
var fileContent = await reader.ReadToEndAsync();

var matches = Regex.Matches(fileContent, pattern);
var jsonLogs = new List<string>();
foreach (Match match in matches)
{
    jsonLogs.Add(match.Value);
}

var b = 02;
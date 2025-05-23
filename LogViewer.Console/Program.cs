using LogViewer.Core.Parsers;

var path = "/Users/arashghelman/Projects/LogViewer/nginx_json_logs.txt";
using var reader = new StreamReader(path);
var fileText = await reader.ReadToEndAsync();

var logParser = new JsonLogParser();
var logs = logParser.Parse(fileText);

var a = 20;
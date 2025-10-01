namespace LogViewer.Model.Types;

public class ParseResult
{
    public List<Dictionary<string, string>> LogEntries { get; set; }
    
    public List<string> FieldNames { get; set; }
}
namespace LogViewer.ViewModel.Types;

public class ParseResultDto
{
    public List<Dictionary<string, string>> LogEntries { get; set; }
    
    public List<string> FieldNames { get; set; }
}
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogViewer.Model.Abstractions;
using LogViewer.ViewModel.Abstractions;
using LogViewer.ViewModel.Types;

namespace LogViewer.ViewModel.ViewModels;

public class MainViewModel : ObservableObject, IMainViewModel
{
    private readonly ILogParser _logParser;
    
    public ObservableCollection<RecentFileInfoDto> RecentFiles { get; set; }
    
    private bool _isFileLoaded;
    public bool IsFileLoaded
    {
        get => _isFileLoaded;
        set => SetProperty(ref _isFileLoaded, value);
    }
    
    public MainViewModel(ILogParser logParser)
    {
        _logParser = logParser;

        RecentFiles = new ObservableCollection<RecentFileInfoDto>
        {
            new() { FileName = "log2023.log", Path = "/Users/arashghelman/Desktop/Masters/Thesis/Research" },
            new() { FileName = "log2024.log", Path = "/Users/arashghelman/Desktop/Masters/Thesis" },
            new() { FileName = "log2025.log", Path = "/Users/arashghelman" },
            new() { FileName = "Loglog.log", Path = "/Users/arashghelman/Desktop" },
        };
    }

    public async Task<ParseResultDto> ParseLogFile(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var logText = await reader.ReadToEndAsync();
        
        var result = _logParser.Parse(logText);

        var resultDto = new ParseResultDto
        {
            FieldNames = result.FieldNames,
            LogEntries = result.LogEntries,
        };
        
        IsFileLoaded = true;
        
        return resultDto;
    }

    public void UnloadLogFile()
    {
        IsFileLoaded = false;
    }
}
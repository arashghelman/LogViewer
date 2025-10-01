using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LogViewer.ViewModel.Types;

namespace LogViewer.ViewModel.Abstractions;

public interface IMainViewModel
{
    public ObservableCollection<RecentFileInfoDto> RecentFiles { get; set; }
    
    public bool IsFileLoaded { get; }
    
    Task<ParseResultDto> ParseLogFile(string filePath);
}
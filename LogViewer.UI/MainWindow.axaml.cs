using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LogViewer.Core.Interfaces;

namespace LogViewer.UI;

public partial class MainWindow : Window
{
    private readonly ILogParser _logParser;
    
    public MainWindow(ILogParser logParser)
    {
        InitializeComponent();
        _logParser = logParser;
    }

    private async void UploadButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Open Log File",
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.TextPlain]
            });

        var selectedFile = files.SingleOrDefault();
        if (selectedFile is null) return;

        await using var fileStream = await selectedFile.OpenReadAsync();
        var buffer = new byte[fileStream.Length];
        await fileStream.ReadAsync(buffer);
        var text = Encoding.UTF8.GetString(buffer);

        var result = _logParser.Parse(text);
    }
}
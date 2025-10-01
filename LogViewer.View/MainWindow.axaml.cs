using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using LogViewer.Model.Parsers;
using LogViewer.ViewModel.Abstractions;
using LogViewer.ViewModel.Types;

namespace LogViewer.View;

public partial class MainWindow : Window
{
    private readonly IMainViewModel _viewModel;
    
    public MainWindow(IMainViewModel viewModel)
    {
        InitializeComponent();
        
        AddHandler(DragDrop.DragOverEvent, HandleDragOver);
        AddHandler(DragDrop.DragLeaveEvent, HandleDragLeave);
        AddHandler(DragDrop.DropEvent, HandleDrop);

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
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
        
        var result = await _viewModel.ParseLogFile(selectedFile.Path.AbsolutePath);
        
        FillLogsGrid(result);
    }
    
    private void HandleDragOver(object? sender, DragEventArgs e)
    {
        ToggleDragZoneElementsColor(true);
    }

    private void HandleDragLeave(object? sender, DragEventArgs e)
    {
        ToggleDragZoneElementsColor(false);
    }

    private async void HandleDrop(object? sender, DragEventArgs e)
    {
        var droppedItem = e.Data.GetFiles()?.SingleOrDefault();
        if (droppedItem is null) return;
        
        var result = await _viewModel.ParseLogFile(droppedItem.Path.AbsolutePath);
        
        FillLogsGrid(result);
    }
    
    private void ToggleDragZoneElementsColor(bool isHovered)
    {
        DropZoneBorder.Stroke = isHovered ? CustomBrushes.Blue : CustomBrushes.Slate300;

        var foregroundColor = isHovered ? CustomBrushes.Blue : CustomBrushes.Slate600;
        UploadFileIcon.Foreground = foregroundColor;
        DragFileText.Foreground = foregroundColor;
        OrText.Foreground = foregroundColor;
    }

    private void FillLogsGrid(ParseResultDto parseResult)
    {
        foreach (var field in parseResult.FieldNames)
        {
            var column = new DataGridTemplateColumn
            {
                Header = field,
                CellTemplate = new FuncDataTemplate<Dictionary<string,string>>((row, _) =>
                    new TextBlock
                    {
                        Text = row.GetValueOrDefault(field, "-"),
                        Padding = new Thickness(12),
                        Background = Brushes.White,
                        Foreground = CustomBrushes.Slate600
                    }, true)
            };
            
            LogsGrid.Columns.Add(column);
        }

        LogsGrid.ItemsSource = parseResult.LogEntries;
    }
}
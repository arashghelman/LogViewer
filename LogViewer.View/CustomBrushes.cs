using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace LogViewer.View;

public static class CustomBrushes
{
    public static IImmutableSolidColorBrush Slate300 => new ImmutableSolidColorBrush(Color.Parse("#cbd5e1"));
    
    public static IImmutableSolidColorBrush Slate600 => new ImmutableSolidColorBrush(Color.Parse("#475569"));
    
    public static IImmutableSolidColorBrush Blue => new ImmutableSolidColorBrush(Color.Parse("#0061ff"));
}
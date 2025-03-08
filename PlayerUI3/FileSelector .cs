using System;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace PlayerUI3;

public class FileSelector
{
    private readonly FileOpenPicker _openPicker = new Windows.Storage.Pickers.FileOpenPicker();

    public FileSelector(Window window)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(_openPicker, hWnd);
    }

    public string? GetFilePathAsync(string[] filters)
    {
        foreach (var filter in filters)
        {
            _openPicker.FileTypeFilter.Add(filter);
        }
        var file = _openPicker.PickSingleFileAsync().AsTask().Result;

        return file?.Path;
    }
}
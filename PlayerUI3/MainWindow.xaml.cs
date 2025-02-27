using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PlayerUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Playback _playback;
        private Decoder _decoder;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }

        private async void FileSelectorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var fileSelector = new FileSelector(this);
            var path = await fileSelector.GetFilePathAsync(["*"]);
            if (path != null)
            {
                if(_playback!=null) _playback.Dis
                _decoder = new Decoder(path);
                _playback = new Playback(_decoder);
                _playback.StartPlayTask();
                myButton.Content = path;
            }
            
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            _playback.Play();
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _playback.Pause();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

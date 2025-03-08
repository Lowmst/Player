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

namespace PlayerUI3
{
    public sealed partial class MainWindow : Window
    {
        private Playback _playback;
        private Decoder _decoder;

        public MainWindow()
        {
            this.InitializeComponent();
        }


        private void FileSelectorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var fileSelector = new FileSelector(this);
            var path = fileSelector.GetFilePathAsync(["*"]);
            if (path != null)
            {
                _decoder = new Decoder(path);
                _playback = new Playback(_decoder);
                _playback.StartPlayTask();
                FileNameText.Text = path;
            }
            else
            {

            }
            
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_playback.PlayState)
            {
                _playback.Pause();
            }
            else
            {
                _playback.Play();
            }
            
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _playback.Pause();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

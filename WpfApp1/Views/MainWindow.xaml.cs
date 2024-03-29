﻿using FrequencyAnalysis.Models;
using FrequencyAnalysis.Views;
using System;
using System.Windows;
using System.Windows.Threading;

namespace FrequencyAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private const int secondsToElapse = 10;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
            this.Unloaded += MainWindow_Unloaded;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void ShowMatrixEventHandler(object sender, MatrixVisualizationEventArgs args)
        {
            MatrixViewModel<int> matrixViewModel = new MatrixViewModel<int>(args.Source, args.Title);
            MatrixView matrixView = new MatrixView() { DataContext = matrixViewModel };
            matrixView.Show();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.ShowMatrixEvent += ShowMatrixEventHandler;
            viewModel.ShowChartEvent += ShowChartEventHandler;
        }

        private void ShowChartEventHandler()
        {
            ChartView chartView = new ChartView() { DataContext = this.DataContext };
            chartView.Show();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.ShowMatrixEvent -= ShowMatrixEventHandler;
            viewModel.ShowChartEvent -= ShowChartEventHandler;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = string.Empty;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }

        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            if (mePlayer.IsMuted)
            {
                btnMute.Content = "Mute";
            }
            else
            {
                btnMute.Content = "Unmute";
            }

            mePlayer.IsMuted = !mePlayer.IsMuted;
        }

        private void backward10sec_Click(object sender, RoutedEventArgs e)
        {
            if (!mePlayer.NaturalDuration.HasTimeSpan) return;

            mePlayer.Position -= TimeSpan.FromSeconds(Math.Min(mePlayer.Position.TotalSeconds, secondsToElapse));
        }

        private void forward10sec_Click(object sender, RoutedEventArgs e)
        {
            if (!mePlayer.NaturalDuration.HasTimeSpan) return;

            mePlayer.Position += TimeSpan.FromSeconds(Math.Min(mePlayer.NaturalDuration.Subtract(new Duration(mePlayer.Position)).TimeSpan.TotalSeconds, secondsToElapse));
        }

        private void CompareImages_Click(object sender, RoutedEventArgs e)
        {
            var compareWindow = new CompareImagesWindowView();
            compareWindow.Show();
        }
    }
}

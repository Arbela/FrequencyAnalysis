using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace FrequencyAnalysis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if(Directory.Exists(Path.Combine(Environment.CurrentDirectory, "LocalImageStorage")))
                Directory.Delete(Path.Combine(Environment.CurrentDirectory, "LocalImageStorage"), true);

            MessageBox.Show(e.Exception.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "LocalImageStorage")))
                Directory.Delete(Path.Combine(Environment.CurrentDirectory, "LocalImageStorage"), true);
        }
    }
}

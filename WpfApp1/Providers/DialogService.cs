using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace FrequencyAnalysis.Providers
{
    internal class DialogService
    {
        public SaveFileDialog ShowSaveFileDialog(string filter = null, string defaultExt = null, string title = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if(!string.IsNullOrWhiteSpace(title))
            {
                dialog.Title = title;
            }
            dialog.Filter = filter;
            dialog.DefaultExt = defaultExt;
            dialog.ShowDialog();

            return dialog;
        }

        public VistaFolderBrowserDialog ShowBrowseFolderDialog()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.ShowDialog();

            return dialog;
        }
    }
}

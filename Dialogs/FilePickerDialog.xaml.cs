using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Where1.WPlot
{
    /// <summary>
    /// Interaction logic for FilePickerDialog.xaml
    /// </summary>
    public partial class FilePickerDialog : Window
    {
        private bool multiselect;
        public string filepath;
        public FilePickerDialog(bool multiselect = false)
        {
            InitializeComponent();
            this.multiselect = multiselect;
            filePathTextBlock.Text = filepath;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void FilePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (multiselect)
                {
                    filepath = String.Join(',', files);
                }
                else
                {
                    filepath = files[0];
                }
                filePathTextBlock.Text = filepath;
            }
        }

        private void OpenFileExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = multiselect;
            if (openFileDialog.ShowDialog() == true)
            {
                if (multiselect)
                {
                    filepath = String.Join(",", openFileDialog.FileNames);
                }
                else
                {
                    filepath = openFileDialog.FileName;
                }

                filePathTextBlock.Text = filepath;
            }
        }
    }
}

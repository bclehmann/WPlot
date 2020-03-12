using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Where1.WPlot 
{
    public partial class SignalFrequencyDialog : Window
    {
        public SignalFrequencyDialog(bool isPrefab=false)
        {
            InitializeComponent();

            Resources["sampleRateVisibility"] = isPrefab ? Visibility.Collapsed : Visibility.Visible;
        }

        public void okButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            this.Close();
        }

        public string frequency { get { return frequencyTextBox.Text; } }
    }
}
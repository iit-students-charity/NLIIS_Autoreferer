using System.Windows;
using Microsoft.Win32;

namespace NLIIS_Autoreferer
{
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog _openFileDialog;
        private string _documentToReferPath;
        
        public MainWindow()
        {
            _openFileDialog = new OpenFileDialog
            {
                DefaultExt = "pdf",
                CheckFileExists = true,
                Multiselect = false
            };
            
            InitializeComponent();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Empty);
        }
        
        private void Authors_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Group 721701:\nSemenikhin Nikita,\nStryzhych Angelika", "Authors");
        }
        
        private void ButtonFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                _openFileDialog.FileName

                if (UploadPath.Text.Length > 0)
                {
                    UploadPath.Text = UploadPath.Text.Substring(0, UploadPath.Text.Length - 1);
                    ButtonUpload.IsEnabled = true;
                }
            }
        }
    }
}
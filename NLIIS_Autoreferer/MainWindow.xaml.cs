using System.Windows;
using Microsoft.Win32;
using NLIIS_Autoreferer.Services;

namespace NLIIS_Autoreferer
{
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog _openFileDialog;
        private readonly IReferer _classyReferer;
        private readonly IReferer _keywordRefer;
        
        public MainWindow()
        {
            _openFileDialog = new OpenFileDialog
            {
                DefaultExt = "pdf",
                CheckFileExists = true,
                Multiselect = false
            };
            _classyReferer = new ClassyReferer();
            _keywordRefer = new KeywordReferer();
            
            InitializeComponent();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Upload a document via open file dialog\nChoose a language of the document\n" +
                            "Press \"Refer\"\nGet paths to classy & keyword refers versions");
        }
        
        private void Authors_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Group 721701:\nSemenikhin Nikita,\nStryzhych Angelika", "Authors");
        }

        private void Refer(object sender, RoutedEventArgs e)
        {
            DocumentService.Language = Language.Text;
            var text = DocumentService.FromPDF(DocumentToReferPath.Text);
            
            var classyRefer = _classyReferer.GenerateRefer(text);
            var keywordRefer = _keywordRefer.GenerateRefer(text);

            ClassyReferPath.Content = "Classy: " + DocumentService.ToPDF(classyRefer);
            KeywordReferPath.Content = "Keyword: " + DocumentService.ToPDF(keywordRefer);
            
            _classyReferer.Clean();
            _keywordRefer.Clean();
        }
        
        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                DocumentToReferPath.Text = _openFileDialog.FileName;
            }
        }
    }
}
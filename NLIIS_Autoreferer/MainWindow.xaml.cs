using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using NLIIS_Autoreferer.Models;
using NLIIS_Autoreferer.Service;

namespace NLIIS_Autoreferer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        public void Upload(IEnumerable<string> paths, string language, string method)
        {
            foreach (var path in paths)
            {
                var termsFromFile = DocumentService.FromPDF(path);
                IDictionary<string, double> termsProbability = null;
            
                if (method == FrequencyWordRecognizer.MethodName)
                {
                    termsProbability = FrequencyWordRecognizer.GetWords(termsFromFile);
                }
                else if (method == ShortWordRecognizer.MethodName)
                {
                    termsProbability = ShortWordRecognizer.GetWords(termsFromFile);
                }

                foreach (var (word, probability) in termsProbability)
                {
                    var newWord = new LanguageWord
                    {
                        Language = language,
                        Method = method,
                        Probability = probability,
                        Word = word
                    };

                    if (!LanguageWord.Words.Contains(newWord, new LanguageWord.Comparer()))
                    {
                        LanguageWord.Words.Add(newWord);
                    }
                }
            }

            ButtonRecognize.IsEnabled = true;
        }
        
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Load documents to fill the base probabilities for each language and method" +
                "\nThen you can choose a document to recognize language in");
        }
        
        private void Authors_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Group 721701:\nSemenikhin Nikita,\nStryzhych Angelika",
                "Authors");
        }
        
        private void ButtonFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (FileDialog.ShowDialog() == true)
            {
                UploadPath.Text = string.Empty;
                
                foreach (var filename in FileDialog.FileNames)
                {
                    UploadPath.Text += filename + "*";
                }

                if (UploadPath.Text.Length > 0)
                {
                    UploadPath.Text = UploadPath.Text.Substring(0, UploadPath.Text.Length - 1);
                    ButtonUpload.IsEnabled = true;
                }
            }
        }
        
        private void ButtonUpload_OnClick(object sender, RoutedEventArgs e)
        {
            Upload(UploadPath.Text.Split("*"), UploadLanguage.Text, UploadMethod.Text);
        }
    }
}
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VKAppearanceToXAML {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e) {
            StartButton.IsEnabled = false;
            Progress<Tuple<LogType, string>> progress = new Progress<Tuple<LogType, string>>();
            progress.ProgressChanged += (a, b) => {
                ProcessLog.Text += $"{b.Item2}\n";
            };
            var files = await Parser.DoItAsync(progress);
            if(files != null) {
                try {
                    CommonOpenFileDialog cofd = new CommonOpenFileDialog();
                    cofd.IsFolderPicker = true;
                    cofd.Title = "Выберите папку для сохранения сгенерированных файлов XAML";
                    var dr = cofd.ShowDialog();
                    if (dr == CommonFileDialogResult.Ok) {
                        foreach (var file in files) {
                            ProcessLog.Text += $"Saving {file.Key}...\n";
                            File.WriteAllText($"{cofd.FileName}\\{file.Key}", file.Value);
                        }
                        //Go to the specified folder
                        var runExplorer = new ProcessStartInfo();
                        runExplorer.FileName = "explorer.exe";
                        runExplorer.Arguments = cofd.FileName;
                        Process.Start(runExplorer);
                    }
                    cofd.Dispose();
                    ProcessLog.Text += $"All done!\n\n";
                } catch(Exception ex) {
                    ProcessLog.Text += $"Exception 0x{ex.HResult.ToString("x8")}: {ex.Message}\n\n";
                }
            } else {
                ProcessLog.Text += $"Not saved :(\n\n";
            }
            StartButton.IsEnabled = true;
        }
    }
}

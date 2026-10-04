using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PlantUML
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _scriptName = "PlantUML.bat";

        public MainWindow()
        {
            InitializeComponent();

            InFile.Text = @"sample\sequence.puml";
            ConfigFile.Text = @"sample\config.txt";
            PlantumlPath.Text = @"plantuml.jar";
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            string commandParam = Logic.BuildCommandParam(PlantumlPath.Text, ConfigFile.Text, File.Exists(ConfigFile.Text), InFile.Text);

            File.WriteAllText(_scriptName, commandParam + Environment.NewLine);

            using (Process process = Process.Start(_scriptName))
            {
                process.WaitForExit();              // プロセスの終了を待つ
            }
        }
    }
}

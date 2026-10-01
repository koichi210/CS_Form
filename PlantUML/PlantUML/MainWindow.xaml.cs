using System;
using System.Windows;
using System.Diagnostics;

namespace PlantUML
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private String scriptName = "PlantUML.bat";
        public MainWindow()
        {
            InitializeComponent();

            InFile.Text = @"sample\sequence.puml";
            ConfigFile.Text = @"sample\config.txt";
            PlantumlPath.Text = @"plantuml.jar";

        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            String commandParam = Logic.BuildCommandParam(PlantumlPath.Text, ConfigFile.Text, System.IO.File.Exists(ConfigFile.Text), InFile.Text);

            System.IO.StreamWriter writer = new System.IO.StreamWriter(scriptName);
            writer.WriteLine(commandParam);
            writer.Close();

            Process process = Process.Start(scriptName);
            process.WaitForExit();              // プロセスの終了を待つ
        }
    }
}

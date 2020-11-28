using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace PlantUML
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private String ScriptName = "PlantUML.bat";
        public MainWindow()
        {
            InitializeComponent();

            InFile.Text = @"sample\sequence.puml";
            ConfigFile.Text = @"sample\config.txt";
            PlantumlPath.Text = @"plantuml.jar";

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            String CommandParam;

            CommandParam = string.Format(@"java -jar " + PlantumlPath.Text);
            if (System.IO.File.Exists(ConfigFile.Text) )
            {
                CommandParam += string.Format(" -config {0}", ConfigFile.Text);
            }
            CommandParam += string.Format(" -charset UTF-8 {0}", InFile.Text);

            System.IO.StreamWriter writer = new System.IO.StreamWriter(ScriptName);
            writer.WriteLine(CommandParam);
            writer.Close();

            Process p = Process.Start(ScriptName);
            p.WaitForExit();              // プロセスの終了を待つ
            int iExitCode = p.ExitCode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace ImageViewer
{
    public partial class ImageViewer : Form
    {
        private StcUtils util = new StcUtils();
        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private readonly String DefaultSaveName = @"ImageViewer.xml";

        public ImageViewer()
        {
            InitializeComponent();
            textBox_FolderPath.Text = @"C:\tmp";
            textBox_Extension.Text = @"*.png";  // TODO：動画も先頭フレームを表示するようにして対応したい。

            hScrollBar_Scaling.Minimum = 2;
            hScrollBar_Scaling.Maximum = 256;
            hScrollBar_Scaling.LargeChange = 50;    // バーと矢印の間クリック
            hScrollBar_Scaling.SmallChange = 1;     // 矢印クリック
            hScrollBar_Scaling.Value = 100;

            this.Icon = Properties.Resources.ImageViewer;

            // カレントディレクトリ移動
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            sr.RegistItem(this);
            sr.LoadXmlFile(DefaultSaveName);
            util.UpdateProfileList(ref comboBox_Profile, DefaultSaveName);
        }

        private void textBox_FolerPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(textBox_FolderPath.Text, e);
        }

        private void button_SampleView_Click(object sender, EventArgs e)
        {
            PreView pv = new PreView();
            pv.SetSize(hScrollBar_Scaling.Value);
            pv.View(imageList, listView_Image, textBox_FolderPath.Text, textBox_Extension.Text, true);
        }

        private void button_ListView_Click(object sender, EventArgs e)
        {
            PreView pv = new PreView();
            pv.SetSize(hScrollBar_Scaling.Value);
            pv.View(imageList, listView_Image, textBox_FolderPath.Text, textBox_Extension.Text);
        }

        private void hScrollBar_Scaling_Scroll(object sender, ScrollEventArgs e)
        {
            // イベントが複数回とんできてしまうので、どのEventTypeのときに処理するか明示する
            if (e.Type != ScrollEventType.EndScroll)
            {
                return;
            }

            if (Directory.Exists(textBox_FolderPath.Text))
            {
                PreView pv = new PreView();
                pv.SetSize(hScrollBar_Scaling.Value);
                pv.View(imageList, listView_Image, textBox_FolderPath.Text, textBox_Extension.Text, true);
            }
        }

        private void listView_Image_DoubleClick(object sender, EventArgs e)
        {
            if (listView_Image.SelectedItems.Count == 1)
            {
                util.ExecutePath(listView_Image.SelectedItems[0].Text);
            }
        }

        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_Profile.Text;
            sr.LoadXmlFile(LoadFileName);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(DefaultSaveName);
            if (sr.LoadXmlFile(LoadFileName))
            {
                comboBox_Profile.Text = Path.GetFileName(LoadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_Profile.Text);
            if (sr.SaveXmlFile(SaveFileName))
            {
                util.UpdateProfileList(ref comboBox_Profile, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }
    }
}

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
    partial class ImageViewer : StcBaseForm<SaveRestore>
    {
        private StcFileInputOutput fio = new StcFileInputOutput();
        private readonly String DefaultSaveName = @"ImageViewer.json";

        // 設定ファイルはJSONが基本。旧XML(ImageViewer.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"ImageViewer.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(sr, filePath) : sr.LoadXmlFile(filePath);
        }

        private Boolean SaveProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(sr, filePath) : sr.SaveXmlFile(filePath);
        }


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

            InitializeCommonSettings(Properties.Resources.ImageViewer);

            sr.RegistItem(this);
            JsonSaveRestore.LoadWithMigration(sr, DefaultSaveName, LegacySettingFileName,
                path => sr.LoadXmlFile(path));
            util.UpdateProfileList(ref comboBox_Profile, ProfileExtensions, DefaultSaveName);
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
            LoadProfile(LoadFileName);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(DefaultSaveName);
            if (LoadProfile(LoadFileName))
            {
                comboBox_Profile.Text = Path.GetFileName(LoadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_Profile, ProfileExtensions, SaveProfile);
        }
    }
}

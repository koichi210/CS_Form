using System;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace ImageViewer
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private StcFileInputOutput fio = new StcFileInputOutput();
        private readonly String DefaultSaveName = @"ImageViewer.json";

        // 設定ファイルはJSONが基本。旧XML(ImageViewer.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"ImageViewer.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\ImageViewer\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String AppName = "ImageViewer";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

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


        public Form1()
        {
            InitializeComponent();
            InitializePlaceholders();
            InitializeToolTips();
            textBox_FolderPath.Text = @"C:\tmp";
            textBox_Extension.Text = @"*.png";  // TODO：動画も先頭フレームを表示するようにして対応したい。

            hScrollBar_Scaling.Minimum = 2;
            hScrollBar_Scaling.Maximum = 256;
            hScrollBar_Scaling.LargeChange = 50;    // バーと矢印の間クリック
            hScrollBar_Scaling.SmallChange = 1;     // 矢印クリック
            hScrollBar_Scaling.Value = 100;

            InitializeCommonSettings(Properties.Resources.ImageViewer);

            sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(userDataFolder, DefaultSaveName);
            String defaultXmlPath = Path.Combine(userDataFolder, LegacySettingFileName);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath,
                path => sr.LoadXmlFile(path));
            util.UpdateProfileList(comboBox_Profile, ProfileExtensions, DefaultSaveName, userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 対象外の欄は無し(2つとも手入力する欄)
        private void InitializePlaceholders()
        {
            textBox_FolderPath.PlaceholderText = @"例: C:\tmp";
            textBox_Extension.PlaceholderText = "例: *.png";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_FolderPath, "画像を探すフォルダ。Enterキーでそのフォルダを開く");
            toolTip.SetToolTip(textBox_Extension, "表示するファイルの絞り込み条件。ワイルドカード(*や?)を使ったパターンを1つだけ指定する");
            toolTip.SetToolTip(hScrollBar_Scaling, "サムネイルの一辺の大きさ(ピクセル)。動かし終えると、フォルダが有効なら先頭の1枚だけで表示し直す");
            toolTip.SetToolTip(listView_Image, "サムネイルをダブルクリックすると、その画像を関連付けられたアプリで開く");
            toolTip.SetToolTip(comboBox_Profile, "保存済みの設定ファイル。選ぶとフォルダパスとファイル拡張子を読み込む");
            toolTip.SetToolTip(button_SampleView, "条件に合うファイルのうち先頭の1枚だけを表示する");
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[Cheetos/Form1.cs]]の同名機能と同じ考え方)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DataFolderMenu.AppendToSystemMenu(this);
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                DataFolderMenu.ChangeDataFolder(AppName, userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, AppName));
                return;
            }

            base.WndProc(ref m);
        }

        private void textBox_FolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(textBox_FolderPath.Text, e);
        }

        // 現在の拡大率でプレビューを表示する(isSample=trueのときは1枚だけ)
        private void ShowPreview(bool isSample)
        {
            PreView pv = new PreView();
            pv.SetSize(hScrollBar_Scaling.Value);
            pv.View(imageList, listView_Image, textBox_FolderPath.Text, textBox_Extension.Text, isSample);
        }

        private void button_SampleView_Click(object sender, EventArgs e)
        {
            ShowPreview(true);
        }

        private void button_ListView_Click(object sender, EventArgs e)
        {
            ShowPreview(false);
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
                ShowPreview(true);
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
            String loadFileName = Path.Combine(userDataFolder, comboBox_Profile.Text);
            LoadProfile(loadFileName);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String loadFileName = fio.SelectLoadFileName(DefaultSaveName, userDataFolder);
            if (LoadProfile(loadFileName))
            {
                comboBox_Profile.Text = Path.GetFileName(loadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_Profile, ProfileExtensions, SaveProfile, userDataFolder);
        }
    }
}

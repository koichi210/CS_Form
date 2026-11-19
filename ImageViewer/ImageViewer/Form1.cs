using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace ImageViewer
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private readonly StcFileInputOutput _fio = new StcFileInputOutput();
        private const String _defaultSaveName = @"ImageViewer.json";

        // 設定ファイルはJSONが基本。旧XML(ImageViewer.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String _legacySettingFileName = @"ImageViewer.xml";
        private static readonly String[] _profileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\ImageViewer\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String _appName = "ImageViewer";
        private String _userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(_sr, filePath) : _sr.LoadXmlFile(filePath);
        }

        private Boolean SaveProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(_sr, filePath) : _sr.SaveXmlFile(filePath);
        }


        public Form1()
        {
            InitializeComponent();
            InitializePlaceholders();
            InitializeToolTips();
            textBox_FolderPath.Text = @"C:\tmp";
            // 既定は従来どおりpngだけ。動画(mp4/mov/avi/wmv/mkv等)は"*.mp4"や"*.png;*.mp4"のように
            // 指定すると、Windowsのシェルが作る先頭付近のフレームのサムネイルで表示する([[Preview.cs]])
            textBox_Extension.Text = @"*.png";

            hScrollBar_Scaling.Minimum = 2;
            hScrollBar_Scaling.Maximum = 256;
            hScrollBar_Scaling.LargeChange = 50;    // バーと矢印の間クリック
            hScrollBar_Scaling.SmallChange = 1;     // 矢印クリック
            hScrollBar_Scaling.Value = 100;

            comboBox_SortOrder.Items.AddRange(FileSorter.DisplayNames);
            comboBox_SortOrder.SelectedIndex = 0;

            InitializeCommonSettings(Properties.Resources.ImageViewer);

            _sr.RegisterItem(this);
            LoadUserData();

            // 設定の読み込みで並び順が変わったときに表示し直さないよう、読み込みの後でつなぐ
            comboBox_SortOrder.SelectedIndexChanged += comboBox_SortOrder_SelectedIndexChanged;
            checkBox_SortDescending.CheckedChanged += checkBox_SortDescending_CheckedChanged;
        }

        // 保存先フォルダの既定設定ファイルとプロファイル一覧を読み込む
        // (起動時と、データ保存先の変更直後に呼ぶ)
        private void LoadUserData()
        {
            // 前の保存先のプロファイルのフォルダから作ったサムネイル一覧を残さない(読み込み中なら打ち切る)
            _loadGeneration++;
            listView_Image.Items.Clear();
            imageList.Images.Clear();

            String defaultJsonPath = Path.Combine(_userDataFolder, _defaultSaveName);
            String defaultXmlPath = Path.Combine(_userDataFolder, _legacySettingFileName);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath,
                path => _sr.LoadXmlFile(path));
            _util.UpdateProfileList(comboBox_Profile, _profileExtensions, _defaultSaveName, _userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 対象外の欄は無し(2つとも手入力する欄)
        private void InitializePlaceholders()
        {
            textBox_FolderPath.PlaceholderText = @"例: C:\tmp";
            textBox_Extension.PlaceholderText = "例: *.png;*.mp4";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_FolderPath, "画像を探すフォルダ。Enterキーでそのフォルダを開く");
            toolTip.SetToolTip(textBox_Extension, "表示するファイルの絞り込み条件。ワイルドカード(*や?)を使ったパターンを指定する。" +
                "複数は;で区切る(例: *.png;*.mp4)。動画は先頭付近のフレームを表示し、取れない動画はサムネイル無しで並べる");
            toolTip.SetToolTip(hScrollBar_Scaling, "サムネイルの一辺の大きさ(ピクセル)。動かし終えると、フォルダが有効なら先頭の1枚だけで表示し直す");
            toolTip.SetToolTip(listView_Image, "サムネイルをダブルクリックすると、その画像を関連付けられたアプリで開く。" +
                "「拡大表示」の画面を開いている間は、選んだ画像(動画は指定した位置のコマ)をそちらに大きく表示する");
            toolTip.SetToolTip(comboBox_SortOrder, "一覧の並び順。名前順はエクスプローラーと同じく数字を数として比べる(img2 → img10)。変えると表示し直す");
            toolTip.SetToolTip(checkBox_SortDescending, "オンなら逆順(Z→A、大きい順、新しい順)に並べる");
            toolTip.SetToolTip(progressBar_Load, "サムネイルの読み込みの進み具合(読み終えた数/全体の数)");
            toolTip.SetToolTip(button_LargeView, "一覧で選んでいる画像を別の画面で大きく表示する(全体表示・拡大縮小・ガンマ補正・動画の指定コマ・PNG保存)");
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
                DataFolderMenu.ChangeDataFolder(_appName, _userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName),
                    newFolder =>
                    {
                        _userDataFolder = newFolder;
                        LoadUserData();
                    });
                return;
            }

            base.WndProc(ref m);
        }

        private void textBox_FolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(textBox_FolderPath.Text, e);
        }

        // サムネイルの読み込みの世代。表示し直すたびに増やし、古い読み込みは途中で打ち切る
        private int _loadGeneration;

        // 「拡大表示」の画面(閉じられたらnullか破棄済み)
        private FormLargeView _largeView;

        // 最後の表示がサンプル表示(1枚だけ)だったか(並び順を変えたときに同じ表示で出し直すため)
        private bool _lastWasSample;

        // 現在の拡大率でプレビューを表示する(isSample=trueのときは1枚だけ)。
        // 以前はUIスレッドで全部読み込んでいたため、枚数が多いと読み終わるまで画面が固まり、どこまで進んだかも
        // 分からなかった。先にファイル名だけ並べ、サムネイルは読み込み用のスレッドで1枚ずつ作って、できた順に入れる。
        // 動画のサムネイル(シェルのCOM)も作るので、スレッドはSTAにする
        private void ShowPreview(bool isSample)
        {
            String folderPath = textBox_FolderPath.Text;
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("無効なパスです。" + Environment.NewLine + folderPath);
                return;
            }

            Preview preview = new Preview
            {
                SortOrder = FileSorter.FromIndex(comboBox_SortOrder.SelectedIndex),
                SortDescending = checkBox_SortDescending.Checked,
            };
            preview.SetSize(hScrollBar_Scaling.Value);

            String[] files = preview.GetSortedTargetFiles(folderPath, textBox_Extension.Text);
            if (isSample)
            {
                files = files.Take(1).ToArray();
            }

            int generation = ++_loadGeneration;
            _lastWasSample = isSample;

            // 前回表示した画像が残っていると、項目のImageIndexが古い画像を指してしまうので消しておく
            imageList.ImageSize = new Size(hScrollBar_Scaling.Value, hScrollBar_Scaling.Value);
            imageList.Images.Clear();
            listView_Image.Clear();
            listView_Image.LargeImageList = imageList;
            ListViewItem[] items = files.Select(file => new ListViewItem(file, -1)).ToArray();
            listView_Image.Items.AddRange(items);

            progressBar_Load.Maximum = Math.Max(1, files.Length);
            progressBar_Load.Value = 0;
            UpdateLoadingTitle(0, files.Length);

            Thread thread = new Thread(() =>
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (generation != _loadGeneration)
                    {
                        return;
                    }

                    Image thumbnail = preview.LoadThumbnail(files[i]);
                    ListViewItem item = items[i];
                    int done = i + 1;
                    try
                    {
                        BeginInvoke(new Action(() =>
                        {
                            if (generation != _loadGeneration)
                            {
                                return;
                            }
                            // 読めない画像・サムネイルが取れない動画はサムネイル無し(ImageIndex=-1)のまま並べる
                            if (thumbnail != null)
                            {
                                imageList.Images.Add(thumbnail);
                                item.ImageIndex = imageList.Images.Count - 1;
                            }
                            progressBar_Load.Value = done;
                            UpdateLoadingTitle(done, files.Length);
                        }));
                    }
                    catch (InvalidOperationException)
                    {
                        // 読み込み中に画面が閉じられた
                        return;
                    }
                }
            });
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        // 読み込み中はタイトルに進み具合(読み終えた数/全体の数)を出す
        private void UpdateLoadingTitle(int done, int total)
        {
            this.Text = (done >= total) ? _appName : _appName + " - 読み込み中 (" + done + "/" + total + ")";
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

        private void comboBox_SortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawIfShown();
        }

        private void checkBox_SortDescending_CheckedChanged(object sender, EventArgs e)
        {
            RedrawIfShown();
        }

        // 並び順を変えたら、一覧が出ているときだけ同じ条件で表示し直す
        // (起動時や設定の読み込みで変わったときに、無効なパスのメッセージを出さないように)
        private void RedrawIfShown()
        {
            if (listView_Image.Items.Count != 0 && Directory.Exists(textBox_FolderPath.Text))
            {
                ShowPreview(_lastWasSample);
            }
        }

        private void button_LargeView_Click(object sender, EventArgs e)
        {
            String filePath = GetSelectedOrFirstFile();
            if (filePath == null)
            {
                MessageBox.Show("表示する画像がありません。先に「一覧表示」か「サンプル表示」をしてください");
                return;
            }

            if (_largeView == null || _largeView.IsDisposed)
            {
                _largeView = new FormLargeView { Owner = this, Icon = this.Icon };
            }
            _largeView.Show();
            _largeView.Activate();
            _largeView.ShowFile(filePath);
        }

        // 拡大表示の画面を開いている間は、一覧で選んだファイルをそちらに表示する
        private void listView_Image_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_largeView == null || _largeView.IsDisposed || !_largeView.Visible || listView_Image.SelectedItems.Count != 1)
            {
                return;
            }
            _largeView.ShowFile(listView_Image.SelectedItems[0].Text);
        }

        private String GetSelectedOrFirstFile()
        {
            if (listView_Image.SelectedItems.Count >= 1)
            {
                return listView_Image.SelectedItems[0].Text;
            }
            return (listView_Image.Items.Count != 0) ? listView_Image.Items[0].Text : null;
        }

        private void listView_Image_DoubleClick(object sender, EventArgs e)
        {
            if (listView_Image.SelectedItems.Count == 1)
            {
                _util.ExecutePath(listView_Image.SelectedItems[0].Text);
            }
        }

        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(_userDataFolder, comboBox_Profile.Text);
            LoadProfile(loadFileName);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String loadFileName = _fio.SelectLoadFileName(_defaultSaveName, _userDataFolder);
            if (LoadProfile(loadFileName))
            {
                comboBox_Profile.Text = Path.GetFileName(loadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, comboBox_Profile, _profileExtensions, SaveProfile, _userDataFolder);
        }
    }
}

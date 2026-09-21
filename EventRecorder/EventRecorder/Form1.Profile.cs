using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StandardTemplate;

namespace EventRecorder
{
    partial class Form1
    {
        // *******************************************************************************
        // 保存/読込

        // コンボボックスで設定ファイルを選び直したら、そのままそれを読み込む(Cheetosと同じ挙動)
        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = System.IO.Path.Combine(userDataFolder, comboBox_Profile.Text);
            LoadProfile(LoadFileName);
        }

        // *******************************************************************************
        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。設定値はこれまでXML(StcSaveRestore)
        // 一本だったが、今後はJSONへ段階的に移行していく方針のため、拡張子で振り分ける。
        // 「既存のXMLをJSONで保存し直す」機能は、専用の変換ボタンを別途作るのではなく、
        // XMLを読み込んだ状態のままファイル保存ダイアログで.json拡張子を選ぶだけで実現できる
        // (BuildProfileFromGridsは読込元の形式を問わず、今グリッドにある内容をそのまま使うため)

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(System.IO.Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 設定ファイル(記録データ+プレイリスト)をまるごと読み込む。拡張子がjsonならJSON、
        // それ以外は従来通りXMLとして読み込む(既定でプレイリストもクリアする、sr.LoadProc相当)
        private void LoadProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                LoadProfileFromJson(filePath, true);
            }
            else
            {
                sr.LoadProc(filePath, this);
            }

            UpdatePlaylistMissingFileHighlights();
        }

        // プレイリスト再生時、各行の設定ファイルを1つずつ読み込む専用(記録データのみ差し替え、
        // プレイリスト自体は触らない。playbackLoader.LoadProc(..., false)のJSON対応版)
        private void LoadProfileForPlayback(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                LoadProfileFromJson(filePath, false);
            }
            else
            {
                playbackLoader.LoadProc(filePath, this, false);
            }
        }

        // JSONファイルを読み込み、記録データ(+clearPlaylistがtrueならプレイリストも)グリッドへ反映する
        private void LoadProfileFromJson(String filePath, Boolean clearPlaylist)
        {
            EventRecorderProfile profile = JsonFileStorage.Load<EventRecorderProfile>(filePath);
            if (profile == null)
            {
                return;
            }

            dataGridView_Events.Rows.Clear();
            if (clearPlaylist)
            {
                dataGridView_Playlist.Rows.Clear();
            }

            textBox_Loop.Text = String.IsNullOrEmpty(profile.LoopCount) ? "1" : profile.LoopCount;

            foreach (MacroEventData ev in profile.Events ?? new List<MacroEventData>())
            {
                int idx = dataGridView_Events.Rows.Add();
                DataGridViewRow row = dataGridView_Events.Rows[idx];
                row.Cells[col_Type.Index].Value = ev.Type;
                row.Cells[col_X.Index].Value = ev.X;
                row.Cells[col_Y.Index].Value = ev.Y;
                row.Cells[col_Key.Index].Value = ev.Key;
                row.Cells[col_Wait.Index].Value = ev.Wait;
                row.Cells[col_Remarks.Index].Value = ev.Remarks;
            }

            if (clearPlaylist)
            {
                foreach (PlaylistEntryData pl in profile.Playlist ?? new List<PlaylistEntryData>())
                {
                    int idx = dataGridView_Playlist.Rows.Add();
                    DataGridViewRow row = dataGridView_Playlist.Rows[idx];
                    row.Cells[col_PlaylistEnabled.Index].Value = pl.Enabled;
                    row.Cells[col_PlaylistFile.Index].Value = pl.FileName;
                    row.Cells[col_PlaylistLoopCount.Index].Value = pl.LoopCount;
                }

                // モード切替ラジオボタン・最小化チェックボックスは、プレイリスト再生時の
                // 各行のファイル読込(clearPlaylist=false)では適用しない(途中でモードが
                // 切り替わってしまうのを防ぐため)
                radioButton_Record.Checked = profile.IsRecordMode;
                radioButton_Playback.Checked = !profile.IsRecordMode;
                checkBox_MinimizeOnPlay.Checked = profile.MinimizeOnPlay;
            }

            // 万一、旧XMLをそのままJSON化しただけ(各行が自分のWaitを持つ旧形式相当)のデータを
            // 読み込んでも安全なように、XML読込時と同じ変換を通しておく
            MigrateWaitColumnToRows();

            // ファイル読込は「ユーザーの編集操作」ではないので、Ctrl+Zで戻せないようにする
            dataGridView_Events.ClearUndoHistory();
            if (clearPlaylist)
            {
                dataGridView_Playlist.ClearUndoHistory();
            }
        }

        // 今のグリッドの中身(記録データ+プレイリスト)をJSON保存用のPOCOに詰め替える
        private EventRecorderProfile BuildProfileFromGrids()
        {
            EventRecorderProfile profile = new EventRecorderProfile();
            profile.LoopCount = textBox_Loop.Text;
            profile.IsRecordMode = radioButton_Record.Checked;
            profile.MinimizeOnPlay = checkBox_MinimizeOnPlay.Checked;

            foreach (DataGridViewRow row in dataGridView_Events.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                profile.Events.Add(new MacroEventData
                {
                    Type = Convert.ToString(row.Cells[col_Type.Index].Value),
                    X = Convert.ToString(row.Cells[col_X.Index].Value),
                    Y = Convert.ToString(row.Cells[col_Y.Index].Value),
                    Key = Convert.ToString(row.Cells[col_Key.Index].Value),
                    Wait = Convert.ToString(row.Cells[col_Wait.Index].Value),
                    Remarks = Convert.ToString(row.Cells[col_Remarks.Index].Value),
                });
            }

            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                profile.Playlist.Add(new PlaylistEntryData
                {
                    Enabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false),
                    FileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value),
                    LoopCount = Convert.ToString(row.Cells[col_PlaylistLoopCount.Index].Value),
                });
            }

            return profile;
        }

        // filePathの拡張子で振り分けて保存する(JSONならJsonFileStorage、XMLなら従来のsr.SaveSetting)
        private Boolean SaveProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                try
                {
                    JsonFileStorage.Save(filePath, BuildProfileFromGrids());
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "保存に失敗したよ: " + ex.Message,
                        "EventRecorder - 保存エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            return sr.SaveSetting(filePath);
        }

        // comboBox_Profile(プレイリストのcol_PlaylistFileも含む)へ、userDataFolder配下の
        // *.xmlと*.jsonの両方をまとめてリストアップする。util.UpdateProfileListは拡張子を
        // 1パターンしか指定できないため、ここでは2回検索した結果をマージして直接セットする
        private void UpdateProfileListAll(String defaultProfileName)
        {
            String[] xmlFiles = System.IO.Directory.GetFiles(userDataFolder, "*.xml", System.IO.SearchOption.AllDirectories);
            // EventRecorder.json(アプリの設定ファイル)はプロファイルではないので除外する
            String[] jsonFiles = System.IO.Directory.GetFiles(userDataFolder, "*.json", System.IO.SearchOption.AllDirectories)
                .Where(f => !IsNonProfileSettingFile(f))
                .ToArray();
            String[] files = xmlFiles.Concat(jsonFiles).ToArray();

            util.SetComboBoxFromArray(comboBox_Profile, files, userDataFolder);
            util.SetComboBoxText(comboBox_Profile, defaultProfileName);
        }

        // UpdateProfileListAllは内部でItems.Clear()するため、comboBox_Profileの選択がいったん
        // 外れてSelectedIndexChanged(=プロファイルの再読み込み。プレイリストもクリアされる)が
        // 誤発火してしまう。保存直後は一覧の見た目を最新化したいだけで、選び直したわけではないので、
        // イベントを一時的に外してから呼ぶ
        private void UpdateProfileListAllWithoutReload(String defaultProfileName)
        {
            comboBox_Profile.SelectedIndexChanged -= comboBox_Profile_SelectedIndexChanged;
            try
            {
                UpdateProfileListAll(defaultProfileName);
            }
            finally
            {
                comboBox_Profile.SelectedIndexChanged += comboBox_Profile_SelectedIndexChanged;
            }
        }

        // 読込ボタンと同じ感覚で使えるよう、「現在のファイルに上書きしますか?」の確認は挟まず、
        // 常にダイアログを直接開く(SelectSaveFileNameのCheetos流の確認ステップはあえて使わない)
        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            // プルダウンで既存ファイルが選ばれている時は、毎回ダイアログを開かず
            // 「上書きしますか?」の確認だけで済ませられるようにする。
            // プルダウンが空の時は、従来通りファイル選択ダイアログを出す
            if (!String.IsNullOrEmpty(comboBox_Profile.Text))
            {
                // はい=上書き保存、いいえ=別名で保存(ダイアログへ進む)、キャンセル=何もせず終了
                DialogResult overwriteResult = MessageBox.Show(
                    comboBox_Profile.Text + " を上書きしますか?",
                    "上書き確認",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (overwriteResult == DialogResult.Cancel)
                {
                    return;
                }

                if (overwriteResult == DialogResult.Yes)
                {
                    String overwriteFileName = System.IO.Path.Combine(userDataFolder, comboBox_Profile.Text);
                    if (!SaveProfile(overwriteFileName))
                    {
                        MessageBox.Show("設定の保存に失敗したよ" + Environment.NewLine + overwriteFileName,
                            AppName + " - エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    UpdateProfileListAllWithoutReload(System.IO.Path.GetFileName(overwriteFileName));
                    SyncPlaylistFileItems();
                    return;
                }
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = comboBox_Profile.Text;
            dlg.InitialDirectory = userDataFolder;
            // 今後はJSON保存を主流にしていく方針なので、フィルタの先頭(既定)をJSONにしてある。
            // 既存のXMLプロファイルを開いた状態でここに来て.jsonを選べば、そのままXML→JSON変換になる
            dlg.Filter = "JSONファイル(*.json)|*.json|XMLファイル(*.xml)|*.xml|すべてのファイル(*.*)|*.*";
            dlg.Title = "保存するプロファイルを選択してください";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            String SaveFileName = dlg.FileName;
            if (!SaveProfile(SaveFileName))
            {
                MessageBox.Show("設定の保存に失敗したよ" + Environment.NewLine + SaveFileName,
                    AppName + " - エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateProfileListAllWithoutReload(System.IO.Path.GetFileName(SaveFileName));
            SyncPlaylistFileItems();
        }
    }
}

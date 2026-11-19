using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using StandardTemplate;

namespace Cheetos
{
    // CaptureWindow
    partial class Cheetos
    {
        private void tabPageCaptureWindow_MouseMove(object sender, MouseEventArgs e)
        {
            cw_TextBox_Status.Text = "X=" + Cursor.Position.X.ToString() + ", Y=" + Cursor.Position.Y.ToString();
        }

        // ドラッグして離した位置の座標を、表の選択中の行のMouseX/MouseYに入れる。
        // タブの余白か座標の表示欄でマウスを押して始め、押している間はマウスを捕まえておくので、
        // このウィンドウの外(座標を取りたい別のアプリの上)で離しても拾える
        private Boolean _isCoordinateDragging = false;
        private Point _coordinateDragStart;

        private void InitializeCoordinateDrag()
        {
            CaptureWindow.MouseDown += CoordinateDrag_MouseDown;
            CaptureWindow.MouseUp += CoordinateDrag_MouseUp;
            cw_TextBox_Status.MouseDown += CoordinateDrag_MouseDown;
            cw_TextBox_Status.MouseUp += CoordinateDrag_MouseUp;
            cw_TextBox_Status.MouseMove += tabPageCaptureWindow_MouseMove;
            cw_TextBox_Status.Cursor = Cursors.Cross;
        }

        private void CoordinateDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _isCaptureRunning)
            {
                return;
            }
            _isCoordinateDragging = true;
            _coordinateDragStart = Cursor.Position;
            ((Control)sender).Capture = true;
            Cursor.Current = Cursors.Cross;
        }

        private void CoordinateDrag_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isCoordinateDragging || e.Button != MouseButtons.Left)
            {
                return;
            }
            _isCoordinateDragging = false;
            ((Control)sender).Capture = false;
            Cursor.Current = Cursors.Default;

            // 押しただけ(ほとんど動かしていない)のときは何もしない
            Point pos = Cursor.Position;
            Size dragSize = SystemInformation.DragSize;
            if (Math.Abs(pos.X - _coordinateDragStart.X) < dragSize.Width
                && Math.Abs(pos.Y - _coordinateDragStart.Y) < dragSize.Height)
            {
                return;
            }
            SetCurrentRowMousePoint(pos);
        }

        private void SetCurrentRowMousePoint(Point pos)
        {
            cw_dataGridView.EndEdit();
            int rowIdx = (cw_dataGridView.CurrentCell == null) ? 0 : cw_dataGridView.CurrentCell.RowIndex;
            if (rowIdx >= cw_dataGridView.RowCount)
            {
                return;
            }
            _util.SetDataGridCell(cw_dataGridView, rowIdx, GetDataGridColumnIdx(_gridHeaderMouseXStr), pos.X.ToString());
            _util.SetDataGridCell(cw_dataGridView, rowIdx, GetDataGridColumnIdx(_gridHeaderMouseYStr), pos.Y.ToString());
            cw_TextBox_Status.Text = "X=" + pos.X + ", Y=" + pos.Y + " →" + (rowIdx + 1) + "行目";
        }

        // Captureボタンを押した瞬間のマウス座標。一連のCapture処理が終わったらここへ戻す
        private Point _captureStartCursorPosition;

        // 次の行の予定時刻をこの時間過ぎても始まっていなければ、赤字で知らせる
        // (キャプチャはクリップボードへ入るのを最大2秒待つので、その分は余裕を見る)
        private static readonly TimeSpan _captureOverdueGrace = TimeSpan.FromSeconds(5);

        private CaptureRunner _captureRunner;
        private Timer _captureMonitorTimer;
        private Boolean _isOverdueWarningShown = false;

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            // 実行中だったら停止する(他タブのBackgroundWorkerと同じ「もう一度押すと中断」)。
            // _isCaptureRunningは終了処理(FinishCapture)が終わるまでtrueのままにしておく(ここでfalseにすると、
            // 終了処理が終わる前にもう一度押されたとき二重起動してしまう)
            if (_isCaptureRunning)
            {
                if (_captureRunner != null)
                {
                    _captureRunner.Stop();
                }
                return;
            }

            cw_dataGridView.EndEdit();

            // 空欄なら1回。数値以外はint.Parseで例外になっていたので先に確かめる
            int loopCount = 1;
            if (cw_TextBox_Loop.Text != String.Empty
                && !TryGetIntWithMessage(cw_TextBox_Loop, "繰り返し数", 1, out loopCount))
            {
                return;
            }

            // 空欄か0なら自動復旧しない
            int watchdogSeconds = 0;
            if (cw_TextBox_Watchdog.Text != String.Empty
                && !TryGetIntWithMessage(cw_TextBox_Watchdog, "自動復旧(秒)", 0, out watchdogSeconds))
            {
                return;
            }

            _fio.EnsureDirectory(cw_TextBox_SavePath.Text, true);
            String filePathPrefix = Logic.BuildFilePathPrefix(cw_TextBox_SavePath.Text, cw_TextBox_SaveFilePrefix.Text, cw_checkBox_AddTimeStamp.Checked);

            // キャプチャ対象を設定。以降はバックグラウンドスレッドで動くため、
            // コントロールの値はここ(UIスレッド)で読み取っておく
            // (以前は1行ごとにInvokeで表から読んでいた。実行中に表を書き換えても今の実行には効かない)
            CaptWindow.CaptureTargetType captTarget;
            if (cw_Radio_FullScreen.Checked)
            {
                captTarget = CaptWindow.CaptureTargetType.FullScreen;
            }
            else if (cw_Radio_CurrentScreen.Checked)
            {
                captTarget = CaptWindow.CaptureTargetType.CurrentScreen;
            }
            else // cw_Radio_CurrentWindow
            {
                captTarget = CaptWindow.CaptureTargetType.CurrentWindow;
            }
            List<CaptureRunner.Step> steps = BuildCaptureSteps();

            // 保存先に同じ名前の画像があれば、上書きしてよいか先に聞く
            // (途中で聞くと、無人で回している間そこで止まってしまうため、始める前に1回だけ聞く)
            if (!ConfirmOverwriteCaptureFiles(filePathPrefix, loopCount, steps.Count(step => step.IsCapture)))
            {
                return;
            }

            _captureStartCursorPosition = Cursor.Position;
            InitProgressBar(loopCount);
            SetStartTime();
            textBox_NextEventTime.Text = "";
            ShowCaptureWarning("");

            CaptureRunner runner = new CaptureRunner
            {
                FilePathPrefix = filePathPrefix,
                LoopCount = loopCount,
                CaptureTarget = captTarget,
                InitialSleepMsec = cw_TextBox_Sleep.Text,
                Steps = steps,
                TargetWindow = this,
                WatchdogSeconds = watchdogSeconds,
            };
            runner.LoopCompleted = doneLoop => this.Invoke((MethodInvoker)delegate ()
            {
                if (textBox_ExpectEndTime.Text == "")
                {
                    SetExpectEndTime(loopCount, doneLoop);
                }
                UpdateProgressBar();
            });
            runner.Completed = errLog => this.Invoke((MethodInvoker)delegate () { FinishCapture(errLog, null); });
            runner.Failed = ex => this.Invoke((MethodInvoker)delegate () { FinishCapture(null, ex); });
            runner.Log = text => _debugLog.WriteData(text);

            _debugLog.WriteData("Capture: START", false);
            _captureRunner = runner;
            _isCaptureRunning = true;
            cw_Button_Capture.Text = "中断";
            runner.Start();
            StartCaptureMonitor();
        }

        // 表の各行を、処理用スレッドへ渡す形にする
        private List<CaptureRunner.Step> BuildCaptureSteps()
        {
            int sleepIdx = GetDataGridColumnIdx(_gridHeaderSleepStr);
            int mouseXIdx = GetDataGridColumnIdx(_gridHeaderMouseXStr);
            int mouseYIdx = GetDataGridColumnIdx(_gridHeaderMouseYStr);
            int mouseActionIdx = GetDataGridColumnIdx(_gridHeaderMouseActionStr);
            int captureIdx = GetDataGridColumnIdx(_gridHeaderCaptureStr);

            List<CaptureRunner.Step> steps = new List<CaptureRunner.Step>();
            for (int row = 0; row < cw_dataGridView.RowCount; row++)
            {
                steps.Add(new CaptureRunner.Step
                {
                    SleepMsec = _util.GetDataGridCell(cw_dataGridView, row, sleepIdx),
                    MouseX = _util.GetDataGridCell(cw_dataGridView, row, mouseXIdx),
                    MouseY = _util.GetDataGridCell(cw_dataGridView, row, mouseYIdx),
                    MouseEvent = GetMouseEvent(_util.GetDataGridCell(cw_dataGridView, row, mouseActionIdx)),
                    IsCapture = IsCaptureEvent(_util.GetDataGridCell(cw_dataGridView, row, captureIdx)),
                });
            }
            return steps;
        }

        // これから保存する画像と同じ名前のファイルが既にあれば、上書きしてよいか聞く。
        // 無ければ聞かずにtrue、「いいえ」ならfalse(Captureを始めない)
        private Boolean ConfirmOverwriteCaptureFiles(String filePathPrefix, int loopCount, int captureRowCount)
        {
            List<String> existingFiles = Logic.BuildCaptureFilePaths(filePathPrefix, loopCount, captureRowCount)
                .Where(File.Exists)
                .ToList();
            if (existingFiles.Count == 0)
            {
                return true;
            }

            const int maxNamesInMessage = 5;
            String names = String.Join(Environment.NewLine, existingFiles.Take(maxNamesInMessage).Select(Path.GetFileName));
            if (existingFiles.Count > maxNamesInMessage)
            {
                names += Environment.NewLine + "ほか" + (existingFiles.Count - maxNamesInMessage) + "件";
            }
            DialogResult result = MessageBox.Show(
                "保存先に同じ名前の画像が" + existingFiles.Count + "件あります。上書きしてよいですか？" + Environment.NewLine + names,
                "上書きの確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            return result == DialogResult.Yes;
        }

        // 一連のCaptureが終わった(中断・エラー・自動復旧の上限も含む)ときにUIスレッドで呼ぶ
        private void FinishCapture(String errLog, Exception error)
        {
            StopCaptureMonitor();
            _isCaptureRunning = false;
            cw_Button_Capture.Text = "Capture";
            textBox_NextEventTime.Text = "";
            if (_isOverdueWarningShown)
            {
                ShowCaptureWarning("");
            }

            // Capture処理が全て終わったら、ボタンを押した時のマウス座標へ戻す
            Cursor.Position = _captureStartCursorPosition;

            if (error != null)
            {
                MessageBox.Show("キャプチャ中にエラーが発生したよ" + Environment.NewLine + error.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TextBox_Status.Text += " 完了";
            if (!String.IsNullOrEmpty(errLog))
            {
                MessageBox.Show(errLog, "エラー", MessageBoxButtons.OK);
            }
        }

        // 実行中は0.5秒ごとに、次の行の予定時刻の表示・遅れの赤字表示・自動復旧(WatchDog)を行う
        private void StartCaptureMonitor()
        {
            if (_captureMonitorTimer == null)
            {
                _captureMonitorTimer = new Timer { Interval = 500 };
                _captureMonitorTimer.Tick += CaptureMonitorTimer_Tick;
                this.Disposed += (s, e) => _captureMonitorTimer.Dispose();
            }
            _captureMonitorTimer.Start();
        }

        private void StopCaptureMonitor()
        {
            if (_captureMonitorTimer != null)
            {
                _captureMonitorTimer.Stop();
            }
        }

        private void CaptureMonitorTimer_Tick(object sender, EventArgs e)
        {
            CaptureRunner runner = _captureRunner;
            if (runner == null || !_isCaptureRunning)
            {
                return;
            }

            DateTime now = DateTime.Now;
            String recovered = runner.CheckWatchdog(now);
            if (recovered != null)
            {
                // 自動復旧したことは、次に遅れるか終わるまで残しておく
                ShowCaptureWarning(recovered);
            }

            DateTime? next = runner.NextEventTime;
            textBox_NextEventTime.Text = next.HasValue ? next.Value.ToString("HH:mm:ss") : "";

            if (next.HasValue && Logic.IsOverdue(now, next.Value, _captureOverdueGrace))
            {
                ShowCaptureWarning("予定(" + next.Value.ToString("HH:mm:ss") + ")を過ぎても次の操作が始まっていません");
                _isOverdueWarningShown = true;
            }
            else if (_isOverdueWarningShown)
            {
                ShowCaptureWarning("");
            }
        }

        // 処理状況の欄に赤字で出す(メッセージボックスは使わない。無人で回している時に止まってしまうため)
        private void ShowCaptureWarning(String message)
        {
            label_CaptureWarning.Text = message;
            _isOverdueWarningShown = false;
        }

        private void cw_TextBox_SavePath_KeyUp(object sender, KeyEventArgs e)
        {
            pt_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            pr_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            do_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            do_DestPortFolderPath.Text = cw_TextBox_SavePath.Text + "_port";
            do_DestLandFolderPath.Text = cw_TextBox_SavePath.Text + "_land";
            pm_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            fc_SourceFolderPath.Text = cw_TextBox_SavePath.Text;

        }
        private void cw_TextBox_SavePath_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(cw_TextBox_SavePath.Text, e);
        }


        private void UpdateProgressBar()
        {
            if (ProgressBar_Status.Value < ProgressBar_Status.Maximum)
            {
                ProgressBar_Status.Value++;
            }
            TextBox_Status.Text = ProgressBar_Status.Value.ToString() + "/" + ProgressBar_Status.Maximum.ToString();
        }
    }
}

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using StandardTemplate;

namespace Cheetos
{
    // CaptureWindow
    partial class Cheetos
    {
        private readonly CaptWindow _captWindow = new CaptWindow();
        private void tabPageCaptureWindow_MouseMove(object sender, MouseEventArgs e)
        {
            cw_TextBox_Status.Text = "X=" + Cursor.Position.X.ToString() + ", Y=" + Cursor.Position.Y.ToString();
        }

        // Captureボタンを押した瞬間のマウス座標。一連のCapture処理が終わったらここへ戻す
        private Point _captureStartCursorPosition;

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            // 実行中だったら停止する(他タブのBackgroundWorkerと同じ「もう一度押すと中断」)。
            // isCaptureRunningはスレッドが終わるまでtrueのままにしておく(ここでfalseにすると、
            // スレッドの終了処理が終わる前にもう一度押されたとき二重起動してしまう)
            if (_isCaptureRunning)
            {
                _captWindow.Stop();
                return;
            }

            _captureStartCursorPosition = Cursor.Position;

            _fio.EnsureDirectory(cw_TextBox_SavePath.Text, true);
            String filePathPrefix = Logic.BuildFilePathPrefix(cw_TextBox_SavePath.Text, cw_TextBox_SaveFilePrefix.Text, cw_checkBox_AddTimeStamp.Checked);

            int loopCount = 1;
            if (cw_TextBox_Loop.Text != String.Empty)
            {
                loopCount = int.Parse(cw_TextBox_Loop.Text);
            }
            InitProgressBar(loopCount);
            SetStartTime();

            // キャプチャ対象を設定。以降はバックグラウンドスレッドで動くため、
            // コントロールの値はここ(UIスレッド)で読み取っておく
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
            String sleepMsecText = cw_TextBox_Sleep.Text;
            int gridRowCount = cw_dataGridView.RowCount;

            _isCaptureRunning = true;
            cw_Button_Capture.Text = "中断";

            CaptureForeground(filePathPrefix, loopCount, captTarget, sleepMsecText, gridRowCount);
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


        // 以前は丸ごとthis.Invoke(...)の中でループを回していたため、ループ中UIスレッドが
        // 占有されっぱなしになり、「中断」のためのボタン再クリックがループ終了まで処理されなかった。
        // (SendKeys/ClipboardはSTAスレッドが必要なため、専用のSTAスレッドでループを回し、
        //  Formのコントロールに触る部分だけInvokeでUIスレッドに戻す。UIスレッドはクリックを
        //  即座に処理できるので中断が効くようになる)
        private void CaptureForeground(String filePathPrefix, int loopCount, CaptWindow.CaptureTargetType captTarget, String sleepMsecText, int gridRowCount)
        {
            Thread captureThread = new Thread(() =>
            {
                try
                {
                    // 初期化
                    _captWindow.Initialize();

                    // マウス移動後にもとの位置へ戻すか
                    _captWindow.SetRestoreMousePosition(false);

                    // 実行前のSleep
                    _captWindow.SetSleepTimeMsec(sleepMsecText);
                    _captWindow.ExecuteSleep();

                    _captWindow.SetCaptureTarget(captTarget);

                    _debugLog.WriteData("Capture: START", false);
                    for (int i = 1; i <= loopCount && !_captWindow.IsStopRequest; i++)
                    {
                        _debugLog.WriteData("Capture: Loop=" + i.ToString() + "/" + loopCount.ToString());

                        // ファイルのIndex番号を初期化
                        _captWindow.SetFileIdx(1);

                        // ファイル名生成
                        String fileFormat = filePathPrefix + String.Format("{0:D4}", i);
                        _captWindow.SetFileFormat(fileFormat);
                        _debugLog.WriteData(" Capture: Filename=" + fileFormat);

                        // いまのところマウス移動しないユースケースは無い
                        _captWindow.SetMouseMove(true);

                        // 順次Capture実行
                        for (int j = 0; j < gridRowCount && !_captWindow.IsStopRequest; j++)
                        {
                            _debugLog.WriteData(" Capture: RowCnt=" + j.ToString() + "/" + gridRowCount.ToString());

                            // グリッドの値はUIスレッドが持つコントロールなのでInvokeで読み出す
                            String captureEventStr = String.Empty, pointX = String.Empty, pointY = String.Empty, mouseEventStr = String.Empty, sleepMsec = String.Empty;
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                captureEventStr = _util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(_gridHeaderCaptureStr));
                                pointX = _util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(_gridHeaderMouseXStr));
                                pointY = _util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(_gridHeaderMouseYStr));
                                mouseEventStr = _util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(_gridHeaderMouseActionStr));
                                sleepMsec = _util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(_gridHeaderSleepStr));
                            });

                            _captWindow.SetCaptureCase(IsCaptureEvent(captureEventStr));
                            _debugLog.WriteData("  Capture: SetCaptureCase() Done");

                            if (_captWindow.SetMousePoint(pointX, pointY))
                            {
                                _captWindow.SetMouseEvent(GetMouseEvent(mouseEventStr));

                                _captWindow.MouseProc();
                                _debugLog.WriteData("  Capture: MouseEvent() Complete");
                            }

                            _captWindow.SetSleepTimeMsec(sleepMsec);
                            _captWindow.ExecuteSleep();
                            _debugLog.WriteData("  Capture: ExecuteSleep() Complete");

                            _captWindow.CaptureProc();
                            _debugLog.WriteData("  Capture: CaptureProc() Complete");
                        }

                        // 終了予想時間・進捗バー更新
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            if (i == 1)
                            {
                                SetExpectEndTime(loopCount);
                            }
                            UpdateProgressBar();
                        });
                    }
                    _debugLog.WriteData("Capture: END" + Environment.NewLine);

                    String errLog = _captWindow.GetErrorLog();
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        TextBox_Status.Text += " 完了";

                        // Capture処理が全て終わったら、ボタンを押した時のマウス座標へ戻す
                        Cursor.Position = _captureStartCursorPosition;

                        if (errLog != String.Empty)
                        {
                            MessageBox.Show(errLog, "エラー", MessageBoxButtons.OK);
                        }
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        Cursor.Position = _captureStartCursorPosition;
                        MessageBox.Show("キャプチャ中にエラーが発生したよ" + Environment.NewLine + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
                finally
                {
                    _isCaptureRunning = false;
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        cw_Button_Capture.Text = "Capture";
                    });
                }
            });

            // SendKeys/ClipboardはSTAスレッドでないと使えないため、STAで起動する
            captureThread.SetApartmentState(ApartmentState.STA);
            captureThread.IsBackground = true;
            captureThread.Start();
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

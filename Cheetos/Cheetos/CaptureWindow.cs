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
        private readonly CaptWindow cw = new CaptWindow();
        private void tabPageCaptureWindow_MouseMove(object sender, MouseEventArgs e)
        {
            cw_TextBox_Status.Text = "X=" + Cursor.Position.X.ToString() + ", Y=" + Cursor.Position.Y.ToString();
        }

        // Captureボタンを押した瞬間のマウス座標。一連のCapture処理が終わったらここへ戻す
        private Point captureStartCursorPosition;

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            // 実行中だったら停止する(他タブのBackgroundWorkerと同じ「もう一度押すと中断」)。
            // isCaptureRunningはスレッドが終わるまでtrueのままにしておく(ここでfalseにすると、
            // スレッドの終了処理が終わる前にもう一度押されたとき二重起動してしまう)
            if (isCaptureRunning)
            {
                cw.Stop();
                return;
            }

            captureStartCursorPosition = Cursor.Position;

            fio.EnsureDirectory(cw_TextBox_SavePath.Text, true);
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
            CaptWindow.CAPTURE_TARGET captTarget;
            if (cw_Radio_FullScreen.Checked)
            {
                captTarget = CaptWindow.CAPTURE_TARGET.FULL_SCREEN;
            }
            else if (cw_Radio_CurrentScreen.Checked)
            {
                captTarget = CaptWindow.CAPTURE_TARGET.CURRENT_SCREEN;
            }
            else // cw_Radio_CurrentWindow
            {
                captTarget = CaptWindow.CAPTURE_TARGET.CURRENT_WINDOW;
            }
            String sleepMsecText = cw_TextBox_Sleep.Text;
            int gridRowCount = cw_dataGridView.RowCount;

            isCaptureRunning = true;
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
            util.ExecutePath(cw_TextBox_SavePath.Text, e);
        }


        // 以前は丸ごとthis.Invoke(...)の中でループを回していたため、ループ中UIスレッドが
        // 占有されっぱなしになり、「中断」のためのボタン再クリックがループ終了まで処理されなかった。
        // (SendKeys/ClipboardはSTAスレッドが必要なため、専用のSTAスレッドでループを回し、
        //  Formのコントロールに触る部分だけInvokeでUIスレッドに戻す。UIスレッドはクリックを
        //  即座に処理できるので中断が効くようになる)
        private void CaptureForeground(String filePathPrefix, int loopCount, CaptWindow.CAPTURE_TARGET captTarget, String sleepMsecText, int gridRowCount)
        {
            Thread captureThread = new Thread(() =>
            {
                try
                {
                    // 初期化
                    cw.Initialize();

                    // マウス移動後にもとの位置へ戻すか
                    cw.SetRestoreMousePosition(false);

                    // 実行前のSleep
                    cw.SetSleepTimeMsec(sleepMsecText);
                    cw.ExecuteSleep();

                    cw.SetCaptureTarget(captTarget);

                    debugLog.WriteData("Capture: START", false);
                    for (int i = 1; i <= loopCount && !cw.IsStopRequest; i++)
                    {
                        debugLog.WriteData("Capture: Loop=" + i.ToString() + "/" + loopCount.ToString());

                        // ファイルのIndex番号を初期化
                        cw.SetFileIdx(1);

                        // ファイル名生成
                        String fileFormat = filePathPrefix + String.Format("{0:D4}", i);
                        cw.SetFileFormat(fileFormat);
                        debugLog.WriteData(" Capture: Filename=" + fileFormat);

                        // いまのところマウス移動しないユースケースは無い
                        cw.SetMouseMove(true);

                        // 順次Capture実行
                        for (int j = 0; j < gridRowCount && !cw.IsStopRequest; j++)
                        {
                            debugLog.WriteData(" Capture: RowCnt=" + j.ToString() + "/" + gridRowCount.ToString());

                            // グリッドの値はUIスレッドが持つコントロールなのでInvokeで読み出す
                            String captureEventStr = String.Empty, pointX = String.Empty, pointY = String.Empty, mouseEventStr = String.Empty, sleepMsec = String.Empty;
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                captureEventStr = util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(GridHeaderCaptureStr));
                                pointX = util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(GridHeaderMouseXStr));
                                pointY = util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(GridHeaderMouseYStr));
                                mouseEventStr = util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(GridHeaderMouseActionStr));
                                sleepMsec = util.GetDataGridCell(cw_dataGridView, j, GetDataGridColumnIdx(GridHeaderSleepStr));
                            });

                            cw.SetCaptureCase(IsCaptureEvent(captureEventStr));
                            debugLog.WriteData("  Capture: SetCaptureCase() Done");

                            if (cw.SetMousePoint(pointX, pointY))
                            {
                                cw.SetMouseEvent(GetMouseEvent(mouseEventStr));

                                cw.MouseProc();
                                debugLog.WriteData("  Capture: MouseEvent() Complete");
                            }

                            cw.SetSleepTimeMsec(sleepMsec);
                            cw.ExecuteSleep();
                            debugLog.WriteData("  Capture: ExecuteSleep() Complete");

                            cw.CaptureProc();
                            debugLog.WriteData("  Capture: CaptureProc() Complete");
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
                    debugLog.WriteData("Capture: END" + Environment.NewLine);

                    String errLog = cw.GetErrorLog();
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        TextBox_Status.Text += " 完了";

                        // Capture処理が全て終わったら、ボタンを押した時のマウス座標へ戻す
                        Cursor.Position = captureStartCursorPosition;

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
                        Cursor.Position = captureStartCursorPosition;
                        MessageBox.Show("キャプチャ中にエラーが発生したよ" + Environment.NewLine + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
                finally
                {
                    isCaptureRunning = false;
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

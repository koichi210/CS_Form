using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
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
            // 実行中だったら停止する
            if (isCaptureRunning)
            {
                isCaptureRunning = false;
                cw.Stop();
                return;
            }

            captureStartCursorPosition = Cursor.Position;

            fio.EnsureDirectory(cw_TextBox_SavePath.Text, true);
            String fileBaseFormat = Logic.GetFileBaseFormat(cw_TextBox_SavePath.Text, cw_TextBox_SaveFilePrefix.Text, cw_checkBox_AddTimeStamp.Checked);

            int loopCount = 1;
            if (cw_TextBox_Loop.Text != String.Empty)
            {
                loopCount = int.Parse(cw_TextBox_Loop.Text);
            }
            InitProgressBar(loopCount);
            SetStartTime();

            CaptureForeground(fileBaseFormat, loopCount);

            String errLog = cw.GetErrorLog();
            if (errLog != String.Empty)
            {
                MessageBox.Show(errLog, "エラー", MessageBoxButtons.OK);
            }
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


        private void CaptureForeground(String fileBaseFormat, int loopCount)
        {
            Task task = new Task(() =>
            {
                isCaptureRunning = true;
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        // 初期化
                        cw.Initialize();

                        // マウス移動後にもとの位置へ戻すか
                        cw.SetRestoreMousePosition(false);

                        // 実行前のSleep
                        cw.SetSleepTimeMsec(cw_TextBox_Sleep.Text);
                        cw.ExecuteSleep();

                        // キャプチャ対象を設定
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
                        cw.SetCaptureTarget(captTarget);

                        debugLog.WriteData("Capture: START", false);
                        for (int i = 1; i <= loopCount && isCaptureRunning; i++, UpdateProgressBar())
                        {
                            debugLog.WriteData("Capture: Loop=" + i.ToString() + "/" + loopCount.ToString());

                            // ファイルのIndex番号を初期化
                            cw.SetFileIdx(1);

                            // ファイル名生成
                            String fileFormat = fileBaseFormat + String.Format("{0:D4}", i);
                            cw.SetFileFormat(fileFormat);
                            debugLog.WriteData(" Capture: Filename=" + fileFormat);

                            // いまのところマウス移動しないユースケースは無い
                            cw.SetMouseMove(true);

                            // 順次Capture実行
                            for (int j = 0; j < cw_dataGridView.RowCount; j++)
                            {
                                debugLog.WriteData(" Capture: RowCnt=" + j.ToString() + "/" + cw_dataGridView.RowCount.ToString());

                                int columnIdx = GetDataGridColumnIdx(GridHeaderCaptureStr);
                                String captureEventStr = util.GetDataGridCell(cw_dataGridView, j, columnIdx);
                                cw.SetCaptureCase(IsCaptureEvent(captureEventStr));
                                debugLog.WriteData("  Capture: SetCaptureCase() Done");

                                columnIdx = GetDataGridColumnIdx(GridHeaderMouseXStr);
                                String pointX = util.GetDataGridCell(cw_dataGridView, j, columnIdx);

                                columnIdx = GetDataGridColumnIdx(GridHeaderMouseYStr);
                                String pointY = util.GetDataGridCell(cw_dataGridView, j, columnIdx);

                                if (cw.SetMousePoint(pointX, pointY))
                                {
                                    columnIdx = GetDataGridColumnIdx(GridHeaderMouseActionStr);
                                    String mouseEventStr = util.GetDataGridCell(cw_dataGridView, j, columnIdx);
                                    cw.SetMouseEvent(GetMouseEvent(mouseEventStr));

                                    cw.MouseProc();
                                    debugLog.WriteData("  Capture: MouseEvent() Complete");
                                }

                                columnIdx = GetDataGridColumnIdx(GridHeaderSleepStr);
                                String sleepMsec = util.GetDataGridCell(cw_dataGridView, j, columnIdx);
                                cw.SetSleepTimeMsec(sleepMsec);
                                cw.ExecuteSleep();
                                debugLog.WriteData("  Capture: ExecuteSleep() Complete");

                                cw.CaptureProc();
                                debugLog.WriteData("  Capture: CaptureProc() Complete");
                            }

                            // 終了予想時間
                            if (i == 1)
                            {
                                SetExpectEndTime(loopCount);
                            }
                        }
                        debugLog.WriteData("Capture: END" + Environment.NewLine);
                        TextBox_Status.Text += " 完了";

                        // Capture処理が全て終わったら、ボタンを押した時のマウス座標へ戻す
                        Cursor.Position = captureStartCursorPosition;
                    });
                isCaptureRunning = false;
            });

            // Task内の例外はどこにも通知されず消えてしまい、isCaptureRunningもtrueのまま残るため、失敗時はUIスレッドで後始末と通知を行う
            task.ContinueWith(t =>
            {
                isCaptureRunning = false;
                Cursor.Position = captureStartCursorPosition;
                MessageBox.Show("キャプチャ中にエラーが発生したよ" + Environment.NewLine + t.Exception.GetBaseException().Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());
            task.Start();
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

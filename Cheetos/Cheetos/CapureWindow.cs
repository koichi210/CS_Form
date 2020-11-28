using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Picture;
using System.Threading.Tasks;
using StandardTemplate;

namespace Cheetos
{
    // PictTrim
    public partial class Cheetos : Form
    {
        public CaptWindow cw = new CaptWindow();

        public struct EventTable
        {
            public Point pt;
            public bool IsMouseMove;
            public bool IsCapture;
        }

        private void tabPageCaptureWindow_MouseMove(object sender, MouseEventArgs e)
        {
            cw_TextBox_Status.Text = "X=" + Cursor.Position.X.ToString() + ", Y=" + Cursor.Position.Y.ToString();
        }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            // 実行中だったら停止する
            if (IsTaskRun)
            {
                IsTaskRun = false;
                cw.Stop();
                return;
            }

            fio.IsExistDirectory(cw_TextBox_SavePath.Text, true);
            String FileBaseFormat = GetFileBaseFormat(cw_TextBox_SavePath.Text, cw_TextBox_SaveFilePrifix.Text);

            int Loop = 1;
            if (cw_TextBox_Loop.Text != String.Empty)
            {
                Loop = int.Parse(cw_TextBox_Loop.Text);
            }
            InitProgressBar(Loop);
            SetStartTime();

            CaptureForeground(FileBaseFormat, Loop);

            String ErrLog = cw.GetErrorLog();
            if (ErrLog != String.Empty)
            {
                MessageBox.Show(ErrLog, "エラー", MessageBoxButtons.OK);
            }
        }

        private void cw_TextBox_SavePath_KeyDown(object sender, KeyEventArgs e)
        {
            pt_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            pr_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            do_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            do_DestPortFolderPath.Text = cw_TextBox_SavePath.Text + "_port";
            do_DestLandFolderPath.Text = cw_TextBox_SavePath.Text + "_land";
            pm_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            fc_SourceFolderPath.Text = cw_TextBox_SavePath.Text;
            util.ExecutePath(cw_TextBox_SavePath.Text, e);
        }

        private String GetFileBaseFormat(String DirectoryPath, String Prifix)
        {
            String FileBaseFormat = DirectoryPath + @"\";
            if (Prifix != String.Empty)
            {
                FileBaseFormat += Prifix + "_";
            }
            if (cw_checkBox_AddTimeStump.Checked)
            {
                FileBaseFormat += System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_");
            }

            return FileBaseFormat;
        }

        private void CaptureForeground(String FileBaseFormat, int Loop)
        {
            Task task = new Task(() =>
            {
                IsTaskRun = true;
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        // 初期化
                        cw.Initialize();

                        // マウス移動後にもとの位置へ戻すか
                        //Boolean IsDebugMode = Debug.GetDebugMode();
                        //cw.RestoreMousePosition(!IsDebugMode);
                        cw.RestoreMousePosition(false);

                        // 実行前のSleep
                        cw.SetSleepTimeMsec(cw_TextBox_Sleep.Text);
                        cw.ExecuteSleep();

                        // キャプチャ対象を設定
                        CaptWindow.CAPTURE_TARGET CaptTarget;
                        if (cw_Radio_FullScreen.Checked == true)
                        {
                            CaptTarget = CaptWindow.CAPTURE_TARGET.FULL_SCREEN;
                        }
                        else if (cw_Radio_CurrentScreen.Checked == true)
                        {
                            CaptTarget = CaptWindow.CAPTURE_TARGET.CURRENT_SCREEN;
                        }
                        else // cw_Radio_CurrentWindow
                        {
                            CaptTarget = CaptWindow.CAPTURE_TARGET.CURRENT_WINDOW;
                        }
                        cw.SetCaptureTarget(CaptTarget);

                        Debug.WriteData("Capture: START", false);
                        for (int i = 1; i <= Loop && IsTaskRun; i++, UpdateProgressBar())
                        {
                            Debug.WriteData("Capture: Loop=" + i.ToString() + "/" + Loop.ToString());

                            // ファイルのIdex番号を初期化
                            cw.SetFileIdx(1);

                            // ファイル名生成
                            String FileFormat = FileBaseFormat + String.Format("{0:D4}", i);
                            cw.SetFileFormat(FileFormat);
                            Debug.WriteData(" Capture: Filename=" + FileFormat);

                            // いまのところマウス移動しないユースケースは無い
                            cw.SetMouseMove(true);

                            // 順次Capture実行
                            for (int j = 0; j < cw_dataGridView.RowCount; j++)
                            {
                                Debug.WriteData(" Capture: RowCnt=" + j.ToString() + "/" + cw_dataGridView.RowCount.ToString());

                                int ColumnIdx = GetDataGridColumnIdx(GridHeaderCaptureStr);
                                String CaptureEvent = util.GetDataGridCell(cw_dataGridView, j, ColumnIdx);
                                Boolean IsCapture = IsCaptureEvent(CaptureEvent);
                                
                                cw.SetCaptureCase(IsCapture);
                                Debug.WriteData("  Capture: SetCaptureCase() Done");

                                ColumnIdx = GetDataGridColumnIdx(GridHeaderMouseXStr);
                                String PointX = util.GetDataGridCell(cw_dataGridView, j, ColumnIdx);

                                ColumnIdx = GetDataGridColumnIdx(GridHeaderMouseYStr);
                                String PointY = util.GetDataGridCell(cw_dataGridView, j, ColumnIdx);

                                if( cw.SetMousePoint(PointX, PointY))
                                {
                                    ColumnIdx = GetDataGridColumnIdx(GridHeaderMouseActionStr);
                                    String MouseEvent = util.GetDataGridCell(cw_dataGridView, j, ColumnIdx);
                                    CaptWindow.MOUSE_EVENT Event = GetMouseEvent(MouseEvent);
                                    cw.SetMouseEvent(Event);

                                    cw.MouseProc();
                                    Debug.WriteData("  Capture: MouseEvent() Complete");
                                }

                                ColumnIdx = GetDataGridColumnIdx(GridHeaderSleepStr);
                                String SleepMsec = util.GetDataGridCell(cw_dataGridView, j, ColumnIdx);
                                cw.SetSleepTimeMsec(SleepMsec);
                                cw.ExecuteSleep();
                                Debug.WriteData("  Capture: ExecuteSleep() Complete");

                                cw.CaptureProc();
                                Debug.WriteData("  Capture: CaptureProc() Complete");
                            }

                            // 終了予想時間
                            if (i == 1)
                            {
                                SetExpectEndTime(Loop);
                            }
                        }
                        Debug.WriteData("Capture: END" + Environment.NewLine);
                        TextBox_Status.Text += " 完了";
                    });
                IsTaskRun = false;
            });
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

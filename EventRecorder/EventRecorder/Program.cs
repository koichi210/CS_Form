using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EventRecorder
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 未処理例外が発生すると、既定では「続行/終了」のJITデバッグ用ダイアログが出る。
            // このダイアログが画面外や裏に隠れて出てしまうと、ユーザーからは
            // 「アプリが固まって操作も終了もできない(実際は見えないダイアログが応答待ちなだけ)」
            // ように見えてしまうため、明示的にハンドリングしてメッセージボックスで通知しつつ
            // 極力アプリを継続させる(強制終了しないと直らない事態を防ぐ)
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show(
                    "予期しないエラーが発生したよ(処理は継続するね)" + Environment.NewLine + Environment.NewLine + e.Exception,
                    "EventRecorder - エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show(
                    "予期しないエラーが発生したよ" + Environment.NewLine + Environment.NewLine + e.ExceptionObject,
                    "EventRecorder - エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

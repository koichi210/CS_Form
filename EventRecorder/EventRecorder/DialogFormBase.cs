using System;
using System.Windows.Forms;

namespace EventRecorder
{
    // EventRecorderのポップアップダイアログ(設定画面・検索/置換など)共通の基底クラス。
    // CancelButtonの有無に関わらず、Escapeキーで閉じる動作を一律で持たせる
    public class DialogFormBase : Form
    {
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

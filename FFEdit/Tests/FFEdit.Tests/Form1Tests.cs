using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FFEdit.Tests
{
    /// <summary>
    /// Form1（名称変換タブのモード別入力欄の扱い）のテスト。
    ///
    /// ChangeName は private のため、FormReflection でコントロールの状態を直接書き換えてから
    /// 呼び出す(他の private メソッドのテストと同じやり方)。UIを実際に操作しなくても
    /// モード切り替え(ラジオボタン)とテキストボックスの中身だけで再現できるバグなので十分。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        // 回帰テスト: textBox_ChangeNumber_FirstVal は「名称変換(連番)」モード専用の入力欄で、
        // 他のモードでは空欄のまま使われる(UpdateNameChangeControlでisChangeNumberがfalseの時
        // Enabled=falseになる)。以前はChangeName()内でモードを見ずに無条件でint.Parseしていたため、
        // 連番以外のモード(例:Add)でFirstValが空欄だとFormatExceptionで落ちていた
        [TestMethod]
        public void ChangeNameは連番モード以外ならFirstValが空欄でも例外にならない()
        {
            using (var form = new Form1())
            {
                // デフォルトは連番モード(radioButton_ChangeNumber.Checked=true)なので、
                // 別モード(Add: 先頭/後方に文字列追加)に切り替える
                var radioButtonChangeAdd = (RadioButton)FormReflection.GetField(form, "radioButton_ChangeAdd");
                radioButtonChangeAdd.Checked = true;

                var textBoxFirstVal = (TextBox)FormReflection.GetField(form, "textBox_ChangeNumber_FirstVal");
                textBoxFirstVal.Text = ""; // 連番モードで無ければ空欄のまま使われるのが通常の状態

                // 対象ファイル一覧(listBox)は空のままなので、実際のファイル操作は走らない。
                // ここで確認したいのはint.Parse(FormatException)で落ちないことだけ
                object errorList = FormReflection.InvokeMethod(form, "ChangeName");

                Assert.AreEqual(string.Empty, errorList);
            }
        }
    }
}

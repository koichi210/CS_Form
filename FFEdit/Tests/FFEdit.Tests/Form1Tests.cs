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

        // 実行前の「最初の値」チェック。整数でない場合はメッセージボックスが出るため、
        // ここでは通る場合(連番モードで整数・連番モード以外で空欄)だけを確認する
        [TestMethod]
        public void IsFirstNumberValidは連番モードで整数なら通し連番モード以外なら空欄でも通す()
        {
            using (var form = new Form1())
            {
                var radioButtonChangeNumber = (RadioButton)FormReflection.GetField(form, "radioButton_ChangeNumber");
                var radioButtonChangeAdd = (RadioButton)FormReflection.GetField(form, "radioButton_ChangeAdd");
                var textBoxFirstVal = (TextBox)FormReflection.GetField(form, "textBox_ChangeNumber_FirstVal");

                radioButtonChangeNumber.Checked = true;
                textBoxFirstVal.Text = "-3";
                Assert.AreEqual(true, FormReflection.InvokeMethod(form, "IsFirstNumberValid"));

                radioButtonChangeAdd.Checked = true;
                textBoxFirstVal.Text = "";
                Assert.AreEqual(true, FormReflection.InvokeMethod(form, "IsFirstNumberValid"));
            }
        }

        // 実行前の「拡張子」チェック。空欄の場合はメッセージボックスが出るため、
        // ここでは通る場合(拡張子変更モードで入力あり・拡張子変更モード以外で空欄)だけを確認する
        [TestMethod]
        public void IsExtensionValidは拡張子変更モードで入力があれば通し拡張子変更モード以外なら空欄でも通す()
        {
            using (var form = new Form1())
            {
                var radioButtonChangeExt = (RadioButton)FormReflection.GetField(form, "radioButton_ChangeExt");
                var radioButtonChangeAdd = (RadioButton)FormReflection.GetField(form, "radioButton_ChangeAdd");
                var comboBoxString1 = (ComboBox)FormReflection.GetField(form, "comboBox_String1");

                radioButtonChangeExt.Checked = true;
                comboBoxString1.Text = ".jpg";
                Assert.AreEqual(true, FormReflection.InvokeMethod(form, "IsExtensionValid"));

                radioButtonChangeAdd.Checked = true;
                comboBoxString1.Text = "";
                Assert.AreEqual(true, FormReflection.InvokeMethod(form, "IsExtensionValid"));
            }
        }
    }
}

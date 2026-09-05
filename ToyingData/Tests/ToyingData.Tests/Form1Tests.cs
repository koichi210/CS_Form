using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToyingData.Tests
{
    /// <summary>
    /// Form1（重複除去・全角半角変換ツール）のテスト。
    ///
    /// ⚠️ 実行ボタンで全角半角変換を選び、かつ変換対象チェックボックスを1つも
    /// 選ばないと MessageBox.Show が出るため、テストでは必ずどれか1つチェックする。
    /// また Clipboard.SetText を呼ぶため、クリップボードにアクセスできないCI環境では
    /// 失敗しうる点に注意（このマシンでは通常のデスクトップセッションのため問題ない）。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 重複除去ラジオを選ぶと重複行が除去される()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Source", "a" + Environment.NewLine + "b" + Environment.NewLine + "a");
                FormReflection.SetChecked(form, "radioButton_DeleteDuplicate", true);

                FormReflection.InvokeHandler(form, "button_Execute_Click");

                Assert.AreEqual("a" + Environment.NewLine + "b", FormReflection.GetText(form, "textBox_Dest"));
            }
        }

        [TestMethod]
        public void 全角半角変換ラジオを選ぶと変換される()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Source", "１２３abc");
                FormReflection.SetChecked(form, "radioButton_ChangeWide2Narrow", true);
                FormReflection.SetChecked(form, "checkBox_Wide2Narrow_Number", true);

                FormReflection.InvokeHandler(form, "button_Execute_Click");

                Assert.AreEqual("123abc", FormReflection.GetText(form, "textBox_Dest"));
            }
        }

        [TestMethod]
        public void ドラッグドロップでファイルパス一覧が改行区切りで入力欄へ入る()
        {
            using (var form = new Form1())
            {
                string[] files = { @"C:\a.txt", @"C:\b.txt" };
                var data = new DataObject();
                data.SetData(typeof(String), files);
                var dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

                FormReflection.InvokeHandler(form, "textBox_Source_DragDrop", null, dragEventArgs);

                Assert.AreEqual(@"C:\a.txt" + Environment.NewLine + @"C:\b.txt", FormReflection.GetText(form, "textBox_Source"));
            }
        }
    }
}

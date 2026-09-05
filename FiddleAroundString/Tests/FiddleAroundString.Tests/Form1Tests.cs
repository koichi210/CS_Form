using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiddleAroundString.Tests
{
    /// <summary>
    /// Form1（構想メモ段階のプレースホルダープロジェクト。ハンドラの中身はまだ空）
    /// のテスト。
    ///
    /// 現時点でこのプロジェクトには実装されたロジックが一切無く(全ハンドラが
    /// 空実装)、抽出できるものも無い。将来ロジックが実装された際にすぐ気づけるよう、
    /// フォームの生成と各空ハンドラが例外なく呼び出せることだけを確認する
    /// スモークテストを置いておく。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void コンストラクタで例外なく生成できる()
        {
            using (var form = new Form1())
            {
                Assert.IsNotNull(form);
            }
        }

        [TestMethod]
        public void 各ハンドラは空実装で例外なく呼び出せる()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "button_Execute_Click", form);
                FormReflection.InvokeHandler(form, "textBox_SearchWord_KeyDown", form, new KeyEventArgs(Keys.None));
                FormReflection.InvokeHandler(form, "textBox_DestList_KeyDown", form, new KeyEventArgs(Keys.None));
                FormReflection.InvokeHandler(form, "textBox_SourceList_KeyDown", form, new KeyEventArgs(Keys.None));
            }
        }
    }
}

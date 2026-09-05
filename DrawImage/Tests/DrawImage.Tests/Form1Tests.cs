using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrawImage.Tests
{
    /// <summary>
    /// Form1（PictureBoxに線や円を描画するサンプル）のテスト。
    /// MessageBox.Show を呼ぶ箇所は無いため、全ハンドラをテスト対象にできる。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 直線ボタンでpictureBoxに青い線が描画される()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "button_DrawLine_Click", form);

                var pictureBox = (PictureBox)FormReflection.GetControl(form, "pictureBox1");
                Assert.IsNotNull(pictureBox.Image);

                using (var bmp = new Bitmap(pictureBox.Image))
                {
                    // (10,20)-(100,200) の線上、太さ3pxのペンなので中間点付近は青いはず
                    Color pixel = bmp.GetPixel(55, 110);
                    Assert.AreEqual(Color.Blue.ToArgb(), pixel.ToArgb());
                }
            }
        }

        [TestMethod]
        public void 円ボタンでpictureBoxに白い円が描画される()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "button_Circle_Click", form);

                var pictureBox = (PictureBox)FormReflection.GetControl(form, "pictureBox1");
                Assert.IsNotNull(pictureBox.Image);

                using (var bmp = new Bitmap(pictureBox.Image))
                {
                    // Rectangle(15, 70, 50, 50) の中心付近は白いはず
                    Color pixel = bmp.GetPixel(40, 95);
                    Assert.AreEqual(Color.White.ToArgb(), pixel.ToArgb());
                }
            }
        }

        [TestMethod]
        public void 削除ボタンでpictureBoxの画像がクリアされる()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "button_DrawLine_Click", form);
                var pictureBox = (PictureBox)FormReflection.GetControl(form, "pictureBox1");
                Assert.IsNotNull(pictureBox.Image);

                FormReflection.InvokeHandler(form, "button_Delete_Click", form);

                Assert.IsNull(pictureBox.Image);
            }
        }

        [TestMethod]
        public void 複数回描画すると前の描画内容が残ったまま重ね描きされる()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "button_DrawLine_Click", form);  // 青い線
                FormReflection.InvokeHandler(form, "button_Circle_Click", form);    // 白い円(別の場所)

                var pictureBox = (PictureBox)FormReflection.GetControl(form, "pictureBox1");
                using (var bmp = new Bitmap(pictureBox.Image))
                {
                    // 線が残っていること(円のRectangle(15,70,50,50)と重ならない箇所で確認)
                    Assert.AreEqual(Color.Blue.ToArgb(), bmp.GetPixel(90, 180).ToArgb());
                    // 円も描画されていること
                    Assert.AreEqual(Color.White.ToArgb(), bmp.GetPixel(40, 95).ToArgb());
                }
            }
        }
    }
}

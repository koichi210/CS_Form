using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace othello.Tests
{
    /// <summary>
    /// Draw（オセロ盤面をPictureBoxに描画するクラス、Form1とは独立した
    /// 通常クラス）のテスト。実際のPictureBoxを使い、描画結果のピクセルを
    /// 確認することで検証する。
    /// </summary>
    [TestClass]
    public class DrawTests
    {
        private PictureBox CreatePictureBox()
        {
            // CellMax=8なので、80x80(1マス10px)にしておくと座標計算がしやすい
            return new PictureBox { Width = 80, Height = 80 };
        }

        [TestMethod]
        public void SetDrawAreaでPictureBoxのサイズに合わせたImageが作成される()
        {
            using (var pb = CreatePictureBox())
            {
                var draw = new Draw();
                draw.SetDrawArea(pb);

                Assert.IsNotNull(pb.Image);
                Assert.AreEqual(80, pb.Image.Width);
                Assert.AreEqual(80, pb.Image.Height);
            }
        }

        [TestMethod]
        public void DeleteCanvasでImageがnullになる()
        {
            using (var pb = CreatePictureBox())
            {
                var draw = new Draw();
                draw.SetDrawArea(pb);
                Assert.IsNotNull(pb.Image);

                draw.DeleteCanvas();

                Assert.IsNull(pb.Image);
            }
        }

        [TestMethod]
        public void FillBackgroundは指定した色で全体を塗りつぶす()
        {
            using (var pb = CreatePictureBox())
            {
                var draw = new Draw();
                draw.SetDrawArea(pb);

                draw.FillBackground(Brushes.Red);

                using (var bmp = new Bitmap(pb.Image))
                {
                    Assert.AreEqual(Color.Red.ToArgb(), bmp.GetPixel(5, 5).ToArgb());
                    Assert.AreEqual(Color.Red.ToArgb(), bmp.GetPixel(75, 75).ToArgb());
                }
            }
        }

        [TestMethod]
        public void WriteLineは指定した色で線を引く()
        {
            using (var pb = CreatePictureBox())
            {
                var draw = new Draw();
                draw.SetDrawArea(pb);
                draw.FillBackground(Brushes.White);

                draw.WriteLine(new Point(0, 40), new Point(80, 40), Color.Blue, 2);

                using (var bmp = new Bitmap(pb.Image))
                {
                    Assert.AreEqual(Color.Blue.ToArgb(), bmp.GetPixel(40, 40).ToArgb());
                }
            }
        }

        [TestMethod]
        public void InitFieldは緑の盤面に黒い格子線を描画する()
        {
            using (var pb = CreatePictureBox())
            {
                var draw = new Draw();
                draw.SetDrawArea(pb);

                draw.InitField();

                using (var bmp = new Bitmap(pb.Image))
                {
                    // マスの内部(格子線から離れた場所)は緑
                    Assert.AreEqual(Color.Green.ToArgb(), bmp.GetPixel(5, 5).ToArgb());

                    // 中央の格子線の交点付近は黒
                    Assert.AreEqual(Color.Black.ToArgb(), bmp.GetPixel(40, 40).ToArgb());
                }
            }
        }
    }
}

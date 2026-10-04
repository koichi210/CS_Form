using System;
using System.Drawing;
using System.Windows.Forms;

namespace Othello
{
    class BoardRenderer
    {
        private const int _cellMax = GameMaster.BoardSize;
        private const int _edgeOffset = 0;
        private const int _lineWidth = 2;
        private static readonly Color _lineColor = Color.Black;

        private PictureBox _pictureBox;

        public void SetDrawArea(PictureBox pictureBox)
        {
            _pictureBox = pictureBox;
            CreateCanvas();
        }

        public void CreateCanvas()
        {
            // リサイズ中の一瞬(最小化直後など)は幅・高さが0以下になることがある。
            // その場合はBitmapが作れないので、既存のImageを残したまま何もしない。
            int width = _pictureBox.Width;
            int height = _pictureBox.Height;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            // 注意: 先に_pictureBox.Image = nullしてからImageを差し替えると、なぜかPictureBoxの
            // Anchorレイアウトが誤作動し、コントロール自体のサイズが元のデザイン時サイズに
            // 巻き戻ってしまう(WinFormsのレイアウトエンジンの癖)。
            // それを避けるため、古いImageは変数に保持しておき、新しいImageに直接差し替えてから破棄する。
            Bitmap oldCanvas = _pictureBox.Image as Bitmap;

            Bitmap canvas = new Bitmap(width, height);
            _pictureBox.Image = canvas;

            oldCanvas?.Dispose();
        }

        public void DeleteCanvas()
        {
            if (_pictureBox.Image != null)
            {
                _pictureBox.Image.Dispose();
                _pictureBox.Image = null;
            }
        }

        /// <summary>
        /// 現在描画中のBitmapそのもの(コピーではなく参照)を返す。
        /// 描画メソッドはこれを直接書き換えることで、毎回盤面全体をコピーする
        /// 無駄なコストを避けている(以前は石や線を1つ描くたびに全体コピーしていたため、
        /// 盤面が埋まってくるほど描画がどんどん重くなっていた)。
        /// </summary>
        private Bitmap GetCanvas()
        {
            return (Bitmap)_pictureBox.Image;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void InitField()
        {
            // Imageが無い(CreateCanvasが無効サイズでスキップされた等)場合は描画できないので何もしない
            if (_pictureBox.Image == null)
            {
                return;
            }

            // 基盤作成
            FillBackground(Brushes.Green);

            // 線描画
            // 注意: 「1マスの幅 = 全体 / 8」を先に整数で求めてから位置を計算すると、
            // 割り切れない端数が最後のマスだけに溜め込まれ、細長い9マス目のような
            // 見た目になってしまう。「i番目の線の位置 = 全体 * i / 8」の形で
            // 掛け算を先に行うことで、端数を各マスに均等に配分し、最後の線が
            // 必ず盤面のちょうど端に来るようにする。
            int width = _pictureBox.Width - (_edgeOffset * 2);
            int height = _pictureBox.Height - (_edgeOffset * 2);

            // 縦線
            for (int i = 0; i <= _cellMax; i++)
            {
                int x = _edgeOffset + (width * i / _cellMax);
                Point startPoint = new Point(x, _edgeOffset);
                Point endPoint = new Point(x, _pictureBox.Height - _edgeOffset);
                DrawLine(startPoint, endPoint, _lineColor, _lineWidth);
            }

            // 横線
            for (int i = 0; i <= _cellMax; i++)
            {
                int y = _edgeOffset + (height * i / _cellMax);
                Point startPoint = new Point(_edgeOffset, y);
                Point endPoint = new Point(_pictureBox.Width - _edgeOffset, y);
                DrawLine(startPoint, endPoint, _lineColor, _lineWidth);
            }
        }

        /// <summary>
        /// マス目座標cellのX方向の範囲(左端・右端のピクセル座標)を返す。
        /// 罫線の描画(InitField)と同じ「掛け算してから割る」計算式を使うことで、
        /// 罫線と石の位置がどのウィンドウサイズでもぴったり一致するようにしている。
        /// </summary>
        private void GetCellRangeX(int cellX, out int left, out int right)
        {
            int width = _pictureBox.Width - (_edgeOffset * 2);
            left = _edgeOffset + (width * cellX / _cellMax);
            right = _edgeOffset + (width * (cellX + 1) / _cellMax);
        }

        private void GetCellRangeY(int cellY, out int top, out int bottom)
        {
            int height = _pictureBox.Height - (_edgeOffset * 2);
            top = _edgeOffset + (height * cellY / _cellMax);
            bottom = _edgeOffset + (height * (cellY + 1) / _cellMax);
        }

        /// <summary>
        /// 盤面(格子線)と全マスの石をまとめて描画する。
        /// validMovesを渡すと、石が無く置ける(true)マスに置ける場所のマークも描画する。
        /// C++版 DrawNotice 相当。
        /// </summary>
        public void DrawField(StoneColor[,] table, bool[,] validMoves = null)
        {
            InitField();

            for (int y = 0; y < _cellMax; y++)
            {
                for (int x = 0; x < _cellMax; x++)
                {
                    DrawStone(x, y, table[y, x]);

                    if (validMoves != null && validMoves[y, x])
                    {
                        DrawNotice(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// マス目座標(cellX, cellY)に「置ける場所」の小さなマークを描画する。
        /// </summary>
        public void DrawNotice(int cellX, int cellY)
        {
            if (_pictureBox.Image == null)
            {
                return;
            }

            GetCellRangeX(cellX, out int left, out int right);
            GetCellRangeY(cellY, out int top, out int bottom);

            int cellWidth = right - left;
            int cellHeight = bottom - top;
            int noticeWidth = Math.Max(2, cellWidth / 4);
            int noticeHeight = Math.Max(2, cellHeight / 4);

            Rectangle rect = new Rectangle(
                left + (cellWidth - noticeWidth) / 2,
                top + (cellHeight - noticeHeight) / 2,
                noticeWidth,
                noticeHeight);

            using (Graphics g = Graphics.FromImage(GetCanvas()))
            using (Brush brush = new SolidBrush(Color.FromArgb(140, Color.DarkGray)))
            {
                g.FillEllipse(brush, rect);
            }

            _pictureBox.Invalidate();
        }

        /// <summary>
        /// マス目座標(cellX, cellY)に石を描画する。colorがEmptyなら何もしない。
        /// </summary>
        public void DrawStone(int cellX, int cellY, StoneColor color)
        {
            if (color == StoneColor.Empty || _pictureBox.Image == null)
            {
                return;
            }

            GetCellRangeX(cellX, out int left, out int right);
            GetCellRangeY(cellY, out int top, out int bottom);
            int margin = Math.Max(2, _lineWidth);

            Rectangle rect = new Rectangle(
                left + margin,
                top + margin,
                (right - left) - margin * 2,
                (bottom - top) - margin * 2);

            Brush brush = color == StoneColor.Black ? Brushes.Black : Brushes.White;

            using (Graphics g = Graphics.FromImage(GetCanvas()))
            {
                g.FillEllipse(brush, rect);
            }

            _pictureBox.Invalidate();
        }

        public void FillBackground(Brush brush)
        {
            Rectangle rect = new Rectangle(0, 0, _pictureBox.Width, _pictureBox.Height);

            using (Graphics g = Graphics.FromImage(GetCanvas()))
            {
                g.FillRectangle(brush, rect);
            }

            _pictureBox.Invalidate();
        }

        public void DrawLine(Point startPoint, Point endPoint, Color color, int width)
        {
            using (Graphics g = Graphics.FromImage(GetCanvas()))
            using (Pen pen = new Pen(color, width))
            {
                g.DrawLine(pen, startPoint, endPoint);
            }

            _pictureBox.Invalidate();
        }
    }
}

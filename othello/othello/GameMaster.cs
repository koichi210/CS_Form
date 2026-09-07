using System;

namespace othello
{
    /// <summary>
    /// 石の色(盤面の状態)
    /// </summary>
    public enum StoneColor
    {
        Unknown = 0,
        Black = 1,
        White = 2,
    }

    /// <summary>
    /// オセロの盤面状態とルール(石を置く・ひっくり返す・手番交代・終局判定)を管理するクラス。
    /// C++版 COthelloBase(othellobase.h/.cpp)のロジックを移植したもの。
    /// </summary>
    class GameMaster
    {
        public const int BoardSize = 8;

        // 盤面(0-indexed) [y, x]
        public StoneColor[,] Table { get; private set; }

        // 現在の手番
        public StoneColor CurrentTurn { get; private set; }

        // どちらも置けなくなり終局したかどうか
        public bool IsGameEnd { get; private set; }

        // 8方向(左, 右, 上, 下, 左上, 左下, 右上, 右下)
        private static readonly int[] DirX = { -1, 1, 0, 0, -1, -1, 1, 1 };
        private static readonly int[] DirY = { 0, 0, -1, 1, -1, 1, -1, 1 };

        public void Initialize()
        {
            Table = new StoneColor[BoardSize, BoardSize];
            Table[3, 3] = StoneColor.White;
            Table[3, 4] = StoneColor.Black;
            Table[4, 3] = StoneColor.Black;
            Table[4, 4] = StoneColor.White;

            CurrentTurn = StoneColor.Black;
            IsGameEnd = false;
        }

        private static StoneColor GetEnemyColor(StoneColor color)
        {
            return color == StoneColor.Black ? StoneColor.White : StoneColor.Black;
        }

        private static bool IsInBoard(int x, int y)
        {
            return x >= 0 && x < BoardSize && y >= 0 && y < BoardSize;
        }

        /// <summary>
        /// (x, y)にcolorを置けるか判定する。置ける場合、方向ごとにひっくり返せる石数をflipCountsに入れて返す。
        /// </summary>
        private bool PutCheck(int x, int y, StoneColor color, int[] flipCounts)
        {
            if (!IsInBoard(x, y) || Table[y, x] != StoneColor.Unknown)
            {
                return false;
            }

            StoneColor enemy = GetEnemyColor(color);
            bool canPut = false;

            for (int dir = 0; dir < DirX.Length; dir++)
            {
                int cx = x + DirX[dir];
                int cy = y + DirY[dir];
                int count = 0;

                while (IsInBoard(cx, cy) && Table[cy, cx] == enemy)
                {
                    count++;
                    cx += DirX[dir];
                    cy += DirY[dir];
                }

                if (count > 0 && IsInBoard(cx, cy) && Table[cy, cx] == color)
                {
                    flipCounts[dir] = count;
                    canPut = true;
                }
            }

            return canPut;
        }

        /// <summary>
        /// colorがどこかに置けるか判定する
        /// </summary>
        private bool CanPutAny(StoneColor color)
        {
            int[] flipCounts = new int[DirX.Length];

            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    Array.Clear(flipCounts, 0, flipCounts.Length);
                    if (PutCheck(x, y, color, flipCounts))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// (x, y)にCurrentTurnの石を置き、ひっくり返し、次の手番(パス・終局判定込み)に進める。
        /// 置けない場所を指定した場合は何もせずfalseを返す。
        /// </summary>
        public bool TryPut(int x, int y)
        {
            if (IsGameEnd)
            {
                return false;
            }

            int[] flipCounts = new int[DirX.Length];
            if (!PutCheck(x, y, CurrentTurn, flipCounts))
            {
                return false;
            }

            Table[y, x] = CurrentTurn;

            for (int dir = 0; dir < DirX.Length; dir++)
            {
                int cx = x;
                int cy = y;
                for (int i = 0; i < flipCounts[dir]; i++)
                {
                    cx += DirX[dir];
                    cy += DirY[dir];
                    Table[cy, cx] = CurrentTurn;
                }
            }

            AdvanceTurn();
            return true;
        }

        /// <summary>
        /// 手番交代。相手が置けなければパスして自分の手番のまま、
        /// どちらも置けなければ終局(IsGameEndがtrueになる)。
        /// </summary>
        private void AdvanceTurn()
        {
            StoneColor enemy = GetEnemyColor(CurrentTurn);

            if (CanPutAny(enemy))
            {
                CurrentTurn = enemy;
            }
            else if (CanPutAny(CurrentTurn))
            {
                // 相手はパス、自分の手番が続く
            }
            else
            {
                // どちらも置けない = 終局(CurrentTurnは維持)
                IsGameEnd = true;
            }
        }

        public void CountStone(out int blackCount, out int whiteCount)
        {
            blackCount = 0;
            whiteCount = 0;

            foreach (StoneColor stone in Table)
            {
                if (stone == StoneColor.Black)
                {
                    blackCount++;
                }
                else if (stone == StoneColor.White)
                {
                    whiteCount++;
                }
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace EventRecorder
{
    // 記録データ配列([Type, X, Y, Key, Wait]の固定順)と、dataGridView_Eventsの行(DataGridViewRow)の
    // 間の変換だけを担う。
    //
    // ⚠️過去の不具合: この変換をCells[0]/Cells[1].../Cells[4]のような列の「物理的な並び順」の
    // 決め打ちで実装していたことがあり、Detail列を追加した際にForm1.Designer.csの
    // Columns.AddRange順序(Type,Detail,Wait,X,Y,Key)が変わったのに追従し忘れ、
    // 記録した値が別の列(Key列にX、Wait列にY等)にずれて入る不具合になっていた
    // (2026-09-12発覚。マウス/キーの座標が読み取れずグリッド全行がピンク色のエラー表示になった)。
    // 二度と物理インデックスの決め打ちに戻さないよう、必ずcol_XXX.Index(列名経由)を使う
    // この専用クラスへ切り出し、単体テストで列の並び順に依存しないことを保証する
    internal static class EventRowMapper
    {
        // r([Type, X, Y, Key, Wait]の順)の値を、列名経由でrowの各セルへ書き込む
        internal static void ApplyToRow(
            DataGridViewRow row,
            DataGridViewColumn colType, DataGridViewColumn colX, DataGridViewColumn colY,
            DataGridViewColumn colKey, DataGridViewColumn colWait,
            String[] r)
        {
            row.Cells[colType.Index].Value = r[0];
            row.Cells[colX.Index].Value = r[1];
            row.Cells[colY.Index].Value = r[2];
            row.Cells[colKey.Index].Value = r[3];
            row.Cells[colWait.Index].Value = r[4];
        }

        // rowの各セルの値を、列名経由で[Type, X, Y, Key, Wait]の順の配列に読み出す(ApplyToRowの逆)
        internal static String[] ReadFromRow(
            DataGridViewRow row,
            DataGridViewColumn colType, DataGridViewColumn colX, DataGridViewColumn colY,
            DataGridViewColumn colKey, DataGridViewColumn colWait)
        {
            return new String[]
            {
                Convert.ToString(row.Cells[colType.Index].Value),
                Convert.ToString(row.Cells[colX.Index].Value),
                Convert.ToString(row.Cells[colY.Index].Value),
                Convert.ToString(row.Cells[colKey.Index].Value),
                Convert.ToString(row.Cells[colWait.Index].Value),
            };
        }
    }
}

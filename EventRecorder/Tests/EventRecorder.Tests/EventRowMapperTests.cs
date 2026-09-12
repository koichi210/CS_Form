using System;
using System.Windows.Forms;
using EventRecorder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventRecorder.Tests
{
    /// <summary>
    /// EventRowMapperのテスト。
    ///
    /// ⚠️2026-09-12に実際に踏んだ不具合の再発防止テスト: Form1.Designer.csの
    /// dataGridView_Events.Columns.AddRangeは(Type, Detail, Wait, X, Y, Key)の順で列を
    /// 追加している(Detail列表示用にX/Y/Key/Waitより前に挿入されている)。
    /// これに対して記録処理側がCells[0]～Cells[4]のような「列の物理的な並び順」を
    /// 決め打ちで[Type, X, Y, Key, Wait]と対応させていたため、記録した値が
    /// 別の列(Key列にX、Wait列にY等)にずれて入り、再生前チェックで全行がエラー
    /// (ピンク表示)になる不具合になっていた。
    ///
    /// このテストでは、実際の列追加順序をそのまま再現したDataGridViewに対して
    /// EventRowMapperを使い、物理的な並び順が[Type,X,Y,Key,Wait]と一致しない
    /// 状態でも正しい列に値が入る/正しい順で読み出せることを確認する
    /// (Cells[0..4]の決め打ちに戻すと、このテストが失敗するはず)。
    /// </summary>
    [TestClass]
    public class EventRowMapperTests
    {
        // Form1.Designer.csの実際の列追加順序(Type, Detail, Wait, X, Y, Key)を再現した
        // DataGridViewを作る。Detail列は今回のテストでは使わないダミー
        private static DataGridView CreateGridWithRealColumnOrder(
            out DataGridViewColumn colType, out DataGridViewColumn colX, out DataGridViewColumn colY,
            out DataGridViewColumn colKey, out DataGridViewColumn colWait)
        {
            DataGridView grid = new DataGridView();

            colType = new DataGridViewTextBoxColumn { Name = "col_Type" };
            DataGridViewColumn colDetail = new DataGridViewTextBoxColumn { Name = "col_Detail" };
            colWait = new DataGridViewTextBoxColumn { Name = "col_Wait" };
            colX = new DataGridViewTextBoxColumn { Name = "col_X" };
            colY = new DataGridViewTextBoxColumn { Name = "col_Y" };
            colKey = new DataGridViewTextBoxColumn { Name = "col_Key" };

            // Form1.Designer.csと全く同じ追加順序(物理インデックス: Type=0,Detail=1,Wait=2,X=3,Y=4,Key=5)。
            // つまりX/Y/Key/Waitの物理位置は[Type,X,Y,Key,Wait]という論理順とは一致しない
            grid.Columns.AddRange(colType, colDetail, colWait, colX, colY, colKey);

            return grid;
        }

        [TestMethod]
        public void ApplyToRowは列の物理的な並び順に関わらず正しい列に値が入る()
        {
            DataGridViewColumn colType, colX, colY, colKey, colWait;
            DataGridView grid = CreateGridWithRealColumnOrder(out colType, out colX, out colY, out colKey, out colWait);
            grid.Rows.Add();
            DataGridViewRow row = grid.Rows[0];

            // r = [Type, X, Y, Key, Wait]
            String[] r = { "LEFT_DOWN", "638", "503", "", "0" };
            EventRowMapper.ApplyToRow(row, colType, colX, colY, colKey, colWait, r);

            Assert.AreEqual("LEFT_DOWN", row.Cells[colType.Index].Value, "Type列に値が入っていない");
            Assert.AreEqual("638", row.Cells[colX.Index].Value, "X列にX座標が入っていない(Key列やWait列にずれていないか)");
            Assert.AreEqual("503", row.Cells[colY.Index].Value, "Y列にY座標が入っていない");
            Assert.AreEqual("", row.Cells[colKey.Index].Value, "Key列が空であるべき(マウスイベントなので)");
            Assert.AreEqual("0", row.Cells[colWait.Index].Value, "Wait列に0が入っていない");
        }

        [TestMethod]
        public void ApplyToRowはWAIT行でもWait列にだけ値が入る()
        {
            DataGridViewColumn colType, colX, colY, colKey, colWait;
            DataGridView grid = CreateGridWithRealColumnOrder(out colType, out colX, out colY, out colKey, out colWait);
            grid.Rows.Add();
            DataGridViewRow row = grid.Rows[0];

            String[] r = { "WAIT", "", "", "", "500" };
            EventRowMapper.ApplyToRow(row, colType, colX, colY, colKey, colWait, r);

            Assert.AreEqual("WAIT", row.Cells[colType.Index].Value);
            Assert.AreEqual("", row.Cells[colX.Index].Value);
            Assert.AreEqual("", row.Cells[colY.Index].Value);
            Assert.AreEqual("", row.Cells[colKey.Index].Value);
            Assert.AreEqual("500", row.Cells[colWait.Index].Value, "WAIT行の待機時間がWait列に入っていない(過去の不具合ではここが空になっていた)");
        }

        [TestMethod]
        public void ReadFromRowはApplyToRowで書いた内容を同じ順で読み戻せる()
        {
            DataGridViewColumn colType, colX, colY, colKey, colWait;
            DataGridView grid = CreateGridWithRealColumnOrder(out colType, out colX, out colY, out colKey, out colWait);
            grid.Rows.Add();
            DataGridViewRow row = grid.Rows[0];

            String[] original = { "KEY_DOWN", "", "", "A", "120" };
            EventRowMapper.ApplyToRow(row, colType, colX, colY, colKey, colWait, original);

            String[] roundTripped = EventRowMapper.ReadFromRow(row, colType, colX, colY, colKey, colWait);

            CollectionAssert.AreEqual(original, roundTripped);
        }
    }
}

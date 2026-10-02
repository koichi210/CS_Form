using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Class_List.Tests
{
    /// <summary>
    /// Form1（Groupインデックス付きリストの追加/削除サンプル）のテスト。
    ///
    /// ⚠️ ResultDump は常に MessageBox.Show を呼ぶため、これを呼び出す
    /// buttonAdd_Click / buttonRestore_Click はテスト対象から除外し、
    /// 実際にロジックを持つ AddList / RemoveIfLastGroup / UpdateIdx を直接呼び出して検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void AddListは現在のLastIdxをGroupとして要素を追加する()
        {
            using (var form = new Form1())
            {
                var list = new List<Form1.Table>();

                FormReflection.InvokeMethod(form, "AddList", list, 5);

                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(0, list[0].Group);
                Assert.AreEqual("Source5", list[0].SrcName);
                Assert.AreEqual("Destination5", list[0].DestName);
            }
        }

        [TestMethod]
        public void UpdateIdxのINCREMENTでLastIdxが増える()
        {
            using (var form = new Form1())
            {
                form.UpdateIdx(Form1.INDEX_COUNTER.INCREMENT);

                int lastIdx = (int)FormReflection.GetField(form, "LastIdx");
                Assert.AreEqual(1, lastIdx);
            }
        }

        [TestMethod]
        public void UpdateIdxのDECREMENTは0未満にならない()
        {
            using (var form = new Form1())
            {
                form.UpdateIdx(Form1.INDEX_COUNTER.DECREMENT);

                int lastIdx = (int)FormReflection.GetField(form, "LastIdx");
                Assert.AreEqual(0, lastIdx);
            }
        }

        [TestMethod]
        public void UpdateIdxのDECREMENTは0より大きければ減る()
        {
            using (var form = new Form1())
            {
                form.UpdateIdx(Form1.INDEX_COUNTER.INCREMENT); // 0 -> 1
                form.UpdateIdx(Form1.INDEX_COUNTER.DECREMENT); // 1 -> 0

                int lastIdx = (int)FormReflection.GetField(form, "LastIdx");
                Assert.AreEqual(0, lastIdx);
            }
        }

        [TestMethod]
        public void RemoveIfLastGroupは現在のLastIdxと一致する要素だけ削除しtrueを返す()
        {
            using (var form = new Form1())
            {
                var list = new List<Form1.Table>();
                FormReflection.InvokeMethod(form, "AddList", list, 1); // Group=0で追加
                form.UpdateIdx(Form1.INDEX_COUNTER.INCREMENT);         // LastIdx: 0 -> 1
                FormReflection.InvokeMethod(form, "AddList", list, 2); // Group=1で追加

                object result = FormReflection.InvokeMethod(form, "RemoveIfLastGroup", list, 1);

                Assert.IsTrue((bool)result);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(0, list[0].Group);
            }
        }

        [TestMethod]
        public void RemoveIfLastGroupは一致しなければ削除せずfalseを返す()
        {
            using (var form = new Form1())
            {
                var list = new List<Form1.Table>();
                FormReflection.InvokeMethod(form, "AddList", list, 1); // Group=0で追加
                form.UpdateIdx(Form1.INDEX_COUNTER.INCREMENT);         // LastIdx: 0 -> 1

                object result = FormReflection.InvokeMethod(form, "RemoveIfLastGroup", list, 0);

                Assert.IsFalse((bool)result);
                Assert.AreEqual(1, list.Count);
            }
        }

        // 回帰テスト: Restoreボタンのループが`i > 0`だった頃は、最初に追加した
        // (かつ唯一の)グループ0の要素がインデックス0のまま残ってしまい、
        // Restoreしても何も消えないバグがあった。AllListはprivateなので
        // FormReflectionで直接積んでから、本体ロジックのRemoveLastGroupを呼び出す
        [TestMethod]
        public void RemoveLastGroupは最初の1グループだけでも全要素を削除する()
        {
            using (var form = new Form1())
            {
                var allList = (List<Form1.Table>)FormReflection.GetField(form, "AllList");
                allList.Add(new Form1.Table(0, "Source0", "Destination0"));
                allList.Add(new Form1.Table(0, "Source1", "Destination1"));

                FormReflection.InvokeMethod(form, "RemoveLastGroup");

                Assert.AreEqual(0, allList.Count);
            }
        }
    }
}

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Class_List.Tests
{
    /// <summary>
    /// Form1（Groupインデックス付きリストの追加/削除サンプル）のテスト。
    ///
    /// ⚠️ ResultDump は常に MessageBox.Show を呼ぶため、これを呼び出す
    /// buttonAdd_Click / buttonRestore_Click はテスト対象から除外し、
    /// 実際にロジックを持つ AddList / SubList / UpdateIdx を直接呼び出して検証する。
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
        public void SubListは現在のLastIdxと一致する要素だけ削除しtrueを返す()
        {
            using (var form = new Form1())
            {
                var list = new List<Form1.Table>();
                FormReflection.InvokeMethod(form, "AddList", list, 1); // Group=0で追加
                form.UpdateIdx(Form1.INDEX_COUNTER.INCREMENT);         // LastIdx: 0 -> 1
                FormReflection.InvokeMethod(form, "AddList", list, 2); // Group=1で追加

                object result = FormReflection.InvokeMethod(form, "SubList", list, 1);

                Assert.IsTrue((bool)result);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(0, list[0].Group);
            }
        }

        [TestMethod]
        public void SubListは一致しなければ削除せずfalseを返す()
        {
            using (var form = new Form1())
            {
                var list = new List<Form1.Table>();
                FormReflection.InvokeMethod(form, "AddList", list, 1); // Group=0で追加
                form.UpdateIdx(Form1.INDEX_COUNTER.INCREMENT);         // LastIdx: 0 -> 1

                object result = FormReflection.InvokeMethod(form, "SubList", list, 0);

                Assert.IsFalse((bool)result);
                Assert.AreEqual(1, list.Count);
            }
        }
    }
}

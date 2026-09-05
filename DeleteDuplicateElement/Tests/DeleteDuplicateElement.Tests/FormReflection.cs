using System;
using System.Reflection;
using System.Windows.Forms;

namespace DeleteDuplicateElement.Tests
{
    /// <summary>
    /// private なコントロールフィールドやイベントハンドラを、production コードを
    /// 一切変更せずにテストから操作するための小さなヘルパー。
    ///
    /// この規模のFormアプリはコントロールもイベントハンドラもすべて private で、
    /// ロジックのほとんどは既にテスト済みの共通クラス呼び出しに過ぎない。
    /// InternalsVisibleTo を足したり private を internal に変える代わりに、
    /// テスト側だけでリフレクションを使うことで、本体には一切手を入れずに
    /// 「ボタンを押したときに実際に何が起きるか」を検証できるようにしている。
    /// </summary>
    internal static class FormReflection
    {
        private const BindingFlags InstanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static Control GetControl(object form, string fieldName)
        {
            FieldInfo field = form.GetType().GetField(fieldName, InstanceAny);
            if (field == null)
            {
                throw new ArgumentException(string.Format("フィールド '{0}' が見つからない（型 {1}）", fieldName, form.GetType().Name));
            }
            return (Control)field.GetValue(form);
        }

        public static string GetText(object form, string fieldName)
        {
            return GetControl(form, fieldName).Text;
        }

        public static void SetText(object form, string fieldName, string value)
        {
            GetControl(form, fieldName).Text = value;
        }

        /// <summary>private なイベントハンドラを (object sender, EventArgs e) 相当で呼び出す。</summary>
        public static void InvokeHandler(object form, string methodName, object sender = null, EventArgs args = null)
        {
            MethodInfo method = form.GetType().GetMethod(methodName, InstanceAny);
            if (method == null)
            {
                throw new ArgumentException(string.Format("メソッド '{0}' が見つからない（型 {1}）", methodName, form.GetType().Name));
            }
            method.Invoke(form, new object[] { sender, args ?? EventArgs.Empty });
        }

        /// <summary>2引数目の型がEventArgsのサブクラス(KeyEventArgs等)のイベントハンドラを呼び出す。</summary>
        public static void InvokeHandler(object form, string methodName, object sender, object eventArgs)
        {
            MethodInfo method = form.GetType().GetMethod(methodName, InstanceAny);
            if (method == null)
            {
                throw new ArgumentException(string.Format("メソッド '{0}' が見つからない（型 {1}）", methodName, form.GetType().Name));
            }
            method.Invoke(form, new object[] { sender, eventArgs });
        }
    }
}

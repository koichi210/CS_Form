using System;
using System.Reflection;
using System.Windows.Forms;

namespace Thumbnail.Tests
{
    /// <summary>
    /// private なコントロールフィールドやイベントハンドラを、production コードを
    /// 一切変更せずにテストから操作するための小さなヘルパー。
    /// </summary>
    internal static class FormReflection
    {
        private const BindingFlags _instanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static Control GetControl(object form, string fieldName)
        {
            FieldInfo field = form.GetType().GetField(fieldName, _instanceAny);
            if (field == null)
            {
                throw new ArgumentException(string.Format("フィールド '{0}' が見つからない（型 {1}）", fieldName, form.GetType().Name));
            }
            return (Control)field.GetValue(form);
        }

        public static void SetText(object form, string fieldName, string value)
        {
            GetControl(form, fieldName).Text = value;
        }

        public static void InvokeHandler(object form, string methodName, object sender = null, EventArgs args = null)
        {
            MethodInfo method = form.GetType().GetMethod(methodName, _instanceAny);
            if (method == null)
            {
                throw new ArgumentException(string.Format("メソッド '{0}' が見つからない（型 {1}）", methodName, form.GetType().Name));
            }
            method.Invoke(form, new object[] { sender, args ?? EventArgs.Empty });
        }
    }
}

using System;
using System.Reflection;

namespace ClassList.Tests
{
    /// <summary>
    /// private なフィールドやメソッドを、production コードを一切変更せずに
    /// テストから操作するための小さなヘルパー。
    /// </summary>
    internal static class FormReflection
    {
        private const BindingFlags _instanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static object GetField(object form, string fieldName)
        {
            FieldInfo field = form.GetType().GetField(fieldName, _instanceAny);
            if (field == null)
            {
                throw new ArgumentException(string.Format("フィールド '{0}' が見つからない（型 {1}）", fieldName, form.GetType().Name));
            }
            return field.GetValue(form);
        }

        public static object InvokeMethod(object form, string methodName, params object[] args)
        {
            MethodInfo method = form.GetType().GetMethod(methodName, _instanceAny);
            if (method == null)
            {
                throw new ArgumentException(string.Format("メソッド '{0}' が見つからない（型 {1}）", methodName, form.GetType().Name));
            }
            return method.Invoke(form, args);
        }
    }
}

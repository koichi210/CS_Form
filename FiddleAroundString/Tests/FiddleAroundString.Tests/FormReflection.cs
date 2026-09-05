using System;
using System.Reflection;

namespace FiddleAroundString.Tests
{
    /// <summary>
    /// private なイベントハンドラを、production コードを一切変更せずに
    /// テストから操作するための小さなヘルパー。
    /// </summary>
    internal static class FormReflection
    {
        private const BindingFlags InstanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static void InvokeHandler(object form, string methodName, object sender = null, EventArgs args = null)
        {
            MethodInfo method = form.GetType().GetMethod(methodName, InstanceAny);
            if (method == null)
            {
                throw new ArgumentException(string.Format("メソッド '{0}' が見つからない（型 {1}）", methodName, form.GetType().Name));
            }
            method.Invoke(form, new object[] { sender, args ?? EventArgs.Empty });
        }
    }
}

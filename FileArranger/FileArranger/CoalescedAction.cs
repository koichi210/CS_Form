using System;
using System.Windows.Forms;

namespace FileArranger
{
    // 短時間に何度も飛んでくるイベントの処理を、それらが一段落した後の1回にまとめる。
    // ListViewでCtrl+Aや範囲選択をすると、選択が変わった項目の数だけSelectedIndexChangedが飛ぶ。
    // そのたびに全選択項目を計算し直すと選択数の2乗(以上)の処理量になり、数百件で固まったように重くなる
    internal sealed class CoalescedAction
    {
        private readonly Control _owner;
        private readonly Action _action;
        private Boolean _isPending;

        public CoalescedAction(Control owner, Action action)
        {
            _owner = owner;
            _action = action;
        }

        // 実行を予約する。予約済みなら何もしない(BeginInvokeで積んだメッセージは、
        // 同じ操作で発生した一連のイベントを処理し終えた後に処理される)
        public void Request()
        {
            if (!_owner.IsHandleCreated)
            {
                _action();
                return;
            }

            if (_isPending)
            {
                return;
            }

            _isPending = true;
            _owner.BeginInvoke((MethodInvoker)Run);
        }

        private void Run()
        {
            _isPending = false;
            if (!_owner.IsDisposed)
            {
                _action();
            }
        }
    }
}

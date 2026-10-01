﻿using System;
using StandardTemplate;

namespace ImageViewer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(ImageViewer parent)
        {
            // コントロールを列挙
            RegisterCtrl("Parent", "textBox_FolderPath", parent.textBox_FolderPath);
            RegisterCtrl("Parent", "textBox_Extension", parent.textBox_Extension);
        }
    }
}

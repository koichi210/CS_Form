﻿using System;
using StandardTemplate;

namespace ImageViewer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(ImageViewer parent)
        {
            // コントロールを列挙
            RegistCtrl("Parent", "textBox_FolderPath", parent.textBox_FolderPath);
            RegistCtrl("Parent", "textBox_Extension", parent.textBox_Extension);
        }
    }
}

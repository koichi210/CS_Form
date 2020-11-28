﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using StandardTemplate;

namespace ImageViewer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(ImageViewer Parent)
        {
            // コントロールを列挙
            RegistCtrl("Parent", "textBox_FolderPath", Parent.textBox_FolderPath);
            RegistCtrl("Parent", "textBox_Extension", Parent.textBox_Extension);
        }
    }
}

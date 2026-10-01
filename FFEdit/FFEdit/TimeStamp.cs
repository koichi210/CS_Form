using System;
using System.Collections.Generic;
using System.IO;

namespace FFEdit
{
    class TimeStamp
    {
        public String BaseDir { get; set; } = "";
        public List<String> FileList { get; set; }
        public long BaseTicks { get; set; } = 0;
        public long IntervalTicks { get; set; } = 0;    // 1ファイルごとにずらす時間
        public Boolean UpdateCreationTime { get; set; } = false;
        public Boolean UpdateLastWriteTime { get; set; } = false;
        public Boolean UpdateLastAccessTime { get; set; } = false;

        public void Execute()
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                String srcName = BaseDir + '\\' + FileList[i];
                long tick = IntervalTicks * i;
                Update(srcName, BaseTicks + tick);
            }
        }

        private void Update(String srcName, long tick)
        {
            DateTime dt = new DateTime(tick);

            // ファイル情報更新
            FileInfo fi = new FileInfo(srcName);
            if (UpdateCreationTime)
            {
                fi.CreationTime = dt;
            }
            if (UpdateLastWriteTime)
            {
                fi.LastWriteTime = dt;
            }
            if (UpdateLastAccessTime)
            {
                fi.LastAccessTime = dt;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _000.Common._04._Struct
{
    public struct ExtDriveInfo
    {
        public DriveInfo DrvInfo;

        public string   Name            => DrvInfo.Name;
        public int      TotalGBSize     => Convert.ToInt32(DrvInfo.TotalSize / 1024 / 1024 / 1024);
        public int      FreeGBSize      => Convert.ToInt32(DrvInfo.AvailableFreeSpace / 1024 / 1024 / 1024);
        public int      UsageGBSize     => Convert.ToInt32(TotalGBSize - FreeGBSize);
        public float    UsagePercent    => ((float)UsageGBSize / (float)TotalGBSize) * 100F;
    }
}

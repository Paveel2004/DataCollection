﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Data_collection
{
    public class InformationGathererDisk
    {
        public static double TotalFreeSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            long totalFreeSpace = 0;
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    totalFreeSpace += d.AvailableFreeSpace;
                }
            }
            return totalFreeSpace / Math.Pow(1024, 3);
        }
        public static double TotalSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            long totalSpace = 0;
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    totalSpace += d.TotalSize;
                }
            }
            return totalSpace / Math.Pow(1024, 3);
        }

    }
}

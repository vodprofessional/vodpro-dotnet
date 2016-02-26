using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;

namespace VP2.BusinessLogic
{
    public static class FolderUtility
    {
        

        public static bool DriveHasSpace(string path, int minspace)
        {
            // path is eaither a file or directory

            string root = Path.GetPathRoot(path);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == root)
                {
                    return drive.TotalFreeSpace > minspace;
                }
            }
            return false;
        }


    }
}

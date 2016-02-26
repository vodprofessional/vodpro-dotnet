using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VUI.VUI3.classes;
using VUI.VUI2.classes;
using System.IO;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {

            // VUI3News.AddNews(VUI3News.NEWSTYPE_SCREENSHOT, relatedServiceId: 17693, relatedService: "4oD", relatedPlatform: "Web", relatedDevice: String.Empty, ScreenshotCount: 34, description: "34 new screenshots added for 4oD on Tablet / iPad");



            /*Importer.InitFeatureMaps();

            foreach (string dname in Importer.featureDirectoryMap.Keys)
            {

                Directory.CreateDirectory(Path.Combine(@"d:\TMP\VODPRO\", dname));

            }
            */

                DownloadAttachment.GetAttachment();

        }
    }
}

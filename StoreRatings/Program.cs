using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoreRatings
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            string[] ingestsToRun = { };
            
            string IngestTag = DateTime.Now.ToString("yyyyMMddhhmmss");
            bool skipcomments = false;
            List<string> serviceNames = new List<string>();

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("-s"))
                    {
                        try
                        {
                            string runners = args[i + 1];
                            ingestsToRun = runners.Split(';');
                        }
                        catch (Exception ex) { }
                    }

                    if(args[i].Equals("-i"))
                    {
                        try
                        {
                            IngestTag = args[i + 1];
                        }
                        catch (Exception ex) { }
                    }

                    if (args[i].Equals("-skipcomments"))
                    {
                        skipcomments = true;
                    }

                    if (args[i].Equals("-services"))
                    {
                        try
                        {
                            string sn = args[i + 1];
                            sn = sn.Replace(@"""", "");
                            serviceNames.AddRange(sn.Split(';').ToList());
                        }
                        catch (Exception ex) { }

                    }

                }
            }

            if (ingestsToRun.Length == 0)
            {
                ingestsToRun = new string[] { "all" };
            }

            RatingsManager.GetRatings(ingestsToRun, IngestTag, skipcomments, serviceNames);
        }
    }
}

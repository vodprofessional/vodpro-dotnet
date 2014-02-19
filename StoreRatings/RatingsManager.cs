using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Configuration;
using VUI.VUI3.classes;

namespace StoreRatings
{
    public class RatingsManager
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(RatingsManager));
        private static int _logCount = 0;
        private static List<ErrorLogEntry> errorLog = new List<ErrorLogEntry>();

        private static int iTunesCounter = 0;
        private static List<string> iTunesSuccesses = new List<string>();
        private static int playCounter = 0;
        private static List<string> playSuccesses = new List<string>();
        private static int windowsCounter = 0;
        private static List<string> windowsSuccesses = new List<string>();
        private static List<string> NewVersions = new List<string>();

        public static void GetRatings(string[] ingestsToRun, string IngestTag, bool skipcomments, List<String> serviceNames)
        {
            log.Info(" --------------------------------------------------------------- ");
            log.Info(" STARTING STORE RATINGS INGEST ");
            log.Info(" --------------------------------------------------------------- ");


      //      WindowsMarketManager.GetWindowsRatings("phoneWindowsAppURL", "Smartphone/Windows");
            // WindowsMarketManager.GetWindowsRatings("Tablet/Windows");

            if (ingestsToRun.Contains("all") || ingestsToRun.Contains("Tablet/iPad"))
            {
                ITunesManager.GetITunesRatings("iPadAppURL", "Tablet/iPad", IngestTag, skipcomments, serviceNames);
                log.Info(" *********************************** ");
                log.Info(" COMPLETED iPad Ingest ");
                log.Info(" *********************************** ");
            }
            if (ingestsToRun.Contains("all") || ingestsToRun.Contains("Smartphone/iPhone"))
            {
                ITunesManager.GetITunesRatings("iPhoneAppURL", "Smartphone/iPhone", IngestTag, skipcomments, serviceNames);
                log.Info(" *********************************** ");
                log.Info(" COMPLETED iPhone Ingest ");
                log.Info(" *********************************** ");
            }
            if (ingestsToRun.Contains("all") || ingestsToRun.Contains("Tablet/Android"))
            {
                PlayStoreManager.GetPlayStoreRatings("tabletPlayAppURL", "Tablet/Android", IngestTag, skipcomments, serviceNames);
                log.Info(" *********************************** ");
                log.Info(" COMPLETED Droid Pad Ingest ");
                log.Info(" *********************************** ");
            }
            if (ingestsToRun.Contains("all") || ingestsToRun.Contains("Smartphone/Android"))
            {
                PlayStoreManager.GetPlayStoreRatings("phonePlayAppURL", "Smartphone/Android", IngestTag, skipcomments, serviceNames);
                log.Info(" *********************************** ");
                log.Info(" COMPLETED Droid Phone Ingest ");
                log.Info(" *********************************** ");
            }
            if (ingestsToRun.Contains("all") || ingestsToRun.Contains("Versions"))
            {
                GetNewVersions(IngestTag);
                log.Info(" *********************************** ");
                log.Info(" COMPLETED Versions Check ");
                log.Info(" *********************************** ");
            }

            log.Debug("Sending Email");
            SendEmail();

            log.Info(" --------------------------------------------------------------- ");
            log.Info(" COMPLETED STORE RATINGS INGEST ");
            log.Info(" --------------------------------------------------------------- ");
            log.Info(" ");
            log.Info(" ");

        }

        public static void GetNewVersions(string IngestTag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                try
                {
                    string sql = @"select R.Version, R.DeviceType, S.ServiceName, S.Id
                                    from  [dbo].[vui_ServiceStoreRating] R inner join vui_ServiceMasters S on R.ServiceID=S.ID
                                    where R.NewVersion = 'Y'
                                    and IngestTag = @IngestTag";

                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    comm.Parameters.Add(new SqlParameter("@IngestTag", IngestTag));
                    SqlDataReader sr = comm.ExecuteReader();
                    while (sr.Read())
                    {
                        string serviceName = (string)sr["ServiceName"];
                        int    serviceId   = (int)sr["Id"];
                        string version     = (string)sr["Version"];
                        string deviceType  = (string)sr["DeviceType"];

                        string[] pd        = deviceType.Split('/');
                        string platform    = pd[0];
                        string device      = pd[1];

                        string AppStore = VUI3News.NEWSSTORE_APPLE;
                        if (device.Equals("iPad") || device.Equals("iPhone"))
                        {
                            AppStore = VUI3News.NEWSSTORE_APPLE;
                        }
                        if (device.Equals("Android"))
                        {
                            AppStore = VUI3News.NEWSSTORE_GOOGLE;
                        } 
                        if (device.Equals("Windows"))
                        {
                            AppStore = VUI3News.NEWSSTORE_WINDOWS;
                        }

                        try 
                        {
                            NewVersions.Add(String.Format(@"New version [{0}] for {1} on {2} ", new object[] { version, serviceName, deviceType }));
                            
                            VUI3News.AddNews(newsType: VUI3News.NEWSTYPE_VERSION, appStore:AppStore,  relatedServiceId: serviceId, relatedService: serviceName, relatedDevice:device, relatedPlatform:platform, version:version );
                        }
                        catch(Exception e) {
                            errorLog.Add(new ErrorLogEntry(-1, String.Format("Error inserting version:[{0}] for {1} on {2}", new object[] {version, serviceName, deviceType }), e));
                        }
                    }
                    sr.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    log.Error("Error collating new versions", ex);
                }
            }
        }
        
        public static void AddITunesSuccess(string service)
        {
            iTunesSuccesses.Add(service);
            iTunesCounter++;
        }
        public static void AddPlaySuccess(string service)
        {
            playSuccesses.Add(service);
            playCounter++;
        }
        public static void AddWindowsSuccess(string service)
        {
            windowsSuccesses.Add(service);
            windowsCounter++;
        }

        public static void AddErrorToLog(ErrorLogEntry e)
        {
            errorLog.Add(e);
        }
        public static void AddErrorToLog(int logCount, string message, Exception ex)
        {
            AddErrorToLog(new ErrorLogEntry(logCount,  message,  ex));
        }

        private static void SendEmail()
        {
            try
            {
                StringBuilder body = new StringBuilder("");

                body.AppendLine("Report of AppStore ingest on " + DateTime.Now.ToString("dd MMM yyyy"));

                body.AppendLine(" == iTunes Services Imported ==");
                body.AppendLine(" [" + iTunesCounter + "] successfully:");
                foreach (string s in iTunesSuccesses)
                {
                    body.AppendLine(" - " + s);
                }
                body.AppendLine();
                body.AppendLine();

                body.AppendLine(" == Play Services Imported ==");
                body.AppendLine(" [" + playCounter + "] successfully:");
                foreach (string s in playSuccesses)
                {
                    body.AppendLine(" - " + s);
                }
                body.AppendLine();
                body.AppendLine();

                body.AppendLine(" == Windows Market Services Imported ==");
                body.AppendLine(" [" + windowsCounter + "] successfully:");
                foreach (string s in windowsSuccesses)
                {
                    body.AppendLine(" - " + s);
                }
                body.AppendLine();
                body.AppendLine();

                body.AppendLine(" == Nwe Versions Found ==");
                body.AppendLine(" [" + NewVersions.Count + "] successfully:");
                foreach (string s in NewVersions)
                {
                    body.AppendLine(" - " + s);
                }
                body.AppendLine();
                body.AppendLine();


                body.AppendLine(" == Errors Encountered ==");
                foreach (ErrorLogEntry e in errorLog)
                {
                    body.AppendLine(e.ToString());
                }

                SmtpClient smtp = new SmtpClient();
                MailMessage msg = new MailMessage();
                msg.To.Add("oliverwood@vodprofessional.com");
                //msg.Bcc.Add("kauserkanji@vodprofessional.com");
                //msg.Bcc.Add("oliverwood@vodprofessional.com");
                msg.Subject = "VUI AppStore Ingest Report";
                msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");
                msg.Body = body.ToString();
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                log.Error("Error Sending Email", ex);
            }
        }

        public static int LogCount 
        { 
            get { _logCount++; return _logCount; }
        }

    }

    public class ErrorLogEntry
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public ErrorLogEntry(int logCount, string message, Exception ex)
        {
            Count = logCount;
            Message = message;
            Exception = ex;
        }

        public string ToString()
        {
            return "[" + Count.ToString() + "] " + Message + ": " + Exception.Message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;

namespace VUI.VUI3.classes
{
    public sealed class VUI3MetaDataThread
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3MetaDataThread));

        private static VUI3MetaDataThread instance = null;
        private static readonly object padlock = new object();

        private Thread runner;
        private int localCounter = 0;
        
        private int lastQueueItem = -1;
        private string lastServiceMaster = String.Empty;
        private DateTime lastOperation = DateTime.MinValue;
        private string status = String.Empty;
        private int sleepTime = 60000;


        VUI3MetaDataThread() 
        {
            string st = ConfigurationManager.AppSettings["VUI_QUEUE_SLEEP_TIME"];
            if (!String.IsNullOrEmpty(st))
            {
                Int32.TryParse(st, out sleepTime);
            }
        }

        public static HttpContext Current
        {
            get;
            set;
        }

        public static VUI3MetaDataThread Instance
        {
            get 
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new VUI3MetaDataThread();
                        Current = HttpContext.Current;
                    }
                    return instance;
                }
            }
        }

        public string Status
        {
            get 
            {
                if (runner == null)
                {
                    return "Thread is stopped";
                }
                else if (!runner.IsAlive)
                {
                    return "Thread is not alive [" + runner.ThreadState.ToString() + "]";
                }
                else
                {
                    string currentService = VUI3MetaData.CurrentService;

                    if (!lastOperation.Equals(DateTime.MinValue))
                    {
                        return "Thread is [" + currentService + "]. Last complete queue item [" + lastServiceMaster + "] at " + lastOperation.ToString("dd MMM yyyy HH:mm:ss");
                    }
                    else
                    {
                        return "Thread is  [" + currentService + "]";
                    }
                }
            }
        }

        public bool HasThread
        {
            get
            {
                if (runner == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void ProcessQueue()
        {
            try
            {
                log.Info("***** MetaData Process Queue Thread Started *****");

                localCounter = 0; // This wil get iterated up to 60 if nothing is on the queue
                while (localCounter < 60)
                {

                    // read the oldest item from the Queue
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand();
                        comm.CommandType = System.Data.CommandType.StoredProcedure;
                        comm.CommandText = @"vui_GetNextMetaDataQueueItem";
                        comm.Connection = conn;

                        SqlDataReader sr = comm.ExecuteReader();

                        sr.Read();
                        // Null returned (therefore no results). Close the DB and iterate the counter
                        if (sr["Id"].GetType() == typeof(DBNull) || sr["ServiceMasterName"].GetType() == typeof(DBNull))
                        {
                            log.Info("***** MetaData Queue Empty [" + localCounter + " iterations]. Trying again in " + sleepTime + "ms ... *****");
                            sr.Close();
                            conn.Close();
                            localCounter++;
                        }
                        // Otherwise, Do stuff and reset the counter
                        else
                        {
                            int currentQueueId = (Int32)sr["Id"];
                            string currentServiceName = (String)sr["ServiceMasterName"];

                            log.Info("***** Popped From Queue: [" + currentQueueId + "] " + currentServiceName + " *****");
                            sr.Close();

                            string status = "";
                            string message = "";
                            // Publish the MetaData

                            try
                            {
                                if (currentServiceName.Equals(VUI3MetaData.ALL_METADATA))
                                {
                                    VUI3MetaData.PublishAllServiceMetadata();
                                    status = "X";
                                    message = "OK";
                                    log.Info("***** Completed metadata publish for: [" + currentQueueId + "] ALL Services!!! *****");
                                }

                                else if (currentServiceName.Equals(VUI3MetaData.CLEAR_METADATA))
                                {
                                    VUI3MetaData.ClearAllMetadata();
                                    status = "X";
                                    message = "OK";
                                    log.Info("***** Completed metadata clearout for: [" + currentQueueId + "] *****");
                                }

                                else
                                {
                                    VUI3MetaData.PublishServiceMetadata(currentServiceName, true);
                                    status = "X";
                                    message = "OK";
                                    log.Info("***** Completed metadata publish for: [" + currentQueueId + "] " + currentServiceName + " *****");
                                }
                            }
                            catch (Exception ex)
                            {
                                status = "E";
                                message = ex.StackTrace;
                                log.Error("***** Error in metadata publish for: [" + currentQueueId + "] " + currentServiceName + " *****", ex);
                            }
                            finally
                            {
                                lastQueueItem = currentQueueId;
                                lastServiceMaster = currentServiceName;
                                lastOperation = DateTime.Now;

                                string sql = @"update vui_MetaDataQueue set Status=@status, Message=@message where Id=@id";
                                comm.CommandType = System.Data.CommandType.Text;
                                comm.CommandText = sql;
                                comm.Parameters.AddWithValue("@status", status);
                                comm.Parameters.AddWithValue("@message", message);
                                comm.Parameters.AddWithValue("@id", currentQueueId);
                                comm.ExecuteNonQuery();
                            }

                            // Reset the counter
                            localCounter = 0;
                        }

                        conn.Close();
                    }
                    Thread.Sleep(sleepTime);
                }
            }
            catch (ThreadAbortException e)
            {
                log.Info("***** MetaData Process Queue Thread Aborted *****");
            }
            catch (Exception ex2)
            {
                log.Error("***** MetaData Process Queue Thread Error", ex2);
            }
            finally
            {
                log.Info("***** MetaData Process Queue Thread Stopped *****");
                runner = null;
            }
        }

        public void StartProcessQueue()
        {
            if (runner == null)
            {
                HttpContext ctx = HttpContext.Current;
                runner = new Thread(new ThreadStart(() => { HttpContext.Current = ctx; ProcessQueue(); }));
                runner.Priority = ThreadPriority.BelowNormal;
            }
            if (!runner.IsAlive)
            {
                runner.Start();
            }
        }

        public void KillProcessQueue()
        {
            runner.Abort();
        }

    }
}
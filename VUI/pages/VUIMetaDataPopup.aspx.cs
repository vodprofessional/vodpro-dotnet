using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using VUI.VUI3.classes;
using System.Text;
using umbraco.MacroEngines;

namespace VUI.pages
{
    public partial class VUIMetaDataPopup : umbraco.BasePages.UmbracoEnsuredPage
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUIMetaDataPopup));

        protected void Page_Load(object sender, EventArgs e)
        {
            DisplayQueue();

            DynamicNode smRoot = new DynamicNode(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);
            List<DynamicNode> serviceMasters = smRoot.Descendants("VUI2ServiceMaster").Items.OrderBy(n => n.Name).ToList();

            if (!Page.IsPostBack)
            {
                lstServices.DataSource = serviceMasters;
                lstServices.DataTextField = "Name";
                lstServices.DataValueField = "Name";
                lstServices.DataBind();
            }
        }


        protected void RegenerateService(object sender, EventArgs e)
        {
            string serviceName = lstServices.SelectedValue;
            VUI3MetaData.AddToQueue(serviceName);
            litMessage.Text = "Added " + serviceName + " to queue";
            DisplayQueue();
        }

        protected void RegenerateAll(object sender, EventArgs e)
        {
            VUI3MetaData.AddToQueue(VUI3MetaData.ALL_METADATA);
            litMessage.Text = "Added ALL to queue";
            DisplayQueue();
        }

        protected void ClearAndRegenerateAll(object sender, EventArgs e)
        {
            VUI3MetaData.AddToQueue(VUI3MetaData.CLEAR_METADATA);
            VUI3MetaData.AddToQueue(VUI3MetaData.ALL_METADATA);
            litMessage.Text = "Added CLEAR ALL and REGENERATE to queue";
            DisplayQueue();
        }


        protected void DisplayQueue()
        {
            string sql;
            
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.Text;
                comm.CommandText = @"select * from vui_MetaDataQueue where Status = 'N' order by QueueDate, ID ASC";
                comm.Connection = conn;
                SqlDataReader sr = comm.ExecuteReader();


                if (sr.HasRows)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"<table class=""metadata-queue"" border=""1"">");
                    sb.AppendLine(@" <tr>");
                    sb.AppendLine(@"  <th>Service</th>");
                    sb.AppendLine(@"  <th>Status</th>");
                    sb.AppendLine(@"  <th>Date Queued</th>");
                    sb.AppendLine(@" </tr>");

                    while (sr.Read())
                    {
                        sb.AppendLine(@" <tr id=""item-" + (Int32)sr["Id"] + @""">");
                        sb.AppendLine(@"  <td>" + (String)sr["ServiceMasterName"] + "</td>");
                        sb.AppendLine(@"  <td>" + (String)sr["Status"] + "</td>");
                        sb.AppendLine(@"  <td>" + ((DateTime)sr["QueueDate"]).ToString("dd MMM yyyy HH:mm") + "</td>");
                        sb.AppendLine(@" </tr>");
                    }
                    sb.AppendLine(@"</table>");

                    litQueueComing.Text = sb.ToString();
                }

                sr.Close();
                comm.CommandText = @"select TOP 10 * from vui_MetaDataQueue where Status != 'N' order by ProcessDate DESC, ID DESC";
                sr = comm.ExecuteReader();
                if (sr.HasRows)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"<table class=""metadata-queue"" border=""1"">");
                    sb.AppendLine(@" <tr>");
                    sb.AppendLine(@"  <th>Service</th>");
                    sb.AppendLine(@"  <th>Status</th>");
                    sb.AppendLine(@"  <th>Date Processed</th>");
                    sb.AppendLine(@"  <th>Message</th>");
                    sb.AppendLine(@" </tr>");

                    while (sr.Read())
                    {
                        sb.AppendLine(@" <tr id=""item-" + (Int32)sr["Id"] + @""">");
                        sb.AppendLine(@"  <td>" + (String)sr["ServiceMasterName"] + "</td>");
                        sb.AppendLine(@"  <td>" + (String)sr["Status"] + "</td>");
                        if (sr["ProcessDate"].GetType() != typeof(DBNull))
                        {
                            sb.AppendLine(@"  <td>" + ((DateTime)sr["ProcessDate"]).ToString("dd MMM yyyy HH:mm") + "</td>");
                        }
                        else
                        {
                            sb.AppendLine(@"  <td></td>");
                        } 
                        if (sr["Message"].GetType() != typeof(DBNull))
                        {
                            sb.AppendLine(@"  <td>" + ((String)sr["Message"]) + "</td>");
                        }
                        else
                        {
                            sb.AppendLine(@"  <td></td>");
                        }
                        sb.AppendLine(@" </tr>");
                    }
                    sb.AppendLine(@"</table>");

                    litQueuePast.Text = sb.ToString();
                }
                sr.Close();
                conn.Close();

            }
        }

        protected void ReloadContinue()
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void ReloadStop()
        {
            Response.Redirect(Request.RawUrl);
        }


        protected void ProcessQueue()
        {
            bool keepRunning = true;

            log.Info("***** MetaData Queue Processor Iterating... *****");
            try
            {
                // read the oldest item from the Queue
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.CommandText = @"vui_GetNextMetaDataQueueItem";
                    comm.Connection = conn;

                    SqlDataReader sdr = comm.ExecuteReader();
                    // Null returned (therefore no results). Close the DB and iterate the counter
                    if (!sdr.HasRows)
                    {
                        log.Info("***** MetaData Queue Empty *****");
                        conn.Close();
                        keepRunning = false;
                    }
                    // Otherwise, Do stuff and reset the counter
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Load(sdr);

                        sdr.Close();
                        sdr = null;

                        foreach (DataRow sr in dt.Rows)
                        {
                            if (sr["Id"].GetType() == typeof(DBNull) || sr["ServiceMasterName"].GetType() == typeof(DBNull))
                            {
                                log.Info("***** MetaData Queue Empty *****");
                                keepRunning = false;
                            }
                            else
                            {

                                int currentQueueId = (Int32)sr["Id"];
                                string currentServiceName = (String)sr["ServiceMasterName"];

                                log.Info("***** Popped From Queue: [" + currentQueueId + "] " + currentServiceName + " *****");

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
                                    LastQueueItem = currentQueueId;
                                    LastServiceMaster = currentServiceName;
                                    
                                    string sql = @"update vui_MetaDataQueue set Status=@status, Message=@message, ProcessDate=GetDate() where Id=@id";
                                    comm.CommandType = System.Data.CommandType.Text;
                                    comm.CommandText = sql;
                                    comm.Parameters.AddWithValue("@status", status);
                                    comm.Parameters.AddWithValue("@message", message);
                                    comm.Parameters.AddWithValue("@id", currentQueueId);
                                    comm.ExecuteNonQuery();
                                }

                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception innerex) 
            {
                log.Error("***** Error in metadata publish - stopping loops *****", innerex);
            }

            if (keepRunning)
            {
                ReloadContinue();
            }

            else
            {
                ReloadStop();
            }
                
        }

        public int LastQueueItem { get; set; }
        public string LastServiceMaster { get; set; }

    }
}
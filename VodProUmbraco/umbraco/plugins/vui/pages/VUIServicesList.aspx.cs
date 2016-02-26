using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using umbraco.cms.businesslogic.web;

namespace VUI.pages
{
    public partial class VUIServicesList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int smRootId = Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);
            Document smRoot = new Document(smRootId);
            Document[] sms = smRoot.Children;
            List<Document> ServiceMasters = new List<Document>();
            ServiceMasters.AddRange(sms);

            rptServices.DataSource = ServiceMasters;
            rptServices.DataBind();
        }

        protected void BindItem(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Document d = e.Item.DataItem as Document;
                Literal litItemName = e.Item.FindControl("litItemName") as Literal;
                Literal litSubscriptionType = e.Item.FindControl("litSubscriptionType") as Literal;

                
                litItemName.Text = d.getProperty("serviceName").Value.ToString();
                if(d.Published)
                {
                    litItemName.Text = "<strong>" + litItemName.Text + "</strong>";
                }


                string subscriptionType = d.getProperty("subscriptionType").Value.ToString();
                string[] substypeids = subscriptionType.Split(',');
                List<string> subsTypes = new List<string>();
                System.Xml.XPath.XPathNodeIterator iterator = umbraco.library.GetPreValues(2852);
                iterator.MoveNext(); //move to first
                System.Xml.XPath.XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
                while (preValues.MoveNext())
                {
                    string id = preValues.Current.GetAttribute("id", "");
                    if (substypeids.Contains(id))
                    {
                        subsTypes.Add(preValues.Current.Value);
                    }
                }
                subscriptionType = String.Join(", ", subsTypes);

                if (subscriptionType.Contains("SVOD"))
                {
                    litSubscriptionType.Text = "Y";
                }
                
            }
        }

    }
}
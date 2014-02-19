using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using umbraco.cms.businesslogic.web;
using System.Xml.XPath;
using VUI.VUI2.classes;

namespace VUI.pages
{
    public partial class VUIService : umbraco.BasePages.UmbracoEnsuredPage
    {
        private string editURL = @"/umbraco/editContent.aspx?id={0}";

        protected void Page_Load(object sender, EventArgs e)
        {
            int servicemasterid = Int32.Parse(Request["id"]);
            HtmlAnchor a = Page.FindControl("lnkEditService") as HtmlAnchor;
            a.HRef = String.Format(editURL, servicemasterid);

            Document sm = new Document(servicemasterid);

            // Check the PreValues
            
            string title = sm.getProperty("serviceName").Value.ToString();
            string description = sm.getProperty("description").Value.ToString();
            string availability = sm.getProperty("availability").Value.ToString();
            string subscriptionType = sm.getProperty("subscriptionType").Value.ToString();
            string[] substypeids = subscriptionType.Split(',');
            List<string> subsTypes = new List<string>();
            XPathNodeIterator iterator = umbraco.library.GetPreValues(2852);
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
            while (preValues.MoveNext())
            {
                string id = preValues.Current.GetAttribute("id", "");
                if (substypeids.Contains(id))
                {
                    subsTypes.Add(preValues.Current.Value);
                }
            }
            subscriptionType = String.Join(", ", subsTypes);

            string serviceCategory = sm.getProperty("serviceCategory").Value.ToString();
            string[] scatids = serviceCategory.Split(',');
            List<string> scats = new List<string>();
            iterator = umbraco.library.GetPreValues(3864);
            iterator.MoveNext(); //move to first
            preValues = iterator.Current.SelectChildren("preValue", "");
            while (preValues.MoveNext())
            {
                string id = preValues.Current.GetAttribute("id", "");
                if (scatids.Contains(id))
                {
                    scats.Add(preValues.Current.Value);
                }
            }
            serviceCategory = String.Join(", ", scats);

            string urlIPhone = sm.getProperty("iPhoneAppURL").Value.ToString();
            string urlIPad = sm.getProperty("iPadAppURL").Value.ToString();
            string urlDroidPhone = sm.getProperty("phonePlayAppURL").Value.ToString();
            string urlDroidTablet = sm.getProperty("tabletPlayAppURL").Value.ToString();
            string urlTwitter = sm.getProperty("twitterURL").Value.ToString();
            string urlYouTube = sm.getProperty("youTubeURL").Value.ToString();
            string urlFacebook = sm.getProperty("facebookURL").Value.ToString();

            litServiceMasterId.Text = @"<span id=""servicemasterid"" data-id=""" + servicemasterid + @""">"+ servicemasterid +"</span>";
            litTitle.Text = title;
            litDescription.Text = description;
            litAvailability.Text = availability;
            litSubscriptionType.Text = subscriptionType;
            litServiceCategory.Text = serviceCategory;

            litStatus.Text = sm.Published ? "Published" : "Not Published";

            litTwitterURL.Text = urlTwitter;
            litYouTubeURL.Text = urlYouTube;
            litFacebookURL.Text = urlFacebook;
            litiPhoneURL.Text = urlIPhone;
            litiPadURL.Text = urlIPad;
            litPlayPhone.Text = urlDroidPhone;
            litPlayTablet.Text = urlDroidTablet;
                        
        }
    }
}
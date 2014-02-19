using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VUI.VUI3.classes;
using umbraco.MacroEngines;
using System.Configuration;

namespace VUI.usercontrols
{
    public partial class vui_news : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DynamicNode services = new DynamicNode(Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString()));
            ddService.Items.Add(new ListItem("","-1"));
            foreach (DynamicNode service in services.Descendants())
            {
                ddService.Items.Add(new ListItem(service.Name, service.Id.ToString()));
            }

            BindNews();
        }

        private void BindNews()
        {
            rptNews.DataSource = VUI3News.GetNewsByStatus(50,"all");
            rptNews.DataBind();
        }

        protected void Refresh(object sender, EventArgs e)
        {
            rptNews.DataSource = VUI3News.GetNewsByStatus(50, "all");
            rptNews.DataBind();
        }

        protected void BindNews(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                NewsItem news = (NewsItem)e.Item.DataItem;
                Literal litNewsDate = e.Item.FindControl("litNewsDate") as Literal;
                Literal litNewsHeadline = e.Item.FindControl("litNewsHeadline") as Literal;
                Literal litIsLive = e.Item.FindControl("litIsLive") as Literal;
                Literal litIsTweeted = e.Item.FindControl("litIsTweeted") as Literal;

                Button btnSetLive = e.Item.FindControl("btnSetLive") as Button;
                Button btnDelete = e.Item.FindControl("btnDelete") as Button;
                Button btnTweet = e.Item.FindControl("btnTweet") as Button;

                litNewsDate.Text = news.DateCreated.ToString("dd MMM yyyy HH:mm:ss");
                litNewsHeadline.Text = news.Description;
                litIsLive.Text = news.IsLive ? "Live" : "Draft";
                litIsTweeted.Text = news.IsTweeted ? "Tweeted" : "";

                btnDelete.CommandArgument = news.ID.ToString();
                btnSetLive.CommandArgument = news.ID.ToString();
                btnTweet.CommandArgument = news.ID.ToString();

                if (news.IsLive)
                {
                    btnSetLive.Visible = false;
                }

                if (news.IsTweeted)
                {
                    btnTweet.Visible = false;
                }

            }
        }

        protected void NewsCommand(object sender, CommandEventArgs e)
        {
            int id = Int32.Parse((string)e.CommandArgument);

            switch (e.CommandName)
            {
                case "Publish":
                    VUI3News.ChangeStatus(id, "Y");
                    break;

                case "Delete" :
                    VUI3News.DeleteNews(id);
                    break;

                case "Tweet":
                    VUI3News.TweetNews(id);
                    break;

            }
            BindNews();
        }


        protected void btnTweet_Click(object sender, EventArgs e)
        {
            VUI3News.TweetNews(txtStatus.Text);
        }


        protected void btnSaveNews_Click(object sender, EventArgs e)
        {
            bool setLive = chkSetLive.Checked;
            bool tweetNow = chkTweetImmediate.Checked;

            string newsType = ddNewsType.SelectedValue;

            if (newsType.Equals(VUI3News.NEWSTYPE_SYSTEM))
            {
                VUI3News.AddNews(newsType: newsType, description: txtDescription.Text, directToLive: setLive, tweetNews: tweetNow);
            }
            if (newsType.Equals(VUI3News.NEWSTYPE_BENCHMARK))
            {
                VUI3News.AddNews(newsType: newsType, relatedServiceId: Int32.Parse(ddService.SelectedValue), relatedService: ddService.SelectedItem.Text, relatedPlatform: ddPlatform.SelectedValue, relatedDevice: ddDevice.SelectedValue, directToLive: setLive, tweetNews: tweetNow);
            }
            if (newsType.Equals(VUI3News.NEWSTYPE_SCREENSHOT))
            {
                VUI3News.AddNews(newsType: newsType, ScreenshotCount: Int32.Parse(txtScreenshotCount.Text), relatedServiceId: Int32.Parse(ddService.SelectedValue), relatedService: ddService.SelectedItem.Text, relatedPlatform: ddPlatform.SelectedValue, relatedDevice: ddDevice.SelectedValue, directToLive: setLive, tweetNews: tweetNow);
            }
            if (newsType.Equals(VUI3News.NEWSTYPE_VERSION))
            {
                VUI3News.AddNews(newsType: newsType, version: txtVersion.Text, appStore: ddAppStore.SelectedValue, relatedPlatform: ddPlatform.SelectedValue, relatedDevice: ddDevice.SelectedValue, directToLive: setLive, tweetNews: tweetNow);
            }
            if (newsType.Equals(VUI3News.NEWSTYPE_NEW))
            {
                VUI3News.AddNews(newsType: newsType, relatedServiceId: Int32.Parse(ddService.SelectedValue), relatedService: ddService.SelectedItem.Text, description: txtDescription.Text, directToLive: setLive, tweetNews: tweetNow);
            }
            BindNews();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using VP2.businesslogic;

namespace VP2.usercontrols
{
    public partial class event_list : System.Web.UI.UserControl
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(event_list));

        private string editUrl = "/members/add-event";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEvents();
            }
        }

        private void BindEvents()
        {
            rptEvents.DataSource = CalendarItem.GetCurrentMembersEvents().OrderByDescending(evt => evt.StartDate).ToList();
            rptEvents.DataBind();
        }

        protected void rptEvents_ItemBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CalendarItem evt = (CalendarItem)e.Item.DataItem;

                HtmlAnchor lnkPreview = e.Item.FindControl("lnkPreview") as HtmlAnchor;
                HtmlAnchor lnkEdit = e.Item.FindControl("lnkEdit") as HtmlAnchor;
                LinkButton btnUnpublish = e.Item.FindControl("btnUnpublish") as LinkButton;
                Literal litName1 = e.Item.FindControl("litName1") as Literal;
                Literal litCity = e.Item.FindControl("litCity") as Literal;
                Literal litDate = e.Item.FindControl("litDate") as Literal;
                Literal litName2 = e.Item.FindControl("litName2") as Literal;

                litCity.Text = evt.City;
                litDate.Text = evt.DateString2;

                lnkEdit.HRef = editUrl + "?i=" + evt.ID;

                if (evt.IsPublished)
                {
                    litName1.Text = evt.Name;
                    lnkPreview.HRef = evt.Url;
                    btnUnpublish.CommandArgument = evt.ID.ToString();
                    btnUnpublish.Attributes["class"] += " btn-primary";
                }
                else
                {
                    lnkPreview.Visible = false;
                    litName2.Text = evt.Name;
                    btnUnpublish.Attributes["click"] = "javascript:void(0)";
                    btnUnpublish.Attributes["class"] += " btn-default disabled";
                }
            }
        }

        protected void Unpublish_Command(object sender, CommandEventArgs e)
        {
            string s = (string)e.CommandArgument;
            int id;
            if (Int32.TryParse(s, out id))
            {
                try
                {
                    CalendarItem evt = new CalendarItem();
                    evt.ID = id;
                    if (evt.UnPublish())
                    {
                        plcUnpubMessage.Visible = true;
                        BindEvents();
                    }
                }
                catch(Exception ex)
                {
                    log.Error("Error unpublishing event", ex);
                }

            }
        }
    }
}
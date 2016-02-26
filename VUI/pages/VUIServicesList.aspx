<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="umbraco.cms.businesslogic.web" %>
<script runat="server">


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
            Literal litPublished = e.Item.FindControl("litPublished") as Literal;

            litItemName.Text = d.getProperty("serviceName").Value.ToString();
            if (d.Published)
            {
                litItemName.Text = "<strong>" + litItemName.Text + "</strong>";
                litPublished.Text = "1";
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
                litSubscriptionType.Text = "1";
            }

        }
    }

</script>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <th>Service</th>
                <th>Published</th>
                <th>SVOD</th>
            </tr>
        <asp:Repeater runat="server" ID="rptServices" OnItemDataBound="BindItem">
            <ItemTemplate>
                <tr>
                    <td><asp:Literal runat="server" ID="litItemName" /></td>
                    <td><asp:Literal runat="server" ID="litPublished" /></td>
                    <td><asp:Literal runat="server" ID="litSubscriptionType" /></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>
    </div>
    </form>
</body>
</html>

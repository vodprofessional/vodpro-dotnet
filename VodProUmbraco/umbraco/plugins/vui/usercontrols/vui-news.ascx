<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-news.ascx.cs" Inherits="VUI.usercontrols.vui_news" %>

<style type="text/css">
p.news-form { font-size:1.2em; line-height: 2.2em; }
p.news-form input, p.news-form select  { font-size:1.3em; }
p.news-form input[disabled], p.news-form select[disabled] { background-color: #eee; }  
p.news-form label { display:inline-block; width:180px; font-weight: bold; }
input.news-text-desc { width:400px; }
input.text-tweet { width:400px; }
</style>
<script type="text/javascript" src="/Scripts/umb-vui.js"></script>

<div class="dashboardWrapper">
    <h2>VUI News</h2>

    <h3>Recent News</h3>
    <em>The latest <asp:Literal runat="server" ID="litNumnewsitems" /> are shown on the site</em>
    <asp:Button runat="server" ID="btnRefresh" OnClick="Refresh" Text="Refresh List" />
    
    <div style="border:2px solid #999; height:400px; width:100%; overflow-y:scroll">
    <asp:Repeater runat="server" ID="rptNews" OnItemDataBound="BindNews">
        <HeaderTemplate><table class="news-layout"><tr><th>Date</th><th>Headline</th><th> </th><th> </th></tr></HeaderTemplate>
        <ItemTemplate>
            <tr style="border-bottom: 1px dotted #999;">
                <td><asp:Literal runat="server" ID="litNewsDate" /></td>
                <td><asp:Literal runat="server" ID="litNewsHeadline" /></td>
                <td>
                    <asp:Literal runat="server" ID="litIsLive" />
                    <asp:Button runat="server" ID="btnSetLive" Text="Publish" CommandName="Publish" CommandArgument="" OnCommand="NewsCommand" />
                    <asp:Button runat="server" ID="btnDelete"  Text="Delete" CommandName="Delete" CommandArgument="" OnCommand="NewsCommand" /></td>
                <td>
                    <asp:Literal runat="server" ID="litIsTweeted" />
                    <asp:Button runat="server" ID="btnTweet"  Text="Tweet" CommandName="Tweet" CommandArgument="" OnCommand="NewsCommand" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:Repeater>
    </div>
    <hr />

    <h3>Add a Manual News Item</h3>
    <p class="guiDialogNormal news-form">
        <label>Type: </label> <asp:DropDownList runat="server" ID="ddNewsType" CssClass="news-dd-type">
            <asp:ListItem Text="System Message" Value="system" />
            <asp:ListItem Text="Benchmark" Value="bench" />
            <asp:ListItem Text="Screenshots" Value="screen" />
            <asp:ListItem Text="New Version" Value="version" />
            <asp:ListItem Text="New Service" Value="new" />
        </asp:DropDownList>
        <br />

        <label>Related Service</label> <asp:DropDownList runat="server" ID="ddService" CssClass="news-dd-svc"></asp:DropDownList>
        <br />

        <label>Platform</label> <asp:DropDownList runat="server" ID="ddPlatform" CssClass="news-dd-plat">
            <asp:ListItem Text="" Value="" />
            <asp:ListItem Text="Web" Value="Web" />
            <asp:ListItem Text="Tablet" Value="Tablet" />
            <asp:ListItem Text="Smartphone" Value="Smartphone" />
            <asp:ListItem Text="Connected TV" Value="Connected TV" />
        </asp:DropDownList>
        <br />

        <label>Device</label>
        <asp:DropDownList runat="server" ID="ddDevice" CssClass="news-dd-dev">
            <asp:ListItem Text="" Value="" />
            <asp:ListItem Text="iPad" Value="iPad" />
            <asp:ListItem Text="Android" Value="Android" />
            <asp:ListItem Text="iPhone" Value="iPhone" />
            <asp:ListItem Text="Windows" Value="Windows" />
            <asp:ListItem Text="Sony" Value="Sony" />
            <asp:ListItem Text="Samsung" Value="Samsung" />
        </asp:DropDownList>
        <br />

        <label>Screenshot Count</label> <asp:TextBox runat="server" id="txtScreenshotCount" CssClass="news-text-scrn" />
        <br />

        <label>New Version Number</label> <asp:TextBox runat="server" id="txtVersion" CssClass="news-text-vers" />
        <br />
        
        <label>AppStore</label> <asp:DropDownList runat="server" ID="ddAppStore" CssClass="news-dd-app">
            <asp:ListItem Text="" Value="" />
            <asp:ListItem Text="Apple AppStore" Value="apple" />
            <asp:ListItem Text="Google Play" Value="google" />
            <asp:ListItem Text="Windows MarketPlace" Value="windows" />
        </asp:DropDownList>
        <br />

        <label>Description</label> <asp:TextBox runat="server" ID="txtDescription" CssClass="news-text-desc" MaxLength="200" />
        <br />
        
        <label>Publish Immediately?</label> <asp:CheckBox runat="server" ID="chkSetLive" />
        <br />
        <label>Tweet Immediately?</label> <asp:CheckBox runat="server" ID="chkTweetImmediate" />
        <br />

        <asp:Button runat="server" ID="btnSaveNews" Text="Save News" OnClick="btnSaveNews_Click"/>
    </p>



    <h3>Manual Tweet on behalf of @VUILibrary</h3>
    <p class="guiDialogNormal news-form">
        <label>Status: </label><asp:TextBox runat="server" ID="txtStatus" CssClass="text-tweet" MaxLength="160" />
        <asp:Button runat="server" ID="btnTweet" Text="Tweet Now" OnClick="btnTweet_Click"/>
    </p>



    <!--
    <p class="guiDialogNormal">
        <div id="VUI-news"></div>
        <div id="VUI-News-template"><div><span item-prop="news-date"></span> <span item-prop="news-type"></span> <span item-prop="description"></span></div></div>
    </p>
    -->

</div>
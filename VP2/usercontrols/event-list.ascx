<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="event-list.ascx.cs" Inherits="VP2.usercontrols.event_list" %>


<div style="border-bottom: 1px solid rgb(225, 225, 225); margin-bottom: 5px; padding-bottom: 15px; margin-top: 15px;" class="row">
    <div class="col-xs-12">
    <a title="Back to calendar" href="/calendar"><i class="fa fa-arrow-circle-left"></i> Events Calendar</a>
    <span class="pull-right">
        <a title="View my events list" href="/members/add-event">Add a new event <i class="fa fa-plus-circle"></i></a>
    </span>
  </div>
</div>

<asp:PlaceHolder runat="server" ID="plcUnpubMessage" visible="false">
    <div class="alert alert-info" role="alert">
        Successfully unpublished event
    </div>
</asp:PlaceHolder>

<asp:Repeater runat="server" ID="rptEvents" OnItemDataBound="rptEvents_ItemBound" OnItemCommand="Unpublish_Command">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="row" style="border-bottom:1px solid #e1e1e1; padding-bottom:5px; margin-bottom: 5px;">
            <div class="col-xs-9">
                <a href="" id="lnkPreview" runat="server"><asp:Literal runat="server" ID="litName1" /></a><asp:Literal runat="server" ID="litName2" />,
            <asp:Literal runat="server" ID="litCity" />, <asp:Literal runat="server" ID="litDate" />
            </div>
            <div class="col-xs-3 text-right">
                <a href="" id="lnkEdit" runat="server" class="btn btn-primary">Edit</a> &nbsp;
                <asp:LinkButton id="btnUnpublish" runat="server" class="btn" CommandName="unpublish" onClientClick="return window.confirm('Confirm that you really want to unpublish this event from the web site')" Text="Unpublish"></asp:LinkButton> &nbsp;
            </div>
        </div>

    </ItemTemplate>
    <FooterTemplate></FooterTemplate>
</asp:Repeater>
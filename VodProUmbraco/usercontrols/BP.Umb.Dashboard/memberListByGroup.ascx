<%@ Control Language="c#" AutoEventWireup="True" Codebehind="memberListByGroup.ascx.cs" Inherits="BP.Umb.Dashboard.memberListByGroup" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<h3 style="MARGIN-LEFT: 0px">List members in groups</h3>
<P class="guiDialogNormal">
	<asp:ListBox ID="MemberGroupListBox" runat="server" 
        Rows="6" SelectionMode="Multiple" 
        DataTextField="text" DataValueField="text" Width="239px">
    </asp:ListBox>
    <asp:CheckBox ID="ReverseFilterCheckBox" runat="server" Text="Reverse filter" />
	<asp:Button id="ButtonSearch" runat="server" Text="Get data" 
        onclick="ButtonSearch_Click"></asp:Button>
</P>

<P class="guiDialogNormal">
	<BR>
</P>
<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
<BR>
<asp:DataGrid id="DataGrid1" runat="server" Visible="False" Width="100%" 
    AllowSorting="True" PageSize="250">
	<AlternatingItemStyle BackColor="#E0E0E0"></AlternatingItemStyle>
	<HeaderStyle BackColor="Silver"></HeaderStyle>
</asp:DataGrid>
<p>
    &nbsp;</p>


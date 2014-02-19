<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="VPMemberSearch.ascx.cs" Inherits="com.vodprofessional.VPMemberSearch" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
  <div class="dashboardWrapper">
    <h2>Email List </h2>
    <img src="/umbraco/dashboard/images/membersearch.png" alt="Videos" class="dashboardIcon" />
<p class="guiDialogNormal">

    (all member signups, regardless whether they are active or not)

    <br />

	<asp:Button id="ButtonSearch" runat="server" Text="Get Email List" onclick="ButtonSearch_Click"></asp:Button></p>

	<cc1:Pane ID="resultsPane" runat="server" Visible="false">
        <textarea id="resultsarea" cols="100" rows="30"><asp:Repeater ID="rp_members" runat="server" OnItemDataBound="bindMember" >
        <HeaderTemplate>Email,Firstname,Surname</HeaderTemplate>
        <ItemTemplate>
        <asp:Literal ID="lt_email" runat="server"></asp:Literal>,<asp:Literal ID="lt_firstname" runat="server"></asp:Literal>,<asp:Literal ID="lt_lastname" runat="server"></asp:Literal></ItemTemplate></asp:Repeater></textarea>
    </cc1:Pane>

    <asp:Button ID="btnGetAllUserCSV" runat="server" Text="Get All Registrant CSV" OnClick="GetAllRegistrantCSV" />
    <asp:Panel ID="pnlAllUsers" runat="server" Visible="false">
        <asp:TextBox runat="server" ID="txtAllUsers" Rows="50" Columns="100" Wrap="false" TextMode="MultiLine" />
    </asp:Panel>
       </div>

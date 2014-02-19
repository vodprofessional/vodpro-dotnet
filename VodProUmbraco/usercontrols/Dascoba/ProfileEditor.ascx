<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileEditor.ascx.cs" Inherits="Dascoba.Umb.ProfileEditor.Editor" %>
<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
<link rel='stylesheet' href='/umbraco_client/propertypane/style.css' />
<p>
    <asp:Button ID="UpdateButton" runat="server" Text="Update Profile" OnClick="UpdateButton_Click"
        OnClientClick="javascript:return confirm('Are you sure you wish to update your profile?');" />
    <asp:Literal ID="litStatus" runat="server"></asp:Literal>
</p>
<asp:Panel CssClass="tabpagecontainer" ID="tabpagecontainer" runat="server" />

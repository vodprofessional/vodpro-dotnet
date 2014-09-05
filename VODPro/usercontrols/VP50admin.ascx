<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VP50admin.ascx.cs" Inherits="VODPro.usercontrols.VP50admin" %>


<div class="dashboardWrapper">
<h2>VODPro 50</h2>

<p class="guiDialogNormal">
 <asp:Button runat="server" CausesValidation="false" ID="btnGenerateVP50" OnClick="btnGenerateVP50_Click" Text="Publish VP 50" />
 <asp:Literal runat="server" ID="litConfirm" />
</p>
</div>
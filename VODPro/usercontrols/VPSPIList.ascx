<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VPSPIList.ascx.cs" Inherits="VODPro.usercontrols.VPSPIList" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<style>

.SPIList { border: 1px solid #999; margin: 15px; height: 200px; overflow-y: scroll; }
.SPIList tr:nth-child(odd) { background-color: #f0f0f0; }
.SPIList tr:nth-child(even) { background-color: #e0e0e0; }

.SPIList tr:first-child { background-color: #ccc; }
.SPIList th, .SPIList td { padding: 5px; margin: 0px }
.SPIList th { font-weight: bold; }
.spiWide { width: 300px; }


</style>
<div class="dashboardWrapper">
<h2>Service Provider Info</h2>

<p class="guiDialogNormal">

<asp:Panel ID="SPIForm" runat="server">
    Add SPI Company <asp:TextBox ID="txtNewSPI" runat="server" />
    <asp:Button OnClick="btnSaveSPI_Click" runat="server" ID="btnSaveSPI" Text="Generate SPI URL"></asp:Button>
    <span id="spiGreyedout" runat="server">&nbsp;&nbsp;<strong>SPI Unique URL</strong> <asp:TextBox ID="txtSPIURL" runat="server" CssClass="spiWide" /></span>

    <asp:Repeater ID="SPIList" runat="server" OnItemDataBound="SPIListItemBound" ViewStateMode="Disabled">
    
        <HeaderTemplate>
            <table class="SPIList" cellspacing="0">
                <tr>
                    <th>SPI Document Name</th>
                    <th>URL</th>
                    <th>Updated</th>
                    <th>Completed</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><asp:Literal ID="lt_Company" runat="server"/></td>
                <td><asp:HyperLink ID="lt_URL" runat="server"/></td>
                <td><asp:Literal ID="lt_Saved" runat="server"/></td>
                <td><asp:Literal ID="lt_Completed" runat="server"/></td>
            </tr>
        </ItemTemplate>

        <FooterTemplate>
            </table>
        </FooterTemplate>
    
    </asp:Repeater>


</asp:Panel>

</p>





</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-membership.ascx.cs" Inherits="VUI.usercontrols.vui_membership" %>


<div class="dashboardWrapper">
    <h2>VUI Members</h2>
    <p class="guiDialogNormal">

        <asp:Panel ID="pnlRegistrant" runat="server" Visible="false">
            This should only be run once! It adds ALL members to the registrant group
                <asp:Button runat="server" ID="btnChangeAllToRegistrant" Text="Add registrant to ALL users" OnClick="AddRegistrant_Click" />

                

        </asp:Panel>

        <asp:Button runat="server" ID="btnSetAllVUIEndDates" Text="Set All VUI Subscription End Dates" OnClick="SetVUIEndDates" />

        <asp:Button runat="server" ID="btnGetUsers" Text="Show VUI Users" OnClick="GetUsers" />
        

        <p>
            <strong>Change the VUI Administrator for a customer</strong>
            <br />
            You'll want to double-check these values before running. THe two users should already be in the system, and the "from" email address must be a VUI Administrator
        </p>
        From (login name - usually email address): <asp:TextBox ID="txtAdminFrom" runat="server" />
        <br />
        To: (login name - usually email address): <asp:TextBox ID="txtAdminTo" runat="server" />
        <br />
        <asp:Button runat="server" ID="btnChangeAdmin" OnClick="ChangeVUIAdmin" Text="Make change >" />


        <asp:Literal runat="server" ID="litUsers" />

    </p>
</div>

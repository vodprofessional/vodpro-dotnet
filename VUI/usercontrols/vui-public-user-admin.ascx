<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-public-user-admin.ascx.cs" Inherits="VUI.usercontrols.vui_public_user_admin" %>

<asp:PlaceHolder ID="plcUserAdmin" runat="server">

    <asp:PlaceHolder ID="plcMyDetails" runat="server">
      <div class="ui-admin-dialog">
        <h2>Your Details</h2>

        <div id="divDetailsMessage" runat="server" class="ui-admin-message">
            <asp:Literal ID="litDetailsMessage" runat="server" />
        </div>

        <div>
            <asp:Label ID="Label1" runat="server" AssociatedControlID="txtFirstName">Name</asp:Label>
            <asp:TextBox runat="server" ID="txtFirstName" Enabled="true" /><asp:TextBox runat="server" ID="txtLastName" Enabled="true" />
        </div>
        <div>
            <asp:Label ID="Label2" runat="server" AssociatedControlID="txtEmail">Email</asp:Label>
            <asp:TextBox runat="server" ID="txtEmail" Enabled="false" CssClass="ui-email" />
            <span class="ui-field-info">(to change your email address, please contact us)</span>
        </div>

        <div class="ui-buttons">
            <asp:Button runat="server" ID="btn" OnClick="UpdateDetails" Text="Save Changes" />
        </div>


        <h3>Change Password</h3>
        <div class="ui-change-password-form">
            <p>Your password should be between 6 and 20 characters and contain one or more numbers and non-alphanumeric characters (e.g. !, &, %)</p>
            <div>
                <asp:Label ID="Label4" runat="server" AssociatedControlID="txtPassword1">Password</asp:Label>
                <asp:TextBox runat="server" ID="txtPassword1" TextMode="Password" />
            </div>
            <div>
                <asp:Label ID="Label5" runat="server" AssociatedControlID="txtPassword2">Confirm</asp:Label>
                <asp:TextBox runat="server" ID="txtPassword2" TextMode="Password" />
                
            </div>
            <div class="ui-buttons"><asp:Button runat="server" ID="btnChangePassword" OnClick="ChangePassword" Text="Save Password" /></div>
            <div class="ui-admin-message" id="divPwdMessage" runat="server">
                <asp:Literal ID="litPwdErr" runat="server" />
            </div>
        </div>
        
        <h3>About your Account</h3>
        <asp:PlaceHolder ID="plcAdministratorDetails" runat="server">
            <p>Account administrator <asp:HyperLink ID="linkAdminName" runat="server" /></p>
        </asp:PlaceHolder>
        <div class="account">
            <label>Account type</label> <asp:TextBox runat="server" ID="txtAccountType" Enabled="false" />
        </div>
        <div class="account">
            <label class="account">Start Date</label>   <asp:TextBox runat="server" ID="txtStartDate" Enabled="false" />
        </div>

        <asp:PlaceHolder runat="server" id="plcInstallments">
            <div class="account">
                <label class="account">Next Payment</label>   <asp:TextBox runat="server" ID="txtInstallment" Enabled="false" />
                <asp:HiddenField runat="server" id="hidNextInstallment"></asp:HiddenField>
            </div>
            <div class="ui-buttons"><asp:Button runat="server" ID="btnPayInstallment" OnClick="BuyInstallment" Text="Save Password" /></div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcInvoices" runat="server">
        <div>
            <h4>Your Invoices</h4>
            <asp:Repeater ID="rptInvoice" runat="server" OnItemDataBound="Invoice_Bound">
                <HeaderTemplate><ul class="ui-invoice-list"></HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <asp:HyperLink ID="lnkInvoice" runat="server" CssClass="ui-invoice-link" />
                        <asp:Literal ID="litInvoice" runat="server" />
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </div>
        </asp:PlaceHolder>
        <!-- <div><label>VUI payment reference</label> <asp:TextBox runat="server" ID="txtPaymentRef" Enabled="false" /></div> -->
      </div>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="plcUsers" runat="server" Visible="false">

      <div class="ui-admin-dialog last">

        <h2>Your Users</h2>
        <p>You can add <asp:Literal ID="litMaxUsers" runat="server" /> users. Each user you create will be sent an email with a temporary password. They will be prompted to login and change this. If they forget their password, they can use the "Forgotten Password" functionality on the site.</p>
        <h3>Add User</h3>
        <div>
            <asp:Label runat="server" AssociatedControlID="txtVuiUserFirstname">Name</asp:Label>
            <asp:TextBox runat="server" ID="txtVuiUserFirstname" /><asp:TextBox runat="server" ID="txtVuiUserLastname" />
            <asp:Literal ID="errName" runat="server" Visible="false"/>
        </div>
        <div>
            <asp:Label ID="Label3" runat="server" AssociatedControlID="txtVuiEmail">Email</asp:Label>
            <asp:TextBox runat="server" ID="txtVuiEmail" CssClass="ui-email" />
            <asp:Literal ID="errEmail" runat="server" Visible="false"/>
        </div>
        <div class="ui-buttons">
            <asp:Button ID="btnAddVUIUser" runat="server" Text="Add User" OnClick="CreateVUIUser" />
            <asp:Literal ID="errGeneral" runat="server"  Visible="false" /> 
        </div>

        <asp:Repeater runat="server" ID="rptVUIusers" OnItemDataBound="VUIUser_Bound" OnItemCommand="VUIUsers_Command">
            <HeaderTemplate>
                <ol>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <asp:Literal runat="server" ID="litUserDetails" />
                    <asp:HiddenField runat="server" ID="hidUserId" />
                    <asp:Button ID="btnDeleteUser" runat="server" OnClientClick="return confirm('Are you sure you want to delete this user?');" CommandName="DeleteVUIUser" Text="Delete" CssClass="ui-button-admin-delete-user" />
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ol>
            </FooterTemplate>
        </asp:Repeater>

        
        </div>
    </asp:PlaceHolder>





</asp:PlaceHolder>
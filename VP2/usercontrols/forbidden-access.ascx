<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="forbidden-access.ascx.cs" Inherits="VP2.usercontrols.forbidden_access" %>

<%@ Register TagName="Login" TagPrefix="vp" Src="~/umbraco/plugins/vp/usercontrols/login.ascx" %>


<asp:PlaceHolder runat="server" ID="plcConfirmation">

    <p>
        We haven't been able to confirm your email address (<asp:Literal ID="litEmail" runat="server" />).  
        You should have received an email to complete the registration process with a confirmation link.
        Click the button below to have it sent again now.
    </p>
    
    <asp:Button runat="server" ID="btnResendConfirmation" Text="Resend Confirmation Email" OnClick="ResendConfirmationEmail" CssClass="btn btn-primary" />
    <asp:PlaceHolder ID="plcEmailSent" runat="server" Visible="false">
        <div>
            <p>
                We have sent a verification email to the address you provided (you may need 
                to check your Junk mail folder). Please click on the link in the email to complete your registration. 
            </p>
        </div>
    </asp:PlaceHolder>

</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="plcVUIRegistrationComplete">
    
    <p>Please could you complete your profile before getting access to the regstration content?</p>

</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="plcRegisterLogin">

    <a href="/register">Register or Login</a>

</asp:PlaceHolder>
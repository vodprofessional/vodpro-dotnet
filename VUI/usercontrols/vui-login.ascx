<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-login.ascx.cs" Inherits="VUI.usercontrols.vui_login" %>

<asp:PlaceHolder ID="plcVUILogin" runat="server" Visible="false">
    <div class="main-ui-dialog">
      <div class="vodform">
        <h2>Login</h2>

        <div class="entry">
            <asp:Label CssClass="ui-email" ID="Label1" AssociatedControlID="txtEmail" runat="server">Email Address</asp:Label><asp:TextBox runat="server" ID="txtEmail" CssClass="ui-email" />
        </div>
        <div class="entry">
            <asp:Label  ID="Label2" AssociatedControlID="txtPassword" runat="server">Password</asp:Label><asp:TextBox runat="server" TextMode="Password" ID="txtPassword" />
        </div>
        <div class="ui-buttons entry">
            <label></label>
            <asp:Button runat="server" ID="btnLogin" Text="Login" OnClick="btnLogin_Click" />
            <asp:LinkButton runat="server" ID="btnForgotPwd" OnClick="btnForgotPwd_Click" Text="Forgot password?" CssClass="forgot-pwd" />
        </div>
        <div class="ui-login-error">
            <asp:Literal runat="server" ID="litError" />
            
            <asp:PlaceHolder ID="plcVUIResetPassword" runat="server" Visible="false">
                <div class="main-ui-dialog password-recovery">
                    <h2>Recover Password</h2>
                    <p>We can email a new password to you if you need it. Just enter your email address below to make it so</p>
                    <div>
                        <asp:Label CssClass="ui-email" ID="Label3" AssociatedControlID="txtEmailRecover" runat="server">Email address</asp:Label><asp:TextBox runat="server" ID="txtEmailRecover" CssClass="ui-email" />
                    </div>
                    <div class="ui-buttons">
                        <asp:Button runat="server" ID="btnRecoverPassword" Text="Send it!" OnClick="btnRecoverPassword_Click" />
                    </div>
                    <div class="ui-login-error" runat="server" visible="false" id="divPwdRecoveryError">
                        Please enter a valid email address and we'll send you a new password post-haste!
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcVUIResetPasswordMessage" runat="server" Visible="false">
                <div class="main-ui-dialog password-recovery">
                    <h2>Recover Password</h2>
                    <div>
                        We have generated a new password for you and sent it to your email address (If you can't see the email in your inbox, check your spam folders too!).  
                        You should change your password to something more memorable when you've logged in.
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
      </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plcViewDetails" runat="server" Visible="false">
  <div class="ui-login">
    <asp:PlaceHolder ID="plcLoggedIn" runat="server" Visible="false">
        You are logged in as <asp:Literal ID="litUser" runat="server" />
        <asp:Button ID="lnkSubscribe" runat="server" OnClick="Subscribe_Click" Text="Subscribe" Visible="false" />
        <asp:Button ID="lnkLogout" runat="server" OnClick="Logout_Click" Text="Logout" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcLoggedOut" runat="server" Visible="false">
        <asp:Button ID="lnkLogin" runat="server" OnClick="Login_Click" Text="Login"/>
    </asp:PlaceHolder>
  </div>
</asp:PlaceHolder>
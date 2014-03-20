<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="login.ascx.cs" Inherits="VP2.usercontrols.login" %>

<ul class="nav nav-tabs">
    <li class="active"><a href="#signin" data-toggle="tab">Sign In</a></li>
    <li><a href="#register" data-toggle="tab">Register</a></li>
</ul>

<!-- Tab panes -->
<div class="tab-content">
    <div class="tab-pane active" id="signin">
      <asp:PlaceHolder runat="server" ID="plcFormLogin">
        <div class="form-signin form" role="form" id="signin-form-main">
          <div class="entry">
            <asp:TextBox runat="server" ID="txtLoginEmail" CssClass="form-control" />
          </div>
          <div class="entry">
            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" />
            <asp:PlaceHolder runat="server" ID="plcErrLogin">
                <div class="form-error" id="regwall-email-error">There was an error logging you in</div>
            </asp:PlaceHolder>
            <!--<label class="checkbox">
                <input type="checkbox" value="remember-me"> Remember me
            </label>-->
          </div>
          <asp:Button runat="server" ID="btnLogin" OnClick="btnLogin_Click" CssClass="btn btn-lg btn-primary btn-block" Text="Sign in" />
          <div id="forgotten-pwd-link">
            <asp:LinkButton runat="server" ID="lnkForgotPwd" OnClick="lnkForgotPwd_Click" Text="Forgotten your password?" />
          </div>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder runat="server" ID="plcFormForgot">
        <div id="forgotten-pwd" class="form-signin form">
            <div class="entry">
                <asp:TextBox runat="server" ID="txtEmailForgotten" CssClass="form-control" />
                <asp:PlaceHolder runat="server" ID="plcForgotError">
                    <div class="form-error">Please enter your email</div>
                </asp:PlaceHolder>
            </div>
            <asp:Button runat="server" ID="btnForgotPwd" OnClick="btnForgotPwd_Click" CssClass="btn btn-lg btn-primary btn-block" Text="Send me a new password!" />
            <asp:PlaceHolder runat="server" ID="plcPwdSent">
                <div class="form-error">Your new password has been sent</div>
            </asp:PlaceHolder>
            <div id="forgotten-pwd-cancel-link">
                <asp:LinkButton runat="server" ID="lnkBackToLogin" OnClick="lnkBackToLogin_Click" Text="Back to login form" />
            </div>
        </div>
      </asp:PlaceHolder>
    </div>
    <div class="tab-pane" id="register">
        <div class="form-signin form" role="form" action="/register" method="post">
            <div class="entry">
                <asp:TextBox runat="server" ID="txtRegEmail" CssClass="form-control" />
            </div>
            <asp:Button runat="server" ID="btnReg" OnClick="btnReg_Click" CssClass="btn btn-lg btn-primary btn-block" Text="Start Registration" />
        </div>
    </div>
</div>
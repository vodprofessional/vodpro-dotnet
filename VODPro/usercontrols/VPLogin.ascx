<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VPLogin.ascx.cs" Inherits="VODPro.usercontrols.VPLogin" %>

<asp:PlaceHolder ID="plcLoginForm" runat="server">
    <div id="LoginForm">
        <div>
            <asp:Label AssociatedControlID="VPUserName" Text="Email address" runat="server" />
            <asp:TextBox runat="server" ID="VPUserName" />
        </div>
        <div>
            <asp:Label AssociatedControlID="VPPassword" Text="Password" runat="server" />
            <asp:TextBox runat="server" TextMode="Password" ID="VPPassword" />
        </div>
        <asp:PlaceHolder runat="server" ID="plcErrorMessage">
            <div>
                <asp:Literal runat="server" ID="Err" />
            </div>
        </asp:PlaceHolder>
        <div>
            <asp:Button runat="server" ID="LoginSubmit" Text="Login" OnClick="Login" /> 
        </div>
    </div>
</asp:PlaceHolder>
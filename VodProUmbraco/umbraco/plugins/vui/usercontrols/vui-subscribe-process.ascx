<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-subscribe-process.ascx.cs" Inherits="VUI.usercontrols.vui_subscribe_process" %>


<asp:PlaceHolder ID="PayPalForm" runat="server" Visible="false">
    <!doctype html>
    <html>
     <head>
      <title>VOD Professional - Redirecting to PayPal</title>
      <style type="text/css">
        html, body { width:100%; height:100%; }
        div { margin:auto; width:300px; height:100%; }    
        a {
            background: url("/vui-media/media/vui-loaderb64.gif") no-repeat scroll center top transparent;
            color: #999999;
            font-family: sans-serif;
            padding-top: 130px;
            position: absolute;
            text-align: center;
            text-decoration: none;
            top: 35%;
            width: 300px;
        }
      </style>
     </head>
     <body>

        <asp:Literal runat="server" ID="litPayPalForm" />
        <div>
            <a href="#" class="submit">Redirecting to PayPal in a matter of seconds...</a>
        </div>
      </body>
        <script src="/Scripts/jquery-1.7.1.min.js"></script>
      <script type="text/javascript">
          $(function () {
              setTimeout(function () {
                  $("#paypalform").submit();
              }, 10);

              $(".submit").onclick(function () {
                  $("#paypalform").submit();
              });
          });
      </script>
    </html>
</asp:PlaceHolder>
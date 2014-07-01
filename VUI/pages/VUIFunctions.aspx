<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VUIFunctions.aspx.cs" Inherits="VUI.pages.VUIFunctions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="/_inc/css/vui-admin-styles.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="serviceContainer" style="overflow:auto;">
        <h1>VUI Functions</h1>

        <h2>MetaData Processing Queue</h2>

        <p>
            <asp:ListBox ID="lstServices" runat="server" Rows="1" />
            <asp:Button runat="server" ID="btnRegenerateServiceMetadata" Text="Add to Queue" OnClick="RegenerateService" />
            &nbsp;
            <asp:Button runat="server" ID="btnRegenerateAll" Text="Add ALL Services to Queue" OnClick="RegenerateAll" />
        </p>

        <p>
            <asp:Button runat="server" ID="btnClearAndRegenerateAll" Text="Clear ALL and Add ALL to Queue (WARNING, only use out-of-hours" OnClick="ClearAndRegenerateAll" />
            <br />
            <asp:Literal runat="server" ID="litMessage" />
        </p>
        <p>
            <asp:Button runat="server" ID="btnStartProcessing" Text="Start / Refresh Queue Processor" OnClick="StartQueue" />
            <asp:Button runat="server" ID="btnEndProcessing" Text="Abort Thread (are you sure?)" OnClick="EndQueue" />
            <br />
            <asp:Literal runat="server" ID="litQueueRunningCounter" />
        </p>
        <hr />
        <h2>VUI Users</h2>
        <asp:Literal ID="litVUIUsers" runat="server" />

    </div>
    </form>
        <script type="text/javascript">
            window.jQuery || document.write("<script src='//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js'>\x3C/script>")
    </script>
    <!-- <script src="/_inc/js/jquery-ui-1.10.3.custom.min.js"></script> -->
    <script src="/_inc/js/jquery.tools.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {

            console.log("In service page");
            $('#serviceContainer').height($(window).height());

        });
    </script>
</body>
</html>

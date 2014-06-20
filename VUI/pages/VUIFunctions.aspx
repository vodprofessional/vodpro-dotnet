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
        <h1>VUI Functions Coming Soon</h1>


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

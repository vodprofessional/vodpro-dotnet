<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VODPro.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="scripts/scripts.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Username: <input type="text" name="username" id="SpecialForm_username" />
        <br />
        Pasword: <input type="password" name="password" id="SpecialForm_password" />
        <br />
        <input type="submit" name="submit" value="Login" id="SpecialForm_btnSubmit" onclick="return Login(this)"/>
    </div>
    </form>
</body>
</html>

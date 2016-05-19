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

        <input type="button" id="openMetaDataWindow" value="Open Metadata Publishing Window" />

        <h2>Update Service Master Name</h2>
        Service: <asp:ListBox ID="lstServices" runat="server" Rows="1" /> 
        New Service Name:<asp:TextBox ID="txtNewServiceName" runat="server" />
        <asp:Button runat="server" ID="btnUpdateServiceMasterName" OnClick="UpdateServiceMasterName" Text="Commit Changes" />

        <h2>VUI Users</h2>
        <asp:Literal ID="litVUIUsers" runat="server" />

        <hr />

        <h2>All Users</h2>
        <asp:Button runat="server" ID="btnExportUsers" OnClick="btnExportMembers_Click" Text="Export Members" />
        <asp:Repeater ID="rptUsers" OnItemDataBound="User_DataBound" runat="server">
            <HeaderTemplate>
                <table>
                    <tr>
                        <th>ID </th>
                        <th>Email </th>
                        <th>Title </th>
                        <th>First </th>
                        <th>Surname </th>
                        <th>Fullname </th>
                        <th>Job title </th>
                        <th>Organisation </th>
                        <th>Organisation type</th>
                        <th>Number of employees </th>
                        <th>Country </th>
                        <th>Address 1 </th>
                        <th>Address 2 </th>
                        <th>City </th>
                        <th>State </th>
                        <th>Post code </th>
                        <th>Phone </th>
                        <th>Mobile </th>
                        <th>Web site registrant? </th>
                        <th>Newsletter?</th>
                        <th>VUI user </th>
                        <th>Date </th>
                    </tr>
            </HeaderTemplate>

            <ItemTemplate>
                    <tr>
                        <td><asp:Literal ID="litID" runat="server" /></td>
                        <td><asp:Literal ID="litEmail" runat="server" /> </td>
                        <td><asp:Literal ID="litTitle" runat="server" /> </td>
                        <td><asp:Literal ID="litFirstName" runat="server" /></td>
                        <td><asp:Literal ID="litSurname" runat="server" /> </td>
                        <td><asp:Literal ID="litFullname" runat="server" /> </td>
                        <td><asp:Literal ID="litJobTitle" runat="server" /> </td>
                        <td><asp:Literal ID="litOrganisation" runat="server" /> </td>
                        <td><asp:Literal ID="litOrganisationType" runat="server" /> </td>
                        <td><asp:Literal ID="litEmplyees" runat="server" /> </td>
                        <td><asp:Literal ID="litCountry" runat="server" /> </td>
                        <td><asp:Literal ID="litAddress1" runat="server" /> </td>
                        <td><asp:Literal ID="litAddress2" runat="server" /> </td>
                        <td><asp:Literal ID="litCity" runat="server" /> </td>
                        <td><asp:Literal ID="litState" runat="server" /> </td>
                        <td><asp:Literal ID="litPostcode" runat="server" /> </td>
                        <td><asp:Literal ID="litPhone" runat="server" /> </td>
                        <td><asp:Literal ID="litMobile" runat="server" /> </td>
                        <td><asp:Literal ID="litRegistrant" runat="server" /> </td>
                        <td><asp:Literal ID="litNewsletter" runat="server" /> </td>
                        <td><asp:Literal ID="litVui" runat="server" /> </td>
                        <td><asp:Literal ID="litDate" runat="server" /> </td>

                    </tr>
            </ItemTemplate>
            <FooterTemplate>                
                </table>
            </FooterTemplate>

        </asp:Repeater>
        <hr/>

        <h2>2016 Scoring Fuctions</h2>
        <p>
            1. Set all analyses to "Pre-2016 Scoring"
            <asp:Button runat="server" ID="btnSetAnalyses2016" OnClick="btnSetAnalyses2016_Click" Text="Run Once Only!" />
        </p>
        <p>
            2. Copy over old scores to new field
            <asp:Button runat="server" ID="Button1" OnClick="btnSetAnalysesScores2016_Click" Text="Run Once Only!" />
        </p>

        <h2>Tidy up - delete child nodes (DANGER)</h2>
        Enter ID, all children will be deleted, leaving original <asp:TextBox runat="server" ID="txtParentId" />
        <asp:Button runat="server" ID="btnDeleteChildren" OnClick="DeleteChildren" Text="Delete Children" />
        <asp:Literal runat="server" ID="litDeleteChildren" />


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

            $("#openMetaDataWindow").on('click', function (e) {
                window.open('VUIMetaDataPopup.aspx', 'metadata', 'width=600,height=400,scrollbars=yes').focus();
            });

        });
    </script>
</body>
</html>

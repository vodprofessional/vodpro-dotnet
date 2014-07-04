<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VUIMetaDataPopup.aspx.cs" Inherits="VUI.pages.VUIMetaDataPopup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VUI MetaData Popup</title>
    <link rel="Stylesheet" type="text/css" href="/_inc/css/vui-admin-styles.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>VUI MetaData Publishing</h1>

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

    <a name="queue"></a>
    <h2>The Queue</h2>

    <asp:Literal runat="server" ID="litQueueComing" />
    <p>
        <input type="button" id="button-process" value="Process MetaData Queue" />
        <div id="processing" style="display:none">
            <h2>Do Not Close This Window!</h2>
            <img src="/_inc/images/ajax-loader-big.gif" />
        </div>
    </p>
    
    <asp:Literal runat="server" ID="litQueuePast" />

    </div>
    </form>  
    <style type="text/css">
    tr.processed 
    {
        text-decoration: line-through;
        background-color: #ffbbbb;
    }
    </style>      
    <script type="text/javascript">
        window.jQuery || document.write("<script src='//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js'>\x3C/script>")
    </script>
    <!-- <script src="/_inc/js/jquery-ui-1.10.3.custom.min.js"></script> -->
    <script src="/_inc/js/jquery.tools.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            var continueProcessing;
            $('#button-process').removeAttr("disabled");

            $('#button-process').on('click', function (e) {
                window.location.hash = 'queue';
                continueProcessing = true;
                $("#processing").show();
                $('#button-process').attr("disabled", "disabled");
                ProcessQueue();
            });


            function ProcessQueueItem() {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                $.ajax({
                    url: xmlActionsURL + "?a=processq",
                    type: "POST",
                    dataType: 'json',
                    success: function (data) {
                        console.log(data.response);
                        if (data.response == 'valid') {
                            console.log("Processed item [" + data.data.lastitem + "] off queue. Continue? [" + data.data.keeprunning + "]");

                            $("#item-" + data.data.lastitem).addClass("processed");

                            if (data.data.keeprunning === 'True') {
                            }
                            else {
                                continueProcessing = false;
                            }
                        }
                        else {
                            console.log("Got an error");
                        }
                        ProcessQueue();
                    },
                    error: function () {
                        console.log("Got an AJAX error");
                        continueProcessing = false;
                        ProcessQueue();
                    }
                })
            }

            function ProcessQueue() {
                console.log("Processing Queue");
                if (continueProcessing) {
                    setTimeout(function () { ProcessQueueItem(); }, 1000);
                }
                else {
                    $("#processing").hide();
                    console.log(window.location.href.replace('#' + window.location.hash, ''));
                    window.location = window.location.href.replace(window.location.hash, '').split('?')[0] + '?r=' + Math.random() + window.location.hash;
                }
            }
        });
    </script>
</body>
</html>

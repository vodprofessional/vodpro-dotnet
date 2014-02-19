<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VUIService.aspx.cs" Inherits="VUI.pages.VUIService"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="/_inc/css/vui-admin-styles.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="serviceContainer" style="overflow:auto;">
        <h1><asp:Literal ID="litTitle" runat="server"></asp:Literal>  (<asp:Literal ID="litServiceMasterId" runat="server"/>)</h1>
        <p>
            <a href="" id="lnkEditService" runat="server">Edit Service Master Details</a>
        </p>
        <strong>Status: <asp:Literal ID="litStatus" runat="server"></asp:Literal></strong>

        <h2>Description</h2>
        <p>
            <asp:Literal ID="litDescription" runat="server"></asp:Literal>
        </p>

        <table border="0">
            <tr>
                <th>Availability</th>
                <td><asp:Literal ID="litAvailability" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Pay Model</th>
                <td><asp:Literal ID="litSubscriptionType" runat="server"></asp:Literal></td>
            </tr>
            <tr class="last">
                <th>Category</th>
                <td><asp:Literal ID="litServiceCategory" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Twitter URL</th>
                <td><asp:Literal ID="litTwitterURL" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>YouTube URL</th>
                <td><asp:Literal ID="litYouTubeURL" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Facebook URL</th>
                <td><asp:Literal ID="litFacebookURL" runat="server"></asp:Literal></td>
            </tr>

        </table>

        <hr />
        <h2>More Stuff to Do with Screenshots and Benchmarks here</h2>
        <p>coming soon...</p>
        <hr />
        <h2>AppStore Stuff</h2>
        
        <div class="b-tabs store-comments">
            <ul class="tabs">
                <li><a href="#" data-devicetype="Tablet/iPad">iPad</a></li>
                <li><a href="#" data-devicetype="Smartphone/iPhone">iPhone</a></li>
                <li><a href="#" data-devicetype="Tablet/Android">Android Tab</a></li>
                <li><a href="#" data-devicetype="Smartphone/Android">Android Phone</a></li>
            </ul>
            <div class="panes">
                <div class="pane">
                    <div data-devicetype="Tablet/iPad" data-store="iTunes" data-startpos="0">
                        <h2>iPad <span class="app-link"><asp:Literal ID="litiPadURL" runat="server"></asp:Literal></span></h2>
                        <div class="version-history"><h3>Version History</h3></div>
                        <span class="comments-list"></span>
                        <input type="button" class="btn-comments" value="Get Some Comments" />
                    </div>
                </div>
                <div class="pane">
                    <div data-devicetype="Smartphone/iPhone" data-store="iTunes" data-startpos="0">
                        <h2>iPhone <span class="app-link"><asp:Literal ID="litiPhoneURL" runat="server"></asp:Literal></span></h2>
                        <div class="version-history"><h3>Version History</h3></div>
                        <span class="comments-list"></span>
                        <input type="button" class="btn-comments" value="Get Some Comments" />
                    </div>
                </div>
                <div class="pane">
                    <div data-devicetype="Tablet/Android" data-store="Google Play" data-startpos="0">
                        <h2>Android Tab <span class="app-link"><asp:Literal ID="litPlayTablet" runat="server"></asp:Literal></span></h2>
                        <div class="version-history"><h3>Version History</h3></div>
                        <span class="comments-list"></span>
                        <input type="button" class="btn-comments" value="Get Some Comments" />
                    </div>
                </div>
                <div class="pane">
                    <div data-devicetype="Smartphone/Android" data-store="Google Play" data-startpos="0">
                        <h2>Android Phone <span class="app-link"><asp:Literal ID="litPlayPhone" runat="server"></asp:Literal></span></h2>
                        <div class="version-history"><h3>Version History</h3></div>
                        <span class="comments-list"></span>
                        <input type="button" class="btn-comments" value="Get Some Comments" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="templates" style="display:none">
        <div id="VUI-Comment-template">
            <div class="comment">
                <div class="title"></div>
                <div class="rating"></div>
                <div class="version"></div>
                <div class="desc"></div>
                <div class="reviewdate"></div>
                <div class="daterecorded"></div>
            </div>
        </div>

        <div id="VUI-Versions-template">
            <div class="version">
                <span class="number"></span>
                <span class="date"></span>
            </div>
        </div>
    </div>




    <script type="text/javascript">
        window.jQuery || document.write("<script src='//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js'>\x3C/script>")
    </script>
    <!-- <script src="/_inc/js/jquery-ui-1.10.3.custom.min.js"></script> -->
    <script src="/_inc/js/jquery.tools.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {

            console.log("In service page");
            $('#serviceContainer').height($(window).height());
            
            if ($('.b-tabs.store-comments').length > 0) {
                var servicemasterid = $('#servicemasterid').attr('data-id');
                LoadCommentsMeta($('.b-tabs.store-comments'), servicemasterid);
            }

            $(window).on('resize', function () {
                $('#serviceContainer').height($(window).height() - 50);
            });

            $(".btn-comments").on('click', function () {
                var target = $(this).parent();
                var servicemasterid = $('#servicemasterid').attr('data-id');
                var store = target.attr('data-store');
                var deviceType = target.attr('data-devicetype');
                var startpos = target.attr('data-startpos');
                LoadComments(target, servicemasterid, store, deviceType, startpos);
            });

        });


        function LoadCommentsMeta(target, servicemasterid) {
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = {  "servicemasterid": servicemasterid };
            $.ajax({
                url: xmlActionsURL + "?a=scmt",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    if (data.data.CommentsMeta.length > 0) {
                        for (i = 0; i < data.data.CommentsMeta.length; i++) {
                            var link = target.find('ul li a[data-devicetype="' + data.data.CommentsMeta[i].DeviceType + '"]');
                            var pane = target.find('.panes .pane>div[data-devicetype="' + data.data.CommentsMeta[i].DeviceType + '"]');
                            if (data.data.CommentsMeta[i].Count == 0) {
                                link.parent().remove();
                                pane.parent().remove();
                            }
                            else {
                                link.text(link.text() + ' (' + data.data.CommentsMeta[i].Count + ')');
                            }
                        }
                    }

                    target.find('.panes .pane>div').each(function () {
                        LoadVersionHistory($(this), servicemasterid, $(this).attr("data-devicetype"));
                    });

                    $('.b-tabs.store-comments').find(".tabs").tabs("div.panes > div", {
                        initialIndex: 0,
                        onClick: function () {
                            this.getCurrentPane().find('.btn-comments').trigger('click');
                        }
                    });
                }
            });
        }
        

        function LoadVersionHistory(target, servicemasterid, deviceType) {
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = {  "servicemasterid": servicemasterid, "deviceType": deviceType };
            $.ajax({
                url: xmlActionsURL + "?a=vers",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    if (data.data.Versions.length > 0) {
                        for (i = 0; i < data.data.Versions.length; i++) {
                            var result = $("#VUI-Versions-template div").clone();
                            result
                                .find('span.number').html(data.data.Versions[i].Version).end()
                                .find('span.date').html(data.data.Versions[i].DateRecordedString).end();
                            target.find('.version-history').append(result);
                        }
                    }
                }
            });
        }


        function LoadComments(target, servicemasterid, store, deviceType, startpos) {
            var numrows = 50;
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var pagetype = '';
            var DataObject = { "startpos": startpos, "numrows": numrows, "servicemasterid": servicemasterid, "store": store, "deviceType": deviceType };
            $.ajax({
                url: xmlActionsURL + "?a=scom",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    if (data.data.Comments.length > 0) {
                        for (i = 0; i < data.data.Comments.length; i++) {
                            var result = $("#VUI-Comment-template div").clone();
                            result
                                .find('div.title').html(data.data.Comments[i].Title).end()
                                .find('div.desc').html(data.data.Comments[i].Comment).end()
                                .find('div.rating').html('Rating: ' + data.data.Comments[i].NormalisedRating).end()
                                .find('div.reviewdate').html(data.data.Comments[i].ReviewDateString).end()
                                .find('div.daterecorded').html(data.data.Comments[i].DateRecorded).end()
                            ;
                            if(data.data.Comments[i].Version != null) {
                                result.find('div.version').html('Version: ' + data.data.Comments[i].Version).end();
                            }
                            else {
                                result.find('div.version').html('');
                            }

                            target.find('.comments-list').append(result);
                            $('#serviceContainer').height($(window).height() - 50);
                        }
                        var n = parseInt(numrows) + parseInt(startpos)
                        target.attr('data-startpos', n);
                    }
                    else {
                        target.find('.btn-comments').attr('disabled', 'disabled');
                    }
                }
            });
        }
    </script>
    </form>
</body>
</html>

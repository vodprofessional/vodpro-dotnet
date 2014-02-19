<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-importer.ascx.cs" Inherits="VUI.usercontrols.vui_importer" %>
<style>
 
 .orgItems { float:left; min-height: 500px; overflow:visible;}
 .listServices {  }
 .pageTypes { position: fixed; width: 200px; z-index: 9; right: 47px; top:70px; background-color:#fff; }
 .listImages { float:left; margin-right: 210px; }
 .listImages ul { list-style: none; margin: 0;}
 .listImages ul li { list-style: none; display: inline; margin: 10px; height: } 
 .listImages ul li.images-title { width: 100%; font-weight: bold; margin-top: 10px; border-top: 1px solid #aaa; padding-top: 5px; display: block; }
 
 .listImages ul li img.ui-img-small { opacity:0.3; filter:alpha(opacity=30); }
 .listImages ul li.ui-hidden { display: none; }
 .listImages ul li.ui-hidden img { display: none; }
 .ui-drop-on   { background-color: Lime }
 .img-data { display:none; }
 .count { font-weight: bold; }
 
 .pageTypes ul { list-style: none; margin: 0px; padding: 0px; }
 .pageTypes ul li { list-style: none; margin: 2px; padding: 5px; border: 1px solid #aaa; }
 .pageTypes ul li.ui-bin { background-color: #ddd; }
 .pageTypes ul li.ui-bin.ui-drop-on { background-color: #aaa; }
 .pageTypes ul li .empty-bin { float:right; color: #f00; }
 
 .listImages ul.ui-service-capability li { float:left; width: 180px; margin: 0 0 3px 0;}
 
 input.data { display:none; }
</style>

<script type="text/javascript" src="/Scripts/jquery-ui-1.8.22.custom.min.js"></script>
<script src="/Scripts/jquery.lazyload.min.js" type="text/javascript"></script>

<script type="text/javascript">
    $(function () {
        var im = '';

        $("img.lazy").lazyload({
            container: $(".listImages")
        });

        $(".tabpagescrollinglayer").scroll(function () {
            $(".listImages").trigger("resize");
        });

        $(".img-draggable").draggable({
            start: function (event, ui) {
                im = $(this).attr("data-id");
                $(this).addClass("ui-img-small");
            },
            stop: function (event, ui) {
                $(this).removeClass("ui-img-small");
                $(".listImages").trigger("resize");
            }
        });

        $(".droppable").droppable({
            over: function (event, ui) {
                $(this).addClass("ui-drop-on");
            },
            out: function (event, ui) {
                $(this).removeClass("ui-drop-on");
            },
            drop: function (event, ui) {
                var target = $(this).find("span.img-data");
                var html = target.html() + ";" + im;
                target.html(html)
                ui.draggable.parent().addClass("ui-hidden");
                console.log(ui.draggable.parent());
                var count = parseInt($(this).find(".count").html());
                count += 1;
                $(this).find(".count").html(count);
                $(this).removeClass("ui-drop-on");
                $(".listImages").trigger("resize");
            }
        });

        $(".empty-bin").click(function () {
            var binData = $(this).parent().find(".img-data").html().split(';');
            console.log(binData);

            for (var i = 0; i < binData.length; i++) {
                console.log(" - restoring: " + binData[i]);
                var img = $(".listImages li img[data-id='" + binData[i] + "']");
                console.log(img);
                $(img).parent().removeClass("ui-hidden");
                $(img).attr("style","position:relative");
            }

            $(this).parent().find(".img-data").html("");
            $(this).parent().find(".count").html("0");
        });
    });

    function UpdateData() {
        var val = '';
        $(".ui-pagetype .img-data").each(function (index) {
            val += $(this).attr("data-id") + $(this).html() + "/";
        });
        $("input.data-pagetype").val(val);

        val = '';
        $(".ui-bin .img-data").each(function (index) {
            val += $(this).attr("data-id") + $(this).html() + "/";
        });
        $("input.data-bin").val(val);


        val = '';
        $(".ui-service-capability").each(function (index) {
            var serviceid = $(this).attr("data-serviceid");
            var servicecaps = [];
            $(this).find("input:checked").each(function (index2) {
                servicecaps.push($(this).val());
            });
            if (servicecaps.length > 0) {
                val += serviceid + ":" + servicecaps.join(',') + ";";
            }
        });
        $("input.data-capabilities").val(val);

        return true;
    }

    function ClearData() {
        $(".droppable").each(function (index) {
            $(this).find(".img-data").html("");
            $(this).find(".count").html("0");
        });
        $("input.data").val("");
        $(".listImages li").each(function (index) {
            $(this).removeClass("ui-hidden");
            $(this).find("img").attr("style","position:relative");
        });
    }
</script>

<div class="dashboardWrapper">
    <h2>VUI Admin</h2>

    <p class="guiDialogNormal">

        <!--<asp:Button runat="server" ID="btnMigrate" Text="Migrate to V2" OnClick="MigrateToVui2" />-->
        <!--<asp:Button runat="server" ID="btnGenerateServiceMasters" Text="Generate Service Masters (Once Only)" OnClick="GenerateServiceMasterItems" /> -->

        <asp:Button ID="btnRegenerateVUIMetaData" runat="server" Text="Regenerate VUI 3 MetaData (Don't leave this page while running!)" OnClick="RegenerateVUIMetaData" />
        <asp:Literal runat="server" ID="lblMeta" Visible="false"><strong>VUI 3 Metadata Regenerated</strong></asp:Literal>
        <asp:Literal runat="server" ID="lblMetaError" Visible="false"></asp:Literal>

        <!--<asp:Button runat="server" ID="Button2" Text="Hide Hot Features" OnClick="SetAllHideHotFeatures" />-->
        <!--<asp:Button runat="server" ID="btnImportURLs" Text="Import App Store URLs" OnClick="UpdateServiceAppStoreURLs" />-->

    </p>

    <p class="guiDialogNormal">
        Import Tag (e.g. 1) <asp:TextBox ID="ImportTag" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Import VUI Images" OnClick="Import" /> <asp:Button ID="ButtonClearDuplicateImages" Text="Clear Duplicate Images From Import Tag" OnClick="ClearImportDuplicateImages" runat="server" />
        <asp:Literal ID="Lit1" runat="server"></asp:Literal>
        <asp:TextBox ID="txtImportMessages" runat="server" TextMode="MultiLine" Visible="false" Width="800px" Rows="10" />
        
        <!--
        <asp:DropDownList runat="server" ID="ServiceList"></asp:DropDownList>
        <asp:Button ID="btnShowUncategorisedImages" runat="server" OnClick="btnShowNoTypePages_Click" Text="Load Uncategorised Images" />
        -->


        <asp:Button ID="btnRegnerateAllServiceScores" runat="server" OnClick="btnRegnerateServiceScores_Click" Text="Regenerate All Service Scores" Visible="false" />


        <asp:Panel ID="pnlBD" runat="server" Visible="false">
            Parent Node ID:<asp:TextBox ID="txtServiceParent" runat="server" /> Device:<asp:DropDownList ID="ddDevice" runat="server" /> Date:<asp:Calendar runat="server" ID="calDate"  /> <asp:Button ID="btnUpdateBenchmarkDetail" runat="server" OnClick="UpdateBenchmarkDetail" Text="Update Benchmark Detail" />
        </asp:Panel>

        

        <asp:Button ID="btnClearImages" runat="server" OnClick="ClearAllPageTypes" Text="Clear Image Page Types" Visible="false" />

        <asp:Button ID="btnUpdateSocialURLs" runat="server" OnClick="UpdateServiceSocialURLs" Text="Update Social URLs Once Only!" Visible="false"/>
        <asp:Literal runat="server" ID="litSocialURLs" />

    </p>
    <p>
    
        Work PackageId <asp:TextBox runat="server" ID="txtWorkpackageid" />
        <asp:Button ID="btnImportTest" runat="server" OnClick="ImportAnalysesFromDB" Text="Import Analyses From DB" />

        <asp:Button ID="btnClearWPImages" runat="server" OnClick="ClearWorkPackageImages" Text="Clear Images From Work Package"/>

        
    </p>

    <asp:Panel ID="pnlOrgItems" runat="server" Visible="false">

        <div class="orgItems">

            <div class="listServices">
                <ul>
                    <asp:PlaceHolder ID="listServices" runat="server"><asp:Literal ID="litOUT" runat="server" Text="" /></asp:PlaceHolder>
                </ul>
            </div>

            <div class="listImages">
                <asp:Repeater ID="rptImageList" runat="server" OnItemDataBound="ImageList_Bound">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal ID="litImage" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <!--
                    <asp:PlaceHolder ID="listImages" runat="server">
                    
                    <ul id="imageListUL" runat="server">
                        <li><img src="/vui/media/th/July-2012-Tablet-iPad-Eurosport-i_2012071021594613.jpg" data-id="2852" class="img-draggable" /></li>
                
                        <li><img src="/vui/media/th/July-2012-Tablet-iPad-Eurosport-i_2012071021594620.jpg" data-id="2853" class="img-draggable" /></li>
                
                        <li><img src="/vui/media/th/July-2012-Tablet-iPad-Eurosport-i_2012071021594624.jpg" data-id="2854" class="img-draggable" /></li>
                
                        <li><img src="/vui/media/th/July-2012-Tablet-iPad-Eurosport-i_2012071021594628.jpg" data-id="2855" class="img-draggable" /></li>
                    </ul>
                    </asp:PlaceHolder>
                -->
            </div>

            <div class="pageTypes">
                <ul>
                    <asp:Literal ID="litPageTypes" runat="server" Text="" />

                    <li class="ui-bin droppable">Rubbish (<span class="count">0</span>)<span class="img-data" data-id="DELETE"></span><a href="#" class="empty-bin">Empty</a></li>

                    <li><asp:TextBox ID="PageTypes" runat="server" CssClass="data data-pagetype"></asp:TextBox>
                        <asp:TextBox ID="Bin" runat="server" CssClass="data data-bin"></asp:TextBox>
                        <asp:TextBox ID="Capabilities" runat="server" CssClass="data data-capabilities"></asp:TextBox>
                        <asp:Button OnClientClick="return UpdateData();" runat="server" ID="UpdatePageTypes" Text="Save" OnClick="UpdateData" />
                        <input type="button" onclick="ClearData()" value="Clear" />
                        </li>
                    <%-- <li data-id="61" class="droppable">Homepage (<span class="count">0</span>)<span class="img-data" data-id="61"></span></li> --%>
                </ul>
            </div>

        </div>
    </asp:Panel>

</div>
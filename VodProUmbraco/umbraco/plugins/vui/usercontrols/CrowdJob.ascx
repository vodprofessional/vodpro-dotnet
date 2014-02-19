<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CrowdJob.ascx.cs" Inherits="VUI.usercontrols.CrowdJob" %>
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>

<asp:UpdatePanel ID="updatePanelUpload" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div>
            <asp:Label ID="lblImage" runat="server" AssociatedControlID="uplImage">The Image to Upload</asp:Label>
            <asp:FileUpload ID="uplImage" runat="server" />
        </div>
        <div>
            <asp:Label ID="lblFeature" runat="server" AssociatedControlID="ddFeature">What Feature does this illustrate</asp:Label>
            <asp:DropDownList ID="ddFeature" runat="server" />
        </div>
        <div>
            <asp:Button ID="btnSubmit" runat="server" Text="Upload the image!" OnClick="btnUpload_Click" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<!-- List of images uploaded -->
<asp:UpdatePanel ID="updatePanelImages" runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
    </Triggers>
    <ContentTemplate>
        <asp:Repeater ID="rptImages" runat="server" OnItemDataBound="CrowdJobImages_OnBound">
            <HeaderTemplate>
                <table id="crowd-job-images">
                    <tr>
                        <th>Image</th>
                        <th>Feature</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>
                        <td><asp:Literal ID="litImage" runat="server" /></td>
                        <td><asp:Literal ID="litImageFeature" runat="server" /></td>
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
</asp:UpdatePanel>



<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="contact-request.ascx.cs" Inherits="VP2.usercontrols.contact_request" %>


<asp:PlaceHolder id="plcContactReqForm" runat="server">
<div class="form form-contact-request">
    <div class="entry">
      <asp:Label ID="Label1" runat="server" AssociatedControlID="txtName" Text="Your name" />
      <asp:TextBox runat="server" ID="txtName" CssClass="form-control" autofocus />
      <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtName"
        ErrorMessage="Your name is a mandatory field." ValidationGroup="contactForm" Cssclass="error" />
    </div>
    
    <div class="entry">
      <asp:Label ID="Label2" runat="server" AssociatedControlID="txtEmail" Text="Your email address" />
      <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" />
      <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtEmail"
        ErrorMessage="Your email address is a mandatory field." ValidationGroup="contactForm" Cssclass="error" />
      <asp:RegularExpressionValidator
        id="regEmail" ValidationGroup="contactForm"
        ControlToValidate="txtEmail"
        Text="Invalid email format" Cssclass="error"
        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
        Runat="server" /> 
    </div>
    
    <div class="entry">
      <asp:Label ID="Label3" runat="server" AssociatedControlID="ddWhy" Text="Why are you contacting us?" />
      <asp:DropDownList runat="server" ID="ddWhy"  CssClass="form-control">
        <asp:ListItem>To discuss the VUI Library</asp:ListItem>     
        <asp:ListItem>To submit a news story</asp:ListItem>      
        <asp:ListItem>To contribute an article</asp:ListItem>     
        <asp:ListItem>To post a job</asp:ListItem>
        <asp:ListItem>To discuss commercial opportunities</asp:ListItem>
        <asp:ListItem>To talk about something else</asp:ListItem>
      </asp:DropDownList>
      <span class="error" style="visibility:hidden">Please select a reason for your contact request</span>
    </div>
        
    <div class="entry">
      <asp:Label ID="Label4" runat="server" AssociatedControlID="txtSubject" Text="Subject line" />
      <asp:TextBox runat="server" ID="txtSubject"  CssClass="form-control" />
      <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtSubject"
        ErrorMessage="Subject is a mandatory field." ValidationGroup="contactForm" Cssclass="error" />
    </div>
    
    <div class="entry">
      <asp:Label ID="Label5" runat="server" AssociatedControlID="txtBody" Text="Your message" />
      <asp:TextBox runat="server" ID="txtBody"  CssClass="form-control" TextMode="Multiline" Height="250px" />
      <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtBody"
        ErrorMessage="Body is a mandatory field." ValidationGroup="contactForm" Cssclass="error" />
    </div>
    
    <div class="entry">
      <asp:Button runat="server" ID="btnContact" CausesValidation="true" ValidationGroup="contactForm" OnClick="SubmitContactForm" CssClass="btn btn-lg btn-primary btn-block" Text="Send your message" />
    </div>
</div>

</asp:PlaceHolder>
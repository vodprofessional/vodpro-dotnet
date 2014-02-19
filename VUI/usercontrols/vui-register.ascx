<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-register.ascx.cs" Inherits="VUI.usercontrols.vui_register" %>

<div class="main-ui-dialog">
<asp:CreateUserWizard ID="CreateUserWizard1" Runat="server"
  OnNextButtonClick="NextButtonClick" OnCreatedUser="CreatedUser" StepNextButtonText="Next"
  CreateUserButtonText="Register me">
    
  <MailDefinition BodyFileName="~/App_Data/reg-email-confirm.txt" Subject="Registration Confirmation" From="VOD Professional <postmaster@vodprofessional.com>" />
  
  <WizardSteps>
    <asp:WizardStep runat="server" ID="WizardStep0">
      
     
     <div class="vodform">

       <asp:HiddenField runat="server" ID="ReturnPage" />      
 
        <h2>Register - Step 1 of 2</h2>

        <div class="entry">
            <asp:Label ID="Label1" runat="server" AssociatedControlID="txtTitle">Title</asp:Label>
        <asp:DropDownList runat="server" ID="txtTitle">
            <asp:ListItem Value="Mr">Mr</asp:ListItem>
            <asp:ListItem Value="Mrs">Mrs</asp:ListItem>
            <asp:ListItem Value="Miss">Miss</asp:ListItem>
            <asp:ListItem Value="Ms">Ms</asp:ListItem>
            <asp:ListItem Value="Dr">Dr</asp:ListItem>
        </asp:DropDownList><asp:Label runat="server" ID="MsgTitle" Visible="False" CssClass="error"/>
        </div>
        <div class="entry">
        <asp:Label ID="Label2" runat="server" AssociatedControlID="txtFirstName">First Name *</asp:Label>
        <asp:TextBox runat="server" iD="txtFirstName" /><asp:Label runat="server" ID="MsgFirstName" Visible="False" CssClass="error"/>
        </div>
        <div class="entry">
        <asp:Label ID="Label3" runat="server" AssociatedControlID="txtLastName">Last Name *</asp:Label>
        <asp:TextBox runat="server" ID="txtLastName" /><asp:Label runat="server" ID="MsgLastName" Visible="False" CssClass="error"/>
        </div>
        <div class="entry">
        <asp:Label ID="Label4" runat="server" AssociatedControlID="txtJobTitle">Job Title *</asp:Label>
        <asp:TextBox runat="server" ID="txtJobTitle" /><asp:Label runat="server" ID="MsgJobTitle" Visible="False" CssClass="error"/>
        </div>
        <div class="entry">
        <asp:Label ID="Label5" runat="server" AssociatedControlID="txtCompanyName">Company Name *</asp:Label>
        <asp:TextBox runat="server" ID="txtCompanyName" /><asp:Label runat="server" ID="MsgCompanyName" Visible="False" CssClass="error"/>
        </div>
        <div class="entry">
        <asp:Label ID="Label6" runat="server" AssociatedControlID="EmailAddress">Email (also used as your login)</asp:Label>
        <asp:TextBox runat="server" ID="EmailAddress" /><asp:Label runat="server" ID="MsgEmail" Visible="False" CssClass="error"/>
        </div>
        <div class="entry">
        <asp:Label ID="Label7" runat="server" AssociatedControlID="PasswordText" Columns="9">Password</asp:Label>
        <asp:TextBox runat="server" ID="PasswordText" TextMode="Password" autocomplete="off" /><asp:Label runat="server" ID="MsgPassword" Visible="False" CssClass="error" />
        </div>
        <div class="entry">
        <asp:Label ID="Label8" runat="server" AssociatedControlID="PasswordConfirm" Columns="9">Confirm Password</asp:Label>
        <asp:TextBox runat="server" ID="PasswordConfirm" TextMode="Password" autocomplete="off" /><asp:Label runat="server" ID="MsgPasswordConfirm" Visible="False" CssClass="error" />
        </div>
        
        <div class="entry">
        <asp:Label ID="Label21" runat="server" AssociatedControlID="PasswordConfirm" Columns="9">Where did you hear about the VUI Library?</asp:Label>
        <asp:DropDownList runat="server" ID="ddWhereHeard" ClientIDMode="Static">
            <asp:ListItem>Informitv</asp:ListItem>
            <asp:ListItem>Appmarket.tv</asp:ListItem>
            <asp:ListItem>Videonet</asp:ListItem>
            <asp:ListItem>VOD Professional mailing</asp:ListItem>
            <asp:ListItem>Via VODProfessional.com</asp:ListItem>
            <asp:ListItem>Word of mouth</asp:ListItem>
            <asp:ListItem>Search engine</asp:ListItem>
        </asp:DropDownList>
        </div>


        <div class="entry">
          <div class="formendtext">
            <p>VOD Professional will never pass on your data to other companies unless you give us permission to do so.
              See our <a href="http://www.vodprofessional.com/about-us/terms-conditions/">terms and conditions</a> and <a href="http://www.vodprofessional.com/about-us/privacy-policy/">privacy policy</a> for more details.
            </p>
      
               <p>   
                  <asp:CheckBox runat="server" ID="chkAccept" Checked="false"/>
                  <asp:Label ID="Label9" CssClass="checkbox" runat="server" AssociatedControlID="chkAccept">
            Tick here if you do want to receive selected email promotions from
                    external companies.</asp:Label>
                  </p>
          </div>
        </div>
      
        <div class="entry recaptcha"> 
                  
              <recaptcha:RecaptchaControl
                ID="recaptcha"
                runat="server"
                PublicKey="6LdTAMQSAAAAAAOV3Hli3WXNMxC-BCXLiumAe9aa"
                PrivateKey="6LdTAMQSAAAAAEGHjwquIw0oqd6Tp0cMi1jA3LhF"
            />   
        </div>
     
      
        <div class="entry">
          <div class="formendtext">
            By clicking "Next" you confirm that you agree to our <a href="http://www.vodprofessional.com/about-us/terms-conditions/">terms and conditions</a>.      
          </div>
        </div>

      
    </div>
    
 </asp:WizardStep>
    
 <asp:CreateUserWizardStep runat="server" Title="Add your details" ID="CreateUserStep">
    <ContentTemplate>
        <asp:TextBox runat="server" ID="UserName" Visible="False" />
        <asp:TextBox runat="server" ID="Email" Visible="False" />
        <asp:TextBox runat="server" ID="Password" Visible="False"/>
        
        <h2>Register - Step 2 of 2</h2>

        <div class="formintrotext">
          <p>
              Please tell us a little more about yourself. This information will help us improve the site
              and is necessary if you want to submit jobs or directory listings.
            </p>
        </div>

<div class="vodform">
  
<asp:Literal runat="server" EnableViewState="false" ID="ErrorMessage"></asp:Literal>
  
  
<div class="entry">
<asp:Label ID="Label10" runat="server" AssociatedControlID="txtCompanyAddress1">Company Address 1 *</asp:Label>
<asp:TextBox runat="server" ID="txtCompanyAddress1" />
  <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtCompanyAddress1"
    ErrorMessage="Company address is a mandatory field." ValidationGroup="CreateUserWizard1" CssClass="error" />
</div>
<div class="entry">
<asp:Label ID="Label11" runat="server" AssociatedControlID="txtCompanyAddress2">Company Address 2</asp:Label>
<asp:TextBox runat="server" ID="txtCompanyAddress2" />
</div>
<div class="entry">
<asp:Label ID="Label12" runat="server" AssociatedControlID="txtCompanyAddress1">Company Address 3</asp:Label>
<asp:TextBox runat="server" ID="txtCompanyAddress3" />
</div>
<div class="entry">
<asp:Label ID="Label13" runat="server" AssociatedControlID="txtCompanyTown">Town *</asp:Label>
<asp:TextBox runat="server" ID="txtCompanyTown" />
  <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtCompanyTown"
    ErrorMessage="Town is a mandatory field." ValidationGroup="CreateUserWizard1" CssClass="error" />
</div>
<div class="entry">
<asp:Label ID="Label14" runat="server" AssociatedControlID="txtCounty">County / State</asp:Label>
<asp:TextBox runat="server" ID="txtCounty" />
</div>
<div class="entry">
<asp:Label ID="Label15" runat="server" AssociatedControlID="txtPostcode">Postcode /ZIP *</asp:Label>
<asp:TextBox runat="server" ID="txtPostcode" Columns="8" />
  <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtPostcode"
  ErrorMessage="Postcode/Zip is a mandatory field." ValidationGroup="CreateUserWizard1" CssClass="error" />
</div>
<div class="entry">
<asp:Label ID="Label16" runat="server" AssociatedControlID="Country">Country *</asp:Label>
<asp:DropDownList ID="Country" runat="server">
  <asp:ListItem value="Albania">Albania</asp:ListItem>
  <asp:ListItem value="Algeria">Algeria</asp:ListItem>
  <asp:ListItem value="American Samoa">American Samoa</asp:ListItem>
  <asp:ListItem value="Andorra">Andorra</asp:ListItem>
  <asp:ListItem value="Angola">Angola</asp:ListItem>
  <asp:ListItem value="Anguilla">Anguilla</asp:ListItem>
  <asp:ListItem value="Antigua">Antigua</asp:ListItem>
  <asp:ListItem value="Argentina">Argentina</asp:ListItem>
  <asp:ListItem value="Armenia">Armenia</asp:ListItem>
  <asp:ListItem value="Aruba">Aruba</asp:ListItem>
  <asp:ListItem value="Australia">Australia</asp:ListItem>
  <asp:ListItem value="Austria">Austria</asp:ListItem>
  <asp:ListItem value="Azerbaijan">Azerbaijan</asp:ListItem>
  <asp:ListItem value="Bahamas">Bahamas</asp:ListItem>
  <asp:ListItem value="Bahrain">Bahrain</asp:ListItem>
  <asp:ListItem value="Bangladesh">Bangladesh</asp:ListItem>
  <asp:ListItem value="Barbados">Barbados</asp:ListItem>
  <asp:ListItem value="Barbuda">Barbuda</asp:ListItem>
  <asp:ListItem value="Belarus">Belarus</asp:ListItem>
  <asp:ListItem value="Belgium">Belgium</asp:ListItem>
  <asp:ListItem value="Belize">Belize</asp:ListItem>
  <asp:ListItem value="Benin">Benin</asp:ListItem>
  <asp:ListItem value="Bermuda">Bermuda</asp:ListItem>
  <asp:ListItem value="Bhutan">Bhutan</asp:ListItem>
  <asp:ListItem value="Bolivia">Bolivia</asp:ListItem>
  <asp:ListItem value="Bonaire">Bonaire</asp:ListItem>
  <asp:ListItem value="Botswana">Botswana</asp:ListItem>
  <asp:ListItem value="Brazil">Brazil</asp:ListItem>
  <asp:ListItem value="Virgin islands">British Virgin isl.</asp:ListItem>
  <asp:ListItem value="Brunei">Brunei</asp:ListItem>
  <asp:ListItem value="Bulgaria">Bulgaria</asp:ListItem>
  <asp:ListItem value="Burundi">Burundi</asp:ListItem>
  <asp:ListItem value="Cambodia">Cambodia</asp:ListItem>
  <asp:ListItem value="Cameroon">Cameroon</asp:ListItem>
  <asp:ListItem value="Canada">Canada</asp:ListItem>
  <asp:ListItem value="Cape Verde">Cape Verde</asp:ListItem>
  <asp:ListItem value="Cayman isl">Cayman Islands</asp:ListItem>
  <asp:ListItem value="Central African Rep">Central African Rep.</asp:ListItem>
  <asp:ListItem value="Chad">Chad</asp:ListItem>
  <asp:ListItem value="Channel isl">Channel Islands</asp:ListItem>
  <asp:ListItem value="Chile">Chile</asp:ListItem>
  <asp:ListItem value="China">China</asp:ListItem>
  <asp:ListItem value="Colombia">Colombia</asp:ListItem>
  <asp:ListItem value="Congo">Congo</asp:ListItem>
  <asp:ListItem value="cook isl">Cook Islands</asp:ListItem>
  <asp:ListItem value="Costa Rica">Costa Rica</asp:ListItem>
  <asp:ListItem value="Croatia">Croatia</asp:ListItem>
  <asp:ListItem value="Curacao">Curacao</asp:ListItem>
  <asp:ListItem value="Cyprus">Cyprus</asp:ListItem>
  <asp:ListItem value="Czech Republic">Czech Republic</asp:ListItem>
  <asp:ListItem value="Denmark">Denmark</asp:ListItem>
  <asp:ListItem value="Djibouti">Djibouti</asp:ListItem>
  <asp:ListItem value="Dominica">Dominica</asp:ListItem>
  <asp:ListItem value="Dominican Republic">Dominican Republic</asp:ListItem>
  <asp:ListItem value="Ecuador">Ecuador</asp:ListItem>
  <asp:ListItem value="Egypt">Egypt</asp:ListItem>
  <asp:ListItem value="El Salvador">El Salvador</asp:ListItem>
  <asp:ListItem value="Equatorial Guinea">Equatorial Guinea</asp:ListItem>
  <asp:ListItem value="Eritrea">Eritrea</asp:ListItem>
  <asp:ListItem value="Estonia">Estonia</asp:ListItem>
  <asp:ListItem value="Ethiopia">Ethiopia</asp:ListItem>
  <asp:ListItem value="Faeroe isl">Faeroe Islands</asp:ListItem>
  <asp:ListItem value="Fiji">Fiji</asp:ListItem>
  <asp:ListItem value="Finland">Finland</asp:ListItem>
  <asp:ListItem value="France">France</asp:ListItem>
  <asp:ListItem value="French Guiana">French Guiana</asp:ListItem>
  <asp:ListItem value="French Polynesia">French Polynesia</asp:ListItem>
  <asp:ListItem value="Gabon">Gabon</asp:ListItem>
  <asp:ListItem value="Gambia">Gambia</asp:ListItem>
  <asp:ListItem value="Georgia">Georgia</asp:ListItem>
  <asp:ListItem value="Gemany">Germany</asp:ListItem>
  <asp:ListItem value="Ghana">Ghana</asp:ListItem>
  <asp:ListItem value="Gibraltar">Gibraltar</asp:ListItem>
  <asp:ListItem value="Greece">Greece</asp:ListItem>
  <asp:ListItem value="Greenland">Greenland</asp:ListItem>
  <asp:ListItem value="Grenada">Grenada</asp:ListItem>
  <asp:ListItem value="Guadeloupe">Guadeloupe</asp:ListItem>
  <asp:ListItem value="Guam">Guam</asp:ListItem>
  <asp:ListItem value="Guatemala">Guatemala</asp:ListItem>
  <asp:ListItem value="Guinea">Guinea</asp:ListItem>
  <asp:ListItem value="Guinea Bissau">Guinea Bissau</asp:ListItem>
  <asp:ListItem value="Guyana">Guyana</asp:ListItem>
  <asp:ListItem value="Haiti">Haiti</asp:ListItem>
  <asp:ListItem value="Honduras">Honduras</asp:ListItem>
  <asp:ListItem value="Hong Kong">Hong Kong</asp:ListItem>
  <asp:ListItem value="Hungary">Hungary</asp:ListItem>
  <asp:ListItem value="Iceland">Iceland</asp:ListItem>
  <asp:ListItem value="India">India</asp:ListItem>
  <asp:ListItem value="Indonesia">Indonesia</asp:ListItem>
  <asp:ListItem value="Irak">Irak</asp:ListItem>
  <asp:ListItem value="Iran">Iran</asp:ListItem>
  <asp:ListItem value="Ireland">Ireland</asp:ListItem>
  <asp:ListItem value="Northern Ireland">Ireland, Northern</asp:ListItem>
  <asp:ListItem value="Israel">Israel</asp:ListItem>
  <asp:ListItem value="Italy">Italy</asp:ListItem>
  <asp:ListItem value="Ivory Coast">Ivory Coast</asp:ListItem>
  <asp:ListItem value="Jamaica">Jamaica</asp:ListItem>
  <asp:ListItem value="Japan">Japan</asp:ListItem>
  <asp:ListItem value="Jordan">Jordan</asp:ListItem>
  <asp:ListItem value="Kazakhstan">Kazakhstan</asp:ListItem>
  <asp:ListItem value="Kenya">Kenya</asp:ListItem>
  <asp:ListItem value="Kuwait">Kuwait</asp:ListItem>
  <asp:ListItem value="Kyrgyzstan">Kyrgyzstan</asp:ListItem>
  <asp:ListItem value="Latvia">Latvia</asp:ListItem>
  <asp:ListItem value="Lebanon">Lebanon</asp:ListItem>
  <asp:ListItem value="Liberia">Liberia</asp:ListItem>
  <asp:ListItem value="Liechtenstein">Liechtenstein</asp:ListItem>
  <asp:ListItem value="Lithuania">Lithuania</asp:ListItem>
  <asp:ListItem value="Luxembourg">Luxembourg</asp:ListItem>
  <asp:ListItem value="Macau">Macau</asp:ListItem>
  <asp:ListItem value="Macedonia">Macedonia</asp:ListItem>
  <asp:ListItem value="Madagascar">Madagascar</asp:ListItem>
  <asp:ListItem value="Malawi">Malawi</asp:ListItem>
  <asp:ListItem value="Malaysia">Malaysia</asp:ListItem>
  <asp:ListItem value="Maldives">Maldives</asp:ListItem>
  <asp:ListItem value="Mali">Mali</asp:ListItem>
  <asp:ListItem value="Malta">Malta</asp:ListItem>
  <asp:ListItem value="Marshall isl">Marshall Islands</asp:ListItem>
  <asp:ListItem value="Martinique">Martinique</asp:ListItem>
  <asp:ListItem value="Mauritania">Mauritania</asp:ListItem>
  <asp:ListItem value="Mauritius">Mauritius</asp:ListItem>
  <asp:ListItem value="Mexico">Mexico</asp:ListItem>
  <asp:ListItem value="Micronesia">Micronesia</asp:ListItem>
  <asp:ListItem value="Moldova">Moldova</asp:ListItem>
  <asp:ListItem value="Monaco">Monaco</asp:ListItem>
  <asp:ListItem value="Mongolia">Mongolia</asp:ListItem>
  <asp:ListItem value="Montserrat">Montserrat</asp:ListItem>
  <asp:ListItem value="Morocco">Morocco</asp:ListItem>
  <asp:ListItem value="Mozambique">Mozambique</asp:ListItem>
  <asp:ListItem value="Myanmar">Myanmar/Burma</asp:ListItem>
  <asp:ListItem value="Namibia">Namibia</asp:ListItem>
  <asp:ListItem value="Nepal">Nepal</asp:ListItem>
  <asp:ListItem value="Netherlands">Netherlands</asp:ListItem>
  <asp:ListItem value="Netherlands Antilles">Netherlands Antilles</asp:ListItem>
  <asp:ListItem value="New Caledonia">New Caledonia</asp:ListItem>
  <asp:ListItem value="New Zealand">New Zealand</asp:ListItem>
  <asp:ListItem value="Nicaragua">Nicaragua</asp:ListItem>
  <asp:ListItem value="Niger">Niger</asp:ListItem>
  <asp:ListItem value="Nigeria">Nigeria</asp:ListItem>
  <asp:ListItem value="Norway">Norway</asp:ListItem>
  <asp:ListItem value="Oman">Oman</asp:ListItem>
  <asp:ListItem value="Palau">Palau</asp:ListItem>
  <asp:ListItem value="Panama">Panama</asp:ListItem>
  <asp:ListItem value="Papua New Guinea">Papua New Guinea</asp:ListItem>
  <asp:ListItem value="Paraguay">Paraguay</asp:ListItem>
  <asp:ListItem value="Peru">Peru</asp:ListItem>
  <asp:ListItem value="Philippines">Philippines</asp:ListItem>
  <asp:ListItem value="Poland">Poland</asp:ListItem>
  <asp:ListItem value="Portugal">Portugal</asp:ListItem>
  <asp:ListItem value="Puerto Rico">Puerto Rico</asp:ListItem>
  <asp:ListItem value="Qatar">Qatar</asp:ListItem>
  <asp:ListItem value="Reunion">Reunion</asp:ListItem>
  <asp:ListItem value="Rwanda">Rwanda</asp:ListItem>
  <asp:ListItem value="Saba">Saba</asp:ListItem>
  <asp:ListItem value="Saipan">Saipan</asp:ListItem>
  <asp:ListItem value="Saudi Arabia">Saudi Arabia</asp:ListItem>
  <asp:ListItem value="Scotland">Scotland</asp:ListItem>
  <asp:ListItem value="Senegal">Senegal</asp:ListItem>
  <asp:ListItem value="Seychelles">Seychelles</asp:ListItem>
  <asp:ListItem value="Sierra Leone">Sierra Leone</asp:ListItem>
  <asp:ListItem value="Singapore">Singapore</asp:ListItem>
  <asp:ListItem value="Slovac Republic">Slovak Republic</asp:ListItem>
  <asp:ListItem value="Slovenia">Slovenia</asp:ListItem>
  <asp:ListItem value="South Africa">South Africa</asp:ListItem>
  <asp:ListItem value="South Korea">South Korea</asp:ListItem>
  <asp:ListItem value="Spain">Spain</asp:ListItem>
  <asp:ListItem value="Sri Lanka">Sri Lanka</asp:ListItem>
  <asp:ListItem value="Sudan">Sudan</asp:ListItem>
  <asp:ListItem value="Suriname">Suriname</asp:ListItem>
  <asp:ListItem value="Swaziland">Swaziland</asp:ListItem>
  <asp:ListItem value="Sweden">Sweden</asp:ListItem>
  <asp:ListItem value="Switzerland">Switzerland</asp:ListItem>
  <asp:ListItem value="Syria">Syria</asp:ListItem>
  <asp:ListItem value="Taiwan">Taiwan</asp:ListItem>
  <asp:ListItem value="Tanzania">Tanzania</asp:ListItem>
  <asp:ListItem value="Thailand">Thailand</asp:ListItem>
  <asp:ListItem value="Togo">Togo</asp:ListItem>
  <asp:ListItem value="Trinidad-Tobago">Trinidad-Tobago</asp:ListItem>
  <asp:ListItem value="Tunesia">Tunisia</asp:ListItem>
  <asp:ListItem value="Turkey">Turkey</asp:ListItem>
  <asp:ListItem value="Turkmenistan">Turkmenistan</asp:ListItem>
  <asp:ListItem value="United Arab Emirates">United Arab Emirates</asp:ListItem>
  <asp:ListItem value="U.S. Virgin isl">U.S. Virgin Islands</asp:ListItem>
  <asp:ListItem value="USA">U.S.A.</asp:ListItem>
  <asp:ListItem value="Uganda">Uganda</asp:ListItem>
  <asp:ListItem value="United Kingdom" selected="true">United Kingdom</asp:ListItem>
  <asp:ListItem value="Urugay">Uruguay</asp:ListItem>
  <asp:ListItem value="Uzbekistan">Uzbekistan</asp:ListItem>
  <asp:ListItem value="Vanuatu">Vanuatu</asp:ListItem>
  <asp:ListItem value="Vatican City">Vatican City</asp:ListItem>
  <asp:ListItem value="Venezuela">Venezuela</asp:ListItem>
  <asp:ListItem value="Vietnam">Vietnam</asp:ListItem>
  <asp:ListItem value="Wales">Wales</asp:ListItem>
  <asp:ListItem value="Yemen">Yemen</asp:ListItem>
  <asp:ListItem value="Zaire">Zaire</asp:ListItem>
  <asp:ListItem value="Zambia">Zambia</asp:ListItem>
  <asp:ListItem value="Zimbabwe">Zimbabwe</asp:ListItem>
</asp:DropDownList>
</div>
<div class="entry">
<asp:Label ID="Label17" runat="server" AssociatedControlID="txtPhone">Business phone *</asp:Label>
<asp:TextBox runat="server" ID="txtPhone" />
  <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtPhone"
    ErrorMessage="Phone is a mandatory field." ValidationGroup="CreateUserWizard1" CssClass="error" />
</div>
<div class="entry">
<asp:Label ID="Label18" runat="server" AssociatedControlID="txtMobile">Mobile</asp:Label>
<asp:TextBox runat="server" ID="txtMobile" />
</div>
<div class="entry">
<asp:Label ID="Label19" runat="server" AssociatedControlID="txtOrgType">Organisation Type (If other, please specify)</asp:Label>
  <asp:DropDownList runat="server" ID="txtOrgType">
    <asp:ListItem value="Advertising">Advertising</asp:ListItem>
    <asp:ListItem value="Analysis &amp; Analytics">Analysis &amp; Analytics</asp:ListItem>
    <asp:ListItem value="Broadcaster">Broadcaster</asp:ListItem>
    <asp:ListItem value="Consultancy">Consultancy</asp:ListItem>
    <asp:ListItem value="Content Acquisition">Content Acquisition</asp:ListItem>
    <asp:ListItem value="Content Management">Content Management</asp:ListItem>
    <asp:ListItem value="Design Services">Design Services</asp:ListItem>
    <asp:ListItem value="Hardware &amp; Infrastructure">Hardware &amp; Infrastructure</asp:ListItem>
    <asp:ListItem value="IT Services">IT Services</asp:ListItem>
    <asp:ListItem value="Manufacturing">Manufacturing</asp:ListItem>
    <asp:ListItem value="Media">Media</asp:ListItem>
    <asp:ListItem value="Media Buying / Selling">Media Buying / Selling</asp:ListItem>
    <asp:ListItem value="MSO">MSO</asp:ListItem>
    <asp:ListItem value="Online Marketing">Online Marketing</asp:ListItem>
    <asp:ListItem value="Online Video">Online Video</asp:ListItem>
    <asp:ListItem value="Production">Production</asp:ListItem>
    <asp:ListItem value="Software Services">Software Services</asp:ListItem>
    <asp:ListItem value="Trade Body">Trade Body</asp:ListItem>
    <asp:ListItem value="Other">Other</asp:ListItem>
  </asp:DropDownList>
  <asp:TextBox runat="server" ID="txtOrgTypeOther" />
</div>
<div class="entry">
<asp:Label ID="Label20" runat="server" AssociatedControlID="txtEmployees">How many employees are there in your company? *</asp:Label>
  <asp:DropDownList runat="server" ID="txtEmployees">
    <asp:ListItem value="1-5">1-5 employees</asp:ListItem>
    <asp:ListItem value="5-20">5-20</asp:ListItem>
    <asp:ListItem value="20-100">20-100</asp:ListItem>
    <asp:ListItem value="100-500">100-500</asp:ListItem>
    <asp:ListItem value="500-1000">500-1000</asp:ListItem>
    <asp:ListItem value="100+">1000+</asp:ListItem>
  </asp:DropDownList>
 <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="txtEmployees"
    ErrorMessage="Number of employees is a mandatory field." ValidationGroup="CreateUserWizard1" />
</div>
        </div>
        
      </ContentTemplate>
    </asp:CreateUserWizardStep>

    <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server" Title="Complete">
      <ContentTemplate>
        <h2>Thanks for Joining VOD Professional</h2>
        <div class="formendtext">
        <p>
          We just sent a verification email to the address you provided (you may need
          to check your Junk mail folder). Please click on the link in the email to complete your registration.
        </p>
        </div>
      </ContentTemplate>
    </asp:CompleteWizardStep>
  </WizardSteps>
</asp:CreateUserWizard>
</div>
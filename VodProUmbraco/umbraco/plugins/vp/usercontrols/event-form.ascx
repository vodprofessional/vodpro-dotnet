<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="event-form.ascx.cs" Inherits="VP2.usercontrols.event_form" %>

<asp:PlaceHolder runat="server" ID="plcEventForm">

    <p>To add a new event simply fill in the form below. Once you’re finished click ‘Save and Preview’, check your work and, if you’re happy with it, 
    submit it to us for review. We’ll let you know by email when it’s been published to the calendar.</p>

    <div style="border-bottom: 1px solid rgb(225, 225, 225); margin-bottom: 5px; padding-bottom: 15px; margin-top: 15px;" class="row">
        <div class="col-xs-12">
            <a href="/calendar" title="Back to calendar"><i class="fa fa-arrow-circle-left"></i> Events Calendar</a>
            <span class="pull-right">
                <a href="/members/list-events" title="View my events list">View my events list <i class="fa fa-arrow-circle-right"></i></a>
            </span>
         </div>
    </div>

    <div class="form" id="eventEntryForm">

        <div class="alert alert-warning alert-dismissible" role="alert" id="dvGeneralWarning" runat="server">
            <button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
            <asp:Literal runat="server" ID="litGeneralWarning"></asp:Literal>
        </div> <!-- alert -->

        <div class="entry">
            <asp:Label ID="Label1" runat="server" AssociatedControlID="txtName" Text="Event name *" />
            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" autofocus />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtName" SetFocusOnError="true"
                ErrorMessage="The event's name is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
        </div>

        <div class="entry">
            <asp:Label ID="Label5" runat="server" AssociatedControlID="lstEventType" Text="Type of Event *" />
            <select id="lstEventType" runat="server" class="form-control">
                <option>Conference / Exhibition</option>
                <option>Networking event</option>
                <option>Product Demonstration</option>
                <option>Financial Results</option>
                <option>Webinar</option>
            </select>
        </div>

        <div class="entry row">
            <div class="col col-sm-6">
                <asp:Label ID="Label2" runat="server" AssociatedControlID="txtStartDate" Text="Start Date and Time *" />
                <div class="form-group">
				    <div class="input-group date datetimepicker" id="dp-start">
					    <asp:TextBox runat="server" CssClass="form-control" ID="txtStartDate" ClientIDMode="Static" />
					    <span class="input-group-addon">
						    <span class="glyphicon glyphicon-calendar"></span>
					    </span>
				    </div>
			    </div>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtStartDate" SetFocusOnError="true"
                    ErrorMessage="Start date is a mandatory field." ValidationGroup="eventForm" Cssclass="error"  Display="Dynamic"/>
                <%--<asp:RegularExpressionValidator runat="server" ID="RegExStartDate" ControlToValidate="txtStartDate" Cssclass="error"
                    ErrorMessage="Please enter a date/time in the format dd/MM/yyyy hh:mm" 
                    ValidationExpression="^(?:31\/(?:(?:0[13578])|(?:1[02]))\/)|(?:(?:29|30)\/(?:(?:0[1,3-9])|(?:1[0-2]))\/)(?:(?:1[6-9]|[2-9]\d)\d{2})$|^(?:29\/02\/(?:(?:(?:1[6-9]|[2-9]\d)(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0[1-9])|(?:1\d)|(?:2[0-8]))\/(?:(?:0[1-9])|(?:1[0-2]))\/(?:(?:1[6-9]|[2-9]\d)\d{2})$" />
                  --%>
            </div>

            <div class="col col-sm-6">
                <asp:Label ID="Label3" runat="server" AssociatedControlID="txtEndDate" Text="End Date and Time *" />
                <div class="form-group">
				    <div class="input-group date datetimepicker" id="dp-end">
					    <asp:TextBox runat="server" CssClass="form-control" ID="txtEndDate" ClientIDMode="Static" />
					    <span class="input-group-addon">
						    <span class="glyphicon glyphicon-calendar"></span>
					    </span>
				    </div>
			    </div>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtEndDate" SetFocusOnError="true"
                    ErrorMessage="End date is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
                <%--<asp:RegularExpressionValidator runat="server" ID="RegularExpressionVaidator1" ControlToValidate="txtEndDate" Cssclass="error"
                    ErrorMessage="Please enter a valid date/time in the format dd/MM/yyyy hh:mm" 
                    ValidationExpression="^(?:31\/(?:(?:0[13578])|(?:1[02]))\/)|(?:(?:29|30)\/(?:(?:0[1,3-9])|(?:1[0-2]))\/)(?:(?:1[6-9]|[2-9]\d)\d{2})$|^(?:29\/02\/(?:(?:(?:1[6-9]|[2-9]\d)(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0[1-9])|(?:1\d)|(?:2[0-8]))\/(?:(?:0[1-9])|(?:1[0-2]))\/(?:(?:1[6-9]|[2-9]\d)\d{2})$" />
                  --%>
           </div>
       </div>

       <div class="entry">
          <div class="checkbox checkbox-primary">
            <asp:Checkbox runat="server" ID="chkTBA" />
            <asp:Label ID="Label22" runat="server" AssociatedControlID="chkTBA" Text="Check this box if the start/end times are not confirmed" />
          </div>
       </div>

       <div class="entry">
            <asp:Label ID="Label7" runat="server" AssociatedControlID="lstTimeZone" Text="Time zone *" />
            <select id="lstTimeZone" runat="server" class="form-control">
	<option timeZoneId="1" value="GMT-12:00" useDaylightTime="0" valueOld="-12">(GMT-12:00) International Date Line West</option>
	<option timeZoneId="2" value="GMT-11:00" useDaylightTime="0" valueOld="-11">(GMT-11:00) Midway Island, Samoa</option>
	<option timeZoneId="3" value="GMT-10:00" useDaylightTime="0" valueOld="-10">(GMT-10:00) Hawaii</option>
	<option timeZoneId="4" value="GMT-09:00" useDaylightTime="1" valueOld="-9">(GMT-09:00) Alaska</option>
	<option timeZoneId="5" value="GMT-08:00" useDaylightTime="1" valueOld="-8">(GMT-08:00) Pacific Time (US & Canada)</option>
	<option timeZoneId="6" value="GMT-08:00" useDaylightTime="1" valueOld="-8">(GMT-08:00) Tijuana, Baja California</option>
	<option timeZoneId="7" value="GMT-07:00" useDaylightTime="0" valueOld="-7">(GMT-07:00) Arizona</option>
	<option timeZoneId="8" value="GMT-07:00" useDaylightTime="1" valueOld="-7">(GMT-07:00) Chihuahua, La Paz, Mazatlan</option>
	<option timeZoneId="9" value="GMT-07:00" useDaylightTime="1" valueOld="-7">(GMT-07:00) Mountain Time (US & Canada)</option>
	<option timeZoneId="10" value="GMT-06:00" useDaylightTime="0" valueOld="-6">(GMT-06:00) Central America</option>
	<option timeZoneId="11" value="GMT-06:00" useDaylightTime="1" valueOld="-6">(GMT-06:00) Central Time (US & Canada)</option>
	<option timeZoneId="12" value="GMT-06:00" useDaylightTime="1" valueOld="-6">(GMT-06:00) Guadalajara, Mexico City, Monterrey</option>
	<option timeZoneId="13" value="GMT-06:00" useDaylightTime="0" valueOld="-6">(GMT-06:00) Saskatchewan</option>
	<option timeZoneId="14" value="GMT-05:00" useDaylightTime="0" valueOld="-5">(GMT-05:00) Bogota, Lima, Quito, Rio Branco</option>
	<option timeZoneId="15" value="GMT-05:00" useDaylightTime="1" valueOld="-5">(GMT-05:00) Eastern Time (US & Canada)</option>
	<option timeZoneId="16" value="GMT-05:00" useDaylightTime="1" valueOld="-5">(GMT-05:00) Indiana (East)</option>
	<option timeZoneId="17" value="GMT-04:00" useDaylightTime="1" valueOld="-4">(GMT-04:00) Atlantic Time (Canada)</option>
	<option timeZoneId="18" value="GMT-04:00" useDaylightTime="0" valueOld="-4">(GMT-04:00) Caracas, La Paz</option>
	<option timeZoneId="19" value="GMT-04:00" useDaylightTime="0" valueOld="-4">(GMT-04:00) Manaus</option>
	<option timeZoneId="20" value="GMT-04:00" useDaylightTime="1" valueOld="-4">(GMT-04:00) Santiago</option>
	<option timeZoneId="21" value="GMT-03:30" useDaylightTime="1" valueOld="-3.5">(GMT-03:30) Newfoundland</option>
	<option timeZoneId="22" value="GMT-03:00" useDaylightTime="1" valueOld="-3">(GMT-03:00) Brasilia</option>
	<option timeZoneId="23" value="GMT-03:00" useDaylightTime="0" valueOld="-3">(GMT-03:00) Buenos Aires, Georgetown</option>
	<option timeZoneId="24" value="GMT-03:00" useDaylightTime="1" valueOld="-3">(GMT-03:00) Greenland</option>
	<option timeZoneId="25" value="GMT-03:00" useDaylightTime="1" valueOld="-3">(GMT-03:00) Montevideo</option>
	<option timeZoneId="26" value="GMT-02:00" useDaylightTime="1" valueOld="-2">(GMT-02:00) Mid-Atlantic</option>
	<option timeZoneId="27" value="GMT-01:00" useDaylightTime="0" valueOld="-1">(GMT-01:00) Cape Verde Is.</option>
	<option timeZoneId="28" value="GMT-01:00" useDaylightTime="1" valueOld="-1">(GMT-01:00) Azores</option>
	<option timeZoneId="29" value="GMT+00:00" useDaylightTime="0" valueOld="0">(GMT+00:00) Casablanca, Monrovia, Reykjavik</option>
	<option timeZoneId="30" value="GMT+00:00" useDaylightTime="1" valueOld="0">(GMT+00:00) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London</option>
	<option timeZoneId="31" value="GMT+01:00" useDaylightTime="1" valueOld="1">(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna</option>
	<option timeZoneId="32" value="GMT+01:00" useDaylightTime="1" valueOld="1">(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague</option>
	<option timeZoneId="33" value="GMT+01:00" useDaylightTime="1" valueOld="1">(GMT+01:00) Brussels, Copenhagen, Madrid, Paris</option>
	<option timeZoneId="34" value="GMT+01:00" useDaylightTime="1" valueOld="1">(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb</option>
	<option timeZoneId="35" value="GMT+01:00" useDaylightTime="1" valueOld="1">(GMT+01:00) West Central Africa</option>
	<option timeZoneId="36" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Amman</option>
	<option timeZoneId="37" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Athens, Bucharest, Istanbul</option>
	<option timeZoneId="38" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Beirut</option>
	<option timeZoneId="39" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Cairo</option>
	<option timeZoneId="40" value="GMT+02:00" useDaylightTime="0" valueOld="2">(GMT+02:00) Harare, Pretoria</option>
	<option timeZoneId="41" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius</option>
	<option timeZoneId="42" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Jerusalem</option>
	<option timeZoneId="43" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Minsk</option>
	<option timeZoneId="44" value="GMT+02:00" useDaylightTime="1" valueOld="2">(GMT+02:00) Windhoek</option>
	<option timeZoneId="45" value="GMT+03:00" useDaylightTime="0" valueOld="3">(GMT+03:00) Kuwait, Riyadh, Baghdad</option>
	<option timeZoneId="46" value="GMT+03:00" useDaylightTime="1" valueOld="3">(GMT+03:00) Moscow, St. Petersburg, Volgograd</option>
	<option timeZoneId="47" value="GMT+03:00" useDaylightTime="0" valueOld="3">(GMT+03:00) Nairobi</option>
	<option timeZoneId="48" value="GMT+03:00" useDaylightTime="0" valueOld="3">(GMT+03:00) Tbilisi</option>
	<option timeZoneId="49" value="GMT+03:30" useDaylightTime="1" valueOld="3.5">(GMT+03:30) Tehran</option>
	<option timeZoneId="50" value="GMT+04:00" useDaylightTime="0" valueOld="4">(GMT+04:00) Abu Dhabi, Muscat</option>
	<option timeZoneId="51" value="GMT+04:00" useDaylightTime="1" valueOld="4">(GMT+04:00) Baku</option>
	<option timeZoneId="52" value="GMT+04:00" useDaylightTime="1" valueOld="4">(GMT+04:00) Yerevan</option>
	<option timeZoneId="53" value="GMT+04:30" useDaylightTime="0" valueOld="4.5">(GMT+04:30) Kabul</option>
	<option timeZoneId="54" value="GMT+05:00" useDaylightTime="1" valueOld="5">(GMT+05:00) Yekaterinburg</option>
	<option timeZoneId="55" value="GMT+05:00" useDaylightTime="0" valueOld="5">(GMT+05:00) Islamabad, Karachi, Tashkent</option>
	<option timeZoneId="56" value="GMT+05:30" useDaylightTime="0" valueOld="5.5">(GMT+05:30) Sri Jayawardenapura</option>
	<option timeZoneId="57" value="GMT+05:30" useDaylightTime="0" valueOld="5.5">(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi</option>
	<option timeZoneId="58" value="GMT+05:45" useDaylightTime="0" valueOld="5.75">(GMT+05:45) Kathmandu</option>
	<option timeZoneId="59" value="GMT+06:00" useDaylightTime="1" valueOld="6">(GMT+06:00) Almaty, Novosibirsk</option>
	<option timeZoneId="60" value="GMT+06:00" useDaylightTime="0" valueOld="6">(GMT+06:00) Astana, Dhaka</option>
	<option timeZoneId="61" value="GMT+06:30" useDaylightTime="0" valueOld="6.5">(GMT+06:30) Yangon (Rangoon)</option>
	<option timeZoneId="62" value="GMT+07:00" useDaylightTime="0" valueOld="7">(GMT+07:00) Bangkok, Hanoi, Jakarta</option>
	<option timeZoneId="63" value="GMT+07:00" useDaylightTime="1" valueOld="7">(GMT+07:00) Krasnoyarsk</option>
	<option timeZoneId="64" value="GMT+08:00" useDaylightTime="0" valueOld="8">(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi</option>
	<option timeZoneId="65" value="GMT+08:00" useDaylightTime="0" valueOld="8">(GMT+08:00) Kuala Lumpur, Singapore</option>
	<option timeZoneId="66" value="GMT+08:00" useDaylightTime="0" valueOld="8">(GMT+08:00) Irkutsk, Ulaan Bataar</option>
	<option timeZoneId="67" value="GMT+08:00" useDaylightTime="0" valueOld="8">(GMT+08:00) Perth</option>
	<option timeZoneId="68" value="GMT+08:00" useDaylightTime="0" valueOld="8">(GMT+08:00) Taipei</option>
	<option timeZoneId="69" value="GMT+09:00" useDaylightTime="0" valueOld="9">(GMT+09:00) Osaka, Sapporo, Tokyo</option>
	<option timeZoneId="70" value="GMT+09:00" useDaylightTime="0" valueOld="9">(GMT+09:00) Seoul</option>
	<option timeZoneId="71" value="GMT+09:00" useDaylightTime="1" valueOld="9">(GMT+09:00) Yakutsk</option>
	<option timeZoneId="72" value="GMT+09:30" useDaylightTime="0" valueOld="9.5">(GMT+09:30) Adelaide</option>
	<option timeZoneId="73" value="GMT+09:30" useDaylightTime="0" valueOld="9.5">(GMT+09:30) Darwin</option>
	<option timeZoneId="74" value="GMT+10:00" useDaylightTime="0" valueOld="10">(GMT+10:00) Brisbane</option>
	<option timeZoneId="75" value="GMT+10:00" useDaylightTime="1" valueOld="10">(GMT+10:00) Canberra, Melbourne, Sydney</option>
	<option timeZoneId="76" value="GMT+10:00" useDaylightTime="1" valueOld="10">(GMT+10:00) Hobart</option>
	<option timeZoneId="77" value="GMT+10:00" useDaylightTime="0" valueOld="10">(GMT+10:00) Guam, Port Moresby</option>
	<option timeZoneId="78" value="GMT+10:00" useDaylightTime="1" valueOld="10">(GMT+10:00) Vladivostok</option>
	<option timeZoneId="79" value="GMT+11:00" useDaylightTime="1" valueOld="11">(GMT+11:00) Magadan, Solomon Is., New Caledonia</option>
	<option timeZoneId="80" value="GMT+12:00" useDaylightTime="1" valueOld="12">(GMT+12:00) Auckland, Wellington</option>
	<option timeZoneId="81" value="GMT+12:00" useDaylightTime="0" valueOld="12">(GMT+12:00) Fiji, Kamchatka, Marshall Is.</option>
	<option timeZoneId="82" value="GMT+13:00" useDaylightTime="0" valueOld="13">(GMT+13:00) Nuku'alofa</option>
</select>
        </div>
        
        <div class="entry">
            <asp:Label ID="Label4" runat="server" AssociatedControlID="txtVenue" Text="Venue *" />
            <asp:TextBox runat="server" ID="txtVenue" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtVenue" SetFocusOnError="true"
                ErrorMessage="The event's venue is a mandatory field." ValidationGroup="eventForm" Cssclass="error"  Display="Dynamic" />
          
        </div>
        <div class="entry">
            <asp:Label ID="Label10" runat="server" AssociatedControlID="txtAddress1" Text="Address *" />
            <asp:TextBox runat="server" ID="txtAddress1" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="txtAddress1" SetFocusOnError="true"
                ErrorMessage="The event's address is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
        </div>

        <div class="entry">    
            <asp:TextBox runat="server" ID="txtAddress2" CssClass="form-control"  />
        </div>
        <div class="entry">
            <asp:Label ID="Label11" runat="server" AssociatedControlID="txtCity" Text="City *" />
            <asp:TextBox runat="server" ID="txtCity" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator7" ControlToValidate="txtCity" SetFocusOnError="true"
                ErrorMessage="City is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic"/>
        </div>
        <div class="entry">
            <asp:Label ID="Label12" runat="server" AssociatedControlID="txtPostcode" Text="Zip  / Postcode *" />
            <asp:TextBox runat="server" ID="txtPostcode" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator8" ControlToValidate="txtPostcode" SetFocusOnError="true"
                ErrorMessage="Zip/Postcode is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic"/>
        </div>

        
        <div class="entry">
            <asp:Label ID="Label8" runat="server" AssociatedControlID="txtExternalUrl" Text="Event URL *" />
            <asp:TextBox runat="server" ID="txtExternalUrl" CssClass="form-control"  />
        </div>


        <div class="entry">
            <asp:Label ID="Label6" runat="server" AssociatedControlID="txtDescription" Text="Description *" />
            <div class="form-help">You have <span class="counter">1000</span> characters available.</div>
            <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" Rows="5" CssClass="form-control text-counted"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" ControlToValidate="txtDescription" SetFocusOnError="true"
                ErrorMessage="The event description is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic"/>

        </div>
        
        

        <div class="entry">
            <asp:Label ID="Label9" runat="server" AssociatedControlID="txtGoogleMapEmbed" Text="Google Map Embed Code" />
            <div class="form-help">
              If you'd like a map to the venue to display, please copy the <strong>embed</strong> code from <a href="https://www.google.com/maps" target="vpmap">Google Maps</a> and paste it into the box below. 
              If you aren't sure what to do, <a href="https://support.google.com/maps/answer/3544418?hl=en" target="vpmapinstr">follow Google's intructions for finding the code.</a>
            </div>
            <asp:TextBox runat="server" ID="txtGoogleMapEmbed" CssClass="form-control text-counted" TextMode="MultiLine" Rows="5" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="hidGoogleMapEmbed" ClientIDMode="Static" Value="" />
            <asp:RegularExpressionValidator runat="server" ID="valInput"
                ControlToValidate="txtGoogleMapEmbed"
                ValidationExpression="^[\s\S]{0,500}$"
                ErrorMessage="Please enter a maximum of 500 characters" SetFocusOnError="true"
                Display="Dynamic">*</asp:RegularExpressionValidator>
        </div>

        <div class="entry">
          <div class="form-group">
            <label ID="Label2" for="testUpload">Add an image</label>
            <div class="form-help">
            Images are limited in to a maximum of 800px wide and 240px high. We recommend these dimensions to make the most of the space available. 
              <a href="http://placehold.it/800x240" target="vpimg">Here's an example image with the maximum dimensions</a>. 
              There is a size limit of <strong>300KB</strong> for your image. We recommend using an optimiser, 
                e.g. <a href="http://tinypng.com" target="vpimg">http://tinypng.com</a> before uploading.
            </div>
            <input type="file" id="fileUpload" multiple="false" class="form-control" runat="server" />
            <asp:PlaceHolder runat="server" ID="plcUploaded" Visible="false">
              <span class="upload-container">
                <asp:Image runat="server" ID="imgUploaded" />
                <button runat="server" id="btnUploadAgain" onclick="$('#hidGoogleMapEmbed').val(window.escape($('#txtGoogleMapEmbed').val())); $('#txtGoogleMapEmbed').val(''); "
                     onserverclick="UploadAgain" class="btn btn-primary">Upload Again</button>
              </span>
            </asp:PlaceHolder>
            <span id="errUpload" class="error" runat="server" visible="false">Please upload a PNG or JPG image, no larger than 300KB in size.</span>
          </div>
        </div>

        
        <p>
            The following fields are needed so we can contact you. You can choose to display as many or as few of these fields as you like 
            on the published event 
        </p>
        
        <div class="entry row">
            
          <div class="col col-xs-10">
            <asp:Label ID="Label13" runat="server" AssociatedControlID="txtContactName" Text="Organiser Name *" />
            <asp:TextBox runat="server" ID="txtContactName" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="txtContactName" SetFocusOnError="true"
                ErrorMessage="Organiser Name is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
          </div>
          <div class="col col-xs-2">
            <label></label>
            <div class="checkbox checkbox-primary">
                <asp:Checkbox runat="server" ID="chkDisplayContactName" />
                <asp:Label ID="Label17" runat="server" AssociatedControlID="chkDisplayContactName" Text="Display?" />
            </div>
          </div>
        </div>

        
        <div class="entry row">
            
          <div class="col col-xs-10">
            <asp:Label ID="Label16" runat="server" AssociatedControlID="txtContactCompany" Text="Company * " />
            <asp:TextBox runat="server" ID="txtContactCompany" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator12" ControlToValidate="txtContactCompany" SetFocusOnError="true"
                ErrorMessage="Company is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
          </div>
          <div class="col col-xs-2">
            <label></label>
            <div class="checkbox checkbox-primary">
                <asp:Checkbox runat="server" ID="chkDisplayContactCompany" />
                <asp:Label ID="Label18" runat="server" AssociatedControlID="chkDisplayContactCompany" Text="Display?" />
            </div>
          </div>
        </div>

        
        <div class="entry row">
          <div class="col col-xs-10">
            <asp:Label ID="Label14" runat="server" AssociatedControlID="txtContactEmail" Text="Organiser Email *" />
            <asp:TextBox runat="server" ID="txtContactEmail" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator10" ControlToValidate="txtContactEmail" SetFocusOnError="true"
                ErrorMessage="Contact email is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
          </div>
          <div class="col col-xs-2">
            <label></label>
            <div class="checkbox checkbox-primary">
                <asp:Checkbox runat="server" ID="chkDisplayContactEmail" />
                <asp:Label ID="Label19" runat="server" AssociatedControlID="chkDisplayContactEmail" Text="Display?" />
            </div>
          </div>
        </div>

        
        <div class="entry row">
          <div class="col col-xs-10">
            <asp:Label ID="Label15" runat="server" AssociatedControlID="txtContactPhone" Text="Organiser Phone *" />
            <asp:TextBox runat="server" ID="txtContactPhone" CssClass="form-control"  />
            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator11" ControlToValidate="txtContactPhone" SetFocusOnError="true"
                ErrorMessage="Contact phone is a mandatory field." ValidationGroup="eventForm" Cssclass="error" Display="Dynamic" />
          </div>
          <div class="col col-xs-2">
            <label></label>
            <div class="checkbox checkbox-primary">
                <asp:Checkbox runat="server" ID="chkDisplayContactPhone" />
                <asp:Label ID="Label20" runat="server" AssociatedControlID="chkDisplayContactPhone" Text="Display?" />
            </div>
          </div>
        </div>


        <div class="entry" runat="server" id="dvRecaptcha">
            <div class="g-recaptcha" data-sitekey="<%= RecaptchaKey %>"></div>
            <span id="errRecaptcha" class="error" runat="server" visible="false">Are you sure you aren't a robot? Try again?</span>
        </div>

        <div class="entry">
          <asp:Button runat="server" ID="btnPreview" ClientIDMode="Static" CausesValidation="true" ValidationGroup="eventForm" OnClick="SaveAndPreview" 
              OnClientClick="$('#hidGoogleMapEmbed').val(window.escape($('#txtGoogleMapEmbed').val())); $('#txtGoogleMapEmbed').val('')"
              CssClass="btn btn-lg btn-primary btn-block" Text="Save and preview event" />
        </div>
        
    </div>

</asp:PlaceHolder>


<asp:Panel runat="server" ID="pnlPreview" Visible="false" CssClass="">

     <asp:Button runat="server" ID="btnBack" OnClick="Back" 
            CssClass="btn btn-lg btn-primary btn-block" Text="Back to editor" />

    <div class="event-container">

        <h1><asp:Literal runat="server" ID="litName" /></h1>
        
        <asp:Literal runat="server" ID="litEventType" />
        
        <asp:PlaceHolder runat="server" ID="plcImg">
            <div class="event-image"><asp:Image runat="server" ID="imgEvent" /></div>
        </asp:PlaceHolder>
        
        <div class="event-city"><asp:Literal runat="server" ID="litCity" /></div>
        
        <div class="event-date"><asp:Literal runat="server" ID="litDate" /></div>
        

        <p><asp:Literal runat="server" ID="litDescription" /></p>

        <asp:PlaceHolder runat="server" ID="plcEventDetail">
          <dl>
            <asp:Literal runat="server" ID="litURL" />
            <asp:Literal runat="server" ID="litContacts" />
          </dl>
        </asp:PlaceHolder>


        <div class="event-venue-container">
            <div class="event-venue">
                <h2>Venue</h2>
                <div class="event-venue-address">
                    <asp:Literal runat="server" ID="litVenue" />
                </div>

                <asp:PlaceHolder runat="server" ID="plcMap">
                    <div class="event-venue-map">
                        <asp:Literal runat="server" ID="litGoogleMapEmbed" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>



    </div>
    <asp:Button runat="server" ID="btnSave" ClientIDMode="Static" OnClick="SaveAndNotify"
            CssClass="btn btn-lg btn-primary btn-block" Text="Save event and notify VOD Professional" />

</asp:Panel>

<asp:PlaceHolder ID="plcThanks" runat="server" Visible="false">

    <p>
        Thanks for entering your event. The VOD Professional team will now check it over and let you know 
        once it has been published to the website, usually within 24 hours. </p>
    <p>
        If you have any changes you’d like to make in the meantime, please let us know by email to <a href="mailto:events@vodprofessional.com">events@vodprofessional.com</a>.
    </p>
    <p>
        <a title="Back to calendar" href="/calendar"><i class="fa fa-arrow-circle-left"></i> Events Calendar</a>
        <span class="pull-right">
            <a title="View my events list" href="/members/list-events">My events list <i class="fa fa-arrow-circle-right"></i></a>
        </span>
    </p>
    
</asp:PlaceHolder>
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VP2.businesslogic;
using VP2.BusinessLogic;
using VPCommon;

namespace VP2.usercontrols
{
    public partial class event_form : System.Web.UI.UserControl
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(event_form));

        private int _maxDimension = 800;
        private int _maxThDimension = 250;
        private string _uploadUrl;
        private string _resizedUrl;
        private string _defaultExtension = ".jpg";
        private int _uploadSizeLimit = 1024000; // 3MB
        private int _minfreespace = 100000000; // 100MB


        protected void Page_Init(object sender, EventArgs e)
        {
            Response.AddHeader("X-XSS-Protection", "0");

            _uploadUrl = ConfigurationManager.AppSettings["USER_UPLOAD_URL"];
            _resizedUrl = ConfigurationManager.AppSettings["USER_UPLOAD_RESIZED_URL"];
            _uploadSizeLimit = Int32.Parse(ConfigurationManager.AppSettings["USER_UPLOAD_SIZE_LIMIT"]);
            _minfreespace = Int32.Parse(ConfigurationManager.AppSettings["USER_UPLOAD_FREESPACE_LIMIT"]);
            _maxDimension = Int32.Parse(ConfigurationManager.AppSettings["CalendarUploadImageMaxDimension"]);
            _maxThDimension = Int32.Parse(ConfigurationManager.AppSettings["CalendarUploadImageThMaxDimension"]);

            HideErrors();

        }

        private void HideErrors()
        {
            dvGeneralWarning.Visible = false;
            pnlPreview.Visible = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                lstTimeZone.SelectedIndex = 29;

                // Editing an event
                if (!String.IsNullOrEmpty(Request["i"]))
                {
                    int id = 0;
                    if (Int32.TryParse(Request["i"], out id))
                    {
                        EventID = id;

                        // Make sure this is the correct member's event
                        CalendarItem evt = new CalendarItem(EventID, "PREVIEW");
                        if(evt.IsCreatedByCurrentMember())
                        {
                            IsHuman = true;
                            PopulateForm(evt);
                        }
                        else
                        {
                            Response.Redirect("/calendar");
                        }
                    }
                }
            }
        }

        private void PopulateForm(CalendarItem evt)
        {
            

            txtName.Text = evt.Name;
            lstEventType.Items.FindByText(evt.EventType);
            txtStartDate.Text = evt.StartDate.ToString("dd/MM/yyyy HH:mm:ss");
            txtEndDate.Text = evt.EndDate.ToString("dd/MM/yyyy HH:mm:ss");
            chkTBA.Checked = evt.TimeTBA;
            lstTimeZone.Items.FindByText(evt.TimeZone);
            txtVenue.Text = evt.Venue;
            txtAddress1.Text = evt.Address1;
            txtAddress2.Text = evt.Address2;
            txtCity.Text = evt.City;
            txtPostcode.Text = evt.Postcode;
            txtExternalUrl.Text = evt.ExternalUrl;
            txtDescription.Text = evt.Description;
            txtGoogleMapEmbed.Text = evt.GoogleMap;
            // hidGoogleMapEmbed.Value = HttpUtility.UrlEncode(evt.GoogleMap);

            // Do things with the uploaded image
            if (!String.IsNullOrEmpty(evt.EventImage))
            {
                ImageUrl = evt.EventImage;
                ImageThUrl = evt.EventImage;
                imgUploaded.ImageUrl = evt.EventImage;
                plcUploaded.Visible = true;
                fileUpload.Visible = false;
            }
            else
            {
                plcUploaded.Visible = false;
                fileUpload.Visible = true;
            }

            txtContactName.Text = evt.ContactName;
            chkDisplayContactName.Checked = evt.DisplayContactName;
            txtContactCompany.Text = evt.Company;
            chkDisplayContactCompany.Checked = evt.DisplayCompany;
            txtContactEmail.Text = evt.ContactEmail;
            chkDisplayContactEmail.Checked = evt.DisplayContactEmail;
            txtContactPhone.Text = evt.ContactPhone;
            chkDisplayContactPhone.Checked = evt.DisplayContactPhone;

            dvRecaptcha.Visible = false;
        }

        protected void SaveAndPreview(object sender, EventArgs e)
        {
            bool isValid = true;

            if (IsHuman)
            {
                dvRecaptcha.Visible = false;
            }
            else
            {
                bool revalid = Recaptcha2.Validate();
                if (!revalid)
                {
                    isValid = false;
                    errRecaptcha.Visible = true;
                    return;
                }
                else
                {
                    IsHuman = true;
                    dvRecaptcha.Visible = false;
                }
            }

            bool hasUpload = false;
            string[] contenttypes = { "image/gif", "image/jpeg", "image/pjpeg", "image/png" };

            if (Request.Files.Count > 0)
            {
                HttpPostedFile file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    log.Debug("- Uploading image");

                   hasUpload = true;
                    if (file.ContentLength > _uploadSizeLimit)
                    {
                        log.Debug("- Image too large [" + file.ContentLength + "]");
                        isValid = false;
                        errUpload.InnerHtml = "Your file was too large! We accept images up to 3MB in size";
                        errUpload.Visible = true;
                    }
                    else if (!contenttypes.Contains(file.ContentType))
                    {
                        log.Debug("- Image wrng format [" + file.ContentType + "]");

                        isValid = false;
                        errUpload.InnerHtml = "We can only process JPG, PNG and GIF images";
                        errUpload.Visible = true;
                    }
                    else
                    {
                        if (hasUpload)
                        {
                            string uploadfilename = file.FileName;
                            string extension = uploadfilename.Substring(uploadfilename.LastIndexOf('.'));

                            string uniqueFileName = ImageUtility.GetUniqueImageFileName() + "-" + uploadfilename.Replace(extension, "");
                            string uploadFileUrl = _uploadUrl + (_uploadUrl.EndsWith("/") ? "" : "/") + uniqueFileName + extension;
                            string resizeFileUrl = _resizedUrl + (_resizedUrl.EndsWith("/") ? "" : "/") + uniqueFileName + _defaultExtension;
                            string thumbFileUrl = _resizedUrl + (_resizedUrl.EndsWith("/") ? "" : "/") + uniqueFileName + "_th" + _defaultExtension;

                            try
                            {
                                string uploadFilePath = Server.MapPath(uploadFileUrl);
                                string resizedSavePath = Server.MapPath(resizeFileUrl);
                                string thumbSavePath = Server.MapPath(thumbFileUrl);

                                if (FolderUtility.DriveHasSpace(uploadFilePath, _minfreespace))
                                {
                                    file.SaveAs(uploadFilePath);
                                    ImageUtility.SetEveryoneAccess(uploadFilePath);
                                    log.Debug("--SAVED IMAGE [" + uploadFilePath + "]");

                                    ImageUtility.ResizeAndSave(uploadFilePath, resizedSavePath, _maxDimension);
                                    log.Debug("--RESIZED IMAGE TO [" + resizedSavePath + "]");

                                    ImageUtility.ResizeAndSave(uploadFilePath, thumbSavePath, _maxThDimension);
                                    log.Debug("--RESIZED IMAGE TO [" + thumbSavePath + "]");

                                    UploadPath = uploadFilePath;
                                    ImageUrl = resizeFileUrl;
                                    ImagePath = resizedSavePath;
                                    ImageThUrl = thumbFileUrl;
                                    ImageThPath = thumbSavePath;
                                }
                                else
                                {
                                    throw new Exception("DISK LOW ON SPACE");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Error saving and resizing image [" + uploadFileUrl + "]", ex);
                                //    ErrorProcessor.Process(ex, new ErrorProcessor.Option[] { ErrorProcessor.Option.Email });
                                isValid = false;
                                litGeneralWarning.Text = "There was an error saving your image. We have notified the support team, but you might want to try again!";
                                litGeneralWarning.Visible = true;
                            }
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(ImageUrl) && !String.IsNullOrEmpty(ImageThUrl))
            {
                fileUpload.Visible = false;
                plcUploaded.Visible = true;
                imgUploaded.ImageUrl = ImageThUrl;
            }
            else
            {
                fileUpload.Visible = true;
                plcUploaded.Visible = false;
            }

            if (isValid)
            {
                CultureInfo cult = new CultureInfo("en-GB");


                CalendarItem evt;
                if (EventID > int.MinValue)
                {
                    evt = new CalendarItem(EventID, "PREVIEW");
                }
                else
                {
                    evt = new CalendarItem();
                    evt.Mode = "PREVIEW";
                }

                evt.Name = txtName.Text;
                evt.EventType = lstEventType.Items[lstEventType.SelectedIndex].Text;

                evt.Description = txtDescription.Text;
                evt.TimeZone = lstTimeZone.Items[lstTimeZone.SelectedIndex].Value;
                evt.StartDate = DateTime.Parse(txtStartDate.Text, cult.DateTimeFormat);
                evt.EndDate = DateTime.Parse(txtEndDate.Text, cult.DateTimeFormat);
                evt.TimeTBA = chkTBA.Checked;

                evt.Venue = txtVenue.Text;
                evt.Address1 = txtAddress1.Text;
                evt.Address2 = txtAddress2.Text;
                evt.City = txtCity.Text;
                evt.Postcode = txtPostcode.Text;

                evt.ContactName = txtContactName.Text;
                evt.DisplayContactName = chkDisplayContactName.Checked;
                evt.ContactEmail = txtContactEmail.Text;
                evt.DisplayContactEmail = chkDisplayContactEmail.Checked;
                evt.ContactPhone = txtContactPhone.Text;
                evt.DisplayContactPhone = chkDisplayContactPhone.Checked;
                evt.Company = txtContactCompany.Text;
                evt.DisplayCompany = chkDisplayContactCompany.Checked;

                evt.ExternalUrl = txtExternalUrl.Text;
                evt.CreatedBy = VPMember.GetLoggedInUser().User.Id;
                evt.GoogleMap = HttpUtility.UrlDecode(hidGoogleMapEmbed.Value);
                evt.SendPublishedEmail = true;

                if (!String.IsNullOrEmpty(ImageUrl))
                {
                    evt.EventImage = ImageUrl;
                }
                int Id = evt.Save();
                EventID = Id;
                evt.ID = Id;


                evt.Init();

                string venue = evt.Venue;
                if (!String.IsNullOrEmpty(evt.Address1))
                {
                    venue += @"<br/>" + evt.Address1;
                }
                if (!String.IsNullOrEmpty(evt.Address2))
                {
                    venue += @"<br/>" + evt.Address2;
                }
                if (!String.IsNullOrEmpty(evt.City))
                {
                    venue += @"<br/>" + evt.City;
                }
                if (!String.IsNullOrEmpty(evt.Postcode))
                {
                    venue += @"<br/>" + evt.Postcode;
                }

                litCity.Text = evt.City;

                litName.Text = evt.Name;
                litEventType.Mode = LiteralMode.PassThrough;
                litEventType.Text = @"<div class=""event-type " + evt.EventClass + @""">" + evt.EventType + @"</div>";

                litDate.Text = evt.DateString2;
                if (!evt.TimeTBA)
                {
                    litDate.Text += @"<br/><span class=""time"">" + evt.TimeString + @"</span>";
                }
                litDescription.Text = evt.Description;

                litVenue.Mode = LiteralMode.PassThrough;
                litVenue.Text = venue;

                if (evt.OrganiserDetails.Length == 0 && String.IsNullOrEmpty(evt.ExternalUrl))
                {
                    plcEventDetail.Visible = false;
                }
                else
                {
                    plcEventDetail.Visible = true;

                    if (evt.OrganiserDetails.Length > 0)
                    {
                        litContacts.Mode = LiteralMode.PassThrough;
                        litContacts.Text = @"<dt>Organiser:</dt> <dd>" + string.Join("<br/>", evt.OrganiserDetails) + @"</dd>";
                    }
                    if (!String.IsNullOrEmpty(evt.ExternalUrl))
                    {
                        litURL.Mode = LiteralMode.PassThrough;
                        litURL.Text = @"<dt>Event web site:</dt><dd><a href=""" + evt.ExternalUrlFormatted + @""">" + evt.ExternalUrlFormatted + @"</a></dd>";
                    }
                }

                if (!String.IsNullOrEmpty(evt.GoogleMap))
                {
                    plcMap.Visible = true;
                    litGoogleMapEmbed.Mode = LiteralMode.PassThrough;
                    litGoogleMapEmbed.Text = evt.GoogleMap;
                }
                else
                {
                    plcMap.Visible = false;
                }

                if (!String.IsNullOrEmpty(evt.EventImage))
                {
                    imgEvent.ImageUrl = evt.EventImage;
                    plcImg.Visible = true;
                }
                else
                {
                    plcImg.Visible = false;
                }

                plcEventForm.Visible = false;
                pnlPreview.Visible = true;
            }
        }

        protected void Back(object sender, EventArgs e)
        {
            pnlPreview.Visible = false;
            plcEventForm.Visible = true;
        }

        protected void SaveAndNotify(object sender, EventArgs e)
        {
            CalendarItem evt = new CalendarItem(EventID, "PREVIEW");
            // Send notification emails
            Emailer em = new Emailer("EMAIL_EVENT_ADMIN");
            //em.ReplaceMemberElements(m);
            em.ReplaceElement("#NAME#", evt.Name);
            em.ReplaceElement("#DATE#", evt.DateString);
            em.ReplaceElement("#ID#", evt.UniqueID);
            string[] emails = ConfigurationManager.AppSettings["CALENDAR_TECHNICAL_RECIPIENTS"].Split(';');
            em.Send(emails);

            plcThanks.Visible = true;
            plcEventForm.Visible = false;
            pnlPreview.Visible = false;

            ViewState.Clear();
        }


        protected void UploadAgain(object sender, EventArgs e)
        {
            try
            {
                File.Delete(UploadPath);
                File.Delete(ImagePath);
                File.Delete(ImageThPath);
            }
            catch (Exception ex)
            {
                //logWriter.Write("Error deleting uploaded files", ex, LogSeverity.Error);
            }
            UploadPath = String.Empty;
            ImageUrl = String.Empty;
            ImagePath = String.Empty;
            ImageThUrl = String.Empty;
            ImageThPath = String.Empty;

            fileUpload.Visible = true;
            plcUploaded.Visible = false;

        }


        #region Viewstate

        public bool IsHuman
        {
            get
            {
                if (ViewState["IsHuman"] != null)
                {
                    return (bool)ViewState["IsHuman"];
                }
                else
                {
                    return false;
                }
            }
            set { ViewState["IsHuman"] = value; }
        }

        public string UploadPath
        {
            get
            {
                if (ViewState["UploadPath"] != null)
                {
                    return (string)ViewState["UploadPath"];
                }
                else
                {
                    return String.Empty;
                }
            }
            set { ViewState["UploadPath"] = value; }
        }
        public string ImageUrl
        {
            get
            {
                if (ViewState["ImageUrl"] != null)
                {
                    return (string)ViewState["ImageUrl"];
                }
                else
                {
                    return String.Empty;
                }
            }
            set { ViewState["ImageUrl"] = value; }
        }
        public string ImagePath
        {
            get
            {
                if (ViewState["ImagePath"] != null)
                {
                    return (string)ViewState["ImagePath"];
                }
                else
                {
                    return String.Empty;
                }
            }
            set { ViewState["ImagePath"] = value; }
        }
        public string ImageThUrl
        {
            get
            {
                if (ViewState["ImageThUrl"] != null)
                {
                    return (string)ViewState["ImageThUrl"];
                }
                else
                {
                    return String.Empty;
                }
            }
            set { ViewState["ImageThUrl"] = value; }
        }
        public string ImageThPath
        {
            get
            {
                if (ViewState["ImageThPath"] != null)
                {
                    return (string)ViewState["ImageThPath"];
                }
                else
                {
                    return String.Empty;
                }
            }
            set { ViewState["ImageThPath"] = value; }
        }
        public int EventID
        {
            get
            {
                if (ViewState["EventID"] != null)
                {
                    return (int)ViewState["EventID"];
                }
                else
                {
                    return int.MinValue;
                }
            }
            set { ViewState["EventID"] = value; }
        }
        #endregion


        public string RecaptchaKey
        {
            get
            {
                return ConfigurationManager.AppSettings["RecaptchaSiteKey"];
            }
        }
    }
}
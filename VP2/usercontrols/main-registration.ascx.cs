﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Security;
using VPCommon;

namespace VP2.usercontrols
{
    public partial class main_registration : System.Web.UI.UserControl
    {

        umbraco.cms.businesslogic.member.Member thisMember;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Request["email"] != null)
                {
                    EmailAddress.Text = Server.UrlDecode(Request["email"].ToString());
                }


                if (Request["page"] != null)
                {
                    ReturnPage.Value = Request["page"].ToString();
                }
                else
                {
                    ReturnPage.Value = "1058";
                }
            }
        }

        protected void CreateUserWizard_SendingMail(object sender, MailMessageEventArgs e)
        {
            Emailer em = new Emailer("EMAIL_CONFIRM_SIGNUP");
            em.ReplaceMemberElements(thisMember);
            e.Message.IsBodyHtml = em.IsHTML;
            e.Message.Subject = em.Subject;
            e.Message.Body = em.Body;
            // Gets sent automatically.

            // Now log in directly to the web site.
            thisMember.getProperty("confirmedFromEmail").Value = true;
            thisMember.AddGroup(1095);  // Group ID
            thisMember.Save();
            FormsAuthentication.SetAuthCookie(thisMember.LoginName, true);

            if (!String.IsNullOrEmpty(ReturnPage.Value))
            {

                PlaceHolder plcplcBackToPage = CompleteWizardStep1.ContentTemplateContainer.FindControl("plcBackToPage") as PlaceHolder;
                HyperLink lnk = plcplcBackToPage.FindControl("lnkBackToArticle") as HyperLink;
                
                plcplcBackToPage.Visible = true;

                if (ReturnPage.Value.Equals("1058"))
                {
                    lnk.NavigateUrl = "/";
                    lnk.Text = "Continue browsing";
                }
                else
                {
                    lnk.NavigateUrl = ReturnPage.Value;
                }

            }

        }


        protected void CreatedUser(Object sender, EventArgs e)
        {
            /* User is created and setting extra parameters to profile */
            TextBox UserName = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("UserName") as TextBox;
            TextBox CompanyName = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtCompanyName") as TextBox;
            TextBox CompanyAddress1 = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtCompanyAddress1") as TextBox;
            TextBox CompanyAddress2 = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtCompanyAddress2") as TextBox;
            TextBox CompanyAddress3 = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtCompanyAddress3") as TextBox;
            TextBox CompanyTown = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtCompanyTown") as TextBox;
            TextBox County = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtCounty") as TextBox;
            TextBox PostCode = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtPostCode") as TextBox;
            DropDownList Country = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Country") as DropDownList;
            TextBox BusinessPhone = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtPhone") as TextBox;
            TextBox Mobile = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtMobile") as TextBox; ;
            DropDownList OrgType = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtOrgType") as DropDownList;
            TextBox OrgTypeOther = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtOrgTypeOther") as TextBox;
            DropDownList Employees = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("txtEmployees") as DropDownList;

            string username = UserName.Text;
            MembershipUser User = Membership.GetUser(username);
            umbraco.cms.businesslogic.member.Member member = new umbraco.cms.businesslogic.member.Member((int)User.ProviderUserKey);

            /*  umbraco.cms.businesslogic.property.Property[] props = member.getProperties;
            for(int i=0; i<props.Length; i++)
            {
              Response.Write("<br/>"+props[i].Id.ToString() + ":" + props[i].Value);
            }*/

            member.Text = txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim();

            umbraco.cms.businesslogic.property.Property propTitle = member.getProperty("title"); // Property alias
            umbraco.cms.businesslogic.property.Property propFirstName = member.getProperty("firstName"); // Property alias
            umbraco.cms.businesslogic.property.Property propLastName = member.getProperty("lastName"); // Property alias
            umbraco.cms.businesslogic.property.Property propFullName = member.getProperty("fullName"); // Property alias
            umbraco.cms.businesslogic.property.Property propReceiveEmails = member.getProperty("receivePromoEmails"); // Property alias
            umbraco.cms.businesslogic.property.Property propJobTitle = member.getProperty("jobTitle"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyName = member.getProperty("companyName"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyAddress1 = member.getProperty("companyAddress1"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyAddress2 = member.getProperty("companyAddress2"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyAddress3 = member.getProperty("companyAddress3"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyTown = member.getProperty("companyTown"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyState = member.getProperty("companyState"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyPostcode = member.getProperty("companyPostcodeZip"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyCountry = member.getProperty("companyCountry"); // Property alias
            umbraco.cms.businesslogic.property.Property propBusinessPhone = member.getProperty("businessPhone"); // Property alias
            umbraco.cms.businesslogic.property.Property propMobile = member.getProperty("mobile"); // Property alias
            umbraco.cms.businesslogic.property.Property propOrgType = member.getProperty("organisationType"); // Property alias
            umbraco.cms.businesslogic.property.Property propOrgTypeOther = member.getProperty("organisationTypeOther"); // Property alias
            umbraco.cms.businesslogic.property.Property propEmployees = member.getProperty("numberOfEmployees"); // Property alias
            umbraco.cms.businesslogic.property.Property propReturnPage = member.getProperty("returnPage"); // Property alias

            propTitle.Value = txtTitle.Text.Trim();
            propFirstName.Value = txtFirstName.Text.Trim();
            propLastName.Value = txtLastName.Text.Trim();
            propFullName.Value = txtTitle.Text.Trim() + " " + txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim();
            propReceiveEmails.Value = chkAccept.Checked;
            propJobTitle.Value = txtJobTitle.Text.Trim();
            propCompanyName.Value = txtCompanyName.Text.Trim();
            propCompanyAddress1.Value = CompanyAddress1.Text.Trim();
            propCompanyAddress2.Value = CompanyAddress2.Text.Trim();
            propCompanyAddress3.Value = CompanyAddress3.Text.Trim();
            propCompanyTown.Value = CompanyTown.Text.Trim();
            propCompanyState.Value = County.Text.Trim();
            propCompanyPostcode.Value = PostCode.Text.Trim();
            propCompanyCountry.Value = Country.Text.Trim();
            propBusinessPhone.Value = BusinessPhone.Text.Trim();
            propMobile.Value = Mobile.Text.Trim();
            propOrgType.Value = OrgType.Text.Trim();
            propOrgTypeOther.Value = OrgTypeOther.Text.Trim();
            propEmployees.Value = Employees.Text.Trim();
            propReturnPage.Value = ReturnPage.Value;

            /* Add member to registrant group */
            member.AddGroup(2869);  // Group ID 2869 = registrant
            member.Save();

            thisMember = member;
        }

        protected void NextButtonClick(Object sender, WizardNavigationEventArgs e)
        {
            string errMessage = "";
            if (e.CurrentStepIndex == 0)
            {
                e.Cancel = false;
                MsgPassword.Visible = false;
                MsgPasswordConfirm.Visible = false;
                MsgTitle.Visible = false;
                MsgFirstName.Visible = false;
                MsgLastName.Visible = false;
                MsgJobTitle.Visible = false;
                MsgCompanyName.Visible = false;
                MsgEmail.Visible = false;

                if (!Page.IsValid) { e.Cancel = true; }

                //    TextBox EmailAddress = CreateUserWizard1.WizardStep0.ContentTemplateContainer.FindControl("EmailAddress") as TextBox;
                //TextBox PasswordText = CreateUserWizard1.WizardStep0.ContentTemplateContainer.FindControl("PasswordText") as TextBox;
                //TextBox PasswordConfirm = CreateUserWizard1.WizardStep0.ContentTemplateContainer.FindControl("PasswordConfirm") as TextBox;

                if (PasswordText.Text.Trim().Equals(""))
                {
                    MsgPassword.Text = "Please enter a password";
                    MsgPassword.Visible = true;
                    e.Cancel = true;
                }
                else if (!PasswordConfirm.Text.Equals(PasswordText.Text))
                {
                    MsgPasswordConfirm.Text = "The passwords you've entered don't match!";
                    MsgPasswordConfirm.Visible = true;
                    e.Cancel = true;
                }

                if (txtTitle.Text.Trim().Equals(""))
                {
                    MsgTitle.Text = "Title is a mandatory field";
                    MsgTitle.Visible = true;
                    e.Cancel = true;
                }
                if (txtFirstName.Text.Trim().Equals(""))
                {
                    MsgFirstName.Text = "First name is a mandatory field";
                    MsgFirstName.Visible = true;
                    e.Cancel = true;
                }
                if (txtLastName.Text.Trim().Equals(""))
                {
                    MsgLastName.Text = "Last name is a mandatory field";
                    MsgLastName.Visible = true;
                    e.Cancel = true;
                }
                if (txtJobTitle.Text.Trim().Equals(""))
                {
                    MsgJobTitle.Text = "Job title is a mandatory field";
                    MsgJobTitle.Visible = true;
                    e.Cancel = true;
                }
                if (txtCompanyName.Text.Trim().Equals(""))
                {
                    MsgCompanyName.Text = "Company name is a mandatory field";
                    MsgCompanyName.Visible = true;
                    e.Cancel = true;
                }

                if (EmailAddress.Text.Trim().Equals(""))
                {
                    MsgEmail.Text = "Email address is a mandatory field";
                    MsgEmail.Visible = true;
                    e.Cancel = true;
                }
                else if (!IsEmail(EmailAddress.Text.Trim()))
                {
                    MsgEmail.Text = "Enter a valid email address";
                    MsgEmail.Visible = true;
                    e.Cancel = true;
                }
                else if (UserExists(EmailAddress.Text.Trim()))
                {
                    MsgEmail.Text = "That email address is already being used. Maybe you have already set up an account?";
                    MsgEmail.Visible = true;
                    e.Cancel = true;

                }
                if (e.Cancel == false)
                {
                    // Set the system textboxes
                    TextBox Email = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email") as TextBox;
                    TextBox UserName = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("UserName") as TextBox;
                    TextBox Password = CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Password") as TextBox;
                    Email.Text = EmailAddress.Text.Trim();
                    UserName.Text = EmailAddress.Text.Trim();
                    Password.Text = PasswordText.Text;
                }
            }
        }




        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        private static bool IsEmail(string email)
        {
            string MatchEmailPattern =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
               + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
               + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
               + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        private bool UserExists(string username)
        {
            if (Membership.GetUser(username) != null) { return true; }

            return false;
        }


    }
}
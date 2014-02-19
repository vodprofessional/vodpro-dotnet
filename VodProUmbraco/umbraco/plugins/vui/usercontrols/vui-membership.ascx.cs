using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VUI.classes;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using System.Text;

namespace VUI.usercontrols
{
    public partial class vui_membership : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool showRegistrantButton = ConfigurationManager.AppSettings["MEM_allowSetAllAsRegistrants"].ToString().Equals("true");
            if(showRegistrantButton)
            {
                pnlRegistrant.Visible = true;
            }
        }

        protected void AddRegistrant_Click(object sender, EventArgs e)
        {
            MemberGroup mg = MemberGroup.GetByName("registrant");
            List<Member> ms = Member.GetAllAsList().ToList();
            foreach (Member m in ms)
            {
                m.AddGroup(mg.Id);
            }

            ConfigurationManager.AppSettings["MEM_allowSetAllAsRegistrants"] = "false";
        }



        protected void SetVUIEndDates(object sender, EventArgs e)
        {
            litUsers.Text = "";
            List<Member> ms = Member.GetAllAsList().ToList();
            string[] admins = Roles.GetUsersInRole("vui_administrator");
            foreach (Member m in ms)
            {
                if (admins.Contains(m.LoginName))
                {
                    try
                    {
                        DateTime vuiStartDate = (DateTime)m.getProperty("vuiJoinDate").Value;
                        DateTime vuiEndDate = vuiStartDate.AddYears(1);
                        m.getProperty("vuiEndDate").Value = vuiEndDate;
                        litUsers.Text = litUsers.Text + "<br/>Set End Date for " + m.LoginName + ": " + vuiEndDate.ToString("dd MMM yyyy");
                    }
                    catch (Exception ex)
                    {
                        litUsers.Text = litUsers.Text + "<br/>Error with: " + m.LoginName + ": " + ex.Message;
                    }
                }
            }
        }



        /// <summary>
        /// Handy function that allows the admin user to be swapped from one to another.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
 
        protected void ChangeVUIAdmin(object sender, EventArgs e)
        {
            // Get the current admin
            MembershipUser currentUser = Membership.GetUser(txtAdminFrom.Text);
            Member currentAdmin = new Member((int)currentUser.ProviderUserKey);

            // Get the new admin
            MembershipUser newUser = Membership.GetUser(txtAdminTo.Text);
            Member newAdmin = new Member((int)newUser.ProviderUserKey);

            // Give them the admin role
            try
            {
                Roles.RemoveUserFromRole(txtAdminTo.Text, "vui_user");
                Roles.AddUserToRole(txtAdminTo.Text, "vui_administrator");

                newAdmin.getProperty("vuiJoinDate").Value = currentAdmin.getProperty("vuiJoinDate").Value;
                newAdmin.getProperty("vuiFullyPaidUp").Value = currentAdmin.getProperty("vuiFullyPaidUp").Value;
                newAdmin.getProperty("vuiEndDate").Value = currentAdmin.getProperty("vuiEndDate").Value;
                newAdmin.getProperty("vuiSubscriptionPackage").Value = currentAdmin.getProperty("vuiSubscriptionPackage").Value;
                newAdmin.getProperty("vUINumberOfUsersAllowed").Value = currentAdmin.getProperty("vUINumberOfUsersAllowed").Value;
                newAdmin.getProperty("vuiJoinDate").Value = currentAdmin.getProperty("vuiJoinDate").Value;
                newAdmin.getProperty("vuiAdministrator").Value = null;
                newAdmin.Save();
            }
            catch (Exception ex)
            {
                // Guessing this might happen if the user is not in this group!
            }
            List<Member> users = VUIfunctions.GetVUIUsersForAdmin(currentAdmin);

            foreach (Member m in users)
            {
                m.getProperty("vuiAdministrator").Value = newAdmin.Id;
                m.Save();
            }

            // For each user who has currentAdmin in admin property, change to new user and save
            
            // Remove Admin role from current admin
            Roles.RemoveUserFromRole(txtAdminFrom.Text, "vui_administrator");
            Roles.AddUserToRole(txtAdminFrom.Text, "vui_user");

            currentAdmin.getProperty("vuiAdministrator").Value = newAdmin.Id;
            currentAdmin.Save();
        }

        protected void GetUsers(object sender, EventArgs e)
        {

            List<Member> admins = VUIfunctions.GetVUIAdmins();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<ul>");
            
            foreach (Member admin in admins)
            {
                string endDate = String.Empty;
                try
                {
                    endDate = ((DateTime)admin.getProperty("vuiJoinDate").Value).ToString("dd MMM yyyy");
                }
                catch (Exception ex)
                {
                    endDate = "ERROR FINDING END DATE!";
                }
                sb.Append(@"<li>");
                sb.Append(admin.Id.ToString() + " - ");
                sb.Append(admin.getProperty("firstName").Value.ToString() + " " + admin.getProperty("lastName").Value.ToString() + " - " + admin.Email + " - ENDING:" + endDate);
                List<Member> users = VUIfunctions.GetVUIUsersForAdmin(admin);
                if (users.Count > 0)
                {
                    sb.Append("<ul>");
                    foreach (Member user in users)
                    {
                        sb.Append(@"<li>");
                        sb.Append(user.Id.ToString() + " - ");
                        sb.Append(user.getProperty("firstName").Value.ToString() + " " + user.getProperty("lastName").Value.ToString() + " " + user.Email);
                        sb.Append(@"</li>");
                    }
                    sb.Append("</ul>");
                }
                sb.Append(@"</li>");
            }
            litUsers.Text = sb.ToString();
        }

    }
}
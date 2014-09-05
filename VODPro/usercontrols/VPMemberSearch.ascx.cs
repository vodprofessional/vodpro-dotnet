using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;
using System.Web.Security;
using umbraco.presentation.umbraco.members;
using umbraco.uicontrols;
using System.Configuration;
using System.Text;

namespace com.vodprofessional
{
    public class VPMemberSearch : System.Web.UI.UserControl
    {
        protected global::System.Web.UI.WebControls.Button ButtonSearch;
        protected global::umbraco.uicontrols.Pane resultsPane;
        protected global::System.Web.UI.WebControls.Repeater rp_members;
        protected global::System.Web.UI.WebControls.Panel pnlAllUsers;
        protected global::System.Web.UI.WebControls.TextBox txtAllUsers;

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void GetAllRegistrantCSV(object sender, EventArgs e)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append(@"""email"",""id"",""fullName"",""title"",""firstName"",""lastName"",""jobTitle"",""receivePromoEmails"",""businessPhone"",""mobile"",""linkedIn"",""confirmedFromEmail"",""companyName"",""companyAddress1"",""companyAddress2"",""companyAddress3"",""companyTown"",""companyState"",""companyPostcodeZip"",""companyCountry"",""organisationType"",""organisationTypeOther"",""numberOfEmployees"",""vuiFullyPaidUp"",""vuiUserImage"",""affiliateId"",""vuiCurrentLoginDate"",""vuiLastLogin"",""vuiJoinDate"",""vuiEndDate"",""vuiSubscriptionPackage"",""vUINumberOfUsersAllowed"",""vuiAdministrator""");

            foreach (Member member in Member.GetAllAsList())
            {
                List<string> props = new List<string>();

                props.Add(member.Email);
                props.Add(member.Id.ToString());
                props.Add(member.getProperty("fullName").Value.ToString());
                props.Add(member.getProperty("title").Value.ToString());
                props.Add(member.getProperty("firstName").Value.ToString());
                props.Add(member.getProperty("lastName").Value.ToString());
                props.Add(member.getProperty("jobTitle").Value.ToString());
                props.Add(member.getProperty("receivePromoEmails").Value.ToString());
                props.Add(member.getProperty("businessPhone").Value.ToString());
                props.Add(member.getProperty("mobile").Value.ToString());
                props.Add(member.getProperty("linkedIn").Value.ToString());
                props.Add(member.getProperty("confirmedFromEmail").Value.ToString());
                props.Add(member.getProperty("companyName").Value.ToString());
                props.Add(member.getProperty("companyAddress1").Value.ToString());
                props.Add(member.getProperty("companyAddress2").Value.ToString());
                props.Add(member.getProperty("companyAddress3").Value.ToString());
                props.Add(member.getProperty("companyTown").Value.ToString());
                props.Add(member.getProperty("companyState").Value.ToString());
                props.Add(member.getProperty("companyPostcodeZip").Value.ToString());
                props.Add(member.getProperty("companyCountry").Value.ToString());
                props.Add(member.getProperty("organisationType").Value.ToString());
                props.Add(member.getProperty("organisationTypeOther").Value.ToString());
                props.Add(member.getProperty("numberOfEmployees").Value.ToString());
                props.Add(member.getProperty("vuiFullyPaidUp").Value.ToString());
                props.Add(member.getProperty("vuiUserImage").Value.ToString());
                props.Add(member.getProperty("affiliateId").Value.ToString());
                props.Add(member.getProperty("vuiCurrentLoginDate").Value.ToString());
                props.Add(member.getProperty("vuiLastLogin").Value.ToString());
                props.Add(member.getProperty("vuiJoinDate").Value.ToString());
                props.Add(member.getProperty("vuiEndDate").Value.ToString());
                props.Add(member.getProperty("vuiSubscriptionPackage").Value.ToString());
                props.Add(member.getProperty("vUINumberOfUsersAllowed").Value.ToString());
                props.Add(member.getProperty("vuiAdministrator").Value.ToString());

                foreach (string s in props)
                {
                    s.Replace(@"""", "");
                }

                sb.AppendLine(@"""" + string.Join(@""",""", props) + @"""");
            }

            txtAllUsers.Text = sb.ToString();
            pnlAllUsers.Visible = true;

        }

        protected void ButtonSearch_Click(object sender, System.EventArgs e)
        {
            resultsPane.Visible = true;
            if (Member.InUmbracoMemberMode())
            {
                //MemberType mt = MemberType.GetByAlias("subscriber");
                //List<Member> ms = Member.GetAllAsList().Where(m => m.ContentType.Alias.Equals("subscribers")).ToList();

                MemberGroup mg = MemberGroup.GetByName("registrant");
                List<Member> ms = mg.GetMembers("%").ToList<Member>();
                rp_members.DataSource = ms; // Member.GetAllAsList();
            }
            else
            {
                rp_members.DataSource = Membership.GetAllUsers();
            }
			rp_members.DataBind();
        }
        public void bindMember(object sender, RepeaterItemEventArgs e) {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
            {
                if (Member.InUmbracoMemberMode())
                {
                    Member mem = (Member)e.Item.DataItem;
                    Literal _email = (Literal)e.Item.FindControl("lt_email");
                    Literal _firstname = (Literal)e.Item.FindControl("lt_firstname");
                    Literal _lastname = (Literal)e.Item.FindControl("lt_lastname");

                    _firstname.Text = mem.getProperty("firstName").Value.ToString();
                    _lastname.Text = mem.getProperty("lastName").Value.ToString();
                    _email.Text = mem.Email;

                }
                else
                {
                    Member mem = (Member)e.Item.DataItem;
                    Literal _email = (Literal)e.Item.FindControl("lt_email");
                    Literal _firstname = (Literal)e.Item.FindControl("lt_firstname");
                    Literal _lastname = (Literal)e.Item.FindControl("lt_lastname");

                    _firstname.Text = mem.getProperty("firstName").ToString();
                    _lastname.Text = mem.getProperty("lastName").ToString();
                    _email.Text = mem.Email;
                }
            }
        }

    }
}
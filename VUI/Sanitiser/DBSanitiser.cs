using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using VUI.classes;

namespace VUI.Sanitiser
{
    public class DBSanitiser
    {
        public static void Sanitise()
        {
            // Manual Task - Sanitise Users in Umbraco

            // Delete all Member information
            List<Member> ms = Member.GetAllAsList().ToList();
            foreach (Member m in ms)
            {
                Membership.DeleteUser(m.LoginName, true);
            }

            // Add new VUI administrator and 5 associated users
            CreateVUIuser("VUI Admin", "VUI Admin", "vuiadmin@vodprofessional.com");
            CreateVUIuser("VUI User1", "VUI User1", "vuiuser1@vodprofessional.com");
            CreateVUIuser("VUI User2", "VUI User2", "vuiuser2@vodprofessional.com");
            CreateVUIuser("VUI User3", "VUI User3", "vuiuser3@vodprofessional.com");
            CreateVUIuser("VUI User4", "VUI User4", "vuiuser4@vodprofessional.com");
            CreateVUIuser("VUI User5", "VUI User5", "vuiuser5@vodprofessional.com");

            MembershipUser u = Membership.GetUser("vuiadmin@vodprofessional.com");
            Member ma = new Member((int)u.ProviderUserKey);

            Roles.RemoveUserFromRole("vuiadmin@vodprofessional.com", "vui_user");
            Roles.AddUserToRole("vuiadmin@vodprofessional.com", "vui_administrator");
            ma.getProperty("vuiJoinDate").Value = DateTime.Now;
            ma.getProperty("vuiFullyPaidUp").Value = true;
            ma.getProperty("vuiEndDate").Value = DateTime.Now.AddYears(1);
            ma.getProperty("vuiSubscriptionPackage").Value = "VUI Test";
            ma.getProperty("vUINumberOfUsersAllowed").Value = -1;
            ma.getProperty("vuiAdministrator").Value = null;
            ma.Save();

            SetVUIAdmin("vuiuser1@vodprofessional.com", ma.Id);
            SetVUIAdmin("vuiuser2@vodprofessional.com", ma.Id);
            SetVUIAdmin("vuiuser3@vodprofessional.com", ma.Id);
            SetVUIAdmin("vuiuser4@vodprofessional.com", ma.Id);
            SetVUIAdmin("vuiuser5@vodprofessional.com", ma.Id);

            string sql;

            // Delete all user favourite images
            sql = @"truncate table vui_UserFavouriteImages;
                    delete from vui_StoreRatingComments where ServiceId NOT IN (select TOP 1 ID from vui_ServiceMasters where ServiceName = '4oD');
                    delete from vui_ServiceStoreRating where ServiceId NOT IN (select TOP 1 ID from vui_ServiceMasters where ServiceName = '4oD');";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        private static void SetVUIAdmin(string uname, int adminid)
        {
            // Get the current admin
            MembershipUser u = Membership.GetUser(uname);
            Member m = new Member((int)u.ProviderUserKey);
            m.getProperty("vuiAdministrator").Value = adminid;
            m.Save();
        }


        private static void CreateVUIuser(string firstname, string lastname, string email)
        {
            // Generate Password
            string pwd = "vp2014";
            // Create user
            try
            {
                Member m;
                if (Membership.FindUsersByName(email).Count > 0)
                {
                    MembershipUser eu = Membership.FindUsersByName(email)[email];
                    Roles.AddUserToRole(email, "vui_user");
                    m = new Member((int)eu.ProviderUserKey);
                    m.Save();
                }

                MembershipUser mu = Membership.CreateUser(email, pwd, email);
                Roles.AddUserToRole(email, "vui_user");
                m = new Member((int)mu.ProviderUserKey);
                m.getProperty("firstName").Value = firstname;
                m.getProperty("lastName").Value = lastname;
                m.getProperty("fullName").Value = firstname + " " + lastname;
                m.Save();
            }
            catch (Exception e)
            {
                ;
            }
        }

    }
}
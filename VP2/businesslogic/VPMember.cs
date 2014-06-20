using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using System.Configuration;
using umbraco.cms.businesslogic.web;
using Newtonsoft.Json;
using System.Net;
using System.Data.SqlClient;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Net.Mail;
using System.IO;
using System.Web.UI;
using umbraco.BusinessLogic;
using VPCommon;

namespace VP2.businesslogic
{
    public class VPMember
    {
        public static bool MemberLogin(string username, string password, bool rememberMyUsername)
        {
            if (rememberMyUsername)
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("vrl", "Y"));
            }
            else
            {
                if (HttpContext.Current.Request.Cookies["vrl"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("vrl");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
            return MemberLogin(username, password);
        }


        public static bool MemberLogin(string username, string password)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser == null)
            {
                log.Debug("Logging in: NO CURRENT USER");


                if (Membership.ValidateUser(username, password))
                {
                    log.Debug("Logging in: Valid username or password - " + username);

                    currentUser = Membership.GetUser(username);
                    string[] roles = Roles.GetRolesForUser(username);
                    HttpContext.Current.Session["userroles"] = roles;
                    FormsAuthentication.SetAuthCookie(username, true);
                    Member m = new Member((int)currentUser.ProviderUserKey);

                    DateTime loginDate = DateTime.Now;

                    // Current Login Date will be moved to Last Login Date
                    if (m.getProperty("vuiCurrentLoginDate").Value != null)
                    {
                        m.getProperty("vuiLastLogin").Value = m.getProperty("vuiCurrentLoginDate").Value;
                    }
                    m.getProperty("vuiCurrentLoginDate").Value = loginDate;
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("uid", MemberVUIStatus(m)));
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("vid", m.Id.ToString()));
                    return true;
                }
                else if (Membership.FindUsersByName(username).Count > 0)
                {
                    log.Debug("Logging in: Multiple Users with username - " + username);

                    return false;
                    throw new Exception("Username or password");
                }
                else
                {
                    log.Debug("Logging in: Invalid username or password - " + username);

                    return false;
                    throw new Exception("Username or password");
                }
            }
            else
            {
                log.Debug("Logging in: CURRENT USER? " + currentUser.Email);
            }

            return false;
            throw new Exception("Username or password");
        }

        public static bool MemberLogout(Member m)
        {
            if (m != null)
            {
                Member.RemoveMemberFromCache(m.Id);
                Member.ClearMemberFromClient(m.Id);
            }
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Request.Cookies["uid"] != null)
            {
                HttpCookie myCookie = new HttpCookie("uid");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
            bool rememberUserId = false;
            if (HttpContext.Current.Request.Cookies["vrl"] != null)
            {
                if (HttpContext.Current.Request.Cookies["vrl"].Value.Equals("Y"))
                {
                    rememberUserId = true;
                }
            }
            if (!rememberUserId)
            {
                if (HttpContext.Current.Request.Cookies["vid"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("vid");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
            return true;
        }



        public static string GetCompany(Member m)
        {
            string orgName = "";

            if (m != null)
            {
                string username = m.LoginName;
                string[] roles = Roles.GetRolesForUser(username);
                if (roles.Contains("vui_user"))
                {
                    int adminId = -1;
                    if (Int32.TryParse(m.getProperty("vuiAdministrator").Value.ToString(), out adminId))
                    {
                        Member admin = new Member(adminId);
                        if (admin.getProperty("companyName") != null)
                        {
                            orgName = admin.getProperty("companyName").Value.ToString();
                        }
                        else
                        {
                            orgName = "Not Set - admin [" + admin.Id + "]";
                        }
                    }
                }
                else if (roles.Contains("vui_administrator"))
                {
                    if (m.getProperty("companyName") != null)
                    {
                        orgName = m.getProperty("companyName").Value.ToString();
                    }
                    else
                    {
                        orgName = "Not Set - admin [" + m.Id + "]";
                    }
                }
            }
            return orgName;
        }


        public string[] UserRoles
        {
            get
            {
                string[] roles = null;
                if (User != null)
                {
                    roles = Roles.GetRolesForUser(User.LoginName);
                }
                return roles;
            }
        }

        public static string MemberVUIStatus(Member m)
        {
            string vuiViewerType = VUI_USERTYPE_NONE;

            if (m != null)
            {
                string username = m.LoginName;
                string[] roles = Roles.GetRolesForUser(username);
                if (roles.Contains("registrant") || roles.Contains("active"))
                {
                    vuiViewerType = VUI_USERTYPE_REGISTRANT;
                }
                if (roles.Contains("vui_user"))
                {
                    // add error handling
                    int adminId = -1;
                    if (Int32.TryParse(m.getProperty("vuiAdministrator").Value.ToString(), out adminId))
                    {
                        Member admin = new Member(adminId);
                        if (admin.getProperty("vuiFullyPaidUp") == null || (int)admin.getProperty("vuiFullyPaidUp").Value == 0)
                        {
                            vuiViewerType = VUI_USERTYPE_USER_NOTPAID;
                        }
                        else
                        {
                            vuiViewerType = VUI_USERTYPE_USER;
                        }
                    }
                }
                if (roles.Contains("vui_administrator"))
                {
                    if (m.getProperty("vuiFullyPaidUp") == null || (int)m.getProperty("vuiFullyPaidUp").Value == 0)
                    {
                        vuiViewerType = VUI_USERTYPE_ADMIN_NOTPAID;
                    }
                    else
                    {
                        vuiViewerType = VUI_USERTYPE_ADMIN;
                    }
                }

                if (HttpContext.Current.Request.Cookies["uid"] == null || String.IsNullOrEmpty(HttpContext.Current.Request.Cookies["uid"].Value) || !HttpContext.Current.Request.Cookies["uid"].Value.Equals(vuiViewerType))
                {
                    log.Debug("Updating UserType Cookie for " + m.LoginName + " to " + vuiViewerType);
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("uid", vuiViewerType));
                }
            }
            else
            {
                if (HttpContext.Current.Request.Cookies["uid"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("uid");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
            return vuiViewerType;
        }


        public static VPMember GetLoggedInUser()
        {
            Member m = Member.GetCurrentMember();
            if (m != null)
            {
                return new VPMember() { User = m, Email = m.Email, FirstName = m.getProperty("firstName").Value.ToString(), LastName = m.getProperty("lastName").Value.ToString() };
            }
            else return null;
        }

        

        public static bool RecoverPassword(string email)
        {
            string pwd = Membership.GeneratePassword(8, 1);
            pwd = Regex.Replace(pwd, @"[^a-zA-Z0-9\?\!\$\&]", m => "9");

            if (Membership.FindUsersByName(email).Count > 0)
            {
                MembershipUser eu = Membership.FindUsersByName(email)[email];
                Member m = new Member((int)eu.ProviderUserKey);
                m.Password = pwd;
                m.Save();

                Emailer em = new Emailer("EMAIL_RESET_PWD");
                em.ReplaceMemberElements(m);
                em.ReplaceElement("#PWD#", pwd);
                em.Send(m);


                /*
                string template;
                string templateFullPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["VUI_password_reset_email_template_path"]);

                if (!File.Exists(templateFullPath))
                    throw new ArgumentException("Template file does not exist: " + templateFullPath);

                using (StreamReader reader = new StreamReader(templateFullPath))
                {
                    template = reader.ReadToEnd();
                    reader.Close();
                }


                string emailBody = template;
                emailBody = emailBody
                                .Replace("#USERNAME#", m.getProperty("firstName").Value.ToString())
                                .Replace("#PWD#", pwd);

                log.Debug("Emailing: " + emailBody);
                SmtpClient smtp = new SmtpClient();
                MailMessage msg = new MailMessage();
                msg.To.Add(m.Email);
                msg.Subject = "VOD Professional password reminder";
                msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");

                msg.Body = emailBody;
                smtp.Send(msg);
                */
                return true;
            }
            else
            {
                return false;
            }
        }


        public VPMember() { }

        public Member User { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VPMember));

        public static string VUI_USER_OR_PWD = "Bad username or password";
        public static string VUI_USERTYPE_NONE = "vui_invalid";
        public static string VUI_USERTYPE_ADMIN = "vui_administrator";
        public static string VUI_USERTYPE_ADMIN_NOTPAID = "vui_administrator_not_paid";
        public static string VUI_USERTYPE_ADMIN_EXPIRED = "vui_administrator_expired";
        public static string VUI_USERTYPE_USER = "vui_user";
        public static string VUI_USERTYPE_USER_NOTPAID = "vui_user_admin_not_paid";
        public static string VUI_USERTYPE_USER_EXPIRED = "vui_user_admin_expired";
        public static string VUI_USERTYPE_REGISTRANT = "vui_user_type_registrant";

        public static string VUI_USERADMIN_STATUS_SUCCESS = "success";
        public static string VUI_USERADMIN_STATUS_EXISTS = "exists";
        public static string VUI_USERADMIN_STATUS_FAILED = "failed";
        
    }
}
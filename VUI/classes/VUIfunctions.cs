using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.MacroEngines;
using umbraco.NodeFactory;
using System.Web.Security;
using System.Web.SessionState;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.datatype;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Net.Mail;
using System.Globalization;


namespace VUI.classes
{
    public static class VUIfunctions
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUIfunctions));


        static int umb_vuiFolderRoot = 0;

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
        

        public static string VUI_FOLDERTYPE = "VUI_Folder";
        public static string VUI_IMAGETYPE = "VUI_Image";
        public static string VUI_PLATFORM = "platform";
        public static string VUI_DEVICE = "device";
        public static string VUI_SERVICE = "service";

        static string VUI_mediafolder = String.Empty;
        static string VUI_previewservice = String.Empty;
        static string VUI_pagetype_scores = String.Empty;
        public static int VUI_pagetypelist;
        static int VUI_function_list;
        
        public static string VUI_confirm_page;
        public static string VUI_confirm_email_template_path;
        public static string VUI_registration_notification_email_template_path;
        public static string VUI_subscription_complete_email_template_path;
        public static string VUI_password_reset_email_template_path;
        public static string VUI_admin_page;
        public static string VUI_subscribe_page;
        public static string VUI_checkout_page;
        public static string VUI_loginregister_page;
        public static string VUI_subscribecomplete_page;
        public static string VUI_preview_benchmark_url;
        
        public static int VUI_information_root;
        public static int VUI_codesnippet_unauth;
        public static int VUI_codesnippet_reg;
        public static int VUI_codesnippet_subs;
        public static int VUI_codesnippet_buy;
        public static int VUI_transaction_log;
        public static int VUI_product_list;
        public static int VUI_invoicestatus_list;

        public static int VUI_paypal_form;
        public static string VUI_paypal_use_sandbox;
        public static string VUI_paypal_ac;
        public static string VUI_paypal_return;
        public static string VUI_paypal_notify_url;

        static Dictionary<string, int> scores = null;


        public static User u = User.GetAllByLoginName("websitecontentuser", false).First();
            //new User("websitecontentuser");

        static VUIfunctions()
        {
            umb_vuiFolderRoot = Int32.Parse(ConfigurationManager.AppSettings["umb_vuiFolderRoot"].ToString());
            VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~","");
            VUI_previewservice = ConfigurationManager.AppSettings["VUI_previewservice"].ToString();
            VUI_pagetypelist = Int32.Parse(ConfigurationManager.AppSettings["VUI_pagetypelist"].ToString());
            VUI_function_list = Int32.Parse(ConfigurationManager.AppSettings["VUI_function_list"].ToString());
            VUI_invoicestatus_list = Int32.Parse(ConfigurationManager.AppSettings["VUI_invoicestatus_list"].ToString());
            VUI_pagetype_scores = ConfigurationManager.AppSettings["VUI_pagetype_scores"].ToString();

            VUI_codesnippet_unauth = Int32.Parse(ConfigurationManager.AppSettings["VUI_codesnippet_unauth"].ToString());
            VUI_codesnippet_reg = Int32.Parse(ConfigurationManager.AppSettings["VUI_codesnippet_reg"].ToString());
            VUI_codesnippet_subs = Int32.Parse(ConfigurationManager.AppSettings["VUI_codesnippet_subs"].ToString());
            VUI_codesnippet_buy = Int32.Parse(ConfigurationManager.AppSettings["VUI_codesnippet_buy"].ToString());
            VUI_information_root = Int32.Parse(ConfigurationManager.AppSettings["VUI_information_root"].ToString());
            
            VUI_login_page = ConfigurationManager.AppSettings["VUI_login_page"].ToString();
            VUI_loginregister_page = ConfigurationManager.AppSettings["VUI_loginregister_page"].ToString();
            VUI_subscribe_page = ConfigurationManager.AppSettings["VUI_subscribe_page"].ToString();
            VUI_checkout_page = ConfigurationManager.AppSettings["VUI_checkout_page"].ToString();
            VUI_subscribecomplete_page = ConfigurationManager.AppSettings["VUI_subscribecomplete_page"].ToString();

            VUI_confirm_page = ConfigurationManager.AppSettings["VUI_confirm_page"].ToString();
            VUI_admin_page = ConfigurationManager.AppSettings["VUI_admin_page"].ToString();
            VUI_preview_benchmark_url = ConfigurationManager.AppSettings["VUI_preview_benchmark_url"].ToString();

            VUI_confirm_email_template_path = ConfigurationManager.AppSettings["VUI_confirm_email_template_path"].ToString();
            VUI_registration_notification_email_template_path = ConfigurationManager.AppSettings["VUI_registration_notification_email_template_path"].ToString();
            VUI_subscription_complete_email_template_path = ConfigurationManager.AppSettings["VUI_subscription_complete_email_template_path"].ToString();
            VUI_password_reset_email_template_path = ConfigurationManager.AppSettings["VUI_password_reset_email_template_path"].ToString();

            VUI_transaction_log = Int32.Parse(ConfigurationManager.AppSettings["VUI_transaction_log"].ToString());
            VUI_product_list = Int32.Parse(ConfigurationManager.AppSettings["VUI_product_list"].ToString());

            VUI_paypal_form = Int32.Parse(ConfigurationManager.AppSettings["VUI_paypal_form"].ToString());
            VUI_paypal_use_sandbox = ConfigurationManager.AppSettings["VUI_paypal_use_sandbox"].ToString();
            VUI_paypal_ac = ConfigurationManager.AppSettings["VUI_paypal_ac"].ToString(); ;
            VUI_paypal_return = ConfigurationManager.AppSettings["VUI_paypal_return"].ToString(); ;
            VUI_paypal_notify_url = ConfigurationManager.AppSettings["VUI_paypal_notify_url"].ToString(); 
        }

        public static string VUIPreviewService { get { return VUI_previewservice; } }

        public static User UpdateUser { get { return u; } }
        public static int TotalFunctionScore { get; set; }

        public static int VUIMediaRootNode { get { return umb_vuiFolderRoot; } }
        public static string VUI_login_page { get; private set; }


   

        public static void InitScores()
        {
            XPathNodeIterator iterator = umbraco.library.GetPreValues(VUI_function_list);
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
            TotalFunctionScore = preValues.Count;


            /*
            scores = new Dictionary<string, int>();
            string[] scoreitems = VUI_pagetype_scores.Split(';');
            foreach (string scoreitem in scoreitems)
            {
                string[] pagetypescore = scoreitem.Split(',');
                scores.Add(pagetypescore[0],Int32.Parse(pagetypescore[1]));
            }
            scores.Add("", 0);
             * */
        }

        public static void UpdateAllServicePageTypesScore()
        {
            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            /*
            XPathNodeIterator iterator = umbraco.library.GetPreValues(VUIfunctions.VUI_function_list);
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");

            List<VUI.classes.VUIfunctions.Preval> prevals = new List<VUI.classes.VUIfunctions.Preval>();
            int sort = 0;

            while (preValues.MoveNext())
            {
                prevals.Add(new VUI.classes.VUIfunctions.Preval(sort++, preValues.Current.Value));
            }
            */
            List<DynamicNode> nodes = node.Descendants(VUI_FOLDERTYPE).Items
                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE)).ToList();
            foreach (DynamicNode s in nodes)
            {
                int currentscore = 0;
                string[] capabilities = s.GetProperty("serviceCapabilities").Value.ToString().Split(',');

                if (s.GetProperty("vuiScore") != null)
                {
                    Int32.TryParse(s.GetProperty("vuiScore").Value, out currentscore);
                }
                int score = capabilities.Length;

                if (score != currentscore || s.GetProperty("vuiScore") == null)
                {
                    Document d = new Document(s.Id);
                    d.getProperty("vuiScore").Value = score;
                    d.Save();
                    d.Publish(u);
                    umbraco.library.UpdateDocumentCache(d.Id);
                }
            }
        }


        public static void UpdateServiceBenchmarkDateDevice(int rootId, DateTime bDate, int device)
        {
            DynamicNode node = new DynamicNode(rootId);
            List<DynamicNode> nodes = node.Descendants("VUI_Folder").Items.Where(n => n.GetProperty("folderLevel").Value.Equals("service")).ToList();
            foreach (DynamicNode s in nodes)
            {
                int score = 0;
                if (s.GetProperty("vuiScore") != null)
                {
                    Int32.TryParse(s.GetProperty("vuiScore").Value, out score);
                }
                if (score > 3)
                {
                    Document d = new Document(s.Id);
                    if(s.GetProperty("benchmarkDate") == null || String.IsNullOrEmpty(s.GetProperty("benchmarkDate").Value))
                    {
                        d.getProperty("benchmarkDate").Value = bDate;
                    }
                    if(s.GetProperty("benchmarkDevice") == null || String.IsNullOrEmpty(s.GetProperty("benchmarkDevice").Value))
                    {  
                        d.getProperty("benchmarkDevice").Value = device;
                    }
                    d.Save();
                    d.Publish(u);
                    umbraco.library.UpdateDocumentCache(d.Id);
                }
            }
        }



        public static int GetServicePageTypesScore(int id)
        {
            if (TotalFunctionScore == null)
                InitScores();
            
            DynamicNode service = new DynamicNode(id);
            int totalscore = 0;

            string[] capabilities = service.GetProperty("serviceCapabilities").Value.ToString().Split(',');
            totalscore = capabilities.Length;
            /*
            List<DynamicNode> images = service.Descendants().Items.ToList<DynamicNode>();

            int totalscore = 0;

            foreach (DynamicNode image in images)
            {
                if (image.GetProperty("pageType") != null)
                {
                    try
                    {

                        string pagetype = image.GetProperty("pageType").Value;
                        totalscore += scores[pagetype];
                    }
                    catch (KeyNotFoundException knfe)
                    {
                        totalscore += 0;
                    }
                }
            }
            */
            return totalscore;
        }

        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email)
        {
            string MatchEmailPattern = @"^([\.\-_a-zA-Z0-9])+@(([a-zA-Z0-9\-])+.)+([a-zA-Z0-9]{2,4})+$";
            /*
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" + 
                @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\." + 
                @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|" + 
                @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
            */

            if (email != null)
            {
                return Regex.IsMatch(email.Replace("\'",String.Empty), MatchEmailPattern);
            }
            else
            {
                return false;
            }
        }

        public static void SendErrorEmail(string message, Exception ex)
        {
            SmtpClient smtp = new SmtpClient();
            MailMessage msg = new MailMessage();
            msg.To.Add("oliverwood@vodprofessiona.com");
            // msg.Bcc.Add("admin@vodprofessional.com");
            msg.Subject = "Welcome to VOD Professional";
            msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");
            msg.Body = "ERROR\n\n" + message + "\n\n" + ex.Message + "\n" + ex.StackTrace;
            smtp.Send(msg);
        }

        public static bool SendSubsConfirmEmail(Member m)
        {
            string emailBody = GetEmailTemplate(HttpContext.Current.Server.MapPath(VUI_subscription_complete_email_template_path));
            emailBody = emailBody
                            .Replace("#USERNAME#", m.getProperty("fullName").Value.ToString())
                            ;
            log.Debug("Emailing: " + emailBody);

            SmtpClient smtp = new SmtpClient();
            MailMessage msg = new MailMessage();
            msg.To.Add(m.Email);
            msg.Bcc.Add("kauserkanji@vodprofessional.com");
            msg.Bcc.Add("oliverwood@vodprofessional.com");
            msg.Subject = "Welcome to VOD Professional";
            msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");
            msg.Body = emailBody;
            smtp.Send(msg);
            return true;
        }

        public static bool SendRegConfirmEmail(Member m)
        {
            string emailBody = GetEmailTemplate(HttpContext.Current.Server.MapPath(VUI_registration_notification_email_template_path));
            emailBody = emailBody
                            .Replace("#USERNAME#", m.getProperty("fullName").Value.ToString())
                            ;
            log.Debug("Emailing: " + emailBody);

            SmtpClient smtp = new SmtpClient();
            MailMessage msg = new MailMessage();
            msg.To.Add(m.Email);
            // msg.Bcc.Add("admin@vodprofessional.com");
            msg.Subject = "Welcome to VOD Professional";
            msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");
            msg.Body = emailBody;
            smtp.Send(msg);
            return true;
        }

        public static bool SendPwdResetEmail(string email)
        {
            string pwd = Membership.GeneratePassword(8, 1);

            if (Membership.FindUsersByName(email).Count > 0)
            {
                MembershipUser eu = Membership.FindUsersByName(email)[email];
                Member m = new Member((int)eu.ProviderUserKey);
                m.Password = pwd;
                m.Save();

                string emailBody = GetEmailTemplate(HttpContext.Current.Server.MapPath(VUI_password_reset_email_template_path));
                emailBody = emailBody
                                .Replace("#USERNAME#", m.getProperty("fullName").Value.ToString())
                                .Replace("#PWD#", pwd)
                                ;

                log.Debug("Emailing: " + emailBody);
                SmtpClient smtp = new SmtpClient();
                MailMessage msg = new MailMessage();
                msg.To.Add(m.Email);
                // msg.Bcc.Add("admin@vodprofessional.com");
                msg.Subject = "VOD Professional password reminder";
                msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");
                
                msg.Body = emailBody;
                smtp.Send(msg);
                return true;

            }
            else
            {
                return false;
            }
        }

        static string[] euList = new string[] {"Austria","Belgium","Bulgaria","Cyprus","Czech Republic","Denmark","Estonia","Finland","France","Germany","Greece","Hungary",
                "Ireland","Italy","Latvia","Lithuania","Luxembourg","Malta","Netherlands","Poland","Portugal","Romania","Slovac Republic","Slovenia","Spain","Sweden" };
        static string[] vatCountries = new string[] { "United Kingdom" };


        public static bool CountryIsVATExempt(string country)
        {
            if (vatCountries.Contains(country))
            {
                return false;
            }
            else if (euList.Contains(country))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CountryIsInEU(string country)
        {
            return euList.Contains(country);
        }


        public static bool PromotionIsValid(DynamicNode promo)
        {
            bool isValid = false;

            DateTime startDate = DateTime.Parse(promo.GetProperty("promotionStartDate").Value);
            DateTime endDate = DateTime.Parse(promo.GetProperty("promotionEndDate").Value);

            if (DateTime.Now >= startDate && DateTime.Now <= endDate)
            {
                isValid = true;
            }
            log.Debug("Promotion " + promo.GetProperty("promotionCode").Value + " is not valid");
            return isValid;
        }


        public static string StringToCurrency(string numString)
        {
            double figure;
            string outstr = numString;
            if (double.TryParse(numString, out figure))
            {
                NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                nfi = (NumberFormatInfo)nfi.Clone();

                nfi.CurrencySymbol = "";
                outstr = string.Format(nfi, "{0:c}", figure);
            }
            return outstr;
        }

        public static string CreateVUIuser(string firstname, string lastname, string email)
        {
            // Generate Password
            string pwd = Membership.GeneratePassword(8, 1);
            // Create user
            try
            {
                Member admin = Member.GetCurrentMember();
                Member m;
                if (Membership.FindUsersByName(email).Count > 0)
                {
                    MembershipUser eu = Membership.FindUsersByName(email)[email];
                    Roles.AddUserToRole(email, "vui_user");
                    m = new Member((int)eu.ProviderUserKey);
                    m.getProperty("vuiAdministrator").Value = admin.Id;
                    m.Save();
                    return VUI_USERADMIN_STATUS_EXISTS;
                }          

                MembershipUser mu = Membership.CreateUser(email, pwd, email);
                Roles.AddUserToRole(email, "vui_user");
                m = new Member((int)mu.ProviderUserKey);
                m.getProperty("firstName").Value = firstname;
                m.getProperty("lastName").Value = lastname;
                m.getProperty("fullName").Value = firstname + " " + lastname;
                m.getProperty("vuiAdministrator").Value = admin.Id;
                m.Save();
                log.Debug("Created new VUI User: " + m.LoginName + " / " + pwd); 

                //Send Confirmation Email

                string emailBody = GetEmailTemplate(HttpContext.Current.Server.MapPath(VUI_confirm_email_template_path));
                emailBody = emailBody
                                .Replace("#ADMINNAME#", admin.getProperty("fullName").Value.ToString())
                                .Replace("#FIRSTNAME#", firstname)
                                .Replace("#USERNAME#", m.LoginName)
                                .Replace("#PWD#", pwd)
                                .Replace("#CONFLINK#", VUIfunctions.VUI_login_page + "?" + HttpUtility.UrlEncode(email))
                                .Replace("#LOGINLINK#", VUIfunctions.VUI_login_page + "!" + HttpUtility.UrlEncode(email))
                                ;
                log.Debug("Emailing: " + emailBody);

                SmtpClient smtp = new SmtpClient();

                MailMessage msg = new MailMessage();
                msg.To.Add(email);
                // msg.Bcc.Add("admin@vodprofessional.com");
                msg.Subject = "You are now a VOD Professional VUI Library user!";
                msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");

                msg.Body = emailBody;
                smtp.Send(msg);

                return VUI_USERADMIN_STATUS_SUCCESS;
            }
            catch (Exception e)
            {
                return e.Message + " " + e.StackTrace + " " + VUI_USERADMIN_STATUS_FAILED;
            }
        }

        private static string GetEmailTemplate(string templateFullPath)
        {
            string template;

            if (!File.Exists(templateFullPath))
                throw new ArgumentException("Template file does not exist: " + templateFullPath);

            using (StreamReader reader = new StreamReader(templateFullPath))
            {
                template = reader.ReadToEnd();
                reader.Close();
            }

            return template;
        }

        public static List<DynamicNode> PlatformList()
        {
            DynamicNode node;
            List<DynamicNode> nodes;
            node = new DynamicNode(umb_vuiFolderRoot);
            nodes = node.GetChildrenAsList.Items.Where(n => n.NodeTypeAlias.Equals(VUI_FOLDERTYPE)).OrderBy(n => n.Name).ToList();
            return nodes;
        }

        public static List<VUIService> CompleteServiceList()
        {
            return CompleteServiceList(null);
        }

        public static List<VUIService> CompleteServiceList(DynamicNode platform)
        {
            List<DynamicNode> nodes;

            if (platform != null)
            {
                nodes = platform.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE))
                                                .OrderBy(n => n.Name)
                                                .ToList();
            }
            else
            {
                DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
                nodes = node.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE))
                                                .OrderBy(n => n.Name)
                                                .ToList();
            }

            List<VUIService> services = new List<VUIService>();


            foreach (DynamicNode n in nodes)
            {
                try
                {
                    int c = n.Descendants().Items.Count;

                    if (c > 0)
                    {
                        if (services.Count(s => s.ServiceName.Equals(n.Name)) == 0)
                        {
                            VUIService service = new VUIService(n.Name, n.Id, 0);
                            services.Add(service);
                        }
                        services.Find(s => s.ServiceName.Equals(n.Name)).NumImages += c;
                    }
                }
                catch (Exception e)
                {
                    // This is where to log some action!  Basically there's been a hang up about the service count
                }
            }
            return services;
        }

        public static List<VUIService> ServiceListByName(string serviceName)
        {
            List<DynamicNode> nodes;

            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            nodes = node.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Equals(serviceName))
                                                .ToList();

            List<VUIService> services = new List<VUIService>();
            foreach (DynamicNode n in nodes)
            {
                string device = n.Parent.Name;
                string platform = n.Parent.Parent.Name;
                VUIService service = new VUIService(n.Name, n.Id, platform, device);
                service.NumImages = n.Descendants().Items.Count;

                if (service.NumImages > 0)
                {
                    service.thumbimage = n.Descendants().Items.First().GetProperty("thFile").Value;
                    services.Add(service);
                }
                
            }
            return services;
        }

        public static List<VUIService> ServiceListByNameAndPlatform(string serviceName, int platformId)
        {
            List<DynamicNode> nodes;

            DynamicNode node = new DynamicNode(platformId);
            nodes = node.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Equals(serviceName))
                                                .ToList();

            List<VUIService> services = new List<VUIService>();
            foreach (DynamicNode n in nodes)
            {
                string device = n.Parent.Name;
                string platform = n.Parent.Parent.Name;
                VUIService service = new VUIService(n.Name, n.Id, platform, device);
                service.SetRatings();
                service.NumImages = n.Descendants().Items.Count;

                if (service.NumImages > 0)
                {
                    service.thumbimage = n.Descendants().Items.First().GetProperty("thFile").Value;
                    services.Add(service);
                }
            }
            return services;
        }


        public static List<VUIService> ServiceListByAllPlatforms()
        {
            List<DynamicNode> nodes;

            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            nodes = node.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE))
                                                .OrderBy(n => n.Name)
                                                .ToList();

            List<VUIService> services = new List<VUIService>();
            foreach (DynamicNode n in nodes)
            {
                try
                {
                    int c = n.Descendants().Items.Count;
                    if (c > 0)
                    {
                        if (services.Count(s => s.ServiceName.Equals(n.Name)) == 0)
                        {
                            VUIService s = new VUIService(n.Name, n.Id, 0);
                            s.thumbimage = n.Descendants().Items.First().GetProperty("thFile").Value;
                            services.Add(s);
                        }
                        services.Find(s => s.ServiceName.Equals(n.Name)).NumImages += c;
                    }
                }
                catch (Exception e)
                {
                    // This is where to log some action!  Basically there's been a hang up about the service count
                }
            }
            return services;
        }


        public static List<VUIService> ServiceListByPlatform(int platformId)
        {
            List<DynamicNode> nodes;

            DynamicNode node = new DynamicNode(platformId);
            nodes = node.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE))
                                                .OrderBy(n => n.Name)
                                                .ToList();

            List<VUIService> services = new List<VUIService>();


            foreach (DynamicNode n in nodes)
            {
                try
                {
                    int c = n.Descendants().Items.Count;

                    if (c > 0)
                    {
                        if (services.Count(s => s.ServiceName.Equals(n.Name)) == 0)
                        {
                            VUIService s = new VUIService(n.Name, n.Id, 0);

                            s.thumbimage = n.Descendants().Items.First().GetProperty("thFile").Value;
                            services.Add(s);
                        }
                        services.Find(s => s.ServiceName.Equals(n.Name)).NumImages += c;
                    }
                }
                catch (Exception e)
                {
                    // This is where to log some action!  Basically there's been a hang up about the service count
                }
            }
            return services;
        }

        

        /// <summary>
        ///   Pass a Platform name, or "All" to return the list of services for that platform
        /// </summary>
        /// <returns></returns>
        public static List<VUIService> ServiceListByPlatform(string platformName)
        {
            return ServiceListByPlatformAndService(platformName, null);
        }

        /// <summary>
        ///   Pass a Platform name and ServiceName, or "All" to return the list of services for that platform
        /// </summary>
        /// <returns></returns>
        public static List<VUIService> ServiceListByPlatformAndService(string platformName, string serviceName)
        {
            List<DynamicNode> nodes;
            DynamicNode platformNode;

            DynamicNode node = new DynamicNode(VUIMediaRootNode);

            if (platformName.ToLower().Equals("all"))
            {
                platformNode = node;
            }
            else
            {
                try
                {
                    platformNode = node.Descendants(VUI_FOLDERTYPE).Items
                                                        .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_PLATFORM) && n.Name.Replace(' ', '-').Equals(platformName))
                                                        .First();
                }
                catch (Exception ex)
                {
                    platformNode = node;
                }
            }

            if(String.IsNullOrEmpty(serviceName))
            {
                nodes = platformNode.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE))
                                                .OrderBy(n => n.Name)
                                                .ToList();
            }
            else
            {
                nodes = platformNode.Descendants(VUI_FOLDERTYPE).Items
                                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Replace(' ', '-').Equals(serviceName))
                                                .OrderBy(n => n.Name)
                                                .ToList();
            }
            List<VUIService> services = new List<VUIService>();

            foreach (DynamicNode n in nodes)
            {
                try
                {
                    int c = n.Descendants().Items.Count;
                    if (c > 0)
                    {
                        if (String.IsNullOrEmpty(serviceName))
                        {
                            if (services.Count(s => s.ServiceName.Equals(n.Name)) == 0)
                            {
                                VUIService s = new VUIService(n.Name, n.Id, 0);
                                s.thumbimage = n.Descendants().Items.First().GetProperty("thFile").Value;
                                services.Add(s);
                            }
                            else
                            {
                                VUIService svc = services.Find(s => s.ServiceName.Equals(n.Name));
                                svc.NumImages += c;
                                svc.ServiceCount += 1;
                            }
                        }
                        else
                        {
                            VUIService s = new VUIService(n.Name, n.Id, 0);
                            s.thumbimage = n.Descendants().Items.First().GetProperty("thFile").Value;
                            services.Add(s);
                        }
                    }
                }
                catch (Exception e)
                {
                    // This is where to log some action!  Basically there's been a hang up about the service count
                }
            }
            return services;
        }


        /// <summary>
        ///   Does the Platform have any devices? If not, go straight from Platform service list to Service/Images
        /// </summary>
        /// <returns></returns>
        public static bool PlatformHasDevices(string platformName)
        {
            if (platformName.ToLower().Equals("all"))
            {
                return true;
            }
            else
            {
                try
                {
                    int devicecount = new DynamicNode(VUIMediaRootNode)
                                                    .Descendants(VUI_FOLDERTYPE).Items
                                                        .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_PLATFORM) && n.Name.Replace(' ', '-').Equals(platformName))
                                                        .First()
                                                        .Descendants(VUI_FOLDERTYPE).Items
                                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_DEVICE))
                                                            .ToList()
                                                            .Count;
                    return (devicecount > 0);
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
        }

        /// <summary>
        ///   Does the Servcice, under the Platform have any devices? If not, go straight from Platform service list to Service/Images
        /// </summary>
        /// <returns></returns>
        public static bool ServiceHasDevicesOnPlatform(string serviceName, string platformName)
        {
            if (platformName.ToLower().Equals("all"))
            {
                try
                {
                    int devicecount = new DynamicNode(VUIMediaRootNode)
                                                        .Descendants(VUI_FOLDERTYPE).Items
                                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Replace(' ', '-').Equals(serviceName))
                                                            .ToList()
                                                            .Count;
                    return (devicecount > 1);
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
            else
            {
                try
                {
                    int devicecount = new DynamicNode(VUIMediaRootNode)
                                                    .Descendants(VUI_FOLDERTYPE).Items
                                                        .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_PLATFORM) && n.Name.Replace(' ', '-').Equals(platformName))
                                                        .First()
                                                        .Descendants(VUI_FOLDERTYPE).Items
                                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Replace(' ', '-').Equals(serviceName))
                                                            .ToList()
                                                            .Count;
                    return (devicecount > 1);
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// USed on the Benchmarking page. Ignores any Services with a score of Zero or with the "isComingSoon" checkbox selected
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static List<VUIService> ServicesOrderedByFunctionScore(string platform, string device)
        {
            DynamicNode root = new DynamicNode(VUIfunctions.VUIMediaRootNode);
            List<DynamicNode> serviceList = null;
            if (!String.IsNullOrEmpty(device))
            {
                serviceList = root.Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                          .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_PLATFORM) && n.Name.Replace(" ", "-").Equals(platform))
                                          .ToList()
                                          .First()
                                            .Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_DEVICE) && n.Name.Replace(" ", "-").Equals(device))
                                            .ToList()
                                            .First()
                                              .Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                              .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_SERVICE))
                                              .ToList();

            }
            else if (platform.ToLower().Equals("all"))
            {
                serviceList = root.Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                              .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_SERVICE))
                                              .ToList();
            }
            else
            {
                serviceList = root.Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                          .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_PLATFORM) && n.Name.Replace(" ", "-").Equals(platform))
                                          .ToList()
                                          .First()
                                              .Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                              .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_SERVICE))
                                              .ToList();

            }
            List<VUIService> services = VUIServiceListFromNodeList(serviceList);

            var orderedServices = from service in services
                                  orderby service.Score descending
                                  select service;
            return orderedServices.Where(os => os.Score > 0 && !os.IsComingSoon).ToList<VUIService>();
        }



        public static string BenchmarkingDescription(string platform, string device)
        {
            string description = String.Empty;
            DynamicNode root = new DynamicNode(VUIfunctions.VUIMediaRootNode);
            DynamicNode devicenode;

            if (!String.IsNullOrEmpty(device))
            {
                devicenode = root.Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                          .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_PLATFORM) && n.Name.Replace(" ", "-").Equals(platform))
                                          .ToList()
                                          .First()
                                            .Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_DEVICE) && n.Name.Replace(" ", "-").Equals(device))
                                            .ToList()
                                            .First();
            }
            else if (platform.ToLower().Equals("all"))
            {
                devicenode = root;
            }
            else
            {
                devicenode = root.Descendants(VUIfunctions.VUI_FOLDERTYPE).Items
                                          .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_PLATFORM))
                                          .ToList()
                                          .First();
            }
            if (devicenode.GetProperty("description") != null)
            {
                description = devicenode.GetProperty("description").Value;
            }
            return description;
        }

        /// <summary>
        /// Utility Function to Convert Nodes to VUIServices
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static List<VUIService> VUIServiceListFromNodeList(List<DynamicNode> nodes)
        {
            List<VUIService> services = new List<VUIService>();
            foreach(DynamicNode n in nodes)
            {
                services.Add(new VUIService(n.Id));
            }
            return services;
        }


        /// <summary>
        ///  Get a full Service Description, including images for a named service
        /// </summary>
        /// <param name="platformName"></param>
        /// <param name="serviceName"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static VUIService ServiceDescription(string platformName, string serviceName, string deviceName)
        {
            
            DynamicNode serviceNode;

            if (!String.IsNullOrEmpty(deviceName))
            {
                serviceNode = new DynamicNode(VUIMediaRootNode).Descendants(VUI_FOLDERTYPE).Items
                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_PLATFORM) && n.Name.Replace(' ', '-').Equals(platformName))
                                .First()
                                .Descendants(VUI_FOLDERTYPE).Items
                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_DEVICE) && n.Name.Replace(' ', '-').Equals(deviceName))
                                .First()
                                .Descendants(VUI_FOLDERTYPE).Items
                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Replace(' ', '-').Equals(serviceName))
                                .First();
            }
            else
            {
                serviceNode = new DynamicNode(VUIMediaRootNode).Descendants(VUI_FOLDERTYPE).Items
                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_PLATFORM) && n.Name.Replace(' ', '-').Equals(platformName))
                                .First()
                                .Descendants(VUI_FOLDERTYPE).Items
                                .Where(n => n.GetProperty("folderLevel").Value.Equals(VUI_SERVICE) && n.Name.Replace(' ', '-').Equals(serviceName))
                                .First();
            }

            
            VUIService s = new VUIService(serviceNode.Name, serviceNode.Id, 0);

            if (ServiceIsPreviewable(serviceNode.Name) || MemberVUIFullMode(MemberVUIStatus(CurrentUser())))
            {
                s.SetRatings();
                s.SetImages();
            }
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static Dictionary<string, int> ScoresForServiceList(List<VUIService> services)
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();

            if (services != null)
            {
                // Set up the Scores Dictionary
                XPathNodeIterator iterator = umbraco.library.GetPreValues(VUI_function_list);
                iterator.MoveNext(); //move to first
                XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
                while (preValues.MoveNext())
                {
                    scores.Add(preValues.Current.Value.Trim(), 0);
                }

                foreach (VUIService s in services)
                {
                    if (s.Capabilities != null)
                    {
                        foreach (string c in s.Capabilities)
                        {
                            try
                            {
                                scores[c] += 1;
                            }
                            catch (Exception ex1)
                            { ;}
                        }
                    }
                }
            }
            return scores;
        }



        /// <summary>
        ///   Get Cleaned Name of VUI_Folder
        /// </summary>
        /// <returns></returns>
        public static string GetTidyVUIFolderName(string nameToCompare, string folderLevel)
        {
            if (String.IsNullOrEmpty(nameToCompare) || !nameToCompare.Contains("-"))
            {
                return nameToCompare;
            }

            try
            {
                string cleanName = new DynamicNode(VUIMediaRootNode)
                                                .Descendants(VUI_FOLDERTYPE).Items
                                                    .Where(n => n.GetProperty("folderLevel").Value.Equals(folderLevel) && n.Name.Replace(' ', '-').Equals(nameToCompare))
                                                    .First()
                                                    .Name;
                return cleanName;
            }
            catch (Exception ex)
            {
                return nameToCompare;
            }
        }



        public static string ServiceListByAllPlatformsToJson()
        {
            return ServiceListToJson(ServiceListByAllPlatforms());
        }
    
        public static string ServiceListByNameAndPlatformToJson(string serviceName, int platformId)
        {
            return ServiceListToJson(ServiceListByNameAndPlatform(serviceName, platformId));
        }

        public static string ServiceListByPlatformToJson(int platformId)
        {
            return ServiceListToJson(ServiceListByPlatform(platformId));
        }

        public static string ServiceListByNameToJson(string serviceName)
        {
            return ServiceListToJson(ServiceListByName(serviceName));
        }

        public static string ServiceListToJson(List<VUIService> services)
        {
            StringBuilder s = new StringBuilder();
            s.Append(@"[");
            foreach (VUIService service in services)
            {

                s.Append(service.GetJson());
                s.Append(",");
            }
            s.Remove(s.Length - 1, 1);
            s.Append(@"]");
            return s.ToString();
        }

        public class Preval
        {
            public int Order { get; set; }
            public string Val { get; set; }
            public Preval(int order, string val) { Order = order; Val = val; }
        }

        public static string ServiceWithImagesToJson(int serviceId)
        {
            XPathNodeIterator iterator = umbraco.library.GetPreValues(VUI_pagetypelist);
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
            
            List<Preval> prevals=  new List<Preval>();
            int sort = 0;

            while (preValues.MoveNext())
            {
                prevals.Add(new Preval(sort++, preValues.Current.Value));
            }
            prevals.Add(new Preval(sort++, ""));


            DynamicNode serviceNode = new DynamicNode(serviceId);
            List<DynamicNode> imageNodes = serviceNode.Descendants(VUI_IMAGETYPE).Items.ToList();

            var orderedImages = from image in imageNodes
                                join preval in prevals
                                    on image.GetProperty("pageType").Value equals preval.Val
                                orderby preval.Order
                                select image;


            string imageJson = ImageListToJson(NodeListToVUIImages(orderedImages.ToList<DynamicNode>()));

            VUIService s = new VUIService(serviceNode.Name, serviceId, 0);
            s.SetRatings();
            return s.GetJsonInjectImages(imageJson);
        }

        public static string ImageListJson(int parentId)
        {
            DynamicNode node = new DynamicNode(parentId);
            List<DynamicNode> nodes = node.Descendants(VUI_IMAGETYPE).Items.ToList();
            return ImageListToJson(NodeListToVUIImages(nodes));
        }

        public static string ImageListJson(string serviceName)
        {
            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            List<DynamicNode> nodes = node.Descendants(VUI_IMAGETYPE).Items.Where(n => n.GetProperty("service").Value.Equals(serviceName)).ToList();
            return ImageListToJson(NodeListToVUIImages(nodes));
        }

        private static string ImageListToJson(List<VUIImage> images)
        {
            StringBuilder s = new StringBuilder();
            s.Append(@"[");
            foreach(VUIImage image in images)
            {
                s.Append(image.GetJson());
                s.Append(",");
            }
            s.Remove(s.Length-1, 1);
            s.Append(@"]");
            return s.ToString();
        }


        public static List<VUIImage> NodeListToVUIImages(List<DynamicNode> nodes)
        {
            List<VUIImage> images = new List<VUIImage>();
            foreach (DynamicNode n in nodes)
            {
                images.Add(new VUIImage(n));
            }
            return images;
        }


        public static Member GetVUIAdminForCurrentUser()
        {
            Member admin;
            try
            {
                int id = (int)CurrentUser().getProperty("vuiAdministrator").Value;
                admin = new Member(id);
            }
            catch (Exception e)
            {
                admin = null;
            }
            return admin;
        }


        

        public static List<Member> GetVUIAdmins()
        {
            MemberGroup mg = MemberGroup.GetByName("vui_administrator");
            List<Member> ms = mg.GetMembers("%").ToList<Member>();
            // return Roles.FindUsersInRole("vui_administrator", "%");
            return ms;
        }

        public static List<Member> GetVUIUsersForAdmin(Member admin)
        {
            int vuiAdministratorId = admin.Id;
            MemberGroup mg = MemberGroup.GetByName("vui_user");
            List<Member> ms = mg.GetMembers("%").ToList<Member>().Where(m => m.getProperty("vuiAdministrator").Value.Equals(vuiAdministratorId)).ToList();
            return ms;

        }

        public static bool MemberVUIFullMode(String status)
        {
            if(status.Equals(VUI_USERTYPE_USER) || status.Equals(VUI_USERTYPE_ADMIN))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool ServiceIsPreviewable(string service)
        {
            return service.Equals(VUI_previewservice);
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



        public static bool CurrentMemberIsVUIAdmin()
        {
            bool isVUIAdmin = false;

            Member m = CurrentUser();
            if(m != null)
            {
                string username = m.LoginName;
                string[] roles = Roles.GetRolesForUser(username);
                if (roles.Contains("vui_administrator"))
                {
                    isVUIAdmin = true;
                }
            }
            return isVUIAdmin;
        }

        public static Member CurrentUser()
        {
            Member m = Member.GetCurrentMember();
            return m;
        }

        public static bool MemberLoginAuto(string username)
        {
            log.Debug("Auto Login:");
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser == null)
            {
                log.Debug("Finding user " + username);
                if (Membership.FindUsersByName(username).Count > 0)
                {
                    currentUser = Membership.GetUser(username);
                    string[] roles = Roles.GetRolesForUser(username);
                    HttpContext.Current.Session["userroles"] = roles;
                    FormsAuthentication.SetAuthCookie(username, true);
                    Member m = new Member((int)currentUser.ProviderUserKey);
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("uid", MemberVUIStatus(m)));

                    log.Debug("Added uid cookie: " + MemberVUIStatus(m));

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
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
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("uid", MemberVUIStatus(m)));
                    return true;
                }
                else if (Membership.FindUsersByName(username).Count > 0)
                {
                    log.Debug("Logging in: Multiple Users with username - " + username);

                    return false;
                    throw new Exception(VUIfunctions.VUI_USER_OR_PWD);
                }
                else
                {
                    log.Debug("Logging in: Invalid username or password - " + username);

                    return false;
                    throw new Exception(VUIfunctions.VUI_USER_OR_PWD);
                }
            }
            else
            {
                log.Debug("Logging in: CURRENT USER? " + currentUser.Email);
            }


            return false;
            throw new Exception(VUIfunctions.VUI_USER_OR_PWD);
        }


        public static string FavouriteImage(int imageid, string action)
        {
            Member m = CurrentUser();
            StringBuilder jsonOut = new StringBuilder(@"");
            if (m != null)
            {
                StringBuilder sb = new StringBuilder("");
                Document node = new Document(imageid);

                string ratings = node.getProperty("ratings").Value.ToString().Replace("{", "").Replace("}", "");
                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(ratings);
                    bool addLine = true;
                    int itemid = 0;
                    int sortOrder = 0;

                    XmlNodeList ratingList;
                    ratingList = xml.SelectSingleNode("items").SelectNodes("item");

                    foreach (XmlNode item in ratingList)
                    {
                        int userid = Int32.Parse(item.SelectSingleNode("vuiRatingsUser").InnerText);
                        int rating = Int32.Parse(item.SelectSingleNode("vuiUserRating").InnerText);

                        if (m.Id == userid)
                        {
                            if (action == "ADD")
                            {
                                sb.Append(item.OuterXml);
                            }
                            // If action = "REMOVE", then simply don't add this line
                            addLine = false;
                        }
                        else
                        {
                            sb.Append(item.OuterXml);
                        }
                        itemid = Int32.Parse(item.Attributes["id"].Value);
                        sortOrder = Int32.Parse(item.Attributes["sortOrder"].Value);
                    }
                    if (addLine)
                    {
                        itemid++; sortOrder++;

                        sb.Append(@"<item id=""" + itemid + @""" sortOrder=""" + sortOrder + @""">");
                        sb.Append(@"<vuiRatingsUser nodeName=""User"" nodeType=""1036"">" + m.Id + @"</vuiRatingsUser>");
                        sb.Append(@"<vuiUserRating nodeName=""Rating"" nodeType=""-51"">1</vuiUserRating>");
                        sb.Append(@"</item>");
                    }
                    node.getProperty("ratings").Value = @"<items>" + sb.ToString() + @"</items>";
                    node.Save();
                    node.Publish(UpdateUser);
                    umbraco.library.UpdateDocumentCache(node.Id);

                    jsonOut.Append(@"{""response"": ""Success""}");
                    return jsonOut.ToString();
                }
                catch (XmlException xex)
                {

                    sb.Append(@"<item id=""1"" sortOrder=""0"">");
                    sb.Append(@"<vuiRatingsUser nodeName=""User"" nodeType=""1036"">" + m.Id + @"</vuiRatingsUser>");
                    sb.Append(@"<vuiUserRating nodeName=""Rating"" nodeType=""-51"">1</vuiUserRating>");
                    sb.Append(@"</item>");


                    node.getProperty("ratings").Value = @"<items>" + sb.ToString() + @"</items>";
                    node.Save();
                    node.Publish(UpdateUser);
                    umbraco.library.UpdateDocumentCache(node.Id);

                    jsonOut.Append(@"{""response"": ""Success""}");
                    return jsonOut.ToString();
                }
                catch (Exception ex)
                {
                    return @"{""response"": ""Failure"", ""Error"": " + ex.Message.Replace(@"""", "\"") + "}";
                }
            }
            else
            {
                return @"{""response"": ""NOTLOGGEDIN""}";
            }
        }

        public static string RateService(int serviceid, int newrating)
        {
            Member m = CurrentUser();
            StringBuilder jsonOut = new StringBuilder(@"");
            if (m != null)
            {
                StringBuilder sb = new StringBuilder("");
                /* 
                 * OUTPUT value should be:
                 * {<items>
                 *  <item id="1" sortOrder="0">
                 *      <vuiRatingsUser nodeName="User" nodeType="1036">2936</vuiRatingsUser>
                 *      <vuiUserRating nodeName="Rating" nodeType="-51">7</vuiUserRating>
                 *  </item>
                 *  <item id="2" sortOrder="1">
                 *      <vuiRatingsUser nodeName="User" nodeType="1036">2935</vuiRatingsUser>
                 *      <vuiUserRating nodeName="Rating" nodeType="-51">9</vuiUserRating>
                 *  </item>
                 *  </items>}
                 */

                Document node = new Document(serviceid);

                string ratings = node.getProperty("ratings").Value.ToString().Replace("{", "").Replace("}", "");


                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(ratings);
                    bool userHasRated = false;
                    int itemid = 0;
                    int sortOrder = 0;

                    XmlNodeList ratingList;
                    ratingList = xml.SelectSingleNode("items").SelectNodes("item");

                    foreach (XmlNode item in ratingList)
                    {
                        int userid = Int32.Parse(item.SelectSingleNode("vuiRatingsUser").InnerText);
                        int rating = Int32.Parse(item.SelectSingleNode("vuiUserRating").InnerText);


                        if (m.Id == userid)
                        {
                            userHasRated = true;
                            item.SelectSingleNode("vuiUserRating").InnerText = newrating.ToString();
                            sb.Append(item.OuterXml);
                        }
                        else
                        {
                            sb.Append(item.OuterXml);
                        }
                        itemid = Int32.Parse(item.Attributes["id"].Value);
                        sortOrder = Int32.Parse(item.Attributes["sortOrder"].Value);
                    }
                    if (!userHasRated)
                    {
                        itemid++; sortOrder++;

                        sb.Append(@"<item id=""" + itemid + @""" sortOrder=""" + sortOrder + @""">");
                        sb.Append(@"<vuiRatingsUser nodeName=""User"" nodeType=""1036"">" + m.Id + @"</vuiRatingsUser>");
                        sb.Append(@"<vuiUserRating nodeName=""Rating"" nodeType=""-51"">" + newrating + @"</vuiUserRating>");
                        sb.Append(@"</item>");
                    }
                    node.getProperty("ratings").Value = @"<items>" + sb.ToString() + @"</items>";
                    node.Save();
                    node.Publish(UpdateUser);
                    umbraco.library.UpdateDocumentCache(node.Id);

                    VUIService s = new VUIService(node.Id);
                    s.SetRatings();
                    jsonOut.Append(@"{""response"": ""Success"", ");
                    jsonOut.Append(@"""servicedata"": " + s.GetJson());
                    jsonOut.Append(@"}");
                    return jsonOut.ToString();
                }
                catch (XmlException xex)
                {

                    sb.Append(@"<item id=""1"" sortOrder=""0"">");
                    sb.Append(@"<vuiRatingsUser nodeName=""User"" nodeType=""1036"">" + m.Id + @"</vuiRatingsUser>");
                    sb.Append(@"<vuiUserRating nodeName=""Rating"" nodeType=""-51"">" + newrating + @"</vuiUserRating>");
                    sb.Append(@"</item>");


                    node.getProperty("ratings").Value = @"<items>" + sb.ToString() + @"</items>";
                    node.Save();
                    node.Publish(UpdateUser);
                    umbraco.library.UpdateDocumentCache(node.Id);

                    VUIService s = new VUIService(node.Id);
                    s.SetRatings();
                    jsonOut.Append(@"{""response"": ""Success"", ");
                    jsonOut.Append(@"""servicedata"": " + s.GetJson());
                    jsonOut.Append(@"}");
                    return jsonOut.ToString();
                }
                catch (Exception ex) 
                {
                    return @"{""response"": ""Failure"", ""Error"": " + ex.Message.Replace(@"""", "\"") + "}";
                }
            }
            else
            {
                return @"{""response"": ""NOTLOGGEDIN""}";
            }

        }

        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeJsString(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '\"':
                            sb.Append("\\\"");
                            break;
                        case '\\':
                            sb.Append("\\\\");
                            break;
                        case '\b':
                            sb.Append("\\b");
                            break;
                        case '\f':
                            sb.Append("\\f");
                            break;
                        case '\n':
                            sb.Append("\\n");
                            break;
                        case '\r':
                            sb.Append("\\r");
                            break;
                        case '\t':
                            sb.Append("\\t");
                            break;
                        default:
                            int i = (int)c;
                            if (i < 32 || i > 127)
                            {
                                sb.AppendFormat("\\u{0:X04}", i);
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                    }
                }

                return sb.ToString();
            }
            else
                return s;
        }
    }

}
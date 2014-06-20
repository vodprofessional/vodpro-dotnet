using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using System.Configuration;
using System.Net.Mail;


namespace VPCommon
{
    public class Emailer
    {
        private DynamicNode emailNode;
        private string _body;
        private string _subject;
        private bool _ishtml = false;

        /// <summary>
        /// Initialise the Emailer
        /// </summary>
        /// <param name="emailAlias"></param>
        public Emailer(string emailAlias)
        {
            int templateFolderID;

            string templateFolderIDstr = ConfigurationManager.AppSettings["VP2014_EMAIL_TEMPLATE_FOLDER"];
            if (Int32.TryParse(templateFolderIDstr, out templateFolderID))
            {
                DynamicNode emailFolder = new DynamicNode(templateFolderID);
                DynamicNode n = emailFolder.Descendants("VUI3Email").Items.Where(no => no.GetProperty("alias").Value.Equals(emailAlias)).First();
                Init(n);
            }
            else
            {
                throw new Exception("Could not create email from template specified");
            }
        }


        private void Init(DynamicNode n)
        {
            emailNode = n;
            if (n.GetProperty("emailType").Value.Equals("HTML"))
            {
                _body = n.GetProperty("bodyhtml").Value;
                _ishtml = true;
            }
            else
            {
                _body = n.GetProperty("bodytext").Value;
            }
            _subject = n.GetProperty("subject").Value;
        }


        public void Send(Member m)
        {
            Send(m.Email);
        }

        /// <summary>
        /// Send to address
        /// </summary>
        /// <param name="toaddress"></param>
        public void Send(string toaddress)
        {
            Send(new string[] { toaddress });
        }

        /// <summary>
        /// Send to Multiple Addresses
        /// </summary>
        /// <param name="toaddress"></param>
        public void Send(string[] toaddress)
        {
            Send(new MailAddress("admin@vodprofessional.com", "VOD Professional Admin"), toaddress);
        }

        /// <summary>
        /// Send form sepcific address
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toaddress"></param>
        public void Send(MailAddress fromAddress, string toaddress)
        {
            Send(fromAddress, new string[] { toaddress });
        }

        /// <summary>
        /// The actual method that does the work
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toaddress"></param>
        public void Send(MailAddress fromAddress, string[] toaddress)
        {
            using (SmtpClient smtp = new SmtpClient())
            {
                MailMessage msg = new MailMessage();
                foreach (string s in toaddress)
                {
                    msg.To.Add(s);
                }
                msg.Subject = Subject;
                msg.From = fromAddress;
                msg.Body = Body;
                smtp.Send(msg);
            }
        }

        /// <summary>
        /// Replace all Member Dynamic Elements in Subject and Body
        /// </summary>
        /// <param name="m"></param>
        public void ReplaceMemberElements(Member m)
        {
            _body = _body.Replace("#USERNAME#", m.getProperty("firstName").Value.ToString())
                         .Replace("#FIRSTNAME#", m.getProperty("firstName").Value.ToString())
                         .Replace("#LASTNAME#", m.getProperty("lastName").Value.ToString())
                         .Replace("#LOGIN#", m.LoginName)
                         .Replace("EMAIL", m.Email)
                         ;
            _subject = _subject.Replace("#USERNAME#", m.getProperty("firstName").Value.ToString())
                         .Replace("#FIRSTNAME#", m.getProperty("firstName").Value.ToString())
                         .Replace("#LASTNAME#", m.getProperty("lastName").Value.ToString())
                         .Replace("#LOGIN#", m.LoginName)
                         .Replace("EMAIL", m.Email)
                         ;
        }

        /// <summary>
        /// Replaces Dynamic Parts of 
        /// </summary>
        /// <param name="tokenString"></param>
        /// <param name="replaceString"></param>
        public void ReplaceElement(string tokenString, string replaceString)
        {
            _body = _body.Replace(tokenString, replaceString);
            _subject = _subject.Replace(tokenString, replaceString);
        }

        public String Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }
        public String Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }
        public Boolean IsHTML
        {
            get
            {
                return _ishtml;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VP2.businesslogic
{
    public class ContactRequest
    {
        public static void SendRequest(string to, string subject, string message, string emailFrom, string emailFromName)
        {
            var m = new System.Net.Mail.MailMessage();
            m.From = new System.Net.Mail.MailAddress(emailFrom, emailFromName);
            m.Subject = subject;
            m.IsBodyHtml = false;
            m.To.Add(to);
            m.CC.Add("support@vodprofessional.com");

            m.Body = "CONTACT REQUEST FROM " + emailFromName + @" <" + emailFrom + @">" + Environment.NewLine + Environment.NewLine + message;

            var s = new System.Net.Mail.SmtpClient();
            s.Send(m);
        } 
    }
}
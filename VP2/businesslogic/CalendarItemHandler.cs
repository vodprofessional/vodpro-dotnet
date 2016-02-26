using System;
using System.Configuration;
using System.Net.Mail;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using VPCommon;

namespace VP2.businesslogic
{
    public class CalendarItemHandler : ApplicationBase
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CalendarItemHandler));
        public CalendarItemHandler()
        {
            Document.AfterSave += new Document.SaveEventHandler(Document_AfterSave);
            Document.AfterPublish += new Document.PublishEventHandler(Document_AfterPublish);
        }

        private void Document_AfterPublish(Document sender, PublishEventArgs e)
        {
            if (sender.ContentType.Alias.Equals("CalendarEvent"))
            {
                if (sender.getProperty("sendNotificationEmail") != null)
                {
                    if (sender.getProperty("sendNotificationEmail").Value.Equals(1))
                    {
                        // Send notification email

                        try
                        {
                            CalendarItem evt = new CalendarItem(sender.Id, "PREVIEW");

                            try
                            {
                                Member m = evt.CreatedByMember;

                                try
                                {
                                    Emailer em = new Emailer("EMAIL_CALENDAR_PUBLISH_NOTIFY");
                                    em.ReplaceMemberElements(m);
                                    em.ReplaceElement("#EVENT_TITLE#", evt.Name);
                                    em.ReplaceElement("#ID#", evt.UniqueID);
                                    em.ReplaceElement("#URL#", evt.Url);

                                    em.Send(new MailAddress("events@vodprofessional.com", "VOD Professional Events"), m.Email);

                                    try
                                    {
                                        evt.SendPublishedEmail = false;
                                        evt.Save();
                                    }
                                    catch(Exception exSave)
                                    {
                                        log.Error("Error after publish - couldn't save event", exSave);
                                    }
                                }
                                catch(Exception exEm)
                                {
                                    log.Error("Error after publis - could send email", exEm);
                                }
                            }
                            catch(Exception exmem)
                            {
                                log.Error("Error after publish - couldn't get member", exmem);
                            }
                        }
                        catch(Exception e1)
                        {
                            log.Error("Error after publish - couldn't retrieve event [" + sender.Id + "]", e1);
                        }
                    }
                }
            }
        }

        void Document_AfterSave(Document sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            if (sender.ContentType.Alias.Equals("CalendarEvent"))
            {
                if (sender.getProperty("uniqueID") != null)
                {
                    if (String.IsNullOrEmpty(sender.getProperty("uniqueID").Value.ToString()))
                    {
                        sender.getProperty("uniqueID").Value = Guid.NewGuid().ToString();
                    }
                }
            }
        }
    }
}
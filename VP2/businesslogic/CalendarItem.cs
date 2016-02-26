using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using umbraco.BusinessLogic;
using umbraco.MacroEngines;
using System.Configuration;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.member;
using Newtonsoft.Json.Linq;

namespace VP2.businesslogic
{
    public class CalendarItem
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CalendarItem));

        private string _mode = "PUBLIC";
        public string Mode { get { return _mode; } set { if (!String.IsNullOrEmpty(value)) { _mode = value; } } }
        public int ID { get; set; }


        public string Name { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool TimeTBA { get; set; }
        public string TimeZone { get; set; }
        public string Url { get; set; }
        public string Venue { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public string GoogleMap { get; set; }
        public string EventImage { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Company { get; set; }

        public bool DisplayContactName { get; set; }
        public bool DisplayContactEmail { get; set; }
        public bool DisplayContactPhone { get; set; }
        public bool DisplayCompany { get; set; }

        public string ExternalUrl { get; set; }
        public int CreatedBy { get; set; }
        public Member CreatedByMember { get; set; }
        public string UniqueID { get; set; }
        public bool IsPublished { get; set; }
        public bool SendPublishedEmail { get; set; }


        public string DateString
        {
            get
            {
                string strDate = "";
                strDate += StartDate.ToString("dd MMMM yyyy");
                strDate += " at ";
                strDate += TimeTBA ? "TBC" : StartDate.ToString("HH:mm");
                strDate += " - ";

                if (!StartDate.ToString("yyyy-MM-dd").Equals(EndDate.ToString("yyyy-MM-dd")))
                {
                    strDate += EndDate.ToString("dd MMMM yyyy");
                    strDate += " at ";
                }
                strDate += TimeTBA ? "TBA" : EndDate.ToString("HH:mm") + " " + TimeZone;
                return strDate;
            }
        }
        public string DateString2
        {
            get
            {
                string strDate = "";
                strDate += StartDate.ToString("dd MMMM yyyy");

                if (!StartDate.ToString("yyyy-MM-dd").Equals(EndDate.ToString("yyyy-MM-dd")))
                {
                    strDate += " - " + EndDate.ToString("dd MMMM yyyy");
                }
                return strDate;
            }
        }
        public string TimeString
        {
            get
            {
                if (TimeTBA)
                {
                    return "Times TBA";
                }
                else
                {
                    string strDate = "";
                    strDate += StartDate.ToString("HH:mm");
                    strDate += " - ";
                    strDate += EndDate.ToString("HH:mm");
                    strDate += " <span>(Time zone: " + TimeZone + ")</span>";
                    return strDate;
                }
            }
        }
        public string ExternalUrlFormatted
        {
            get
            {
                string strUrl = ExternalUrl;
                if(!String.IsNullOrEmpty(strUrl))
                {
                    if(!(ExternalUrl.StartsWith("http") || ExternalUrl.StartsWith("//")))
                    {
                        strUrl = "http://" + strUrl;
                    }
                    
                }
                return strUrl;
            }
        }
        public string EventClass
        {
            get
            {
                switch(EventType.ToLower())
                {
                    case "conference / exhibition":
                        {
                            return "event-conference";
                        }
                    case "networking event":
                        {
                            return "event-networking";
                        }
                    case "product demonstration":
                        {
                            return "event-demo";
                        }
                    case "financial results":
                        {
                            return "event-financial";
                        }
                    case "webinar":
                        {
                            return "event-webinar";
                        }
                    default:
                        return "";
                }
            }
        }
        public int EventRootId = 1;

        public string DescriptionAsHtml
        {
            get { return Description.Replace("\n", "<br/>"); }
        }
        public string[] OrganiserDetails
        {
            get
            {
                List<string> org = new List<string>();
                if (DisplayContactName)
                {
                    org.Add(ContactName);
                }
                if (DisplayCompany)
                {
                    org.Add(Company);
                }
                if (DisplayContactEmail)
                {
                    org.Add(@"<a href=""mailto:" + ContactEmail + @""">" + ContactEmail + @"</a>");
                }
                if (DisplayContactPhone)
                {
                    org.Add(ContactPhone);
                }
                return org.ToArray();
            }
        }
        public string[] VenueDetails
        {
            get
            {
                List<string> venue = new List<string>();
                venue.Add(Venue);
                venue.Add(Address1);
                if (!String.IsNullOrEmpty(Address2))
                {

                    venue.Add(Address2);
                }
                venue.Add(City);
                venue.Add(Postcode);
                return venue.ToArray();
            }
        }


        public CalendarItem()
        {
            
        }
        public CalendarItem(int id, string mode)
        {
            ID = id;
            Mode = mode;
            Init();
        }
        

        public void Init()
        {
            // Initialise from DynamicNode
            if(_mode.Equals("PUBLIC"))
            {
                try
                {
                    //log.Debug("Initialising Event from Node");
                    //log.Debug("- Getting Node ["+ID+"]");
                    DynamicNode evt = new DynamicNode(ID);

                    //log.Debug("- Setting Properties");

                    Name = evt.GetProperty("title").Value;
                    EventType = evt.GetProperty("eventType").Value;
                    Description = evt.GetProperty("eventDescription").Value;
                    StartDate = DateTime.Parse(evt.GetProperty("startDate").Value);
                    EndDate = DateTime.Parse(evt.GetProperty("endDate").Value);
                    TimeZone = evt.GetProperty("timeZone").Value;
                    TimeTBA = (evt.GetProperty("timeTBA").Value == "1");

                    //log.Debug("- Venue Details");
                    Venue = evt.GetProperty("venue").Value;
                    Address1 = evt.GetProperty("address1").Value;
                    Address2 = evt.GetProperty("address2").Value;
                    City = evt.GetProperty("city").Value;
                    Postcode = evt.GetProperty("postcode").Value;
                    GoogleMap = evt.GetProperty("googleMapEmbed").Value;
                    Url = evt.Url;

                    //log.Debug("- Contact details");
                    ContactName = evt.GetProperty("contactName").Value;
                    ContactEmail = evt.GetProperty("contactEmail").Value;
                    ContactPhone = evt.GetProperty("contactPhone").Value;
                    DisplayContactName = evt.GetProperty("displayContactName").Value == "1";
                    DisplayContactEmail = evt.GetProperty("displayContactEmail").Value == "1";
                    DisplayContactPhone = evt.GetProperty("displayContactPhone").Value == "1";

                    Company = evt.GetProperty("company").Value;
                    DisplayCompany = evt.GetProperty("displayCompany").Value == "1";

                    //log.Debug("I- Mage details");
                    EventImage = evt.GetProperty("eventImage").Value;
                    ExternalUrl = evt.GetProperty("externalURL").Value;

                    //log.Debug("- Send Notification Email");
                    if (evt.GetProperty("sendNotificationEmail") != null)
                        SendPublishedEmail = evt.GetProperty("sendNotificationEmail").Value == "1";

                    //log.Debug("- Created By Details");
                    int cb;
                    if (Int32.TryParse(evt.GetProperty("createdBy").Value, out cb))
                    {
                        CreatedBy = cb;
                        CreatedByMember = new Member(cb);
                    }
                    IsPublished = true;
                    UniqueID = evt.GetProperty("uniqueID").Value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    // log.Debug("Initialising Event from Document store");
                    // log.Debug("- Getting Node [" + ID + "]");
                    Document evt = new Document(ID);
                    Name = evt.getProperty("title").Value.ToString();
                    if (evt.getProperty("eventType") != null)
                        EventType = evt.getProperty("eventType").Value.ToString();
                    if (evt.getProperty("eventDescription") != null)
                        Description = evt.getProperty("eventDescription").Value.ToString();
                    if (evt.getProperty("startDate") != null)
                        StartDate = DateTime.Parse(evt.getProperty("startDate").Value.ToString());
                    if (evt.getProperty("endDate") != null)
                        EndDate = DateTime.Parse(evt.getProperty("endDate").Value.ToString());
                    if (evt.getProperty("timeZone") != null)
                        TimeZone = evt.getProperty("timeZone").Value.ToString();
                    if (evt.getProperty("timeTBA") != null)
                        TimeTBA = (evt.getProperty("timeTBA").Value.Equals(1));

                    if (evt.getProperty("venue") != null)
                        Venue = evt.getProperty("venue").Value.ToString();
                    if (evt.getProperty("address1") != null)
                        Address1 = evt.getProperty("address1").Value.ToString();
                    if (evt.getProperty("address2") != null)
                        Address2 = evt.getProperty("address2").Value.ToString();
                    if (evt.getProperty("city") != null)
                        City = evt.getProperty("city").Value.ToString();
                    if (evt.getProperty("postcode") != null)
                        Postcode = evt.getProperty("postcode").Value.ToString();

                    if (evt.getProperty("googleMapEmbed") != null)
                        GoogleMap = evt.getProperty("googleMapEmbed").Value.ToString();

                    if (evt.getProperty("contactName") != null)
                        ContactName = evt.getProperty("contactName").Value.ToString();
                    if (evt.getProperty("contactEmail") != null)
                        ContactEmail = evt.getProperty("contactEmail").Value.ToString();
                    if (evt.getProperty("contactPhone") != null)
                        ContactPhone = evt.getProperty("contactPhone").Value.ToString();
                    if (evt.getProperty("displayContactName") != null)
                        DisplayContactName = evt.getProperty("displayContactName").Value.Equals(1);
                    if (evt.getProperty("displayContactEmail") != null)
                        DisplayContactEmail = evt.getProperty("displayContactEmail").Value.Equals(1);
                    if (evt.getProperty("displayContactPhone") != null)
                        DisplayContactPhone = evt.getProperty("displayContactPhone").Value.Equals(1);

                    if (evt.getProperty("company") != null)
                        Company = evt.getProperty("company").Value.ToString();
                    if (evt.getProperty("displayCompany") != null)
                        DisplayCompany = evt.getProperty("displayCompany").Value.Equals(1);

                    if (evt.getProperty("eventImage") != null)
                        EventImage = evt.getProperty("eventImage").Value.ToString();
                    if (evt.getProperty("externalURL") != null)
                        ExternalUrl = evt.getProperty("externalURL").Value.ToString();
                    if (evt.getProperty("sendNotificationEmail") != null)
                        SendPublishedEmail = evt.getProperty("sendNotificationEmail").Value.Equals(1);

                    if (evt.getProperty("createdBy") != null)
                    { 
                        int cb;
                        if (Int32.TryParse(evt.getProperty("createdBy").Value.ToString(), out cb))
                        {
                            CreatedBy = cb;
                            CreatedByMember = new Member(cb);
                        }
                    }
                    UniqueID = evt.getProperty("uniqueID").Value.ToString();
                    IsPublished = evt.Published;
                    if(IsPublished)
                    {
                        try
                        {
                            DynamicNode n = new DynamicNode(ID);
                            Url = n.Url;
                            
                            n = null;
                        }
                        catch(Exception exUrl)
                        {
                            log.Error("Couldn't generate URL for document - probably not in cache yet",exUrl);
                            Url = "#";
                        }
                        if (Url.Equals("#"))
                        {
                        //    log.Debug("Setting url of # to [/calendar]");
                            Url = "/calendar";
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void GenerateID()
        {
            Guid g = Guid.NewGuid();
            UniqueID = g.ToString();
        }

        public bool Publish()
        {
            Document evt = new Document(ID);
            User u = User.GetAllByLoginName("websitecontentuser", false).First();
            evt.Publish(u);
            umbraco.library.UpdateDocumentCache(ID);
            return true;
        }

        public bool UnPublish()
        {
            Document evt = new Document(ID);
            evt.UnPublish();
            umbraco.library.UnPublishSingleNode(ID);
            try
            {
                umbraco.library.UpdateDocumentCache(ID);
            }
            catch(Exception ex)
            {
                log.Error("Error unpublishing -  couldn't UpdateDocumentCahce", ex);
            }
            return true;
        }

        public bool IsCreatedByCurrentMember()
        {
            Member m = Member.GetCurrentMember();
            if (m == null)
            {
                return false;
            }
            else
            {
                return (m.Id == CreatedBy);
            }
        }

        /// <summary>
        /// Return a list of the events associated to the current logged in user.
        /// </summary>
        /// <returns></returns>
        public static List<CalendarItem> GetCurrentMembersEvents()
        {
            List<CalendarItem> evts = new List<CalendarItem>();
            Member m = Member.GetCurrentMember();
            int mid = m.Id;

            int eventRootId;
            if(Int32.TryParse(ConfigurationManager.AppSettings["VP2014_CALENDAR_ROOT"].ToString(), out eventRootId))
            {
                DocumentType dt = DocumentType.GetByAlias("CalendarEvent");
                Document root = new Document(eventRootId);
                Document[] evtDocs = root.Children;
                foreach(Document d in evtDocs)
                {
                    if(d.ContentType.Equals(dt))
                    {
                        if(d.getProperty("createdBy") != null)
                        {
                            try
                            {
                                int cbid = (int)d.getProperty("createdBy").Value;
                                if(cbid == mid)
                                {
                                    CalendarItem evt = new CalendarItem(d.Id, "PREVIEW");
                                    evts.Add(evt);
                                }
                            }
                            catch(Exception)
                            {

                            }
                        }
                    }
                }
            }
            return evts;
        }


        public int Save()
        {
            Document evt;
            if (ID > 0)
            {
                evt = new Document(ID);
            }
            else
            {
                GenerateID();
                Int32.TryParse(ConfigurationManager.AppSettings["VP2014_CALENDAR_ROOT"].ToString(), out EventRootId);
                User u = User.GetAllByLoginName("websitecontentuser", false).First();
                DocumentType dt = DocumentType.GetByAlias("CalendarEvent");
                Member m = Member.GetCurrentMember();

                evt = Document.MakeNew(Name, dt, u, EventRootId);
                evt.getProperty("createdBy").Value = m.Id;
                evt.getProperty("uniqueID").Value = UniqueID;
                
            }

            evt.getProperty("title").Value = Name;
            evt.getProperty("eventType").Value = EventType;
            evt.getProperty("eventDescription").Value = Description;
            evt.getProperty("startDate").Value = StartDate;
            evt.getProperty("endDate").Value = EndDate;
            evt.getProperty("timeZone").Value = TimeZone;
            evt.getProperty("timeTBA").Value = TimeTBA;

            evt.getProperty("venue").Value = Venue;
            evt.getProperty("address1").Value = Address1;
            evt.getProperty("address2").Value = Address2;
            evt.getProperty("city").Value = City;
            evt.getProperty("postcode").Value = Postcode;

            evt.getProperty("googleMapEmbed").Value = GoogleMap;

            evt.getProperty("contactName").Value = ContactName;
            evt.getProperty("contactEmail").Value = ContactEmail;
            evt.getProperty("contactPhone").Value = ContactPhone;
            evt.getProperty("company").Value = Company;

            evt.getProperty("displayContactName").Value = DisplayContactName;
            evt.getProperty("displayContactEmail").Value = DisplayContactEmail;
            evt.getProperty("displayContactPhone").Value = DisplayContactPhone;
            evt.getProperty("displayCompany").Value = DisplayCompany;

            evt.getProperty("eventImage").Value = EventImage;
            evt.getProperty("externalURL").Value = ExternalUrl;
            evt.getProperty("sendNotificationEmail").Value = SendPublishedEmail;
            evt.Save();

            return evt.Id;
        }

        public string AsJSON
        {
            get
            {
                JObject o = JObject.FromObject(new
                    {
                        id = ID,
                        title = Name,
			            description = Description,
                        url = Url ,
			            img_url = EventImage ,
                        evtclass = EventClass ,
                        type = EventType ,
                        city = City,
                        start = StartDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                        end = EndDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds 
                    });
                /*
                string json = @"{
                    ""id"": " + ID + @",
                    ""title"": """ + Name + @""",
			        ""description"": """ + Description + @""",
                    ""url"": """ + Url + @""",
			        ""img_url"": """ + EventImage + @""",
                    ""class"": """ + EventClass + @""",
                    ""type"": """ + EventType + @""",
                    ""start"": " + StartDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + @",
                    ""end"":   " + EndDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + @"
                }";
                */

                return o.ToString().Replace("evtclass","class");
            }
        }

        /// <summary>
        /// Searches the events for the one with the UID specified
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static CalendarItem GetFromUID(string uid)
        {
            if(!String.IsNullOrEmpty(uid))
            {
                Guid g;
                if(Guid.TryParse(uid, out g))
                {
                    int EventRootId;
                    Int32.TryParse(ConfigurationManager.AppSettings["VP2014_CALENDAR_ROOT"].ToString(), out EventRootId);
                    // Load all the events in the even folder
                    Document f = new Document(EventRootId);
                    Document[] events = f.Children;
                    
                    foreach(Document evt in events)
                    {
                        if(evt.getProperty("uniqueID") != null)
                        {
                            string u = evt.getProperty("uniqueID").Value.ToString();
                            if (u.Equals(uid,StringComparison.InvariantCultureIgnoreCase))
                            {
                                CalendarItem ci = new CalendarItem(evt.Id, "PREVIEW");
                                return ci;
                            }
                        }
                    }

                }
            }
            return null;
        }

        public static List<CalendarItem> GetEvents(string mode)
        {
            List<CalendarItem> events = new List<CalendarItem>();
            if (mode.Equals("PUBLIC"))
            {
                int EventRootId;
                Int32.TryParse(ConfigurationManager.AppSettings["VP2014_CALENDAR_ROOT"].ToString(), out EventRootId);
                DynamicNode cal = new DynamicNode(EventRootId);
                DynamicNodeList nodes = cal.Descendants("CalendarEvent");
                foreach(DynamicNode n in nodes)
                {
                    log.Debug("Loading event " + n.Id + " for Calendar");
                    try
                    {
                        CalendarItem evt = new CalendarItem(n.Id, "PUBLIC");
                        events.Add(evt);
                    }
                    catch(Exception ex)
                    {
                        log.Error("Error initialising Calendar Item", ex);
                    }
                }
            }
            return events;
        }
    }
}
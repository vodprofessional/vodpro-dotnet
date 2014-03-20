using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace VP2.businesslogic
{
    public class VPJobs
    {

        public int NumResults { get; set; }
        public List<Job> JobList { get; set; }
        public bool EndReached { get; set; }

        [JsonIgnore]
        public int DescriptionSize { get; set; }
        [JsonIgnore]
        public string Url { get; set; }

        public VPJobs()
        {
            Url = ConfigurationManager.AppSettings["JOBSERVE_XML"];
            JobList = new List<Job>();
            DescriptionSize = 300;
            EndReached = false;
        }

        public void PopulateJobList(int startnum, int count)
        {
            if (!string.IsNullOrEmpty(Url))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.KeepAlive = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    XmlDocument xml = new XmlDocument();
                    XmlNamespaceManager ns = new XmlNamespaceManager(xml.NameTable);
                    ns.AddNamespace("j", @"http://www.labs.jobserve.com/services/v0.3");
                    xml.Load(reader);

                    XmlNodeList jobs = xml.GetElementsByTagName("Job");

                    this.NumResults = jobs.Count;

                    if (startnum < NumResults)
                    {
                        if (startnum + count > NumResults)
                        {
                            count = NumResults - startnum;
                            EndReached = true;
                        }
                    }
                    else
                    {
                        EndReached = true;
                        return;
                    }

                    int i = 0;
                    foreach (XmlNode job in jobs)
                    {
                        i++;
                        if (i < startnum)
                        {
                            continue;
                        }
                        else if (i <= startnum + count)
                        {
                            Job j = new Job();
                            try
                            {
                                j.Url = job.SelectSingleNode(@"j:JobURL", ns).InnerText;
                                j.Title = job.SelectSingleNode(@"j:Position", ns).InnerText;
                                if(job.SelectSingleNode(@"j:Rate", ns) != null)
                                    j.Rate = job.SelectSingleNode(@"j:Rate", ns).InnerText;
                                j.Description = Regex.Replace(job.SelectSingleNode(@"j:Skills", ns).InnerText, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", " ").Substring(0, DescriptionSize) + "...";
                                if (job.SelectSingleNode(@"j:Location", ns) != null)
                                    j.Location = job.SelectSingleNode(@"j:Location", ns).InnerText;
                                if (job.SelectSingleNode(@"j:JobType", ns) != null)
                                    j.Contract = job.SelectSingleNode(@"j:JobType", ns).InnerText;
                                j.Date = DateTime.Parse(job.SelectSingleNode(@"j:PostedDate", ns).InnerText).ToString("dd MMM yyyy");
                            }
                            catch (Exception ex)
                            {
                                ;
                            }
                            JobList.Add(j);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Job
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Contract { get; set; }
        public string Rate { get; set; }

        public string Metadata
        {
            get
            {
                List<string> outstr = new List<string>();
                if(!String.IsNullOrEmpty(Location)) 
                {
                    outstr.Add(Location);
                }
                if(!String.IsNullOrEmpty(Contract)) 
                {
                    outstr.Add(Contract);
                }
                if(!String.IsNullOrEmpty(Rate)) 
                {
                    outstr.Add(Rate);
                }
                return String.Join(" / ", outstr);
            }
        }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
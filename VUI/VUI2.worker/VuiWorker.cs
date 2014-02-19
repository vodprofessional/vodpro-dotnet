using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VUI2.classes;
using System.Data.SqlClient;
using System.Configuration;

namespace VUI.VUI2.worker
{
    public class VuiWorker
    {
        public VuiWorker()
        {

        }


        /// <summary>
        /// Create a Worker Profile in the database (email address and password)
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        public static int CreateWorker(string emailAddress, string password)
        {
            int workerId = -1;
            string sql = String.Format(@"set nocount on; insert into vui_CrowdWorker (emailaddress,password) values('{0}','{1}'); select @@identity;", emailAddress.ToLower(), password);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    workerId = (Int32)sr[0];
                }
                conn.Close();
            }
            return workerId;
        }



        /// <summary>
        /// Returns the Job Number (a hash of workerid-analysisid)
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="description"></param>
        /// <param name="service"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static string CreateJob(int workerId, string description, string service, int serviceId)
        {
            /*
            // Create new Analysis Document
            int analysisId = 12345;

            // Create the Unique Job Number (a hash)
            string jobId = GenerateHash(workerId.ToString() + '-' + analysisId.ToString());

            // If the job number generated is not unique, add a character and try again.
            while (!IsJobHashUnique(jobId))
            {
                jobId += "X";
            }

            string sql = String.Format(@"set nocount on; insert into vui_CrowdJob (emailaddress,password) values('{0}','{1}'); select @@identity;", emailAddress.ToLower(), password);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    workerId = (Int32)sr[0];
                }
                conn.Close();
            }
            return workerId;*
             */
            return 0;
        }


        /// <summary>
        /// Login either returns true or false. Session variables etc can be handled at the font end.
        /// The out parameter returns the workerId if there is one
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Login(string emailAddress, string password, out int workerId)
        {
            workerId = -1;
            string sql = String.Format(@"select top 1 id from vui_Worker where emailaddress='{0}' and password='{1}'", emailAddress.ToLower(), password);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    workerId = (Int32)sr[0];
                }
                conn.Close();
            }
            if (workerId > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        /// <summary>
        /// Use to create a password for sending out via email
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword()
        {
            return "generated password";
        }


        public static string GenerateHash(string stringToHash)
        {
            CRC32 crc32 = new CRC32();
            return CRC32.FormatCRC32Result(stringToHash);
        }


        public static bool IsJobHashUnique(string jobId)
        {
            int countId = 0;
            string sql = String.Format(@"select count(*) from vui_CrowdJob where ID='{0}'", jobId);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    countId = (Int32)sr[0];
                }
                conn.Close();
            }
            if (countId == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
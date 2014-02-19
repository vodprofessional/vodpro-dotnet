using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.member;
using VUI.classes;
using umbraco.MacroEngines;
using Newtonsoft.Json;

namespace VUI.VUI3.classes
{
    public class VUIUser
    {

        public Member Member { get; private set; }
        public string UserStatus { get; private set; }

        private ServiceFavourites _FavouriteServices = null;
        public ServiceFavourites FavouriteServices
        {
            get
            {
                if (_FavouriteServices == null)
                {
                    SetFavouriteServices();
                }
                return _FavouriteServices;
            }
            private set
            {
                _FavouriteServices = value;
            }
        }

        private VUI3ServiceSnapshotList _FavouriteServiceSnapshots = null;
        public VUI3ServiceSnapshotList FavouriteServiceSnapshots
        {
            get
            {
                if (_FavouriteServiceSnapshots == null)
                {
                    SetFavouriteServiceSnapshots();
                }
                return _FavouriteServiceSnapshots;
            }
            private set 
            {
                _FavouriteServiceSnapshots = value;
            }
        }

        private ImageFavourites _FavouriteImages = null;
        public ImageFavourites FavouriteImages
        {
            get
            {
                if (_FavouriteImages == null)
                {
                    SetFavouriteImages();
                }
                return _FavouriteImages;
            }
            private set
            {
                _FavouriteImages = value;
            }
        }


        /// <summary>
        /// Get the Current User and no other
        /// </summary>
        public VUIUser()
        {
            this.Member = VUIfunctions.CurrentUser();
            this.UserStatus = VUIfunctions.VUI_USERTYPE_NONE;
            if (this.Member != null)
            {
                this.UserStatus = VUIfunctions.MemberVUIStatus(this.Member);
            }
        }


        /// <summary>
        /// INitialises the FavouriteServices property
        /// </summary>
        private void SetFavouriteServices()
        {
            string json = String.Empty;
            if (this.Member.getProperty("favouriteServices") != null && !String.IsNullOrEmpty(this.Member.getProperty("favouriteServices").Value.ToString()))
            {
                json = this.Member.getProperty("favouriteServices").Value.ToString();
            }
            VUI3ServiceSnapshotList ssl = new VUI3ServiceSnapshotList();

            FavouriteServices = new ServiceFavourites();
            if (!String.IsNullOrEmpty(json))
            {
                try
                {
                    ServiceFavourites favs = VUI3Utility.DeserialiseJson<ServiceFavourites>(json);
                    FavouriteServices = favs;
                }
                catch (Exception ex) { ; }
            }
        }

        /// <summary>
        /// Initialises the FavouriteServiceSnapshots Property
        /// </summary>
        private void SetFavouriteServiceSnapshots()
        {
            VUI3ServiceSnapshotList ssl = new VUI3ServiceSnapshotList();
            ServiceFavourites favs = this.FavouriteServices;

            if (Member.getProperty("vuiLastLogin").Value != null && !String.IsNullOrEmpty(Member.getProperty("vuiLastLogin").Value.ToString()))
            {
                DateTime retrospectiveDate = (DateTime)Member.getProperty("vuiLastLogin").Value;
                ssl.isRetrospective = true;
                ssl.retrospectiveDate = retrospectiveDate;

                ///TODO
                ssl.isRetrospective = false;

            }
            try
            {
                foreach (ServiceFavourite svc in favs.serviceFavourites)
                {
                    VUI3ServiceMaster sm = new VUI3ServiceMaster(Int32.Parse(svc.id));

                    if (ssl.isRetrospective)
                    {
                        ssl.results.Add(sm.GetSnapshotByDate(ssl.retrospectiveDate));
                    }
                    else
                    {
                        ssl.results.Add(sm.GetSnapshot());
                    }
                }
            }
            catch(Exception ex) { ; }
        
            _FavouriteServiceSnapshots = ssl;
        }

        /// <summary>
        /// Save Services back to the Member item in Umbraco (JSON in a textarea)
        /// </summary>
        private void SaveFavouriteServices()
        {
            this.Member.getProperty("favouriteServices").Value = FavouriteServices.AsJson();
        }

        /// <summary>
        /// Update the FavouriteServices for a User (and save them). Normally call FavoutireServices or FavouriteServiceSnapshots to get the updated data.
        /// </summary>
        /// <param name="ServiceName">THe Service Name string</param>
        public void AddFavouriteService(string ServiceName)
        {
            if (FavouriteServices.serviceFavourites.Where<ServiceFavourite>(f => f.serviceName.Equals(ServiceName)).Count() == 0)
            {
                VUI3ServiceMaster sm = new VUI3ServiceMaster(ServiceName);
                ServiceFavourite f = new ServiceFavourite(sm);
                FavouriteServices.serviceFavourites.Add(f);
                SaveFavouriteServices();
            }
        }

        /// <summary>
        /// Update the FavouriteServices for a User (and save them). Normally call FavoutireServices or FavouriteServiceSnapshots to get the updated data.
        /// </summary>
        /// <param name="ServiceName">THe Service Id</param>
        public void AddFavouriteService(int Id)
        {
            if (FavouriteServices.serviceFavourites.Where<ServiceFavourite>(f => f.id.Equals(Id.ToString())).Count() == 0)
            {
                VUI3ServiceMaster sm = new VUI3ServiceMaster(Id);
                ServiceFavourite f = new ServiceFavourite(sm);
                FavouriteServices.serviceFavourites.Add(f);
                SaveFavouriteServices();
            }
        }

        /// <summary>
        /// Delete a Service from FavouriteServices for a User (and save them). Normally call FavoutireServices or FavouriteServiceSnapshots to get the updated data.
        /// </summary>
        /// <param name="ServiceName">THe Service Name string</param>
        public void DeleteFavouriteService(string ServiceName)
        {
            if (FavouriteServices.serviceFavourites.Where<ServiceFavourite>(f => f.serviceName.Equals(ServiceName)).Count() > 0)
            {
                FavouriteServices.serviceFavourites.RemoveAll(f => f.serviceName.Equals(ServiceName));
                SaveFavouriteServices();
            }
        }

        /// <summary>
        /// Delete a Service from FavouriteServices for a User (and save them). Normally call FavoutireServices or FavouriteServiceSnapshots to get the updated data.
        /// </summary>
        /// <param name="ServiceName">THe Service Id</param>
        public void DeleteFavouriteService(int Id)
        {
            if (FavouriteServices.serviceFavourites.Where<ServiceFavourite>(f => f.id.Equals(Id.ToString())).Count() > 0)
            {
                FavouriteServices.serviceFavourites.RemoveAll(f => f.id.Equals(Id.ToString()));
                SaveFavouriteServices();
            }
        }

        /// <summary>
        /// Set the Favourite Images from the Database
        /// </summary>
        private void SetFavouriteImages()
        {
            _FavouriteImages = VUI3Utility.FavouriteScreenshotList(this.Member.Id);
        }

        public void AddFavouriteImage(int id)
        {
            AddFavouriteImage(id, ImageFavouriteCollection.DEFAULT_COLLECTION);
        }

        /// <summary>
        /// Add a screenshot to the list of favourites.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        public void AddFavouriteImage(int id, string collection)
        {
            ImageFavouriteCollection col;
            if (FavouriteImages.collections.Where<ImageFavouriteCollection>(c => c.collectionName.Equals(collection)).Count() == 0)
            {
                col = new ImageFavouriteCollection(collection);
                FavouriteImages.collections.Add(col);
            }
            else
            {
                col = FavouriteImages.collections.First<ImageFavouriteCollection>(c => c.collectionName.Equals(collection));
            }

            if (col.imageFavourites.Where(i => i.Id == id).Count() == 0)
            {
                VUI3Screenshot s = VUI3Utility.GetScreenshot(id);
                if (s != null)
                {
                    s.DateAdded = DateTime.Now;
                    col.imageFavourites.Add(s);
                }
                VUI3Utility.SaveFavouriteScreenshots(FavouriteImages, Member.Id);
            }
        }


        public List<Member> GetVUIUsers()
        {
            List<Member> vuiusers = new List<Member>();

            if (UserStatus.Equals(VUIfunctions.VUI_USERTYPE_ADMIN))
            {
                vuiusers = VUIfunctions.GetVUIUsersForAdmin(Member);
            }
            return vuiusers;
        }


        public string GetVUIUsersAsJSON()
        {
            List<Member> vuiusers = GetVUIUsers();

            
            string memberdata = @"[ {0} ]";
            string[] ms = new string[vuiusers.Count];

            int i = 0;
            foreach (Member m in vuiusers)
            {
                ms[i] = string.Format(@"{{ ""id"":{0}, ""emailaddress"":""{1}"" }}", m.Id, m.Email);
                i++;
            }
            memberdata = String.Format(memberdata, String.Join(",", ms));
            return string.Format(@" {{ ""response"":""valid"", ""count"":{0}, ""data"": {1} }}", ms.Length, memberdata);
        }

        public int MaxUsers()
        {
            string dummy;
            return MaxUsers(out dummy);
        }

        public int MaxUsers(out string maxUsersString)
        {
            string maxUsers = VUIfunctions.CurrentUser().getProperty("vUINumberOfUsersAllowed").Value.ToString();
            int max = 0;
            if (Int32.TryParse(maxUsers, out max))
            {
                if (max < 0)
                {
                    maxUsersString = @" an unlimited number of ";
                }
                else
                {
                    maxUsersString = @" up to " + maxUsers + " ";
                }
            }
            else
            {
                maxUsersString = VUIfunctions.CurrentUser().getProperty("vUINumberOfUsersAllowed").Value.ToString();
            }
            /*
             * if (max > 0 && vuiusers.Count >= max)
            {
                log.Debug("Admin - How many users allowed?");
                btnAddVUIUser.ToolTip = "You can't add more users";
                btnAddVUIUser.Enabled = false;
            }
             */
            return max;
        }


        public bool ChangePassword(string pwd, out string message)
        {
            message = "";
            List<string> errsPwd = new List<string>();

            if (String.IsNullOrEmpty(pwd))
            {
                errsPwd.Add("You cannot have an empty password");
            }
            else if (pwd.Length < 6)
            {
                errsPwd.Add("That password is too short, enter something at least 6 characters long");
            }
            if (pwd.Length > 20)
            {
                errsPwd.Add("A password can have a maximum of 20 characters, you entered " + pwd.Length.ToString());
            }
            if (!VUI3Utility.PasswordOk(pwd))
            {
                errsPwd.Add("Your password should contain a mixture of letters and numbers and at least one funny character");
            }
            if (errsPwd.Count > 0)
            {
                message = String.Join("<br/>", errsPwd);
                return false;
            }
            else
            {
                Member.Password = pwd;
                Member.Save();
                message = "Password Changed";
                return true;
            }
        }

        public bool DeleteFavouriteImage(int id)
        {
            return DeleteFavouriteImage(id, ImageFavouriteCollection.DEFAULT_COLLECTION);
        }
        public bool DeleteFavouriteImage(int id, string collection)
        {
            bool ret = false;
            if (FavouriteImages.collections.Where<ImageFavouriteCollection>(c => c.collectionName.Equals(collection)).Count() > 0)
            {
                ImageFavouriteCollection col = FavouriteImages.collections.First<ImageFavouriteCollection>(c => c.collectionName.Equals(collection));
                ret = (col.imageFavourites.RemoveAll(s => s.Id == id) > 0);
            }
            VUI3Utility.SaveFavouriteScreenshots(FavouriteImages, Member.Id);
            return ret;
        }

    }
}
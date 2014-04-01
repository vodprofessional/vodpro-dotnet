using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VUI.VUI3.classes
{
    public class DailySnapshotCache
    {
        private int cachetime = 30;
        private string _json = String.Empty;

        public DailySnapshotCache()
        {

        }
        
        public String DailySnapshotJSON
        {
            get
            {
                if (String.IsNullOrEmpty(_json))
                {
                    _json = HttpContext.Current.Cache["DailySnapshotJSON"] as String;
                    if (_json == null)
                    {
                        // log.Debug("Refreshing StationDetails Cache");
                        _json = DailySnapshot.GetDailySnapshot();
                        HttpContext.Current.Cache.Insert("DailySnapshotJSON", _json, null, DateTime.UtcNow.AddMinutes(cachetime), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                }
                return _json;
            }
            set
            {
                _json = value;
                HttpContext.Current.Cache.Insert("DailySnapshotJSON", _json, null, DateTime.UtcNow.AddMinutes(cachetime), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

    }
}
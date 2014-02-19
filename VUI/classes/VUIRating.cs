using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VUI.classes
{
    
    public class VUIRating
    {
        public static string MODE_PERSONAL = "P";
        public static string MODE_TEAM = "T";
        public static string MODE_OVERALL = "O";

        public VUIRating(string _mode)
        {
            Mode = _mode;
            NumRaters = 0;
            TotalRatings = 0;
        }

        public void AddRating(int rating)
        {
            NumRaters++;
            TotalRatings += rating;
        }

        public string Mode { get; set; }
        public int NumRaters { get; set; }
        public int TotalRatings { get; set; }
        public float Rating
        {
            get
            {
                if (NumRaters == 0)
                {
                    return 0;
                }
                else
                {
                    return (float)Decimal.Divide(TotalRatings, NumRaters);
                }
            }
        }
    }
}
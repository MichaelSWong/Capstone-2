using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Site
    {
        public int SiteID { get; set; }
        public int CampgroundID { get; set; }
        public int SiteNumber { get; set; }
        public int MaxOccupancy { get; set; }
        public bool HandicapAccess { get; set; }
        public int MaxRVLength { get; set; }
        public bool Utilities { get; set; }
    }
}

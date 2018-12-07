using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Campground
    {
        public int Campground_id { get; set; }

        public int Park_id { get; set; }

        public string Name { get; set; }

        public int Open_from_mm { get; set; }

        public int Open_to_mm { get; set; }

        public decimal Daily_fee { get; set; }

        public decimal GetTotalCost(DateTime startDate, DateTime endDate)
        {
            return Daily_fee * (decimal)(endDate - startDate).TotalDays;
        }
    }
}

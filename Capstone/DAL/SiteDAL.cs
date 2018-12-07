using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using Capstone.Models;

namespace Capstone.DAL
{
    public class SiteDAL
    {
        string connectionString;

        public SiteDAL(string connection)
        {
            connectionString = connection;
        }

        public SiteDAL()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        }


        /// <summary>
        /// Populate a list of site objects using the given reader.
        /// </summary>
        /// <param name="reader">Reader with SQL result.</param>
        /// <returns>List of site objects.</returns>
        private List<Site> PopulateList(SqlDataReader reader)
        {
            List<Site> outputs = new List<Site>();

            while (reader.Read())
            {
                outputs.Add(new Site());
                int lastI = outputs.Count - 1;

                outputs[lastI].SiteID = Convert.ToInt32(reader["site_id"]);
                outputs[lastI].CampgroundID = Convert.ToInt32(reader["campground_id"]);
                outputs[lastI].SiteNumber = Convert.ToInt32(reader["site_number"]);
                outputs[lastI].MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                outputs[lastI].HandicapAccess = Convert.ToBoolean(reader["accessible"]);
                outputs[lastI].MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
                outputs[lastI].Utilities = Convert.ToBoolean(reader["utilities"]);
            }

            return outputs;
        }

        /// <summary>
        /// Gets all the sites found in the database.
        /// </summary>
        /// <returns>List of site objects.</returns>
        public List<Site> GetSites()
        {
            List<Site> outputs = new List<Site>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site;", conn);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return outputs;
        }

        /// <summary>
        /// Get the top x sites that are available within our date range.
        /// </summary>
        /// <param name="startDate">Our start date.</param>
        /// <param name="endDate">Our end date.</param>
        /// <param name="topX">The top x sites. Default is 5.</param>
        /// <returns></returns>
        public List<Site> GetTopAvailableSites(DateTime startDate, DateTime endDate, int topX = 5, string testConnStr = "")
        {
            List<Site> cheapestSites = new List<Site>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT site.* FROM site JOIN campground ON site.campground_id = campground.campground_id ORDER BY campground.daily_fee", conn);
                    cheapestSites = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            ReservationDAL rDAL = (testConnStr != "") ? new ReservationDAL(testConnStr) : new ReservationDAL();
            List<Site> topFiveSites = new List<Site>();
            foreach(Site site in cheapestSites)
            {
                if(rDAL.CheckReservationAvailability(site, startDate, endDate))
                {
                    topFiveSites.Add(site);
                    if(topFiveSites.Count == topX)
                    {
                        break;
                    }
                }
            }

            return topFiveSites;
        }
    }
}

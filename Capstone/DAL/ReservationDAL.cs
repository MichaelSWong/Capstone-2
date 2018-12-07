using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationDAL
    {
        string connectionString;

        public ReservationDAL(string connection)
        {
            connectionString = connection;
        }

        public ReservationDAL()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        }


        /// <summary>
        /// Populate a list of reservation objects using the given reader.
        /// </summary>
        /// <param name="reader">Reader with SQL result.</param>
        /// <returns>List of reservation objects.</returns>
        private List<Reservation> PopulateList(SqlDataReader reader)
        {
            List<Reservation> outputs = new List<Reservation>();

            while (reader.Read())
            {
                outputs.Add(new Reservation());
                int lastI = outputs.Count - 1;

                outputs[lastI].ReservationID = Convert.ToInt32(reader["reservation_id"]);
                outputs[lastI].SiteID = Convert.ToInt32(reader["site_id"]);
                outputs[lastI].Name = Convert.ToString(reader["name"]);
                outputs[lastI].StartDate = Convert.ToDateTime(reader["from_date"]);
                outputs[lastI].EndDate = Convert.ToDateTime(reader["to_date"]);
                outputs[lastI].CreateDate = Convert.ToDateTime(reader["create_date"]);
            }

            return outputs;
        }

        /// <summary>
        /// Gets all the reservations found in the database.
        /// </summary>
        /// <returns>List of reservation objects.</returns>
        public List<Reservation> GetReservations()
        {
            List<Reservation> outputs = new List<Reservation>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM reservation;", conn);
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
        /// Checks to see the the given reservations' start and end dates both fall before/after our given start and end dates.
        /// </summary>
        /// <param name="reservations">The reservations to check.</param>
        /// <param name="startDate">Our start date.</param>
        /// <param name="endDate">Our end date.</param>
        /// <returns>True if no dates no conflicting dates.</returns>
        private bool CheckOuterBoundryDate(List<Reservation> reservations, DateTime startDate, DateTime endDate)
        {
            List<Reservation> outputs = new List<Reservation>();
            foreach (Reservation obj in reservations)
            {
                if (obj.StartDate <= startDate && obj.EndDate >= endDate)
                {
                    return false;
                }

                outputs.Add(obj);
            }

            return true;
        }

        /// <summary>
        /// Checks to see if our given dates conflict with other reservations.
        /// </summary>
        /// <param name="startDate">Our start date.</param>
        /// <param name="endDate">Our end date.</param>
        /// <returns>True if no dates no conflicting dates.</returns>
        public bool CheckReservationAvailability(DateTime startDate, DateTime endDate)
        {
            List<Reservation> outputs = new List<Reservation>();

            // Get all reservations whose start and end dates fall within our range
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if reservation start/end dates fall within our range
                    SqlCommand cmd = new SqlCommand("SELECT reservation.* FROM site JOIN campground ON site.campground_id = campground.campground_id JOIN reservation ON site.site_id = reservation.site_id WHERE (reservation.from_date BETWEEN @start AND @end OR reservation.to_date BETWEEN @start AND @end)", conn);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    outputs = PopulateList(cmd.ExecuteReader());
                    if (outputs.Count > 0) return false;
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get the good dates that don't fall within our range
                    SqlCommand cmd = new SqlCommand("SELECT reservation.* FROM site JOIN campground ON site.campground_id = campground.campground_id JOIN reservation ON site.site_id = reservation.site_id WHERE (reservation.from_date NOT BETWEEN @start AND @end AND reservation.to_date NOT BETWEEN @start AND @end)", conn);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            // Check that there are no good dates that have a start/end date outside our start/end dates
            return CheckOuterBoundryDate(outputs, startDate, endDate);
        }
        /// <summary>
        /// Checks to see if our given dates conflict with other reservations within the given site.
        /// </summary>
        /// <param name="site">The site to check for reservations.</param>
        /// <param name="startDate">Our start date.</param>
        /// <param name="endDate">Our end date.</param>
        /// <returns>True if no dates no conflicting dates.</returns>
        public bool CheckReservationAvailability(Site site, DateTime startDate, DateTime endDate)
        {
            List<Reservation> outputs = new List<Reservation>();

            // Get all reservations whose start and end dates fall within our range
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if reservation start/end dates fall within our range
                    SqlCommand cmd = new SqlCommand("SELECT reservation.* FROM site JOIN campground ON site.campground_id = campground.campground_id JOIN reservation ON site.site_id = reservation.site_id WHERE site.site_id = @siteID AND (reservation.from_date BETWEEN @start AND @end OR reservation.to_date BETWEEN @start AND @end)", conn);
                    cmd.Parameters.AddWithValue("@siteID", site.SiteID);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    outputs = PopulateList(cmd.ExecuteReader());
                    if (outputs.Count > 0) return false;
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get the good dates that don't fall within our range
                    SqlCommand cmd = new SqlCommand("SELECT reservation.* FROM site JOIN campground ON site.campground_id = campground.campground_id JOIN reservation ON site.site_id = reservation.site_id WHERE site.site_id = @siteID AND (reservation.from_date NOT BETWEEN @start AND @end AND reservation.to_date NOT BETWEEN @start AND @end)", conn);
                    cmd.Parameters.AddWithValue("@siteID", site.SiteID);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            // Check that there are no good dates that have a start/end date outside our start/end dates
            return CheckOuterBoundryDate(outputs, startDate, endDate);
        }
        /// <summary>
        /// Checks to see if our given dates conflict with other reservations within the given campground.
        /// </summary>
        /// <param name="campground">The campground to check for reservations.</param>
        /// <param name="startDate">Our start date.</param>
        /// <param name="endDate">Our end date.</param>
        /// <returns>True if no dates no conflicting dates.</returns>
        public bool CheckReservationAvailability(Campground campground, DateTime startDate, DateTime endDate)
        {
            List<Reservation> outputs = new List<Reservation>();

            // Get all reservations whose start and end dates fall within our range
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if reservation start/end dates fall within our range
                    SqlCommand cmd = new SqlCommand("SELECT reservation.* FROM site JOIN campground ON site.campground_id = campground.campground_id JOIN reservation ON site.site_id = reservation.site_id WHERE campground.campground_id = @campgroundID AND (reservation.from_date BETWEEN @start AND @end OR reservation.to_date BETWEEN @start AND @end)", conn);
                    cmd.Parameters.AddWithValue("@campgroundID", campground.Campground_id);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    outputs = PopulateList(cmd.ExecuteReader());
                    if (outputs.Count > 0) return false;

                    
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get the good dates that don't fall within our range
                    SqlCommand cmd = new SqlCommand("SELECT reservation.* FROM site JOIN campground ON site.campground_id = campground.campground_id JOIN reservation ON site.site_id = reservation.site_id WHERE campground.campground_id = @campgroundID AND (reservation.from_date NOT BETWEEN @start AND @end AND reservation.to_date NOT BETWEEN @start AND @end)", conn);
                    cmd.Parameters.AddWithValue("@campgroundID", campground.Campground_id);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }
            
            // Check that there are no good dates that have a start/end date outside our start/end dates
            return CheckOuterBoundryDate(outputs, startDate, endDate);
        }

        /// <summary>
        /// Get reservations that are upcoming by dist days away.
        /// </summary>
        /// <param name="dist">Number of days away.</param>
        /// <returns>Reservations upcoming.</returns>
        public List<Reservation> GetUpcoming(int dist)
        {
            List<Reservation> outputs = new List<Reservation>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM reservation WHERE from_date BETWEEN GETDATE() AND GETDATE() + @dist ORDER BY from_date;", conn);
                    cmd.Parameters.AddWithValue("@dist", dist);
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
        /// Create a reservation.
        /// </summary>
        /// <param name="site">The site of the reservation.</param>
        /// <param name="name">The name of the customer.</param>
        /// <param name="startDate">Start date of the reservation.</param>
        /// <param name="endDate">End date of the reservation.</param>
        /// <returns>The confrimation number (reservation ID).</returns>
        public int CreateReservation(Site site, string name, DateTime startDate, DateTime endDate)
        {
            if (!CheckReservationAvailability(site, startDate, endDate))
            {
                return -1;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT reservation (site_id, name, from_date, to_date) VALUES (@siteID, @name, @startDate, @endDate)", conn);
                    cmd.Parameters.AddWithValue("@siteID", site.SiteID);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0) return -1;
                    
                    cmd.CommandText = "SELECT * FROM reservation WHERE site_id = @siteID AND name = @name AND from_date = @startDate AND to_date = @endDate";
                    return PopulateList(cmd.ExecuteReader())[0].ReservationID;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }
    }
}

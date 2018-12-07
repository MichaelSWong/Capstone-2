using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using Capstone.Models;

namespace Capstone.DAL
{
    public class CampgroundSqlDAL
    {
        public string connection;// = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;

        public CampgroundSqlDAL(string connectionString)
        {
            connection = connectionString;
        }

        public CampgroundSqlDAL()
        {
            connection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        }               

        private List<Campground> PopulateList(SqlDataReader reader)
        {
            List<Campground> outputs = new List<Campground>();

            while (reader.Read())
            {
                outputs.Add(new Campground());
                outputs[outputs.Count - 1].Campground_id = Convert.ToInt32(reader["campground_id"]);
                outputs[outputs.Count - 1].Park_id = Convert.ToInt32(reader["park_id"]);
                outputs[outputs.Count - 1].Name = Convert.ToString(reader["name"]);
                outputs[outputs.Count - 1].Open_from_mm = Convert.ToInt32(reader["open_from_mm"]);
                outputs[outputs.Count - 1].Open_to_mm = Convert.ToInt32(reader["open_to_mm"]);
                outputs[outputs.Count - 1].Daily_fee = Convert.ToInt32(reader["daily_fee"]);
                
            }
            return outputs;
            
        }

        public List<Campground> GetCampgrounds()
        {
            List<Campground> outputs = new List<Campground>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Select * from campground ORDER BY name;");
                    cmd.Connection = conn;

                    outputs = PopulateList(cmd.ExecuteReader());
                    
                }
            }
            catch (SqlException)
            {

                throw;
            }

            return outputs;
        }        
        public List<Campground> GetCampgrounds(Park park)
        {
            List<Campground> outputs = new List<Campground>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE park_id = @parkID", conn);
                    cmd.Parameters.AddWithValue("@parkID", park.Park_id);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return outputs;
        }

        public List<Campground> GetTopFiveCost()
        {
            List<Campground> outputs = new List<Campground>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 5 * FROM campground ORDER BY daily_fee", conn);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return outputs;
        }
        public List<Campground> GetTopFiveCost(Park park)
        {
            List<Campground> outputs = new List<Campground>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 5 * FROM campground WHERE park_id = @parkID ORDER BY daily_fee", conn);
                    cmd.Parameters.AddWithValue("@parkID", park.Park_id);
                    outputs = PopulateList(cmd.ExecuteReader());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return outputs;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkSqlDAL
    {
        public string connection;// = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;

        public ParkSqlDAL(string connectionString)
        {
            connection = connectionString;
        }

        public ParkSqlDAL()
        {
            connection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        }

        private List<Park> PopulateList(SqlDataReader reader)
        {
            List<Park> outputs = new List<Park>();

            while (reader.Read())
            {
                outputs.Add(new Park());
                outputs[outputs.Count - 1].Park_id = Convert.ToInt32(reader["park_id"]);
                outputs[outputs.Count - 1].Name = Convert.ToString(reader["name"]);
                outputs[outputs.Count - 1].Location = Convert.ToString(reader["location"]);
                outputs[outputs.Count - 1].Establish_date = Convert.ToDateTime(reader["establish_date"]);
                outputs[outputs.Count - 1].Area = Convert.ToString(reader["area"]);
                outputs[outputs.Count - 1].Visitors = Convert.ToInt32(reader["visitors"]);
                outputs[outputs.Count - 1].Description = Convert.ToString(reader["description"]);
            }

            return outputs;
        }

        public List<Park> GetParks()
        {
            List<Park> outputs = new List<Park>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Select * from park ORDER BY name;");
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
        public List<Park> GetParks(int select)
        {
            List<Park> outputs = new List<Park>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Select * from park where park_id = @park_id ORDER BY name;");
                    cmd.Parameters.AddWithValue("@park_id", select);
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


    }
}

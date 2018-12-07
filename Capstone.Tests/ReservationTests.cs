using System;
using System.IO;
using System.Transactions;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.DAL;

namespace Capstone.Tests
{
    [TestClass]
    public class ReservationTests
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=NationalParkDB;Integrated Security=True";
        private TransactionScope trans;

        [TestInitialize]
        public void Init()
        {
            trans = new TransactionScope();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT reservation VALUES(1, 'TEST-1', '2018-12-9', '2018-12-13', GETDATE());", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }
        [TestCleanup]
        public void CleanUp()
        {
            trans.Dispose();
        }

        [TestMethod]
        public void TestGetReservations()
        {
            ReservationDAL rDAL = new ReservationDAL(connectionString);

            Assert.IsNotNull(rDAL.GetReservations());
        }

        [TestMethod]
        public void TestNoAvailability()
        {
            ReservationDAL rDAL = new ReservationDAL(connectionString);

            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 9), new DateTime(2018, 12, 13)));

            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 9), new DateTime(2018, 12, 12)));
            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 10), new DateTime(2018, 12, 13)));
            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 10), new DateTime(2018, 12, 12)));

            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 9), new DateTime(2018, 12, 14)));
            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 8), new DateTime(2018, 12, 13)));
            Assert.IsFalse(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 8), new DateTime(2018, 12, 14)));
        }
        [TestMethod]
        public void TestTrueAvailability()
        {
            ReservationDAL rDAL = new ReservationDAL(connectionString);

            Assert.IsTrue(rDAL.CheckReservationAvailability(new DateTime(2018, 12, 16), new DateTime(2018, 12, 20)));
        }

        [TestMethod]
        public void GetUpcoming()
        {
            ReservationDAL rDAL = new ReservationDAL(connectionString);

            Assert.IsNotNull(rDAL.GetUpcoming(60));
        }

        [TestMethod]
        public void CreateReservation()
        {
            ReservationDAL rDAL = new ReservationDAL(connectionString);

            Models.Site s = new Models.Site()
            {
                SiteID = 1
            };
            Assert.AreNotEqual(0, rDAL.CreateReservation(s, "CREATION-TEST", new DateTime(2019, 1, 11), new DateTime(2019, 1, 12)));
        }
    }
}

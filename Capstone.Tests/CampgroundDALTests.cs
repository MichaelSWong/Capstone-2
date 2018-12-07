using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.DAL;
using System.Transactions;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using Capstone;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class CampgroundDALTests
    {
        private string configPath = System.IO.Path.Combine(Environment.CurrentDirectory, "App.config");
        private string NationalParkDB;
        private TransactionScope myTransaction;
        

        CampgroundSqlDAL testObj = null;

        [TestInitialize]
        public void Setup()
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configPath;
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            NationalParkDB = cfg.ConnectionStrings.ConnectionStrings["CapstoneDatabase"].ToString();
            myTransaction = new TransactionScope();
            using (SqlConnection connection = new SqlConnection(NationalParkDB))
            {
                SqlCommand command;
                connection.Open();                

                command = new SqlCommand("insert into campground values (1,'Madison', 1, 1, '35.00')", connection);
                command.ExecuteNonQuery();                
                
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            myTransaction.Dispose();
        }

        [TestMethod]
        public void GetCampgroundsTest()
        {
            testObj = new CampgroundSqlDAL(NationalParkDB);

            IList<Campground> objs = testObj.GetCampgrounds();

            Assert.IsNotNull(objs);

            List<string> name = new List<string>();
            foreach  (Campground obj in objs)
            {
                name.Add(obj.Name);
            }
            CollectionAssert.Contains(name, "Madison");
        }
        [TestMethod]
        public void GetCampgroundsParamaterTest()
        {
            testObj = new CampgroundSqlDAL(NationalParkDB);
            Park testPark = new Park();
            testPark.Name = "Yosemite";
            testPark.Location = "California";
            testPark.Establish_date = DateTime.Parse("1872-03-01");
            testPark.Area = "47389";
            testPark.Visitors = 2593128;
            testPark.Description = "This is a very beautiful park";

            Assert.AreNotEqual(0, testObj.GetCampgrounds(testPark));
        }

        [TestMethod]
        public void GetTopFiveCostTest()
        {
            //Arrange

            testObj = new CampgroundSqlDAL(NationalParkDB);
            IList<Campground> objs = testObj.GetTopFiveCost();            

            //Act
            List<string> names = new List<string>();
            foreach (Campground obj in objs)
            {
                names.Add(obj.Name);
            }
            
            //Assert
            Assert.IsNotNull(objs);
            Assert.AreEqual(5, names.Count);
        }

        [TestMethod]
        public void GetTopFiveCostTestParameter()
        {
            //Arrange

            testObj = new CampgroundSqlDAL(NationalParkDB);
            IList<Campground> objs = testObj.GetTopFiveCost();

            //Act
            List<string> names = new List<string>(5);
            foreach (Campground obj in objs)
            {
                names.Add(obj.Name);
            }

            //Assert
            Assert.IsNotNull(objs);
            Assert.AreEqual(5, names.Count);
        }

    }   

}

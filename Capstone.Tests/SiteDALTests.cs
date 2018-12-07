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
    public class SiteDALTests
    {        
        private string configPath = System.IO.Path.Combine(Environment.CurrentDirectory, "App.config");
        private string NationalParkDB;
        private TransactionScope myTransaction;
        
        SiteDAL testObj = null;
        

        [TestInitialize]
        public void Setup()
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configPath;
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            NationalParkDB = cfg.ConnectionStrings.ConnectionStrings["CapstoneDatabase"].ToString();

            testObj = new SiteDAL(NationalParkDB);
            myTransaction = new TransactionScope();
            using (SqlConnection connection = new SqlConnection(NationalParkDB))
            {
                SqlCommand command;
                connection.Open();

                command = new SqlCommand("insert into campground values (1,'Madison', 1, 1, '35.00')", connection);
                command.ExecuteNonQuery();                                                                                                                                             

                command = new SqlCommand("insert into site values (1,1,6,0,20,1)", connection);
                command.ExecuteNonQuery();
            }                                                                         
        }
        [TestCleanup]             
        public void CleanUp()                                                                        
        {
            myTransaction.Dispose();
        }

        [TestMethod]
        public void GetSitesTest()
        {
            

            IList<Site> objs = testObj.GetSites();

            Assert.IsNotNull(objs);

            List<string> siteNo = new List<string>();
            foreach (Site obj in objs)
            {
                siteNo.Add(obj.SiteNumber.ToString());
            }
            CollectionAssert.Contains(siteNo, "1");
        }

        [TestMethod]
        public void GetTopAvailableSitesTest()
        {
            testObj = new SiteDAL(NationalParkDB);
            IList<Site> objs = testObj.GetTopAvailableSites(new DateTime(2018, 12, 01), new DateTime(2018, 12, 02),5, NationalParkDB);
            Assert.IsNotNull(objs);
        }


    }
}

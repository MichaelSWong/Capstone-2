using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.DAL;
using System.Transactions;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using Capstone;
using Capstone.Models;

namespace CapstoneTests
{
    [TestClass]
    public class ParkDALTests
    {
        private string configPath = System.IO.Path.Combine(Environment.CurrentDirectory, "App.config");
        private string NationalParkDB;
        private TransactionScope myTransaction;
        //const string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=NationalParkDB;Integrated Security=True";
        ParkSqlDAL testObj = null;



        [TestInitialize]
        public void Setup()
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configPath;
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            NationalParkDB = cfg.ConnectionStrings.ConnectionStrings["CapstoneDatabase"].ToString();

            testObj = new ParkSqlDAL(NationalParkDB);
            myTransaction = new TransactionScope();



            using (SqlConnection connection = new SqlConnection(NationalParkDB))
            {
                SqlCommand command;
                connection.Open();

                command = new SqlCommand("insert into park values ('Yellowstone', 'Wyoming', '1872-03-01', 2216978, 4257177, 'The greatest park on earth')", connection);
                command.ExecuteNonQuery();
                
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            myTransaction.Dispose();
        }

        [TestMethod]
        public void GetParksTest()
        {
            //arrange
            
            ParkSqlDAL parkDal = new ParkSqlDAL(NationalParkDB);
            IList<Park> objs = testObj.GetParks();


            //assert
            Assert.IsNotNull(objs);
            List<string> names = new List<string>();
            foreach (Park obj in objs)
            {
                names.Add(obj.Name);
            }
            CollectionAssert.Contains(names, "Yellowstone");
        }
        [TestMethod]
        public void GetParksTest2()
        {
            //arrange
            //testObj = new ParkSqlDAL(NationalParkDB);
            ParkSqlDAL parkDal = new ParkSqlDAL(NationalParkDB);
            IList<Park> objs = testObj.GetParks(1);


            //assert
            Assert.IsNotNull(objs);
            List<string> names = new List<string>();
            foreach (Park obj in objs)
            {
                names.Add(obj.Name);
            }
            Assert.AreEqual(1, names.Count);
        }
    }
}
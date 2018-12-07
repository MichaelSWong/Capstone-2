using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            //ReservationDAL r = new ReservationDAL();
            //Campground c = new Campground
            //{
            //    Campground_id = 1
            //};
            //r.GetAvailableReservations(new DateTime(2018, 11, 10), new DateTime(2018, 11, 12));

            //Console.ReadLine();
            CapstoneCLI cli = new CapstoneCLI();
            cli.RunCLI();


        }
    }
}

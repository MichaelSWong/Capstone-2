using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;
using Capstone.Class;

namespace Capstone
{
    public class CapstoneCLI
    {
        public string connection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;

        public void RunCLI()
        {
            
            //PrintHeader();
            //GetTheParks();
            MainMenu();
        }

        /// <summary>
        /// The Header of the reservation system.
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine(@" _      _               ________    _____     _____     _      _                _                                 ");
            Console.WriteLine(@"| \    | |       /\    |__    __|  |_   _|   / ___ \   | \    | |      /\      | |                             ");
            Console.WriteLine(@"|  \   | |      /  \      |  |       | |    / /   \ \  |  \   | |     /  \     | |                     ");
            Console.WriteLine(@"|   \  | |     / /\ \     |  |       | |   | |     | | |   \  | |    / /\ \    | |                      ");
            Console.WriteLine(@"| |\ \ | |    / /__\ \    |  |       | |   | |     | | | |\ \ | |   / /__\ \   | |                      ");
            Console.WriteLine(@"| | \ \| |   /  ____  \   |  |      _| |_   \ \___/ /  | | \ \| |  / ______ \  | |_____                 ");
            Console.WriteLine(@"|_|  \___|  /_/      \_\  |__|     |_____|   \_____/   |_|  \___| /_/      \_\ |_______|                          ");
            Console.WriteLine(@"                                                                                                          ");
            Console.WriteLine();
            Console.WriteLine("Select a Park for Further Details");
        }
        /// <summary>
        /// The main menu of the Reservation System.
        /// </summary>
        private void MainMenu()
        {
            bool isDone = false;
            while (!isDone)
            {

                Console.WriteLine("Welcome to the National Park Reservations\n");

                GetTheParks();

                Console.WriteLine();
                Console.WriteLine("Q - Quit");
                Console.WriteLine();

                string select = Console.ReadLine();
                while (!GeneralAssistance.CheckIfIntNumber(select) && select.ToLower() != "q")
                {
                    Console.Write("\nMust enter either Number or Q: ");
                    select = Console.ReadLine();
                }
                Console.Clear();
                // Process input
                if (select.ToLower() == "q")
                {
                    isDone = true;
                    return;
                }
                else
                {
                    GetParkInfo(select);
                }

            }

        }
        /// <summary>
        /// Gets the parks for the main menu
        /// </summary>
        private void GetTheParks()
        {
            Console.WriteLine("Select a Park for Further Details");
            ParkSqlDAL dal = new ParkSqlDAL(connection);
            IList<Park> parks = dal.GetParks();

            if (parks.Count > 0)
            {
                for (int i = 0; i < parks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {parks[i].Name}");
                }
            }
            else
            {
                Console.WriteLine("No results");
            }

            
        }


        /// <summary>
        /// Gets park info for selected Park
        /// </summary>
        private void GetParkInfo(string select)
        {
            ParkSqlDAL dal = new ParkSqlDAL(connection);
            Park park = dal.GetParks(int.Parse(select))[0];

            
            Console.WriteLine($"{park.Name}" + " National Park");
            Console.WriteLine($"Location:\t {park.Location.ToString()}");
            Console.WriteLine($"Established:\t {park.Establish_date.Month}/{park.Establish_date.Day}/{park.Establish_date.Year}");
            Console.WriteLine($"Area:\t\t {park.Area}"+ " sq km");
            Console.WriteLine($"Annual Visitors: {park.Visitors.ToString("N0")}");            
            Console.WriteLine($"\n{park.Description}");
                
            
            
            Console.WriteLine("\n1) View Campgrounds");
            //Console.WriteLine("2) Search for Reservation");
            Console.WriteLine("2) Return to Main Menu");

            string selected = Console.ReadLine();
            while (!GeneralAssistance.CheckIfIntNumber(selected) && (selected != "1" && selected != "2" && selected != "2"))
            {
                Console.Write("\nMust enter either '1', '2', or '3': ");
                selected = Console.ReadLine();
            }
            Console.Clear();
            // Process input
            if (selected == "2")
            {
                return;
            }
            else
            {
                CampgroundMenu(park);
            }

        }

        /// <summary>
        /// String array to get the months from ints
        /// </summary>
        private string[] MonthIndex =
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "Septemeber",
            "October",
            "November",
            "December"
        };

        /// <summary>
        /// Gets Campgrounds for selected park
        /// </summary>
        public void CampgroundMenu(Park select)
        {
            CampgroundSqlDAL dal = new CampgroundSqlDAL();
            List<Campground> campgrounds = dal.GetCampgrounds(select);

            
            Console.Clear();
            if (campgrounds.Count > 0)
            {
                Console.WriteLine("Name\t\t\t\t\t\t Open\t\t     Close\t\t      Daily Fee");
                
                for (int i = 0; i < campgrounds.Count; i++)
                {
                    string monthOpen = MonthIndex[campgrounds[i].Open_from_mm - 1];
                    string monthClosed = MonthIndex[campgrounds[i].Open_to_mm - 1];
                    Console.WriteLine($"#{i + 1} {campgrounds[i].Name.PadRight(45)} {monthOpen.PadRight(20)}{monthClosed.PadRight(24)} {campgrounds[i].Daily_fee.ToString("C").PadRight(50)}");
                }


            }
            else
            {
                Console.WriteLine("No results");
            }
            Console.WriteLine();
            Console.WriteLine("1) Search for Available Reservation");
            Console.WriteLine("2) Return to main menu");

            string selected = Console.ReadLine();
            while (!GeneralAssistance.CheckIfIntNumber(selected) && (selected != "1" && selected != "2") )
            {
                Console.Write("\nMust enter either '1', or '2': ");
                selected = Console.ReadLine();
            }
            Console.Clear();
            if (selected == "1")
            {
                Console.Clear();
                ReservationMenu(campgrounds);
            }
            else if (selected == "2")
            {
                Console.Clear();
                return;
            }
            else
            {
                Console.WriteLine("Enter a vaild number:");
            }
            


        }
        
        /// <summary>
        /// The reservation menu.
        /// </summary>
        /// <param name="campgrounds">The campgrounds selected by user.</param>
        public void ReservationMenu(List<Campground> campgrounds)
        {
            ReservationDAL rDAL = new ReservationDAL();
            bool isDone = false;
            while (!isDone)
            {
                Console.Clear();
                if (campgrounds.Count > 0)
                {
                    Console.WriteLine("Name\t\t\t\t\t\t Open\t\t     Close\t\t      Daily Fee");

                    for (int i = 0; i < campgrounds.Count; i++)
                    {
                        string monthOpen = MonthIndex[campgrounds[i].Open_from_mm - 1];
                        string monthClosed = MonthIndex[campgrounds[i].Open_to_mm - 1];
                        Console.WriteLine($"#{i + 1} {campgrounds[i].Name.PadRight(45)} {monthOpen.PadRight(20)}{monthClosed.PadRight(24)} {campgrounds[i].Daily_fee.ToString("C").PadRight(50)}");
                    }
                }

                Console.Write("\nWhich campground (enter 0 to cancel): ");
                int campgroundInt = GetIntInput(0, campgrounds.Count + 1);
                if(campgroundInt == 0)
                {
                    Console.Clear();
                    return;
                }
                campgroundInt--;

                Console.WriteLine("What is the arrival date");
                DateTime startDate = GetDateInput();
                Console.WriteLine("What is the departure date ");
                DateTime endDate = GetDateInput();
                while(endDate < startDate)
                {
                    Console.Write("\nEnd date can't be before start date.\n");

                    Console.WriteLine("What is the arrival date");
                    startDate = GetDateInput();
                    Console.WriteLine("What is the departure date ");
                    endDate = GetDateInput();
                }

                if (rDAL.CheckReservationAvailability(campgrounds[campgroundInt], startDate, endDate))
                {
                    SiteDAL sDAL = new SiteDAL();
                    List<Site> availableSites = sDAL.GetTopAvailableSites(startDate, endDate);

                    Console.WriteLine("\nSite Number\tMax Occupancy\tAccessability\tMax RV Length\tUtilities\tCost");
                    for (int i = 0; i < availableSites.Count; i++)
                    {
                        Console.WriteLine($"{availableSites[i].SiteNumber}\t\t{availableSites[i].MaxOccupancy}\t\t{availableSites[i].HandicapAccess}\t\t{availableSites[i].MaxRVLength}\t\t{availableSites[i].Utilities}\t\t{(campgrounds[campgroundInt].Daily_fee * (decimal)(endDate - startDate).TotalDays).ToString("C")}");
                    }

                    int highestNum = -1;
                    List<int> siteNums = new List<int>();
                    List<Site> sites = new List<Site>();
                    foreach(Site site in availableSites)
                    {
                        siteNums.Add(site.SiteNumber);
                        sites.Add(site);
                        if(site.SiteNumber > highestNum)
                        {
                            highestNum = site.SiteNumber;
                        }
                    }
                    Console.Write("\nWhich site should be reserved (enter 0 to cancel): ");
                    int responseInt = GetIntInput(0, highestNum);
                    bool isDoneTwo = false;
                    while(!isDoneTwo && responseInt != 0)
                    {
                        if(responseInt == 0)
                        {
                            break;
                        }

                        if(siteNums.Contains(responseInt))
                        {
                            Console.Write("What name should the reservation be made under?: ");
                            string name = Console.ReadLine();

                            int reservationNum = rDAL.CreateReservation(sites[siteNums.IndexOf(responseInt)], name, startDate, endDate);
                            if (reservationNum != -1)
                            {
                                Console.WriteLine($"The reservation has been made and the confirmatin number is #{reservationNum}.");

                                Console.WriteLine("Press any key to contiue...");
                                Console.ReadKey();
                                isDone = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Failed to create reservation!");

                                Console.WriteLine("Press any key to contiue...");
                                Console.ReadKey();
                                isDone = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No available reservations with your dates.");
                    Console.Write("Cancel? ");
                    string response = Console.ReadLine();
                    if(response.ToLower() == "y" || response.ToLower() == "yes")
                    {
                        isDone = true;
                    }
                }
            }

            Console.Clear();
        }


        
        

        public int GetIntInput(int min, int max)
        {
            string response = Console.ReadLine();
            int responeInt = -1;
            while (responeInt < min || responeInt >= max)
            {
                while (!GeneralAssistance.CheckIfIntNumber(response))
                {
                    Console.WriteLine("Invalid input!");
                    response = Console.ReadLine();
                }

                responeInt = int.Parse(response);
                if (responeInt < min && responeInt > max)
                {
                    Console.WriteLine($"Invalid range! Must be 1-{max}");
                }
            }

            return responeInt;
        }

        /// <summary>
        /// List of months with thier total days.
        /// </summary>
        public enum MonthDays
        {
            Jan = 31,
            Feb = 28,
            Mar = 31,
            Apr = 30,
            May = 31,
            Jun = 30,
            Jul = 31,
            Aug = 31,
            Sep = 30,
            Oct = 31,
            Nov = 30,
            Dec = 31
        };
        /// <summary>
        /// Checks the given year to see if it is a leap year. Returns true if it is.
        /// </summary>
        /// <param name="year">The year to check. Uses current year by default.</param>
        public bool CheckLeapYear(int year)
        {
            if (year > -1 && (year % 4 == 0 && (!(year % 100 == 0) || year % 400 == 0))) return true;

            return false;
        }
        /// <summary>
        /// Gets the total days in the given month.
        /// </summary>
        /// <param name="month">The month to check.</param>
        public int GetTotalMonthDays(int month, int year)
        {
            if (month == 0) return (int)MonthDays.Jan;
            else if (month == 1)
            {
                if (CheckLeapYear(year)) return 29;
                else return 28;
            }
            else if (month == 2) return (int)MonthDays.Mar;
            else if (month == 3) return (int)MonthDays.Apr;
            else if (month == 4) return (int)MonthDays.May;
            else if (month == 5) return (int)MonthDays.Jun;
            else if (month == 6) return (int)MonthDays.Jul;
            else if (month == 7) return (int)MonthDays.Aug;
            else if (month == 8) return (int)MonthDays.Sep;
            else if (month == 9) return (int)MonthDays.Oct;
            else if (month == 10) return (int)MonthDays.Nov;
            else if (month == 11) return (int)MonthDays.Dec;
            
            return 0;
        }
        public DateTime GetDateInput()
        {
            int month = -1;
            int day = -1;
            int year = -1;
            bool isDone = false;
            while (!isDone)
            {
                Console.Write("(MM/DD/YYYY): ");
                string response = Console.ReadLine();
                string[] dateSections = response.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if(dateSections.Length != 3)
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }
                if(!GeneralAssistance.CheckIfIntNumber(dateSections[0]) || !GeneralAssistance.CheckIfIntNumber(dateSections[1]) || !GeneralAssistance.CheckIfIntNumber(dateSections[2]))
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }

                month = int.Parse(dateSections[0]);
                if(month < 1 || month > 12)
                {
                    Console.WriteLine("Not a month!");
                    continue;
                }
                year = int.Parse(dateSections[2]);
                if (year < 0 || year > 9999)
                {
                    Console.WriteLine("Not a valid year!");
                    continue;
                }
                day = int.Parse(dateSections[1]);
                if (day < 1 || day > GetTotalMonthDays(month - 1, year))
                {
                    Console.WriteLine("Not a day!");
                    continue;
                }

                isDone = true;
            }

            return new DateTime(year, month, day);
        }
    }
}



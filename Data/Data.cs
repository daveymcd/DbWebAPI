using DbWebAPI.Helpers;
using DbWebAPI.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Data
{
    /// <summary>
    /// 
    ///     DbWebAPI.Data - SCx Test Data Class
    ///     
    ///     Setup the SCxDb Database with test data.
    /// </summary>
    /// <remarks>
    /// 
    ///     Called from DbWebApi.Program.cs.CreateDbIfNotExists(IHost)
    ///     to setup the SQL Server database context.
    ///     
    /// </remarks>
    public static class DbInitializer
    {
        /// <summary>
        ///     DbWebAPI.Data.DbInitializer.Initialise
        ///     
        ///     Called by DbWebAPI.Program
        ///     Invoke DbWebAPI.Models.OnModelCreating to associate Tables.
        ///     and add test archive data to SCxDb database 
        /// </summary>
        /// <param name="context">DbContext service for SQL Server database</param>
        public static void Initialize(SCxItemContext context)
        {
            context.Database.EnsureCreated();
            if (!context.SC1Items.Any() &&
                !context.SC2Items.Any() &&
                !context.SC3Items.Any() &&
                !context.SC4Items.Any() &&
                !context.SC9Items.Any())
            {                                               // If no data - setup test data for the past 3 years.
                for (var month = 0; month <= 9; month++)    // changed from 36 to 9
                {
                    AddThisMonthsSCxData(context, month);
                }
            }
            else
            {                                               // If no documents exist for Today - setup todays document data
                var todaysItem = from item in context.SC1Items.Where(arg => arg.TimeStamp.Date == DateTime.Today.Date) select item.Dept;
                if (!todaysItem.Any()) 
                {
                    AddTodaysSCxData(context, 0);
                }
            }
        }

        /// <summary>
        ///     DbWebAPI.Data.DbInitializer.AddThisMonthsSCxData
        ///     
        ///     Called by DbWebAPI.Data.DbInitializer.Initialize
        ///     Add a months worth of test archive data to SCxDb database 
        ///     Method can be repeated any number of times to create years 
        ///     worth of data.    
        /// </summary>
        /// <param name="context">DbContext Service for SQL Server Database</param>
        /// <param name="monthOffset">month offset from current month, to setup data for</param>
        /// <returns></returns>
        public static void AddThisMonthsSCxData(SCxItemContext context, double? monthOffset = 0)
        {

            monthOffset = 31 * -monthOffset;
            var sc1Items = new ObservableCollection<SC1Item>
            {
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-31),
                    Dept = "Pantry",
                    Food = "Lamb Shank in Onion Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 2,
                    Temperature = 94,
                    Comment = string.Empty,
                    Sign = string.Empty,
                    SignOff = string.Empty

                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-27),
                    Dept = "Pantry",
                    Food = "Pork Loin in Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 1,
                    Temperature = 32,
                    Comment = "Packaging torn open in transit. 12 servings ruined",
                    Sign = string.Empty,
                    SignOff = "Chef"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-25),
                    Dept = "Pantry",
                    Food = "Sole",
                    Supplier = "Fish Monger",
                    CheckUBD = 2,
                    Temperature = 10,
                    Comment = "Fish held up in transit and arrived after UBD expired",
                    Sign = string.Empty,
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-24),
                    Dept = "Pantry",
                    Food = "Lamb Cuttlets in Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 2,
                    Temperature = 90,
                    Comment = "Expired",
                    Sign = string.Empty,
                    SignOff = string.Empty
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-23),
                    Dept = "Pantry",
                    Food = "Fish in Batter",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = -5,
                    Comment = "Delivery of 12 Battered Cod portions under weight" ,
                    Sign = string.Empty,
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-22),
                    Dept = "Prep-Area",
                    Food = "Chicken",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 0,
                    Temperature = 92,
                    Comment = "16 ready cooked Chickens whole. Had to be prep'd before serving",
                    Sign = string.Empty,
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-21),
                    Dept = "Prep-Area",
                    Food = "Fish",
                    Supplier = "Fish Monger",
                    CheckUBD = 0,
                    Temperature = 65,
                    Comment = "12 Cod fillets, 6 whole Sole, 12 salmon steaks. Individual Boil-in-the-bag portions ",
                    Sign = string.Empty,
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-19),
                    Dept = "Prep-Area",
                    Food = "Cod",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = 1,
                    Comment = "12 fillets arrived partly defrosted. Use today",
                    Sign = string.Empty,
                    SignOff = "Chef"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-17),
                    Dept = "Pantry",
                    Food = "Fish",
                    Supplier = "Fish Monger",
                    CheckUBD = 0,
                    Temperature = 2,
                    Comment = "12 Cod steaks, 6 salmon steaks. Chilled",
                    Sign = string.Empty,
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-16),
                    Dept = "Pantry",
                    Food = "Kebab",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 66,
                    Comment = "24 ready cooked lamb, onion & pepper skewers",
                    Sign = "Deliveries",
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-10),
                    Dept = "Pantry",
                    Food = "Chicken Nuggets",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 81,
                    Comment = "20 child portions in individual foil trays.",
                    Sign = "Deliveries",
                    SignOff = "Charlie"
                }
            };
            foreach (var item in sc1Items) {
                context.SC1Items.Add(item);
                SCxItem itemX = new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = item.Type,
                    TimeStamp = item.TimeStamp,
                    Dept = item.Dept,
                    Food = item.Food,
                    Supplier = item.Supplier,
                    CheckUBD = item.CheckUBD,
                    Temperature = item.Temperature,
                    Comment = item.Comment,
                    Sign = item.Sign,
                    SignOff = item.SignOff
                };
                context.SCxItems.Add(itemX);
            }

            var sc3Items = new ObservableCollection<SC3Item>
            {
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-29),
                    Dept = "Kitchen",
                    Food = "Lamb Leg Steaks in Red Wine Sauce",
                    CookTemp = 82,
                    Comment = "Customer requested bloody rare steak",
                    SignOff = "Manager"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-28),
                    Dept = "Kitchen",
                    Food = "Beef",
                    CookTemp = 65,
                    Comment = "2 medium rare, 1 rare",
                    SignOff = "Supervisor"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-26),
                    Dept = "Prep-Area",
                    Food = "Lamb in onion Gravy",
                    CookTemp = 80,
                    Comment = "12 servings. UBD expired",
                    SignOff = string.Empty
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-20),
                    Dept = "Prep-Area",
                    Food = "Venison",
                    CookTemp = 2,
                    Comment = "2 Haunches defrosted",
                    SignOff = "Charlie"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-18),
                    Dept = "Prep-Area",
                    Food = "Scampi",
                    CoolTemp = -6,
                    Comment = "Frozen product passed its UBD",
                    SignOff = "Supervisor"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-15),
                    Dept = "Kitchen",
                    Food = "Haddock in white wine sauce",
                    CookTemp = 80,
                    Comment = "12 portions prepared",
                    SignOff = "Chef"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-12),
                    Dept = "Kitchen",
                    Food = "Singapore Noodles",
                    CoolTemp = 1,
                    Comment = "12 portions defrosted",
                    SignOff = "Chef"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-11),
                    Dept = "Kitchen",
                    Food = "Lentil Soup",
                    ReheatTemp = 75,
                    Comment = "Pan of 9 portions from hot-holding reheated",
                    SignOff = "Chef"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-9),
                    Dept = "Kitchen",
                    Food = "Penne Carbonara",
                    CookTemp = 80,
                    Comment = "12 Boil-in-the-bag portions ready to serve",
                    SignOff = "Supervisor"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-8),
                    Dept = "Kitchen",
                    Food = "Cod Fishcakes",
                    CookTemp = 79,
                    Comment = "12 cakes defrosted (2 per portion)",
                    SignOff = "Manager"
                }
            };
            foreach (var item in sc3Items) { context.SC3Items.Add(item); }

            var sc4Items = new ObservableCollection<SC4Item>
            {
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-30),
                    Dept = "Front-Of-House",
                    Food = "Chicken & Leek Pie",
                    HoldTemp2 = 70,
                    Comment = "Charlie - Crust burnt on corner. 1 portion rejected",
                    Sign = "Hot Hold",
                    SignOff = "Chef"
                },
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-14),
                    Dept = "Front-Of-House",
                    Food = "Pork & Leek Pie",
                    HoldTemp2 = 81,
                    Comment = "6 pies received into hot-holding",
                    Sign = "Hot Hold",
                    SignOff = string.Empty
                },
                new SC4Item{
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-13),
                    Dept = "Front-Of-House",
                    Food = "Beef Wellington",
                    HoldTemp2 = 71,
                    Comment = "2 trays of 12 portions total",
                    Sign = "Hot Hold",
                    SignOff = "Chef"
                }
            };
            foreach (var item in sc4Items) { context.SC4Items.Add(item); }
            
            context.SaveChanges();

            AddThisWeeksSCxData(context, monthOffset);
        }

        ///<summary> 
        ///     DbWebAPI.Data.DbInitializer.AddThisWeeksSCxData
        ///     
        ///     Called by DbWebAPI.Data.DbInitializer.AddThisMonthsSCxData
        ///     to setup 'Last week of the month' test archive data 
        ///</summary>
        ///<param name="context">DbContext service for SQL Server database</param>
        ///<param name="monthOffset">Month offset (from current Month) to set data up for</param>
        public static void AddThisWeeksSCxData(SCxItemContext context, double? monthOffset = 0)
        {
            var sc1Items = new ObservableCollection<SC1Item>
            {
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-1),
                    Dept = "Prep-Area",
                    Food = "Lamb Passanda",
                    Supplier = "Curry House ltd",
                    CheckUBD = 2,
                    Temperature = 50,
                    Comment = "Product UBD exceeded",
                    Sign = "Deliveries In",
                    SignOff = string.Empty
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-20),
                    Dept = "Prep-Area",
                    Food = "Beef Stir Fry",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 1,
                    Temperature = 65,
                    Comment = "16 Boil in the bag portions",
                    Sign = "Deliveries In",
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1),
                    Dept = "Prep-Area",
                    Food = "Fillet of Sole",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = 60,
                    Comment = "8 fillets of good size",
                    Sign = "Deliveries In",
                    SignOff = "Supervisor"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-4).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Moules Marinier",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = 72,
                    Comment = "8 Boil-in-the-bag starter portions",
                    Sign = "Deliveries In",
                    SignOff = "Charlie"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-4).AddMinutes(-5),
                    Dept = "Kitchen",
                    Food = "Apple & Blackberry Pie",
                    Supplier = "Sams Baker's",
                    CheckUBD = 0,
                    Temperature = 43,
                    Comment = "1kg deep fill pie (8 portions)",
                    Sign = "Deliveries In",
                    SignOff = "Manager"
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-2),
                    Dept = "Pantry",
                    Food = "Salt & Pepper Squid",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 0,
                    Temperature = 65,
                    Comment = "12 portions in foil trays defrosted.",
                    Sign = "Deliveries In",
                    SignOff = "Supervisor"
                }
            };
            foreach (var item in sc1Items) { 
                context.SC1Items.Add(item);
                SCxItem itemX = new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = item.Type,
                    TimeStamp = item.TimeStamp,
                    Dept = item.Dept,
                    Food = item.Food,
                    Supplier = item.Supplier,
                    CheckUBD = item.CheckUBD,
                    Temperature = item.Temperature,
                    Comment = item.Comment,
                    Sign = item.Sign,
                    SignOff = item.SignOff
                };
                context.SCxItems.Add(itemX);
            }


            var sc2Items = new ObservableCollection<SC2Item>
            {
                new SC2Item {
                    Id = Guid.NewGuid(),
                    Type = "SC2:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-245),
                    Dept = "Kitchen",
                    Temperature = 3,
                    Comment = "12 portions in foil trays defrosted.",
                    Sign = "Close Check",
                    SignOff = "Supervisor"
                },
                new SC2Item {
                    Id = Guid.NewGuid(),
                    Type = "SC2:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset),
                    Dept = "Kitchen",
                    Temperature = 2,
                    Comment = "12 portions in foil trays defrosted.",
                    Sign = "Open Check",
                    SignOff = "Supervisor"
                }
            };
            foreach (var item in sc2Items) { context.SC2Items.Add(item); }

            var sc3Items = new ObservableCollection<SC3Item>
            {
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-5).AddMinutes(-45),
                    Dept = "Kitchen",
                    Food = "Fries",
                    CookTemp = 85,
                    Comment = "16 portions prepared",
                    SignOff = "Charlie"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-5).AddMinutes(-30),
                    Dept = "Kitchen",
                    Food = "Hamburger",
                    CookTemp = 87,
                    Comment = "16 patties prepared",
                    SignOff = "Charlie"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-5).AddMinutes(-110),
                    Dept = "Kitchen",
                    Food = "Hamburger",
                    CoolTemp = 2,
                    Comment = "24 Frozen patties defrosted",
                    SignOff = string.Empty
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-3).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Beef Sirloin",
                    CookTemp = 79,
                    Comment = "6 16oz size steaks Medium Rare",
                    SignOff = string.Empty
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-2),
                    Dept = "Kitchen",
                    Food = "Salt & Pepper Squid",
                    CookTemp = 88,
                    Comment = "12 portions in foil trays reheated.",
                    SignOff = "Chef"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1).AddMinutes(-50),
                    Dept = "Kitchen",
                    Food = "Southern Fried Chicken",
                    CookTemp = 1,
                    Comment = "20 Frozen portions defrosted",
                    SignOff = string.Empty
                },
                    new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1).AddMinutes(-45),
                    Dept = "Kitchen",
                    Food = "Fries",
                    CookTemp = 81,
                    Comment = "14 portions prepared",
                    SignOff = "Charlie"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1).AddMinutes(-40),
                    Dept = "Kitchen",
                    Food = "Southern Fried Chicken",
                    CookTemp = 84,
                    Comment = "12 portions prepared",
                    SignOff = "Charlie"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Beef Stir Fry",
                    CookTemp = 85,
                    Comment = "reheat",
                    SignOff = "Chef"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-3),
                    Dept = "Kitchen",
                    Food = "Pork & Leek Pie",
                    CookTemp = 88,
                    Comment = "Batch of 24 pies reheated",
                    SignOff = "Manager"
                }
            };
            foreach (var item in sc3Items) { context.SC3Items.Add(item); }

            var sc4Items = new ObservableCollection<SC4Item>
            {
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-5).AddMinutes(-25),
                    Dept = "Front-Of-House",
                    Food = "Fries",
                    HoldTemp2 = 75,
                    Comment = "16 portions received into Hot-Holding",
                    Sign = "Hot Hold",
                    SignOff = "Staff"
                },
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-5).AddMinutes(-20),
                    Dept = "Front-Of-House",
                    Food = "Hamburger",
                    HoldTemp2 = 80,
                    Comment = "16 patties recieved into Hot-Holding",
                    Sign = "Hot Hold",
                    SignOff = "Staff"
                },
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-3).AddMinutes(-20),
                    Dept = "Front-Of-House",
                    Food = "Beef Sirloin",
                    HoldTemp2 = 73,
                    Comment = "6 16oz size steaks received into Hot-Holding",
                    Sign = "Hot Hold",
                    SignOff = string.Empty
                },
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-2),
                    Dept = "Front-Of-House",
                    Food = "Salt & Pepper Squid",
                    HoldTemp2 = 75,
                    Comment = "12 portions in foil trays received into hot-holding.",
                    Sign = "Hot Hold",
                    SignOff = "Staff"
                },
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1).AddMinutes(-30),
                    Dept = "Front-Of-House",
                    Food = "Fries",
                    HoldTemp2 = 79,
                    Comment = "14 portions received into Hot-Holding",
                    Sign = "Hot Hold",
                    SignOff = "Staff"
                },
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1).AddMinutes(-20),
                    Dept = "Front-Of-House",
                    Food = "Southern Fried Chicken",
                    HoldTemp2 = 81,
                    Comment = "12 portions recieved into Hot-Holding",
                    Sign = "Hot Hold",
                    SignOff = "Staff"
                }
            };
            foreach (var item in sc4Items) { context.SC4Items.Add(item); }

            var sc9Items = new ObservableCollection<SC9Item>
            {
                new SC9Item {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-5).AddMinutes(-10),
                    Dept = "Dispatch",
                    Food = "Burger & Chips",
                    CustName = "Dr John H Watson",
                    CustAddr = "221b Baker St.",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "2 1/4 pounders with cheese, 2 Fries. ",
                    Sign = "Take Away",
                    SignOff = "Point-Of-Sale"
                },
                new SC9Item {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-1).AddMinutes(-10),
                    Dept = "Dispatch",
                    Food = "Chicken & Chips",
                    CustName = "Mr Philip Marlow", 
                    CustAddr = "Hobart Arms, Franklin Ave, Los Angeles County",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "Family Chicken Bucket and 2 Fries. ",
                    Sign = "Take Away",
                    SignOff = "Point-Of-Sale"
                },
                new SC9Item {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-5),
                    Dept = "Dispatch",
                    Food = "Chinese Take Away",
                    CustName = "Mr Sam Spade", 
                    CustAddr = "Falcon Hotel, Malta.",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "1 Beef Stir Fry, 1 King Prawn Kung Po, 1 Singapore Noodles, 2 Salt & Pepper Squid & 2 Fried Rice",
                    Sign = "Take Away",
                    SignOff = "Point-Of-Sale"
                },
                new SC9Item {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-7).AddMinutes(-45),
                    Dept = "Dispatch",
                    Food = "Pizza",
                    CustName = "Mr Hercule Poirot",
                    CustAddr = "Coach 3, Orient Express",
                    CheckUBD = 0,
                    Temperature = 60,
                    Comment = "2 pepperoni Passion, 1 Sloppy Joe & 1 Veggie Volcano",
                    Sign = "Take Away",
                    SignOff = "Point-Of-Sale"
                },
                new SC9Item {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-6).AddMinutes(-45),
                    Dept = "Dispatch",
                    Food = "Curry Take Away",
                    CustName = "Miss Marple",
                    CustAddr = "Milchester cottage, St. Mary Mead",
                    CheckUBD = 0,
                    Temperature = 69,
                    Comment = "1 Chicken Vindaloo, 1 Fried Rice & 1 Garlic Naan",
                    Sign = "Take Away",
                    SignOff = "Point-Of-Sale"
                }
            };
            foreach (var item in sc9Items) { context.SC9Items.Add(item); }

            context.SaveChanges();
        }
        ///<summary> 
        ///     DbWebAPI.Data.DbInitializer.AddTodaysSCxData
        ///     
        ///     Called by DbWebAPI.Data.DbInitialiser.Initialize
        ///     to setup 'Last day of the month' test archive data 
        ///</summary>
        ///<param name="context">DbContext service for SQL Server database</param>
        ///<param name="monthOffset">Month offset (from current Month) to set data up for</param>
        public static void AddTodaysSCxData(SCxItemContext context, double? monthOffset = 0)
        {
            var sc1Items = new ObservableCollection<SC1Item>
            {
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-1),
                    Dept = "Prep-Area",
                    Food = "Lamb Passanda",
                    Supplier = "Curry House ltd",
                    CheckUBD = 2,
                    Temperature = 50,
                    Comment = "Product UBD exceeded",
                    Sign = "Del In",
                    SignOff = string.Empty
                },
                new SC1Item {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-20),
                    Dept = "Prep-Area",
                    Food = "Beef Stir Fry",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 1,
                    Temperature = 65,
                    Comment = "16 Boil in the bag portions",
                    Sign = "Del In",
                    SignOff = "Manager"
                }
            };
            foreach (var item in sc1Items) {
                context.SC1Items.Add(item);
                SCxItem itemX = new SCxItem
                {
                    Id = Guid.NewGuid(),
                    Type = item.Type,
                    TimeStamp = item.TimeStamp,
                    Dept = item.Dept,
                    Food = item.Food,
                    Supplier = item.Supplier,
                    CheckUBD = item.CheckUBD,
                    Temperature = item.Temperature,
                    Comment = item.Comment,
                    Sign = item.Sign,
                    SignOff = item.SignOff
                };
                context.SCxItems.Add(itemX);
            }


            var sc2Items = new ObservableCollection<SC2Item>
            {
                new SC2Item {
                    Id = Guid.NewGuid(),
                    Type = "SC2:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-245),
                    Dept = "Kitchen",
                    Temperature = 3,
                    Comment = "12 portions in foil trays defrosted.",
                    Sign = "Close Check",
                    SignOff = "Supervisor"
                },
                new SC2Item {
                    Id = Guid.NewGuid(),
                    Type = "SC2:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset),
                    Dept = "Kitchen",
                    Temperature = 2,
                    Comment = "12 portions in foil trays defrosted.",
                    Sign = "Open Check",
                    SignOff = "Supervisor"
                }
            };
            foreach (var item in sc2Items) { context.SC2Items.Add(item); }

            var sc3Items = new ObservableCollection<SC3Item>
            {                
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Beef Stir Fry",
                    CookTemp = 85,
                    Comment = "reheat",
                    SignOff = "Chef"
                },
                new SC3Item {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-3),
                    Dept = "Kitchen",
                    Food = "Pork & Leek Pie",
                    CookTemp = 88,
                    Comment = "Batch of 24 pies reheated",
                    SignOff = "Manager"
                }
            };
            foreach (var item in sc3Items) { context.SC3Items.Add(item); }

            var sc4Items = new ObservableCollection<SC4Item>
            {
                new SC4Item {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-25),
                    Dept = "Front-Of-House",
                    Food = "Fries",
                    HoldTemp2 = 75,
                    Comment = "16 portions received into Hot-Holding",
                    Sign = "Front-Of-House",
                    SignOff = "Staff"
                }                
            };
            foreach (var item in sc4Items) { context.SC4Items.Add(item); }

            var sc9Items = new ObservableCollection<SC9Item>
            {
                new SC9Item {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset).AddMinutes(-5),
                    Dept = "Dispatch",
                    Food = "Chinese Take Away",
                    CustName = "Mr Sam Spade",
                    CustAddr = "Falcon Hotel, Malta.",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "1 Beef Stir Fry, 1 King Prawn Kung Po, 1 Singapore Noodles, 2 Salt & Pepper Squid & 2 Fried Rice",
                    Sign = "Front-Of-House",
                    SignOff = "Point-Of-Sale"
                }
            };
            foreach (var item in sc9Items) { context.SC9Items.Add(item); }

            context.SaveChanges();
        }

        /// <summary>
        ///     DbWebAPI.Controllers.SqlLoadItemsDtoAsync
        ///     Loads a 'Data Transfer Object' subset of all Document data.
        ///     Called by SCxItemsController.GetArchiveDtoAsync 
        /// </summary>
        /// <remarks>
        ///     Uses a SQL sub-select query to merge all the document data 
        ///     (SC1 - SC9) into a 'ArchiveItemsDto' which consists of 
        ///     the following columns...
        ///     
        ///         Id
        ///         TimeStamp
        ///         Type
        ///         Dept. 
        ///         
        ///     The ArchiveItemsDto data that will be sorted into 
        ///     year/month/day/dept/Type and is ultimately passed back to  
        ///     the calling APP to create the archive folders layout.
        /// </remarks>
        /// <param name="_context">Database set</param>
        public static async Task<ObservableCollection<SCxItemDto>> SqlLoadItemsDtoAsync(SCxItemContext _context)
        {
            // Create DTO subset of all document data 
            var SCxItemsDto = new ObservableCollection<SCxItemDto>();

            try 
            {
                string connString = @"Server =.\SQLEXPRESS; Database = SCxDb; Trusted_Connection = True;";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"SELECT Id, TimeStamp, Type, Dept " +
                                    "FROM(" +
                                        "SELECT dbo.SC1.Id, dbo.SC1.TimeStamp, dbo.SC1.Type, dbo.SC1.Dept FROM dbo.SC1 UNION ALL " +
                                        "SELECT dbo.SC2.Id, dbo.SC2.TimeStamp, dbo.SC2.Type, dbo.SC2.Dept FROM dbo.SC2 UNION ALL " +
                                        "SELECT dbo.SC3.Id, dbo.SC3.TimeStamp, dbo.SC3.Type, dbo.SC3.Dept FROM dbo.SC3 UNION ALL " +
                                        "SELECT dbo.SC4.Id, dbo.SC4.TimeStamp, dbo.SC4.Type, dbo.SC4.Dept FROM dbo.SC4 UNION ALL " +
                                        "SELECT dbo.SC9.Id, dbo.SC9.TimeStamp, dbo.SC9.Type, dbo.SC9.Dept FROM dbo.SC9 " +
                                    ")Dto ORDER BY TimeStamp";
                    SqlCommand tSqlCmd = new SqlCommand(query, conn);       // Setup the T-SQL command
                    conn.Open();                                            // Open the connection
                    SqlDataReader sqlReader = await tSqlCmd.ExecuteReaderAsync();      //execute the SQLCommand
                    if (sqlReader.HasRows)
                    {
                        while (await sqlReader.ReadAsync())
                        {
                            SCxItemDto newDto = new SCxItemDto
                            {
                                Id = sqlReader.GetGuid(0),
                                TimeStamp = sqlReader.GetDateTime(1),
                                Type = sqlReader.GetString(2),
                                Dept = sqlReader.GetString(3)
                            };
                            SCxItemsDto.Add(newDto);                    //Create a list of all document types in DTO form

                        }

                    }
                    sqlReader.Close();                                  //close data reader
                    conn.Close();                                       //close connection
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC1): " + ex.Message, ex); }
            MessageHandler.MessageLog("SCxItems Count = " + SCxItemsDto.Count().ToString(), "Trace");
            return SCxItemsDto;
        }
    }
}

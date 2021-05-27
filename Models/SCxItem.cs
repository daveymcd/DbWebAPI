using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Models
{
    /// <summary>
    /// 
    ///     DbWebAPI.Models.SCxItem - SCx Documents Data
    ///     
    ///     SCxItem is a class holding Food Hygiene Documents data. The Documents are 'TimeStamp' ordered 
    ///     and the document type is defined by the 'Type' attribute which ranges from SC1: to SC9:. 
    /// 
    /// </summary>
    /// <remarks>
    /// 
    ///     These are regulatory Documents required by the government Food Standards Agency to be archived and 
    ///     held by catering companies as a record of their compliance with UK food hygiene regulation.
    ///     
    /// </remarks>>
    


    /// <summary>
    /// 
    ///     DbWebAPI.Models.SCxItemDto - SCx Documents DTO Data Model
    ///     
    /// </summary>
    /// <remarks>
    /// 
    ///     Data Transfer Object for SCxItem. Used by the mobile app to
    ///     scaffold the folder and document structure for the archive screen.
    ///     It returns only minimal document data.
    ///     
    /// </remarks>
    public class SCxItemDto  //SCxItem.SCxItemDTO ???
    {
        /// <summary>Guid Unique Id</summary>
        [Key] public Guid Id { get; set; }
        /// <summary>Date of transaction</summary>
        [DataType(DataType.DateTime)] public DateTime TimeStamp { get; set; }
        /// <summary>Document type SC1: - SC9:</summary>
        public string Type { get; set; }
        /// <summary>Catering Department (Kitchen, Prep-area, Stores etc)</summary>
        public string Dept { get; set; }                                            
    }

    /// <summary>
    /// 
    ///     Search class for List of SCxItem.
    ///     
    /// </summary>
    /// <remarks>
    /// 
    ///     These properties are all the possible search criteria for the document archive (SCx).
    ///     The class is used as by a search method to retirve a selection of documents.
    ///     All the properties are optional.
    ///     
    ///     Note: Using seperate fields for Date and Time allow for greater compatablity across browsers
    ///         
    /// </remarks>        
    public class SCxSearchCriteria
    {
        ///<summary>Search start date</summary>
        [DataType(DataType.Date)] public DateTime? FromDate { get; set; }
        ///<summary>Search start time</summary>
        [DataType(DataType.Time)] public DateTime? FromTime { get; set; }
        ///<summary>Search end date</summary>
        [DataType(DataType.Date)] public DateTime? ToDate { get; set; }
        ///<summary>Search end time</summary>
        [DataType(DataType.Time)] public DateTime? ToTime { get; set; }
        ///<summary>Document Type (SC1 - SC9)</summary>
        public string? Type { get; set; }
        ///<summary>Catering Department. Kitchen, Prep-area, Stores etc</summary>
        public string? Dept { get; set; }
        ///<summary>Food Item or Ingredient</summary>
        public string? Food { get; set; }
        ///<summary>Stock Supplier Name</summary>
        public string? Supplier { get; set; }
        ///<summary>Use-By-Date indicator (not-applicable/checked/Expired)</summary>
        public int? CheckUBD { get; set; }
        ///<summary>Supervisor sign-off of completed Document</summary>
        public string? SignOff { get; set; }

        /// <summary>
        /// 
        ///     DbWebAPI.Model.SCxSearchCriteria.SCxSearch() - search method for list of SCx documents
        /// 
        /// </summary>
        /// <remarks>
        ///     
        ///     This is a search method of the SCxSearchCriteria class with access to the bound search 
        ///     criteria entered by the user (this = SCxSearchParams). 
        ///     
        ///     The method uses the search critera entered by the user (SCxSearchParams) to select a 
        ///     subset of document records from the database to pass back to the client (sCxItems).
        /// 
        ///     The following SCxSearchCriteria class values are setup (optionaly) by the client...
        ///     
        ///         this.FromDate   - Search start date.
        ///         this.FromTime   - Search start time.
        ///         this.ToDate     - Search end date.
        ///         this.ToTime     - Search end time.
        ///         this.Type       - SCx document type (SC1 - SC9).
        ///         this.Dept       - Catering department
        ///         this.Supplier   - Stock supplier
        ///         this.CheckUBD   - Use-By-Date status (n/a, OK or Expired)
        ///         this.SignOff    - Name of supervisor who signed-off of the completed document
        /// 
        ///     Note: 
        ///         *   Using seperate fields for Date and Time, allows for greater compatablity across browsers.
        ///         *   If the user does not input a value then the coresponding property is null.
        ///         *   if no date or time is entered, the appropriate default DateTime.MinValue / DateTime.MaxValue is used (ie all documents).  
        ///         *   If time is entered without the accompanying date, the date defaults to today or FromDate, whichever is appropriate. 
        ///         *   If a date is entered without an accompanying time, the default 'search from' time is 00:00.00, the default 'search to' time is 23:59.59.
        ///         *   SCxSearchCriteria DateTime values are nullable so the DateTime members have to be exposed via '.Value' property.
        /// 
        /// </remarks>
        /// <example>
        ///     sCxItems = searchParams.SCxSearch(await _context.SCxItems.ToListAsync());
        /// </example>
        /// <param name="sCxSearchItems">List of documents to search</param>
        public IList<SCxItem> SCxSearch(IList<SCxItem> sCxSearchItems)
        {
            // *** Try and make sense of the Date/Time search criteria entered by the user...

            var FromDateTime = DateTime.MinValue;                           // If user has entered no date/time search criteria, the  
            var ToDateTime = DateTime.MaxValue;                             // default search will be from earliest entry to latest.

            // *** 'Search From' Date/Time...
            if (this.FromDate is not null)
            {
                FromDateTime = this.FromDate.Value.Date;                    // If FromDate was entered, use it.
            }
            else if (this.FromTime is not null)                             // otherwise, if FromDate is null but FromTime was entered,
            {
                FromDateTime = DateTime.Today;                              // assume search is from todays date.
            }

            if (this.FromTime is not null)                                  // If FromTime was entered, add it to the 'search from' date.
            {
                FromDateTime = FromDateTime.Add(this.FromTime.Value.TimeOfDay);
            }

            // *** 'Search To' Date/Time...
            if (this.ToDate is not null)
            {
                ToDateTime = this.ToDate.Value.Date;                        // If ToDate was entered, use it.
            }
            else if (this.FromDate is not null ||                           // otherwise, if ToDate is null but FromDate or FromTime was entered, 
                     this.FromTime is not null)
            {
                ToDateTime = FromDateTime.Date;                             // assume 'search To' date is same day as 'search from' date (zero time).
            }
            else if (this.ToTime is not null)                               // otherwise, if 'search from' date and time and 'search to' date are 
            {                                                               // all null but ToTime was entered,
                ToDateTime = DateTime.Today;                                // assume 'search to' is for today, up to the time entered (ToTime added below).
                FromDateTime = DateTime.Today;                              // assume 'search from' is from the begining of today (zero time).
            }

            if (this.ToTime is not null)                                    // If ToTime was entered, add it to the 'search to' date.
            {
                ToDateTime = ToDateTime.Add(this.ToTime.Value.TimeOfDay);
            }
            else if (ToDateTime.TimeOfDay.TotalSeconds == 0)                // for the ToDateTime time element to be zero, ToDateTime must 
            {                                                               // have been initialised with a date so...
                ToDateTime = ToDateTime.AddDays(1);                         // If no ToTime was entered, set 'search to' time 
                ToDateTime = ToDateTime.AddTicks(-1);                       // to midnight.
            }

            if (FromDateTime > ToDateTime)                                  // Client dates are messed up
            {
                Exception ex = new ArgumentOutOfRangeException("Date Range", "Search From Date entered is Later than Search To Date. Please correct the search criteria.");
                //TempData["error"] = ex.Message;
                throw ex;
            }
            else
            {   // *** use the search criteria to select a subset of the SCx documents from the Db
                
                var sCxItemSelection = sCxSearchItems.Where((SCxItem arg) =>
                                        (arg.TimeStamp >= FromDateTime && arg.TimeStamp <= ToDateTime) &&
                                        (this.Type == null      || arg.Type == this.Type) &&
                                        (this.Dept == null      || arg.Dept == this.Dept) &&
                                        (this.Food == null      || arg.Food == this.Food) &&
                                        (this.Supplier == null  || arg.Supplier == this.Supplier) &&
                                        (this.CheckUBD == null  || arg.CheckUBD == this.CheckUBD) &&
                                        (this.SignOff == null   || arg.SignOff == this.SignOff))
                                     .OrderByDescending(item => item.TimeStamp)
                                     .ToList();
                return sCxItemSelection;
            }
        }
    }

    /// <summary>
    /// 
    ///     DbWebAPI.Models.SCxItem - SCx Documents Data Model
    ///     Food Standards Agency 'Safe Catering' Regulatory Food Monitoring Records (SC1-SC9).
    ///     
    /// </summary>
    /// <remarks>
    ///
    ///     Id          - Guid unique Key. 
    ///     TimeStamp   - Documents transaction date/time stamp used to order the document archive.
    ///     Type        - Safe Catering Document Type...
    ///
    ///         SC1: Deliveries In      – Food Delivery Record. To record the monitoring of incoming deliveries (high risk, ready-to-eat food only).
    ///         SC2: Chiller Checks     – Fridge/Cold room/Display Chiller Temperature records. To record the monitoring of the chill units, 
    ///                                   refrigerator's, cold units (and the function of freezer's).
    ///         SC3: Cooking Log        – Cooking/Cooling/Reheating Records. To record the monitoring of cooking, cooling and reheating temperatures.
    ///         SC4: Hot-Holding        – Hot Hold/Display Records. To record hot holding temperatures of food.
    ///         SC5: Hygiene Inspection – Hygiene Inspection Checklist. To record managers/supervisors checks of premises.
    ///         SC6: Hygiene Training   – Hygiene Training Records. To record training of staff.
    ///         SC7: Fitness To Work    – Fitness to Work Assessment Form. To record assessment of staff fitness to work.
    ///         SC8: All-In-One Form    – All-in-one Record. An alternative to SC1-4 (not used).
    ///         SC9: Deliveries Out     – Customer Delivery Record. To record monitoring of food deliveries out to customers.
    ///         COP: Opening Checks     - Daily opening checks by supervisor.
    ///         CCL: Closing Checks     - Daily closing checks by supervisor.
    /// 
    ///     Dept        - Catering department (Kitchen, Prep-area, Stores etc).
    ///     Food        - Food type or ingredient.
    ///     Supplier    - Stock Supplier Name.
    ///     CheckUBD    - Use-By-Date indicator (not-applicable/checked/out-of-date)
    ///     Temperature - Food Temperature in Celsius
    ///     Comment     - General comment box
    ///     SignOff     - Manger sign-off of completed Document
    ///     
    ///     
    /// </remarks>
    ///
    public class SCxItem
    {
        /// <summary>Guid Unique Key</summary>
        [Key] public Guid Id { get; set; }

        /// <summary>Date/Time Document Created</summary>
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)] 
        public DateTime TimeStamp { get; set; }

        /// <summary>SCx Document Type (SC1: - SC9:)</summary>
        [DisplayName("Document")]
        //[Required(ErrorMessage = "Document Type Is Required.")]
        public string Type { get; set; }

        /// <summary>Catering department (Kitchen, Prep-area, Stores etc)</summary>
        public string Dept { get; set; }

        ///// <summary>Batch Number</summary>
        //public string Batch { get; set; }

        /// <summary>Food type</summary>
        public string Food { get; set; }

        /// <summary>SC1: Stock Supplier Name</summary>
        public string Supplier { get; set; }

        /// <summary>SC1: Use-By-Date indicator (not-applicable/checked-OK/expired)</summary>
        [DisplayName("Use By Date")]
        public int CheckUBD {get; set; }

        /// <summary>Food Temperature in Celsius</summary>  
        [DisplayName("\u00B0C")]
        public double Temperature { get; set; }

        /// <summary>General comment box</summary>
        public string Comment { get; set; }

        /// <summary>Manger sign-off of completed Document</summary>
        public string SignOff { get; set; }


        /////     Note: *** Future enhancements COMMENTED OUT...

        ///// <summary>Document Logically Deleted</summary>
        //public bool SCxDeleted { get; set; }

        ///// <summary>Date/Time Document Last Amended</summary>
        //[DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)] 
        //public DateTime TimeStampLastUpdate { get; set; } 

        ///// <summary>Last User to Amended Document</summary>
        //public string NameLastUpdate { get; set; } 

        ///// <summary>Document SubType - n/a, Cook, cool or Reheat (SC3:Cooking Log)</summary>
        //[DisplayName("Action")]
        //public double SC3SubType { get; set; }

        ///// <summary>SC9: Customer Name</summary>
        //public string Customer { get; set; }

        ///// <summary>SC9: Customer Details</summary>
        //public string CustomerAddress { get; set; } // Customer table ???

        ///// <summary>SC9: Food Item Quantity</summary>
        //[DisplayName("Quantity")]
        //public int Quantity { get; set; }


        ///// <summary>SC4: Food Temperature (+4 Hrs) in Celsius</summary>  
        //[DisplayName("\u00B0C (+4 Hours)")]
        //public double TempPlus4 { get; set; }

        ///// <summary>SC4: Food Temperature (+6 Hrs) in Celsius</summary>  
        //[DisplayName("+6 Hrs\u00B0C "]
        //public double TempPlus6 { get; set; }

        ///// <summary>Item Accepted/Rejected tick box</summary>
        //public bool Accepted { get; set; } // can be infered!

        ///// <summary>Staff Name Completion Details</summary>
        //public string StaffName { get; set; }

        ///// <summary>SC3: Date/Time task completed</summary>
        //[DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)] 
        //public DateTime TimeStampCompleted { get; set; } 

        ///// <summary>SC9: Seperation of Raw or Ready-To-Eat foods boolean</summary>
        //public bool CheckRawRTE { get; set; }

        /// <summary>
        ///     DbWebAPI.Models.SCxItem.Initialise();
        ///     On Creation of an SCxItem, any uninitialised data will be given default value.
        /// </summary>
        /// <param name="item">Document to initialise</param>
        public void Initialise(SCxItem item = null)
        {
            if (item.Id == Guid.Empty) this.Id = Guid.NewGuid();
            if (item.TimeStamp == DateTime.MinValue) this.TimeStamp = DateTime.Now;
            if (item.Type is null) this.Type = string.Empty;
            if (item.Dept is null) this.Dept = string.Empty;
            if (item.Food is null) this.Food = string.Empty;
            if (item.Supplier is null) this.Supplier = string.Empty;
            if (item.CheckUBD < 0 || item.CheckUBD > 2) this.CheckUBD = 0;
            if (item.Comment is null) this.Comment = string.Empty;
            if (item.SignOff is null) this.SignOff = string.Empty;
        }

        /// <summary>Add test archive data to database </summary>
        public static ObservableCollection<SCxItem> AddThisMonthsSCxData(double? monthOffset = 0)
        {

            monthOffset = 31 * -monthOffset;
            var scxItems = new ObservableCollection<SCxItem>
            {
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-31),
                    Dept = "Pantry",
                    Food = "Lamb Shank in Onion Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 2,
                    Temperature = 94,
                    Comment = string.Empty,
                    SignOff = string.Empty
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-30),
                    Dept = "Front-Of-House",
                    Food = "Chicken & Leek Pie",
                    Supplier = "Porkies Pies",
                    CheckUBD = 1,
                    Temperature = 70,
                    Comment = "Charlie - Crust burnt on corner. 1 portion rejected",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-29),
                    Dept = "Kitchen",
                    Food = "Lamb Leg Steaks in Red Wine Sauce",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 1,
                    Temperature = 82,
                    Comment = "Customer requested bloody rare steak",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-28),
                    Dept = "Kitchen",
                    Food = "Beef",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 0,
                    Temperature = 65,
                    Comment = "2 medium rare, 1 rare",
                    SignOff = "Supervisor"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-27),
                    Dept = "Pantry",
                    Food = "Pork Loin in Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 1,
                    Temperature = 32,
                    Comment = "Packaging torn open in transit. 12 servings ruined",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-26),
                    Dept = "Prep-Area",
                    Food = "Lamb in onion Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 2,
                    Temperature = 80,
                    Comment = "12 servings. UBD expired",
                    SignOff = string.Empty
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-25),
                    Dept = "Pantry",
                    Food = "Sole",
                    Supplier = "Fish Monger",
                    CheckUBD = 2,
                    Temperature = 10,
                    Comment = "Fish held up in transit and arrived after UBD expired",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-24),
                    Dept = "Pantry",
                    Food = "Lamb Cuttlets in Gravy",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 2,
                    Temperature = 90,
                    Comment = "Expired",
                    SignOff = string.Empty
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-23),
                    Dept = "Pantry",
                    Food = "Fish in Batter",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = -5,
                    Comment = "Delivery of 12 Battered Cod portions under weight" ,
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-22),
                    Dept = "Prep-Area",
                    Food = "Chicken",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 0,
                    Temperature = 92,
                    Comment = "16 ready cooked Chickens whole. Had to be prep'd before serving",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-21),
                    Dept = "Prep-Area",
                    Food = "Fish",
                    Supplier = "Fish Monger",
                    CheckUBD = 0,
                    Temperature = 65,
                    Comment = "12 Cod fillets, 6 whole Sole, 12 salmon steaks. Individual Boil-in-the-bag portions ",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-20),
                    Dept = "Prep-Area",
                    Food = "Venison",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 0,
                    Temperature = 2,
                    Comment = "2 Haunches defrosted",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-19),
                    Dept = "Prep-Area",
                    Food = "Cod",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = 1,
                    Comment = "12 fillets arrived partly defrosted. Use today",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-18),
                    Dept = "Prep-Area",
                    Food = "Scampi",
                    Supplier = "Fish Monger",
                    CheckUBD = 2,
                    Temperature = -6,
                    Comment = "Frozen product passed its UBD",
                    SignOff = "Supervisor"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-17),
                    Dept = "Pantry",
                    Food = "Fish",
                    Supplier = "Fish Monger",
                    CheckUBD = 0,
                    Temperature = 2,
                    Comment = "12 Cod steaks, 6 salmon steaks. Chilled",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-16),
                    Dept = "Pantry",
                    Food = "Kebab",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 66,
                    Comment = "24 ready cooked lamb, onion & pepper skewers",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-15),
                    Dept = "Kitchen",
                    Food = "Haddock in white wine sauce",
                    Supplier = "Fish Monger",
                    CheckUBD = 0,
                    Temperature = 80,
                    Comment = "12 portions prepared",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-14),
                    Dept = "Front-of-House",
                    Food = "Pork & Leek Pie",
                    Supplier = "Porkies Pies",
                    CheckUBD = 1,
                    Temperature = 81,
                    Comment = "6 pies received into hot-holding",
                    SignOff = string.Empty
                },
                new SCxItem{
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-13),
                    Dept = "Front-of-House",
                    Food = "Beef Wellington",
                    Supplier = "Bob's Bakers",
                    CheckUBD = 1,
                    Temperature = 71,
                    Comment = "2 trays of 12 portions total",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-12),
                    Dept = "Kitchen",
                    Food = "Singapore Noodles",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 1,
                    Temperature = 1,
                    Comment = "12 portions defrosted",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-11),
                    Dept = "Kitchen",
                    Food = "Lentil Soup",
                    Supplier = "Corner Greengrocer",
                    CheckUBD = 0,
                    Temperature = 75,
                    Comment = "Pan of 9 portions from hot-holding reheated",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-10),
                    Dept = "Pantry",
                    Food = "Chicken Nuggets",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 81,
                    Comment = "20 child portions in individual foil trays.",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-9),
                    Dept = "Kitchen",
                    Food = "Penne Carbonara",
                    Supplier = "Little Italy",
                    CheckUBD = 1,
                    Temperature = 80,
                    Comment = "12 Boil-in-the-bag portions ready to serve",
                    SignOff = "Supervisor"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)monthOffset-8),
                    Dept = "Kitchen",
                    Food = "Cod Fishcakes",
                    Supplier = "Fish Monger",
                    CheckUBD = 0,
                    Temperature = 79,
                    Comment = "12 cakes defrosted (2 per portion)",
                    SignOff = "Manager"
                }
            };
            foreach (SCxItem sCxItem in AddThisWeeksSCxData(monthOffset)) { scxItems.Add(sCxItem); }
            return scxItems;
        }

        ///<summary> Add 'This weeks' test archive data to database </summary>
        public static ObservableCollection<SCxItem> AddThisWeeksSCxData(double? weekOffset = 0)
        {
            var scxItems = new ObservableCollection<SCxItem>
            {
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-7).AddMinutes(-45),
                    Dept = "Dispatch",
                    Food = "Pizza",
                    Supplier = "Mr Hercule Poirot, Coach 3, Orient Express",
                    CheckUBD = 0,
                    Temperature = 60,
                    Comment = "2 pepperoni Passion, 1 Sloppy Joe & 1 Veggie Volcano",
                    SignOff = "Point-Of-Sale"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-6).AddMinutes(-45),
                    Dept = "Dispatch",
                    Food = "Curry Take Away",
                    Supplier = "Miss Marple, Milchester cottage, St. Mary Mead",
                    CheckUBD = 0,
                    Temperature = 69,
                    Comment = "1 Chicken Vindaloo, 1 Fried Rice & 1 Garlic Naan",
                    SignOff = "Point-Of-Sale"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-5).AddMinutes(-45),
                    Dept = "Kitchen",
                    Food = "Fries",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 85,
                    Comment = "16 portions prepared",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-5).AddMinutes(-30),
                    Dept = "Kitchen",
                    Food = "Hamburger",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 87,
                    Comment = "16 patties prepared",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-5).AddMinutes(-25),
                    Dept = "Front-Of-House",
                    Food = "Fries",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 75,
                    Comment = "16 portions received into Hot-Holding",
                    SignOff = "Staff"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-5).AddMinutes(-20),
                    Dept = "Front-Of-House",
                    Food = "Hamburger",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 80,
                    Comment = "16 patties recieved into Hot-Holding",
                    SignOff = "Staff"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-5).AddMinutes(-110),
                    Dept = "Kitchen",
                    Food = "Hamburger",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 2,
                    Comment = "24 Frozen patties defrosted",
                    SignOff = string.Empty
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-5).AddMinutes(-10),
                    Dept = "Dispatch",
                    Food = "Burger & Chips",
                    Supplier = "Dr John H Watson, 221b Baker St.",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "2 1/4 pounders with cheese, 2 Fries. ",
                    SignOff = "Point-Of-Sale"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-4).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Moules Marinier",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = 72,
                    Comment = "8 Boil-in-the-bag starter portions",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-4).AddMinutes(-5),
                    Dept = "Kitchen",
                    Food = "Apple & Blackberry Pie",
                    Supplier = "Sams Baker's",
                    CheckUBD = 0,
                    Temperature = 43,
                    Comment = "1kg deep fill pie (8 portions)",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-3).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Beef Sirloin",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 1,
                    Temperature = 79,
                    Comment = "6 16oz size steaks Medium Rare",
                    SignOff = string.Empty
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-3).AddMinutes(-20),
                    Dept = "Front-Of-House",
                    Food = "Beef Sirloin",
                    Supplier = "Jones the Butcher",
                    CheckUBD = 1,
                    Temperature = 73,
                    Comment = "6 16oz size steaks received into Hot-Holding",
                    SignOff = string.Empty
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-2),
                    Dept = "Pantry",
                    Food = "Salt & Pepper Squid",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 0,
                    Temperature = 65,
                    Comment = "12 portions in foil trays defrosted.",
                    SignOff = "Supervisor"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-2),
                    Dept = "Kitchen",
                    Food = "Salt & Pepper Squid",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 0,
                    Temperature = 88,
                    Comment = "12 portions in foil trays reheated.",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-2),
                    Dept = "Front-Of-House",
                    Food = "Salt & Pepper Squid",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 0,
                    Temperature = 75,
                    Comment = "12 portions in foil trays received into hot-holding.",
                    SignOff = "Staff"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1),
                    Dept = "Prep-Area",
                    Food = "Fillet of Sole",
                    Supplier = "Fish Monger",
                    CheckUBD = 1,
                    Temperature = 60,
                    Comment = "8 fillets of good size",
                    SignOff = "Supervisor"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1).AddMinutes(-50),
                    Dept = "Kitchen",
                    Food = "Southern Fried Chicken",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 1,
                    Comment = "20 Frozen portions defrosted",
                    SignOff = string.Empty
                },
                 new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1).AddMinutes(-45),
                    Dept = "Kitchen",
                    Food = "Fries",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 81,
                    Comment = "14 portions prepared",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1).AddMinutes(-40),
                    Dept = "Kitchen",
                    Food = "Southern Fried Chicken",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 84,
                    Comment = "12 portions prepared",
                    SignOff = "Charlie"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1).AddMinutes(-30),
                    Dept = "Front-Of-House",
                    Food = "Fries",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 79,
                    Comment = "14 portions received into Hot-Holding",
                    SignOff = "Staff"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC4:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1).AddMinutes(-20),
                    Dept = "Front-Of-House",
                    Food = "Southern Fried Chicken",
                    Supplier = "Fast Food Supplies",
                    CheckUBD = 1,
                    Temperature = 81,
                    Comment = "12 portions recieved into Hot-Holding",
                    SignOff = "Staff"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset-1).AddMinutes(-10),
                    Dept = "Dispatch",
                    Food = "Chicken & Chips",
                    Supplier = "Mr Philip Marlow, Hobart Arms, Franklin Ave, Los Angeles County",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "Family Chicken Bucket and 2 Fries. ",
                    SignOff = "Point-Of-Sale"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset).AddMinutes(-20),
                    Dept = "Prep-Area",
                    Food = "Beef Stir Fry",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 1,
                    Temperature = 65,
                    Comment = "16 Boil in the bag portions",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset).AddMinutes(-10),
                    Dept = "Kitchen",
                    Food = "Beef Stir Fry",
                    Supplier = "Singapore Supplies ltd",
                    CheckUBD = 1,
                    Temperature = 85,
                    Comment = "reheat",
                    SignOff = "Chef"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC9:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset).AddMinutes(-5),
                    Dept = "Dispatch",
                    Food = "Chinese Take Away",
                    Supplier = "Mr Sam Spade, Falcon Hotel, Malta.",
                    CheckUBD = 0,
                    Temperature = 77,
                    Comment = "1 Beef Stir Fry, 1 King Prawn Kung Po, 1 Singapore Noodles, 2 Salt & Pepper Squid & 2 Fried Rice",
                    SignOff = "Point-Of-Sale"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC3:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset).AddMinutes(-3),
                    Dept = "Kitchen",
                    Food = "Pork & Leek Pie",
                    Supplier = "Porkies Pies",
                    CheckUBD = 1,
                    Temperature = 88,
                    Comment = "Batch of 24 pies reheated",
                    SignOff = "Manager"
                },
                new SCxItem {
                    Id = Guid.NewGuid(),
                    Type = "SC1:",
                    TimeStamp = DateTime.Now.AddDays((double)weekOffset).AddMinutes(-1),
                    Dept = "Prep-Area",
                    Food = "Lamb Passanda",
                    Supplier = "Curry House ltd",
                    CheckUBD = 2,
                    Temperature = 50,
                    Comment = "Product UBD exceeded",
                    SignOff = string.Empty
                }
            };
            return scxItems;
        }
    }
}

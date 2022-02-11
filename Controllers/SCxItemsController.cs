using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbWebAPI.Models;
using System.Net.Mime;
using System.Net;
using Microsoft.AspNetCore.Http;
using DbWebAPI.Helpers;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DbWebAPI.Controllers
{
    /// <summary> 
    /// 
    ///     DbWebApi.Controllers.SCxItemsController - Database Web Open API Controller
    ///     
    ///     This controller exposes the web API CRUD endpoints for mobile app to access the SCx 
    ///     Documents database.
    ///     
    /// </summary>
    /// <remarks>
    /// 
    ///     The Database holds various food industry regulatory documents, required by the UK  
    ///     governments 'Food Standards Agency', to be archived and held by catering companies 
    ///     as a record of their compliance with UK food hygiene regulation.
    ///     
    ///     * SCxItem.cs is the document archive Class, holding the Food Hygiene Document data. 
    ///       Each Documents is TimeStamped and Typed. 
    ///       
    ///     * SCxItemDto is the document archive DTO Class, holding a document subset of...
    ///         Id 
    ///         TimeStamp 
    ///         Type 
    ///         Dept
    ///       
    /// </remarks>

    //[Route("api/[controller]")]                   // Conventional routing
    [Route("Home/[controller]/[action]")]           // Attribute routing
    [ApiController]
#if ProducesConsumes  
    [Produces(MediaTypeNames.Application.Json)]     // Only Json will be used over HTTP
    [Consumes(MediaTypeNames.Application.Json)]
#endif
    [NSwag.Annotations.OpenApiController("DbWebApi")]

    public class SCxItemsController : ControllerBase
    {
        private readonly SCxItemContext _context;
        /// <summary>Document Archive List returned to caller</summary>
        public ObservableCollection<SCxItemDto> ArchiveItemsDto { get; set; }
        /// <summary>DTO subset of all document data</summary>
        public ObservableCollection<SCxItemDto> SCxItemsDto { get; set; }
        public IEnumerable<SC1Item> sc1OC { get; set; }
        /// <summary>
        ///     DbWebAPI.Controllers.SCxItemContext
        ///     This method gets called by the runtime and configures the Database context.
        /// </summary>
        /// <param name="context">Database set</param>
        public SCxItemsController(SCxItemContext context)
        {
            _context = context;
        }

        //*** API PROCESSING... ***//

        /// <summary>
        ///     DbWebAPI.Controllers.GetSCxItemsAsync
        ///     GET: SCxItems/GetSCxItemsAsync 
        ///     HTTP get all documents request endpoint. 
        ///     This method responds with all SCx Document data.
        /// </summary>
        [NSwag.Annotations.OpenApiOperation("GetSCxItemsAsync", "Get All Documents for specified document type", "Request Responds with all the Document data for the specified document 'Type'")]
        [HttpGet(Name = nameof(GetSCxItemsAsync))]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<object>>> GetSCxItemsAsync([FromQuery] SCxItemDto? scxItemDto = null)
        {
            MessageHandler.MessageLog("GetSCxItemsAsync");
            scxItemDto.Initialise(scxItemDto);
            if (scxItemDto.Type == string.Empty) { return BadRequest(); }
            else
            {
                switch (scxItemDto.Type)
                {
                    case "SC1":
                        {
                            if (_context.SC1Items.Count() == 0) { return NotFound(); }
                            else { return _context.SC1Items.ToList(); }     // replace with SQL - where!
                        }
                    case "SC2":
                        {
                            if (_context.SC1Items.Count() == 0) { return NotFound(); }
                            else { return _context.SC2Items.ToList(); }
                        }
                    case "SC3":
                        {
                            if (_context.SC1Items.Count() == 0) { return NotFound(); }
                            else { return _context.SC3Items.ToList(); }
                        }
                    case "SC4":
                        {
                            if (_context.SC1Items.Count() == 0) { return NotFound(); }
                            else { return _context.SC4Items.ToList(); }
                        }
                    case "SC9":
                        {
                            if (_context.SC1Items.Count() == 0) { return NotFound(); }
                            else { return _context.SC9Items.ToList(); }
                        }
                    default: { return BadRequest(); }
                }
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.GetSCxItemAsync (override)
        ///     GET: SCxItems/GetSCxItemAsync 
        ///     HTTP get by id request endpoint. 
        ///     This method responds with single SCx Document data.
        /// </summary>
        /// <param name="scxItemDto">Document Dto</param>
        [NSwag.Annotations.OpenApiOperation("GetSCxItemAsync", "Get Single SCx Document By Guid", "Request Responds with single SCx Document")]
        [HttpGet(Name = nameof(GetSCxItemAsync))]
        [ActionName(nameof(GetSCxItemAsync))]                              // Needed to get around route error on create in Swagger - "InvalidOperationException: No route matches the supplied values".
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetSCxItemAsync([FromQuery] SCxItemDto? scxItemDto = null)
        {
            MessageHandler.MessageLog("GetSCxItemAsync[" + scxItemDto.Id.ToString() + "]");

            scxItemDto.Initialise(scxItemDto);
            if (scxItemDto.Id == Guid.Empty || scxItemDto.Type == string.Empty) { return BadRequest(); }
            else
            {
                switch (scxItemDto.Type)
                {
                    case "SC1":
                        {
                            if (!_context.SC1Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else { return (await _context.SC1Items.FindAsync(scxItemDto.Id)); }
                        }
                    case "SC2":
                        {
                            if (!_context.SC2Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else { return (await _context.SC2Items.FindAsync(scxItemDto.Id)); }
                        }
                    case "SC3":
                        {
                            if (!_context.SC3Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else { return (await _context.SC3Items.FindAsync(scxItemDto.Id)); }
                        }
                    case "SC4":
                        {
                            if (!_context.SC4Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else { return (await _context.SC4Items.FindAsync(scxItemDto.Id)); }
                        }
                    case "SC9":
                        {
                            if (!_context.SC9Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else { return (await _context.SC9Items.FindAsync(scxItemDto.Id)); }
                        }
                    default: { return BadRequest(); }
                }
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.GetSCxItemsSelectAsync
        ///     GET: SCxItems/GetSCxItemsSelectAsync
        ///     HTTP get by selection request endpoint. 
        ///     This method responds with selected SCx Documents.
        /// </summary>
        ///// <param name="SearchFromTimeStamp">Search start Date/Time</param>
        ///// <param name="SearchToTimeStamp">Search end Date/Time</param>
        ///// <param name="Type">Document Type</param>
        ///// <param name="Dept">Catering Department</param>
        ///// <param name="Supplier">Supplier Name</param>
        ///// <param name="CheckUBD">Use-By-Date status</param>
        ///// <param name="SignOff">Supervisor sign off</param>
        ///<param name="scxItemDto">Document selection criteria</param>
        [NSwag.Annotations.OpenApiOperation("GetSCxItemSelectAsync", "Select SCx Documents", "The Request Responds with a subset of SCx Document data, filtered according to search criteria. \nSelection is by Date/Time Range, Document Type, Department, Supplier, Use-By-Date check, Sign-off signitary or any combination. \nIf a Date & Time pair are supplied the search begins or ends at that point, however, if the Time element is not supplied then the whole days data for the Date is searched.")]
        [HttpGet(Name = nameof(GetSCxItemsSelectAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IList<SCxItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        public async Task<object> GetSCxItemsSelectAsync([FromQuery] SCxItemDto? scxItemDto = null)
                                                                  //DateTime? SearchFromTimeStamp=null,
                                                                  //DateTime? SearchToTimeStamp = null,
                                                                  //string? Type = null,
                                                                  //string? Dept = null,
                                                                  //string? Supplier = null,
                                                                  //int? CheckUBD = null,
                                                                  //string? SignOff = null)
        {
            MessageHandler.MessageLog("GetSCxItemSelectAsync");
            //if (Type is null || Type == string.Empty) { return BadRequest(); }
            //SCxSearchCriteria SCxSearchParams = new();

            //if (SearchFromTimeStamp is not null)
            //{
            //    SCxSearchParams.FromDate = SearchFromTimeStamp.Value.Date;
            //    SCxSearchParams.FromTime = SearchFromTimeStamp;
            //}
            //if (SearchToTimeStamp is not null)
            //{
            //    SCxSearchParams.ToDate = SearchToTimeStamp.Value.Date;
            //    SCxSearchParams.ToTime = SearchToTimeStamp;
            //}
            //if (Type is not null) { SCxSearchParams.Type = Type; }
            //if (Dept is not null) { SCxSearchParams.Dept = Dept; }
            //if (Supplier is not null) { SCxSearchParams.Supplier = Supplier; }
            //if (CheckUBD is not null) { SCxSearchParams.CheckUBD = CheckUBD; }
            //if (SignOff is not null) { SCxSearchParams.SignOff = SignOff; }

            var scx = await SqlLoadSCxItemsAsync(_context, scxItemDto);
            List<SC1Item> a = new();
            foreach (var row in scx)
            {
                SC1Item scxitem = row as SC1Item;
                string c = string.Empty;
                a.Add(scxitem);
                foreach (var column in row)
                {
                    c += (JsonConvert.SerializeObject(column.ToString()));
                }
                var b = JsonConvert.DeserializeObject<SC1Item>(c.ToString());
            }
            return scx.ToList(); //a as List<object>;

            // extract subset of SCx documents, filtered by search params
            //switch (SCxSearchParams.Type)
            //{
            //    case "SC1":
            //        {
            //            var scxItemsDto = from item in _context.SC1Items
            //                              .Where((SCxItemDto arg) => (arg.TimeStamp >= SCxSearchParams.FromDate &&
            //                                                          arg.TimeStamp <= SCxSearchParams.ToDate) &&
            //                                                          (Type == null || arg.Type == SCxSearchParams.Type) &&
            //                                                          (Dept == null || arg.Dept == SCxSearchParams.Dept))
            //                              select new SCxItemDto()
            //                              {
            //                                  Id = item.Id,
            //                                  TimeStamp = item.TimeStamp,
            //                                  Type = item.Type,
            //                                  Dept = item.Dept
            //                              };
            //            break;
            //        }
            //    case "SC2": { break; }
            //    default: { return BadRequest(); }
            //}
            //return (List<SCxItem>) SCxSearchParams.SCxSearch(await _context.SCxItems.ToListAsync());
        }

        /// <summary>
        ///     DbWebAPI.Controllers.GetSCxItemsSelectDtoAsync
        ///     GET: SCxItems/GetSCxItemsSelectDtoAsync
        ///     HTTP get subset of document data by selection request endpoint. 
        ///     This method responds with a Data Transfer Object subset of selected SCx Document data.
        /// </summary>
        /// <param name="SearchFromTimeStamp">Search Start Date/Time</param>
        /// <param name="SearchToTimeStamp">Search End Date/Time</param>
        /// <param name="Type">Document Type</param>
        /// <param name="Dept">Department</param>
        [NSwag.Annotations.OpenApiOperation("GetSCxItemSelectDtoAsync", "Select DTO Subset Of Data From SCx Documents", "'Data Transfer Object' version of GetSCxItemSelect(). \nThe Request Responds with a DTO subset of SCx Document data (Id, TimeStamp, Document Type & Dept.) filtered according to search criteria. \nSelection is by Date/Time Range, Document Type, Department or any combination. \nIf a Date & Time pair is supplied the search begins or ends at that point, however, if the Time element is not supplied then the whole days data for the Date is searched.")]
        [HttpGet(Name = nameof(GetSCxItemsSelectDtoAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IList<SCxItemDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<SCxItemDto>> GetSCxItemsSelectDtoAsync(DateTime? SearchFromTimeStamp = null,
                                                                             DateTime? SearchToTimeStamp = null,
                                                                             string? Type = null,
                                                                             string? Dept = null)
        {
            MessageHandler.MessageLog("GetSCxItemSelectDtoAsync");
            var FromDate = DateTime.MinValue;
            var ToDate = DateTime.MaxValue;

            if (SearchFromTimeStamp is not null)
            {
                FromDate = (DateTime)SearchFromTimeStamp;   // If SearchFromTimeStamp is passed in, use it as 'search from' date & time.
            }

            if (SearchToTimeStamp is not null)
            {
                ToDate = (DateTime)SearchToTimeStamp;       // If SearchToTimeStamp is passed in,use it as 'search to' date & time.
            }
            else
            {
                if (SearchFromTimeStamp is not null)
                    ToDate = (DateTime)SearchFromTimeStamp; // If SearchToTimeStamp was not passed in, use 'search from' date & time
            }

            if (ToDate.TimeOfDay.TotalSeconds == 0)
            {
                ToDate = ToDate.AddDays(1);                 // If no ToDate time element is specified, set 'search to' time 
                ToDate = ToDate.AddTicks(-1);               // to midnight. Otherwise use the time specified by ToDate
            }

            SCxItemsDto = await Data.DbInitializer.SqlLoadItemsDtoAsync(_context);        // load DTO of all document data (SC1 - SC9)

            var scxItemsDto = from item in SCxItemsDto       // extract subset of DTO filtered by search params
                             .Where((SCxItemDto arg) => (arg.TimeStamp >= FromDate          &&
                                                         arg.TimeStamp <= ToDate)           &&
                                                         (Type == null || arg.Type == Type) &&
                                                         (Dept == null || arg.Dept == Dept))
                             select new SCxItemDto()
                             {
                                 Id = item.Id,
                                 TimeStamp = item.TimeStamp,
                                 Type = item.Type,
                                 Dept = item.Dept
                             };

            return scxItemsDto; // Ok(sCxItemDto);

            // Needs further work to use SCxSearchParams! - Task<ActionResult<IList<SCxItemDTO>>>???
            //SCxSearchCriteria SCxSearchParams = new();
            //if (SearchFromTimeStamp is not null)
            //{
            //    SCxSearchParams.FromDate = SearchFromTimeStamp.Value.Date;
            //    SCxSearchParams.FromTime = SearchFromTimeStamp;
            //}
            //if (SearchToTimeStamp is not null)
            //{
            //    SCxSearchParams.ToDate = SearchToTimeStamp.Value.Date;
            //    SCxSearchParams.ToTime = SearchToTimeStamp;
            //}
            //if (Type is not null) { SCxSearchParams.Type = Type; }
            //if (Dept is not null) { SCxSearchParams.Dept = Dept; }

            //var sCxItemsFull = SCxSearchParams.SCxSearch(await _context.SCxItems.ToListAsync());

            //var sCxItemDto = (  from sCxItems in sCxItemsFull
            //                    select new SCxItemDto() {
            //                        Id = sCxItems.Id,
            //                        TimeStamp = sCxItems.TimeStamp,
            //                        Type = sCxItems.Type,
            //                        Dept = sCxItems.Dept
            //                    });
            //                 //.Include(dto => dto.Id)
            //                 //.Include(dto => dto.TimeStamp)
            //                 //.Include(dto => dto.Type)
            //                 //.Include(dto => dto.Dept)

            //return (List<SCxItemDto>)sCxItemDto; // Ok(sCxItemDto);
        }


        /// <summary>
        ///     DbWebAPI.Controllers.GetArchiveDtoAsync
        ///     GET: SCxItems/GetArchiveDtoAsync
        ///     HTTP get Archive 'Data Transfer Object' request endpoint. 
        ///     This method responds with a selected subset of document archival data.
        ///     
        ///     The DTO is used to transfer the archive date based structure (year/month/day folders), 
        ///     without any document detail (used by calling program to create a folder based display of archive).
        /// </summary>
        /// <remarks>
        ///     This Endpoint retrieves the minimum amount of document data to allow the App to display the document folder structure.
        ///     This results in the lightest payload possible over HTTP:
        ///     
        ///         Period = Years  - results in one archive DTO for each year of archival material
        ///         Period = Months - results in one archive DTO for each month of archival material for the year specified by sCxItem.TimeStamp
        ///         Period = Days   - results in one archive DTO for each day of archival material for the year and month specified by sCxItem.TimeStamp
        ///         Period = Day    - results in one archive DTO for each department's archival material for the year, month and day specified by sCxItem.TimeStamp
        ///         Period = All    - results in all of the above
        ///         
        /// </remarks>
        /// <param name="scxItemDto">Document buffer contains additional selection critera (Search Start Date/Time, Dept). Default is Today.</param>
        /// <param name="Period">Period data should be returned for - 'All', 'Years', 'Months'. 'Days' or 'Day'. Default is "All"</param>
        [NSwag.Annotations.OpenApiOperation("GetArchiveDtoAsync", "Select DTO Subset Of Data From SCx Documents", "'Data Transfer Object' of archive date structure. \nThe Request Responds with a single DTO subset of SCx Document data (Id, TimeStamp, Document Type & Dept.) for each 'Year', each 'Month' for the year, each 'Day' of the month and each department for the 'Day'.")]
        [HttpGet(Name = nameof(GetArchiveDtoAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SCxItemDto), StatusCodes.Status200OK)]
        public async Task<IList<SCxItemDto>> GetArchiveDtoAsync([FromQuery]SCxItemDto? scxItemDto = null, string? Period = "All")
        {
            MessageHandler.MessageLog("GetArchiveDtoAsync");
            scxItemDto.Initialise(scxItemDto);
            ArchiveItemsDto = new ObservableCollection<SCxItemDto>();
            SCxItemsDto = new ObservableCollection<SCxItemDto>();
            SCxItemsDto = await Data.DbInitializer.SqlLoadItemsDtoAsync(_context);
            //object scxItems = _context.SC1Items; // scxItems = _context.SC1Items;
            if (Period == "All" || Period == "Years")
            {
                var groupedByYears = from item in SCxItemsDto.Where(arg =>  (scxItemDto.Type == string.Empty || arg.Type == scxItemDto.Type) &&
                                                                            (scxItemDto.Dept == string.Empty || arg.Dept == scxItemDto.Dept))
                                     group item by item.TimeStamp.Year into yearGroup
                                     orderby yearGroup.Key
                                     select yearGroup.Key;
                foreach (var yearGroup in groupedByYears)
                {
                    SCxItemDto newYearFolder = new();
                    newYearFolder.Initialise();
                    newYearFolder.TimeStamp = new DateTime(yearGroup, 01, 01, 00, 00, 00, 00);
                    ArchiveItemsDto.Add(newYearFolder);
                };
            }
            
            if (Period == "All" || Period == "Months")
            {
                var groupedByMonths = from item in SCxItemsDto.Where(arg => arg.TimeStamp.Year == scxItemDto.TimeStamp.Year &&
                                                                            (scxItemDto.Type == string.Empty || arg.Type == scxItemDto.Type) &&
                                                                            (scxItemDto.Dept == string.Empty || arg.Dept == scxItemDto.Dept))
                                      group item by item.TimeStamp.Month into monthGroup
                                      orderby monthGroup.Key
                                      select monthGroup.Key;

                foreach (var monthGroup in groupedByMonths)
                {
                    SCxItemDto newMonthFolder = new();
                    newMonthFolder.Initialise();
                    newMonthFolder.TimeStamp = new DateTime(scxItemDto.TimeStamp.Year, monthGroup, 01, 00, 00, 00, 00);

                    if (monthGroup == 1 && ArchiveItemsDto.Count > 0)
                    {   // First month entry for the year may be the last entry added to ArchiveItemsDto by the year processing,
                        // so delete the archive entry to avoid duplications.
                        ArchiveItemsDto.RemoveAt(ArchiveItemsDto.IndexOf(ArchiveItemsDto.Last()));
                    }
                    ArchiveItemsDto.Add(newMonthFolder);
                }
            }

            if (Period == "All" || Period == "Days")
            {
                var groupedByDays = from item in SCxItemsDto.Where(arg => arg.TimeStamp.Year == scxItemDto.TimeStamp.Year &&
                                                                          arg.TimeStamp.Month == scxItemDto.TimeStamp.Month &&
                                                                          (scxItemDto.Type == string.Empty || arg.Type == scxItemDto.Type) &&
                                                                          (scxItemDto.Dept == string.Empty || arg.Dept == scxItemDto.Dept))
                                    group item by item.TimeStamp.Day into dayGroup
                                    orderby dayGroup.Key
                                    select dayGroup.Key;

                foreach (var dayGroup in groupedByDays)
                {
                    SCxItemDto newDayFolder = new();
                    newDayFolder.Initialise();
                    newDayFolder.TimeStamp = new DateTime(scxItemDto.TimeStamp.Year, scxItemDto.TimeStamp.Month, dayGroup, 00, 00, 00);

                    if (dayGroup == 1 && ArchiveItemsDto.Count > 0)
                    {   // First day entry for the month may be the last entry added to ArchiveItemsDto by the month processing,
                        // so delete the archive entry to avoid duplications.
                        ArchiveItemsDto.RemoveAt(ArchiveItemsDto.IndexOf(ArchiveItemsDto.Last()));
                    }
                    ArchiveItemsDto.Add(newDayFolder);
                }
            }

            if (Period == "All" || Period == "Day")
            {                
                // group all this days documents by Department...
                var groupedByDept = from item in SCxItemsDto.Where(arg => arg.TimeStamp.Date == scxItemDto.TimeStamp.Date                  &&
                                                                          (scxItemDto.Type == string.Empty || arg.Type == scxItemDto.Type) &&
                                                                          (scxItemDto.Dept == string.Empty || arg.Dept == scxItemDto.Dept))
                                    group item.Dept by new { Dept = item.Dept, Type = item.Type} into deptsGroup
                                    orderby deptsGroup.Key.Dept, deptsGroup.Key.Type
                                    select deptsGroup.Key;

                if (groupedByDept.Count() > 0 && 
                    ArchiveItemsDto.Count() > 0 &&
                    ArchiveItemsDto.Last().TimeStamp.Date == scxItemDto.TimeStamp.Date)
                {   // First department entry for the month may be for the same date as the last entry added to ArchiveItemsDto 
                    // by the days processing, so delete the archive entry to avoid duplications.
                    ArchiveItemsDto.RemoveAt(ArchiveItemsDto.IndexOf(ArchiveItemsDto.Last()));
                }

                foreach (var deptGroup in groupedByDept)
                {
                    // group all the this departments documents by document type, into a sub group...
                    SCxItemDto newDeptFolder = new();
                    newDeptFolder.Initialise();
                    newDeptFolder.TimeStamp = scxItemDto.TimeStamp.Date;
                    newDeptFolder.Type = deptGroup.Type;
                    newDeptFolder.Dept = deptGroup.Dept;
                    ArchiveItemsDto.Add(newDeptFolder);
                }
            }

            return ArchiveItemsDto; // Ok(IList<SCxItemDto>);
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PutSC1ItemAsync
        ///     PUT: SCxItem/PutSC1ItemAsync
        ///     HTTP Put by Id request endpoint. 
        ///     This method responds by updating a single Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PutSC1ItemAsync", "Update SC1 Document", "Request performs Put operation on amended SC1 Document data")]
        [HttpPut(Name = nameof(PutSC1ItemAsync))]
        [ActionName(nameof(PutSC1ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutSC1ItemAsync([FromQuery] SC1Item? item = null)
        {
            MessageHandler.MessageLog("PutSC1ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);

            //var oldItem = await _context.SC1Items.FindAsync(item.Id);
            //if (oldItem is null) { return NotFound(); }
            //else {
            //  item.TimeStamp = oldItem.TimeStamp;                      // preserve original time stamp.
            //  _context.Entry(oldItem).State = EntityState.Detached; }  //Explicitly Detach the orphan tracked instance 
            //}

            if (!_context.SC1Items.Any(e => e.Id == item.Id)) { return NotFound(); }
            else if (item.Type != "SC1" || item.Dept == string.Empty) { return BadRequest(); }
            else
            {
                var entEntry = _context.Entry(item).State = EntityState.Modified;
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateConcurrencyException("PutSC1ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PutSC1ItemAsync: " + ex.Message, ex); }
                return NoContent();
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PutSC2ItemAsync
        ///     PUT: SCxItem/PutSC2ItemAsync
        ///     HTTP Put by Id request endpoint. 
        ///     This method responds by updating a single Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PutSC2ItemAsync", "Update SC2 Document", "Request performs Put operation on amended SC2 Document data")]
        [HttpPut(Name = nameof(PutSC2ItemAsync))]
        [ActionName(nameof(PutSC2ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutSC2ItemAsync([FromQuery] SC2Item? item = null)
        {
            MessageHandler.MessageLog("PutSC2ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);
            if (!_context.SC2Items.Any(e => e.Id == item.Id)) { return NotFound(); }
            else if (item.Type != "SC2" || item.Dept == string.Empty) { return BadRequest(); }
            else
            {
                var entEntry = _context.Entry(item).State = EntityState.Modified;
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateConcurrencyException("PutSC2ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PutSC2ItemAsync: " + ex.Message, ex); }
                return NoContent();
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PutSC3ItemAsync
        ///     PUT: SCxItem/PutSC3ItemAsync
        ///     HTTP Put by Id request endpoint. 
        ///     This method responds by updating a single Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PutSC3ItemAsync", "Update SC3 Document", "Request performs Put operation on amended SC3 Document data")]
        [HttpPut(Name = nameof(PutSC3ItemAsync))]
        [ActionName(nameof(PutSC3ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutSC3ItemAsync([FromQuery] SC3Item? item = null)
        {
            MessageHandler.MessageLog("PutSC3ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);
            if (!_context.SC3Items.Any(e => e.Id == item.Id)) { return NotFound(); }
            else if (item.Type != "SC3" || item.Dept == string.Empty) { return BadRequest(); }
            else
            {
                var entEntry = _context.Entry(item).State = EntityState.Modified;
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateConcurrencyException("PutSC3ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PutSC3ItemAsync: " + ex.Message, ex); }
                return NoContent();
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PutSC4ItemAsync
        ///     PUT: SCxItem/PutSC4ItemAsync
        ///     HTTP Put by Id request endpoint. 
        ///     This method responds by updating a single Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PutSC4ItemAsync", "Update SC4 Document", "Request performs Put operation on amended SC4 Document data")]
        [HttpPut(Name = nameof(PutSC4ItemAsync))]
        [ActionName(nameof(PutSC4ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutSC4ItemAsync([FromQuery] SC4Item? item = null)
        {
            MessageHandler.MessageLog("PutSC4ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);
            if (!_context.SC4Items.Any(e => e.Id == item.Id)) { return NotFound(); }
            else if (item.Type != "SC4" || item.Dept == string.Empty) { return BadRequest(); }
            else
            {
                var entEntry = _context.Entry(item).State = EntityState.Modified;
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateConcurrencyException("PutSC4ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PutSC4ItemAsync: " + ex.Message, ex); }
                return NoContent();
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PutSC9ItemAsync
        ///     PUT: SCxItem/PutSC9ItemAsync
        ///     HTTP Put by Id request endpoint. 
        ///     This method responds by updating a single Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PutSC9ItemAsync", "Update SC9 Document", "Request performs Put operation on amended SC9 Document data")]
        [HttpPut(Name = nameof(PutSC9ItemAsync))]
        [ActionName(nameof(PutSC9ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutSC9ItemAsync([FromQuery] SC9Item? item = null)
        {
            MessageHandler.MessageLog("PutSC9ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);
            if (!_context.SC9Items.Any(e => e.Id == item.Id)) { return NotFound(); }
            else if (item.Type != "SC9" || item.Dept == string.Empty) { return BadRequest(); }
            else
            {
                var entEntry = _context.Entry(item).State = EntityState.Modified;
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateConcurrencyException("PutSC9ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PutSC9ItemAsync: " + ex.Message, ex); }
                return NoContent();
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PostSC1ItemAsync
        ///     POST: SCxItems/PostSC1ItemAsync
        ///     HTTP Post SC1 data request endpoint. 
        ///     This method responds by creating a new SCx Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PostSC1ItemAsync", "Create New SC1 Document", "Request performs Create operation on new SC1 Document data")]
        [HttpPost(Name = nameof(PostSC1ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSC1ItemAsync([FromQuery] SC1Item? item = null)
        {            
            MessageHandler.MessageLog("PostSC1ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);                    // Default Class initialisation for uninitialise attributes
            if (_context.SC1Items.Any(e => e.Id == item.Id)) { return Conflict(); }
            else if (item.Dept == string.Empty || item.Type != "SC1") { return BadRequest(); }
            else
            {
                var entEntry = _context.SC1Items.Add(item);
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("PostSC1ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PostSC1ItemAsync: " + ex.Message, ex); }
                return CreatedAtAction(nameof(PostSC1ItemAsync), new { item = item }, item);
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PostSC2ItemAsync (override)
        ///     POST: SCxItems/PostSC2ItemAsync
        ///     HTTP Post SC2 data request endpoint. 
        ///     This method responds by creating a new SCx Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PostSC2ItemAsync", "Create New SC2 Document", "Request performs Create operation on new SC2 Document data")]
        [HttpPost(Name = nameof(PostSC2ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSC2ItemAsync([FromQuery] SC2Item? item = null)
        {
            MessageHandler.MessageLog("PostSC2ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);                    // Default Class initialisation for uninitialise attributes
            if (_context.SC2Items.Any(e => e.Id == item.Id)) { return Conflict(); }
            else if (item.Dept == string.Empty || item.Type != "SC2") { return BadRequest(); }
            else
            {
                var entEntry = _context.SC2Items.Add(item);
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("PostSC2ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PostSC2ItemAsync: " + ex.Message, ex); }
                return CreatedAtAction(nameof(PostSC2ItemAsync), new { item = item }, item);
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PostSC3ItemAsync (override)
        ///     POST: SCxItems/PostSC3ItemAsync
        ///     HTTP Post SC3 data request endpoint. 
        ///     This method responds by creating a new SCx Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PostSC3ItemAsync", "Create New SC3 Document", "Request performs Create operation on new SC3 Document data")]
        [HttpPost(Name = nameof(PostSC3ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSC3ItemAsync([FromQuery] SC3Item? item = null)
        {
            MessageHandler.MessageLog("PostSC3ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);                    // Default Class initialisation for uninitialise attributes
            if (_context.SC3Items.Any(e => e.Id == item.Id)) { return Conflict(); }
            else if (item.Dept == string.Empty || item.Type != "SC3") { return BadRequest(); }
            else
            {
                var entEntry = _context.SC3Items.Add(item);
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("PostSC3ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PostSC3ItemAsync: " + ex.Message, ex); }
                return CreatedAtAction(nameof(PostSC3ItemAsync), new { item = item }, item);
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PostSC4ItemAsync (override)
        ///     POST: SCxItems/PostSC4ItemAsync
        ///     HTTP Post SC4 data request endpoint. 
        ///     This method responds by creating a new SCx Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PostSC4ItemAsync", "Create New SC4 Document", "Request performs Create operation on new SC4 Document data")]
        [HttpPost(Name = nameof(PostSC4ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSC4ItemAsync([FromQuery] SC4Item? item = null)
        {
            MessageHandler.MessageLog("PostSC4ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);                    // Default Class initialisation for uninitialise attributes
            if (_context.SC4Items.Any(e => e.Id == item.Id)) { return Conflict(); }
            else if (item.Dept == string.Empty || item.Type != "SC4") { return BadRequest(); }
            else
            {
                var entEntry = _context.SC4Items.Add(item);
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("PostSC4ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PostSC4ItemAsync: " + ex.Message, ex); }
                return CreatedAtAction(nameof(PostSC4ItemAsync), new { item = item }, item);
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.PostSC9ItemAsync (override)
        ///     POST: SCxItems/PostSC9ItemAsync
        ///     HTTP Post SC9 data request endpoint. 
        ///     This method responds by creating a new SCx Document.
        /// </summary>
        /// <param name="item">Document</param>
        [NSwag.Annotations.OpenApiOperation("PostSC9ItemAsync", "Create New SC9 Document", "Request performs Create operation on new SC9 Document data")]
        [HttpPost(Name = nameof(PostSC9ItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSC9ItemAsync([FromQuery] SC9Item? item = null)
        {
            MessageHandler.MessageLog("PostSC9ItemAsync[" + item.Id.ToString() + "]");
            item.Initialise(item);                    // Default Class initialisation for uninitialise attributes
            if (_context.SC9Items.Any(e => e.Id == item.Id)) { return Conflict(); }
            else if (item.Dept == string.Empty || item.Type != "SC9") { return BadRequest(); }
            else
            {
                var entEntry = _context.SC9Items.Add(item);
                try { var taskResult = await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("PostSC9ItemAsync: " + ex.Message, ex); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("PostSC9ItemAsync: " + ex.Message, ex); }
                return CreatedAtAction(nameof(PostSC9ItemAsync), new { item = item }, item);
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.DeleteSCxItemAsync
        ///     DELETE: SCxItems/DeleteSCxItemAsync/5
        ///     HTTP Delete by id request endpoint. 
        ///     This method responds by deleting a single SCx Document.
        /// </summary>
        /// <param name="scxItemDto">Document DTO</param>
        [NSwag.Annotations.OpenApiOperation("DeleteSCxItemAsync", "Delete SCx Document By Guid", "Request performs Delete operation on an SCx Document")]
        [HttpDelete(Name = nameof(DeleteSCxItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSCxItemAsync([FromQuery] SCxItemDto? scxItemDto = null)
        {
            MessageHandler.MessageLog("DeleteSCxItemAsync[" + scxItemDto.Id.ToString() + "]");

            scxItemDto.Initialise(scxItemDto);

            if (scxItemDto.Id == Guid.Empty || scxItemDto.Type == string.Empty) { return BadRequest(); }
            else
            {
                switch (scxItemDto.Type)
                {
                    case "SC1": {
                            if (!_context.SC1Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else {
                                var sc1Item = await _context.SC1Items.FindAsync(scxItemDto.Id);
                                _context.SC1Items.Remove(sc1Item);
                                try { await _context.SaveChangesAsync(); }
                                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("DeleteSCxItemAsync(SC1): " + ex.Message, ex); }
                                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC1): " + ex.Message, ex); }
                                return NoContent();
                            }
                        }
                    case "SC2": {
                            if (!_context.SC2Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else {
                                var sc2Item = await _context.SC2Items.FindAsync(scxItemDto.Id);
                                _context.SC2Items.Remove(sc2Item);
                                try { await _context.SaveChangesAsync(); }
                                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("DeleteSCxItemAsync(SC2): " + ex.Message, ex); }
                                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC2): " + ex.Message, ex); }
                                return NoContent();
                            }
                        }
                    case "SC3":
                        {
                            if (!_context.SC3Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else {
                                var sc3Item = await _context.SC3Items.FindAsync(scxItemDto.Id);
                                _context.SC3Items.Remove(sc3Item);
                                try { await _context.SaveChangesAsync(); }
                                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("DeleteSCxItemAsync(SC3): " + ex.Message, ex); }
                                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC3): " + ex.Message, ex); }
                                return NoContent();
                            }
                        }
                    case "SC4":
                        {
                            if (!_context.SC4Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else {
                                var sc4Item = await _context.SC4Items.FindAsync(scxItemDto.Id);
                                _context.SC4Items.Remove(sc4Item);
                                try { await _context.SaveChangesAsync(); }
                                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("DeleteSCxItemAsync(SC4): " + ex.Message, ex); }
                                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC4): " + ex.Message, ex); }
                                return NoContent();
                            }
                        }
                    case "SC9":
                        {
                            if (!_context.SC9Items.Any(e => e.Id == scxItemDto.Id)) { return NotFound(); }
                            else {
                                var sc9Item = await _context.SC9Items.FindAsync(scxItemDto.Id);
                                _context.SC9Items.Remove(sc9Item);
                                try { await _context.SaveChangesAsync(); }
                                catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); throw new DbUpdateException("DeleteSCxItemAsync(SC9): " + ex.Message, ex); }
                                catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC9): " + ex.Message, ex); }
                                return NoContent();
                            }
                        }
                    default: { return BadRequest(); }
                }
            }
        }

        /// <summary>
        ///     DbWebAPI.Controllers.Grouping
        ///     Grouping Class - Grouping fuctionallity for sort
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        private class Grouping<K, T> : ObservableCollection<T>
        {
            public K Key { get; private set; }

            /// <summary>
            ///     Grouping - Group Documents together.
            /// </summary>
            /// <param name="key">Sort Key</param>
            /// <param name="items">Document list</param>
            public Grouping(K key, IEnumerable<T> items)
            {
                Key = key;
                foreach (T item in items)
                {
                    Items.Add(item);
                }
            }
        }

        // Check by id if the SCxItem already exists
        private bool SCxItemExists(Guid id)
        {
            return _context.SCxItems.Any(e => e.Id == id);
        }

        ///// <summary>
        /////     DbWebAPI.Controllers.SqlLoadItemsDtoAsync
        /////     Loads a 'Data Transfer Object' subset of all Document data.
        /////     Called by SCxItemsController.GetArchiveDtoAsync 
        ///// </summary>
        ///// <remarks>
        /////     Uses a SQL sub-select query to merge all the document data 
        /////     (SC1 - SC9) into a 'ArchiveItemsDto' which consists of 
        /////     the following columns...
        /////     
        /////         Id
        /////         TimeStamp
        /////         Type
        /////         Dept. 
        /////         
        /////     The ArchiveItemsDto data that will be sorted into 
        /////     year/month/day/dept/Type and is ultimately passed back to  
        /////     the calling APP to create the archive folders layout.
        ///// </remarks>
        ///// <param name="_context">Database set</param>
        //public static async Task<ObservableCollection<SCxItemDto>> SqlLoadItemsDtoAsync(SCxItemContext _context)
        //{
        //    // Create DTO subset of all document data 
        //    var SCxItemsDto = new ObservableCollection<SCxItemDto>();

        //    try 
        //    {
        //        string connString = @"Server =.\SQLEXPRESS; Database = SCxDb; Trusted_Connection = True;";
        //        using (SqlConnection conn = new SqlConnection(connString))
        //        {
        //            string query = @"SELECT Id, TimeStamp, Type, Dept " +
        //                            "FROM(" +
        //                                "SELECT dbo.SC1.Id, dbo.SC1.TimeStamp, dbo.SC1.Type, dbo.SC1.Dept FROM dbo.SC1 UNION ALL " +
        //                                "SELECT dbo.SC2.Id, dbo.SC2.TimeStamp, dbo.SC2.Type, dbo.SC2.Dept FROM dbo.SC2 UNION ALL " +
        //                                "SELECT dbo.SC3.Id, dbo.SC3.TimeStamp, dbo.SC3.Type, dbo.SC3.Dept FROM dbo.SC3 UNION ALL " +
        //                                "SELECT dbo.SC4.Id, dbo.SC4.TimeStamp, dbo.SC4.Type, dbo.SC4.Dept FROM dbo.SC4 UNION ALL " +
        //                                "SELECT dbo.SC9.Id, dbo.SC9.TimeStamp, dbo.SC9.Type, dbo.SC9.Dept FROM dbo.SC9 " +
        //                            ")Dto ORDER BY TimeStamp";
        //            SqlCommand tSqlCmd = new SqlCommand(query, conn);       // Setup the T-SQL command
        //            conn.Open();                                            // Open the connection
        //            SqlDataReader sqlReader = await tSqlCmd.ExecuteReaderAsync();      //execute the SQLCommand
        //            if (sqlReader.HasRows)
        //            {
        //                while (await sqlReader.ReadAsync())
        //                {
        //                    SCxItemDto newDto = new SCxItemDto
        //                    {
        //                        Id = sqlReader.GetGuid(0),
        //                        TimeStamp = sqlReader.GetDateTime(1),
        //                        Type = sqlReader.GetString(2),
        //                        Dept = sqlReader.GetString(3)
        //                    };
        //                    SCxItemsDto.Add(newDto);                    //Create a list of all document types in DTO form

        //                }

        //            }
        //            sqlReader.Close();                                  //close data reader
        //            conn.Close();                                       //close connection
        //        }
        //    }
        //    catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC1): " + ex.Message, ex); }
        //    MessageHandler.MessageLog("SCxItems Count = " + SCxItemsDto.Count().ToString(), "Trace");
        //    return SCxItemsDto;
        //}

        /// <summary>
        ///     DbWebAPI.Controllers.SqlLoadSCxItemsAsync
        ///     Loads all Document data from the selected document table (SC1 - SC9).
        ///     Called by SCxItemsController.GetSCxItemsSelectAsync 
        /// </summary>
        /// <remarks>
        ///     Uses a SQL select query to select rows from the specified table (SC1 - SC9). 
        ///         
        ///     The ArchiveItemsDto data that will be sorted into 
        ///     year/month/day/dept/Type.
        /// </remarks>
        /// <param name="_context">Database set</param>
        /// <param name="scxItemDto">SCxItem DTO used as parameters for selection criteria</param>
        public static async Task<List<IEnumerable<object>>> SqlLoadSCxItemsAsync(SCxItemContext _context, SCxItemDto scxItemDto)
        {
            // Create DTO subset of all document data 
            int count = 0;
            object scxData;
            SC1Item sc1item = new();
            List<IEnumerable<SC1Item>> sc1Items = new();
            List<IEnumerable<SC2Item>> sc2Items = new(); 
            List<IEnumerable<SC3Item>> sc3Items = new(); 
            List<IEnumerable<SC4Item>> sc4Items = new(); 
            List<IEnumerable<SC9Item>> sc9Items = new();
            object scxItems, nextItem ;
            switch (scxItemDto.Type)
            {// List<List<SC1Item>> data = new List<List<SC1Item>>();
                case "SC1": { scxData = sc1Items; nextItem = new SC1Item(); break; }
                case "SC2": { scxData = sc2Items; nextItem = new SC2Item(); break; }
                case "SC3": { scxData = sc3Items; nextItem = new SC3Item(); break; }
                case "SC4": { scxData = sc4Items; nextItem = new SC4Item(); break; }
                case "SC9": { scxData = sc9Items; nextItem = new SC9Item(); break; }
                default: { return null; }
            }

            try
            {
                string connString = @"Server =.\SQLEXPRESS; Database = SCxDb; Trusted_Connection = True;";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"SELECT * FROM dbo." + scxItemDto.Type + " ORDER BY TimeStamp";
                    SqlCommand tSqlCmd = new SqlCommand(query, conn);               // Setup the T-SQL command
                    conn.Open();                                                    // Open the connection
                    SqlDataReader sqlReader = await tSqlCmd.ExecuteReaderAsync();   // execute the SQLCommand
                    if (sqlReader.HasRows)
                    {
                        //while (await sqlReader.ReadAsync())
                        //{
                        //    nextItem = new SCxItem()
                        //    {
                        //        Id = sqlReader.GetGuid(0),
                        //        TimeStamp = sqlReader.GetDateTime(1),
                        //        Type = sqlReader.GetString(2),
                        //        Dept = sqlReader.GetString(3)
                        //    };
                        //    scxItems.Add(nextItem);                                //Create a list of all document types in DTO form
                        //    count++;

                        //}
                        int sqlCount = sqlReader.FieldCount;                        // Read the stream bring a row back each time 
                        while (await sqlReader.ReadAsync())                         // into the relevant format.
                        {
                            SC1Item sc1row = new();
                            object[] columnData = new object[sqlReader.FieldCount];
                            object[] x = new SC1Item[200];
                            sqlReader.GetSqlValues(columnData);
                            sc1item.Id = (Guid) columnData.GetValue(0);
                            //sc1item = (SC1Item)columnData;
                            //scxData.Add(columnData);
                            //scxData.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)) as IEnumerable<SC1Item>);
                            //sc1.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)).ToString());
                            //switch (scxItemDto.Type)
                            //{// List<List<SC1Item>> data = new List<List<SC1Item>>();
                            //    case "SC1": { scxData.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)) as IEnumerable<SC1Item>); break; }
                            //    case "SC2": { dataSC2.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)) as IEnumerable<SC2Item>); break; }
                            //    case "SC3": { dataSC3.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)) as IEnumerable<SC3Item>); break; }
                            //    case "SC4": { dataSC4.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)) as IEnumerable<SC4Item>); break; }
                            //    case "SC9": { dataSC9.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)) as IEnumerable<SC9Item>); break; }
                            //    default: { return null; }
                            //}
                            //scxItems.Add(Enumerable.Range(0, sqlCount - 1).Select(it => sqlReader.GetValue(it)));
                            //count = sqlReader.GetSqlValues((object[])nextItem); //Create a list of all documents for specified type
                        }
                    }
                    sqlReader.Close();                                              //close data reader
                    conn.Close();                                                   //close connection
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); throw new NotSupportedException("DeleteSCxItemAsync(SC1): " + ex.Message, ex); }
            MessageHandler.MessageLog("SCxItems Count = " + count.ToString(), "Trace");
            return (List<IEnumerable<object>>)scxData;
        }
    }
}

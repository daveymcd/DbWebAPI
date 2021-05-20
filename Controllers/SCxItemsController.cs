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

        /// <summary>
        ///     This method gets called by the runtime and configures the Database context.
        /// </summary>
        public SCxItemsController(SCxItemContext context)
        {
            _context = context;
            if (!_context.SCxItems.Any())
            { // If no data - setup test data.
                foreach (SCxItem sCxItem in SCxItem.AddSCxData()) { _context.SCxItems.Add(sCxItem); }
                foreach (SCxItem sCxItem in SCxItem.AddThisWeeksSCxData()) { _context.SCxItems.Add(sCxItem); }
                _context.SaveChangesAsync();
            }
        }

        //*** API PROCESSING... ***//

        /// <summary>
        ///     GET: SCxItems/GetSCxItemsAsync - Get all. 
        ///     HTTP get request endpoint. This method responds with all SCx Document data.
        /// </summary>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.GetSCxItemsAsync()", "Get All SCx Documents", "Request Responds with all SCx Document data")]
        [HttpGet(Name = nameof(GetSCxItemsAsync))]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SCxItem>>> GetSCxItemsAsync()
        {
            return await _context.SCxItems.ToListAsync();
        }

        /// <summary>
        ///     GET: SCxItems/GetSCxItemAsync/5 - Get by Id.
        ///     HTTP get by id request endpoint. This method responds with single SCx Document data.
        /// </summary>
        /// <param name="id">Document Id</param>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.GetSCxItemAsync(Guid)", "Get Single SCx Document By Guid", "Request Responds with single SCx Document")]
        [HttpGet("{id}", Name = nameof(GetSCxItemAsync))]
        [ActionName(nameof(GetSCxItemAsync))]                              // Needed to get around route error on create in Swagger - "InvalidOperationException: No route matches the supplied values".
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SCxItem>> GetSCxItemAsync(Guid id)
        {
            if (id == Guid.Empty) { return BadRequest(); }
            else if (!SCxItemExists(id)) { return NotFound(); }
            else { return (await _context.SCxItems.FindAsync(id)); }
        }

        /// <summary>
        ///     GET: SCxItems/GetSCxItemsSelectAsync - Select entries.
        ///     HTTP get by selection request endpoint. This method responds with selected SCx Documents.
        /// </summary>
        /// <param name="SearchFromTimeStamp">Search start Date/Time</param>
        /// <param name="SearchToTimeStamp">Search end Date/Time</param>
        /// <param name="Type">Document Type</param>
        /// <param name="Dept">Catering Department</param>
        /// <param name="Supplier">Supplier Name</param>
        /// <param name="CheckUBD">Use-By-Date status</param>
        /// <param name="SignOff">Supervisor sign off</param>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.GetSCxItemSelectAsync(DateTime, DateTime, String, String, String, Int, String)", "Select SCx Documents", "The Request Responds with a subset of SCx Document data, filtered according to search criteria. \nSelection is by Date/Time Range, Document Type, Department, Supplier, Use-By-Date check, Sign-off signitary or any combination. \nIf a Date & Time pair are supplied the search begins or ends at that point, however, if the Time element is not supplied then the whole days data for the Date is searched.")]
        [HttpGet(Name = nameof(GetSCxItemsSelectAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<IList<SCxItem>>> GetSCxItemsSelectAsync( DateTime? SearchFromTimeStamp=null,
                                                                                DateTime? SearchToTimeStamp = null,
                                                                                string? Type = null,
                                                                                string? Dept = null,
                                                                                string? Supplier = null,
                                                                                int? CheckUBD = null,
                                                                                string? SignOff = null)
        {
            SCxSearchCriteria SCxSearchParams = new();
            if (SearchFromTimeStamp is not null)
            {
                SCxSearchParams.FromDate = SearchFromTimeStamp.Value.Date;
                SCxSearchParams.FromTime = SearchFromTimeStamp;
            }
            if (SearchToTimeStamp is not null)
            {
                SCxSearchParams.ToDate = SearchToTimeStamp.Value.Date;
                SCxSearchParams.ToTime = SearchToTimeStamp;
            }
            if (Type is not null) { SCxSearchParams.Type = Type; }
            if (Dept is not null) { SCxSearchParams.Dept = Dept; }
            if (Supplier is not null) { SCxSearchParams.Supplier = Supplier; }
            if (CheckUBD is not null) { SCxSearchParams.CheckUBD = CheckUBD; }
            if (SignOff is not null) { SCxSearchParams.SignOff = SignOff; }

            return (List<SCxItem>) SCxSearchParams.SCxSearch(await _context.SCxItems.ToListAsync());
        }

        /// <summary>
        ///     GET: SCxItems/GetSCxItemsSelectDtoAsync - select entries DTO.
        ///     HTTP get by selection request endpoint. This method responds with a Data Transfer Object subset of selected SCx Document data.
        /// </summary>
        /// <param name="SearchFromTimeStamp">Search Start Date/Time</param>
        /// <param name="SearchToTimeStamp">Search End Date/Time</param>
        /// <param name="Type">Document Type</param>
        /// <param name="Dept">Department</param>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.GetSCxItemSelectDtoAsync(DateTime, DateTime, String, String)", "Select DTO Subset Of Data From SCx Documents", "'Data Transfer Object' version of GetSCxItemSelect(). \nThe Request Responds with a DTO subset of SCx Document data (Id, TimeStamp, Document Type & Dept.) filtered according to search criteria. \nSelection is by Date/Time Range, Document Type, Department, Supplier or any combination. \nIf a Date & Time pair is supplied the search begins or ends at that point, however, if the Time element is not supplied then the whole days data for the Date is searched.")]
        [HttpGet(Name = nameof(GetSCxItemsSelectDtoAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status200OK)]
        public async Task<IQueryable<SCxItemDto>> GetSCxItemsSelectDtoAsync(DateTime? SearchFromTimeStamp = null,
                                                                            DateTime? SearchToTimeStamp = null,
                                                                            string? Type = null,
                                                                            string? Dept = null)
        {
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

            var sCxItemDto = from sCxItems in _context.SCxItems
                             .Where((SCxItem arg) => (arg.TimeStamp >= FromDate &&
                                                      arg.TimeStamp <= ToDate) &&
                                                     (Type == null || arg.Type == Type) &&
                                                     (Dept == null || arg.Dept == Dept))
                                 //.Include(dto => dto.Id)
                                 //.Include(dto => dto.TimeStamp)
                                 //.Include(dto => dto.Type)
                                 //.Include(dto => dto.Dept)
                             select new SCxItemDto()
                             {
                                 Id = sCxItems.Id,
                                 TimeStamp = sCxItems.TimeStamp,
                                 Type = sCxItems.Type,
                                 Dept = sCxItems.Dept
                             };

            return sCxItemDto; // Ok(sCxItemDto);

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
        ///     PUT: SCxItem/PutSCxItemAsync/5 - update by Id.
        ///     HTTP Put by id request endpoint. This method responds by updating a single SCx Document.
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <param name="sCxItem">Document</param>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.PutSCxItemAsync(Guid, SCxItem)", "Update SCx Document By Guid", "Request preforms Put operation on SCx Document data")]
        [HttpPut("{id}", Name = nameof(PutSCxItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutSCxItemAsync(Guid id, SCxItem sCxItem)
        {
            if (id != sCxItem.Id) { return BadRequest(); }
            else if (!SCxItemExists(id)) { return NotFound(); }
            else
            {
                _context.Entry(sCxItem).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return NoContent();
            }
        }

        /// <summary>
        ///     POST: SCxItems/PostSCxItemAsync - create SCxItem.
        ///     HTTP Post request endpoint. This method responds by creating a new SCx Document.
        /// </summary>
        /// <param name="sCxItem">Document</param>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.PostSCxItemAsync(SCxItem)", "Create New SCx Document", "Request preforms Create operation on new SCx Document data")]
        [HttpPost(Name = nameof(PostSCxItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SCxItem>> PostSCxItemAsync(SCxItem sCxItem)
        {
            if (SCxItemExists(sCxItem.Id)) { return Conflict(); }
            sCxItem.Initialise(sCxItem);            // Default Class initialisation for uninitialise attributes
            _context.SCxItems.Add(sCxItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction(nameof(GetSCxItemAsync), new { id = sCxItem.Id }, sCxItem);
        }

        /// <summary>
        ///     DELETE: SCxItems/DeleteSCxItemAsync/5 - delete by Id.
        ///     HTTP Delete by id request endpoint. This method responds by deleting a single SCx Document.
        /// </summary>
        /// <param name="id">Document Id</param>
        [NSwag.Annotations.OpenApiOperation("DbWebApi.DeleteSCxItemAsync(Guid)", "Delete SCx Document By Guid", "Request preforms Delete operation on an SCx Document")]
        [HttpDelete("{id}", Name = nameof(DeleteSCxItemAsync))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SCxItem), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSCxItemAsync(Guid id)
        {
            if (id == Guid.Empty) { return BadRequest(); }
            else if (!SCxItemExists(id)) { return NotFound(); }
            else
            {
                var sCxItem = await _context.SCxItems.FindAsync(id);
                _context.SCxItems.Remove(sCxItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        // Check by id if the SCxItem already exists
        private bool SCxItemExists(Guid id)
        {
            return _context.SCxItems.Any(e => e.Id == id);
        }
    }
}

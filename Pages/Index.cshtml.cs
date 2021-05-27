using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using DbWebAPI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;

namespace DbWebAPI
{
    /// <summary>
    /// 
    ///     DbWebAPI.Models.Index.cshtml.cs - SCx Documents View
    ///     
    ///     Main Landing page for Razor Pages. initially displays all documents in table form.
    ///     
    /// </summary>
    /// <remarks>
    /// 
    ///     Index.cshtml displays a tabular list of documents and allows the client to perform 
    ///     the following actions via the pages clickables...
    ///     
    ///         Search  - Search the document archive for a subset of data based on 
    ///                   date-Range/Document-Type/Department etc. 
    ///         Sort    - Sort the table by column.
    ///         New     - Create a new document.
    ///         Edit    - Click on an individual row to invoke _popupEdit modal where 
    ///                   a document can be Updated, Deleted or Copied. The client can 
    ///                   also navigate the documents without leaving the modal via
    ///                   the Next and previous buttons.
    ///                   
    ///         Crud operations do not have their own page (as per the VS boilerplate). 
    ///         most functionallity is provided by the Edit modal which allows the client 
    ///         to navigate the document whilst making amendments.
    ///         
    ///         Edit, Create and Search operations are provided from the modal by the 
    ///         relevant partial view.
    ///     
    /// </remarks>>
    public class IndexModel : PageModel
    {
        /// <summary>Document List</summary>
        public IList<SCxItem> sCxItems { get;set; }
        /// <summary>Document</summary>
        [BindProperty(SupportsGet = true)] public SCxItem sCxItem { get; set; }
        /// <summary>Document Search Criteria</summary>
        [BindProperty(SupportsGet = true)] public SCxSearchCriteria SCxSearchParams { get; set; }

        private readonly DbWebAPI.Models.SCxItemContext _context;

        /// <summary>Set Razor Pages Databas Context</summary>
        public IndexModel(DbWebAPI.Models.SCxItemContext context)
        {
            _context = context;
            if (!_context.SCxItems.Any())
            {                                           // If no data - setup test data for the last 31 days.
                foreach (SCxItem sCxItem in SCxItem.AddThisMonthsSCxData()) { _context.SCxItems.Add(sCxItem); }
                _context.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     DbWebAPI.IndexModel.OnGetAsync() -  Entry point.
        ///     On 1st use, all document records are retrieved for display.
        /// </summary>
        public async Task OnGetAsync()
        {
            sCxItems = await _context.SCxItems.OrderByDescending(item => item.TimeStamp).ToListAsync();
            sCxItem = new();                                  // empty single document used by Create and Search handlers etc
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnPostSortAsync("column") - Sort Handler
        ///     Sort handler POST end point for '/Index?handler=sort'
        /// 
        ///     Sorts a table of data by column when a column heading is clicked.
        /// 
        /// </summary>
        /// <remarks>
        /// 
        ///     Each column heading is wrapped in a form statement in which
        ///     the method type is set to post and the asp-page-handler is set to 'sort'. 
        ///     when clicked, invokes this handler OnPost'Sort'Async (inserts 'Sort' into OnPostAsync).
        /// 
        ///     The method is passed the name of the column which identifies the column to sort on.
        /// 
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="sort" asp-route-column="Supplier" method="post">
        ///         <input type = "hidden" />
        ///         <input type="submit" value="Supplier"/>
        ///     </form>
        /// </example>
        /// <param name="column">Column name to sort on</param>
        public async void OnPostSortAsync(string column)
        {
            //sCxItem = await _context.SCxItems.FirstOrDefaultAsync();
            //var typeInfo = sCxItem.GetType();
            //var propertyInfo = typeof(SCxItem).GetProperty(column);
            //var value = propertyInfo.GetValue(sCxItem);
            //var allInOne = sCxItem.GetType().GetProperty(column).GetValue(sCxItem, null);
            //var reflected = Helpers.GetReflectedPropertyValue(sCxItem, column);
            //sCxItems = await _context.SCxItems.OrderBy(arg => Helpers.GetReflectedPropertyValue(arg, column)).ToListAsync();
            sCxItems = column switch
            {
                "TimeStamp" => await _context.SCxItems.OrderBy(arg => arg.TimeStamp).ToListAsync(),
                "Document" => await _context.SCxItems.OrderBy(arg => arg.Type).ToListAsync(),
                "Dept" => await _context.SCxItems.OrderBy(arg => arg.Dept).ToListAsync(),
                "Food" => await _context.SCxItems.OrderBy(arg => arg.Food).ToListAsync(),
                "Supplier" => await _context.SCxItems.OrderBy(arg => arg.Supplier).ToListAsync(),
                "Temperature" => await _context.SCxItems.OrderBy(arg => arg.Temperature).ToListAsync(),
                "CheckUBD" => await _context.SCxItems.OrderBy(arg => arg.CheckUBD).ToListAsync(),
                "SignOff" => await _context.SCxItems.OrderBy(arg => arg.SignOff).ToListAsync(),
                _ => await _context.SCxItems.OrderByDescending(arg => arg.TimeStamp).ToListAsync()
            };
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetSearch() - Search Handler (data entry)
        ///     Search Handler. GET End point for '/Index?handler=search'
        ///     
        /// </summary>
        /// <remarks>
        /// 
        ///     This handler is invoked by the Search button in Index.cshtml.
        ///     Returns the modal _PopupSearch.cshtml for user input
        ///     
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="search" method="get">
        ///         <input type="hidden" />
        ///         <input id="modal" class="search" type="submit" value="search" data-toggle="modal" data-target="#popup-modal" data-url="@Url.Page('Index', 'search')" />
        ///     </form>
        /// </example>
        public PartialViewResult OnGetSearch()
        {
            return Partial("_PopupSearch", SCxSearchParams);
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnPostSearchAsync() - Search Handler - (retrieve list)
        ///     Search handler - POST end point for '/Index?handler=search'
        /// 
        /// </summary>
        /// <remarks>
        /// 
        ///     The view uses a Search button to invoke a JavaScript function which loads a popup 
        ///     (_popupSearch.cshtml) and prompts the user to enter the search criteria.
        ///     
        ///     the popup specifies a Search button with the method type set to 'post'
        ///     and the asp-page-handler set to 'search'. When clicked this OnPost'Search'Async handler
        ///     is invoked (injects 'Search' into OnPostAsync).
        /// 
        ///     This method then calls the ScxSearchCriteria search method SCxSearch(), which uses 
        ///     the bound search criteria (SCxSearchParams) entered by the user, to return a subset 
        ///     of documents from SCxItems.
        /// 
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="search" method="post">
        ///         <input type="hidden" />
        ///         <input class="search" type="submit" value="Search" data-toggle="modal" data-target="#popup-modal" data-url="@Url.Page('Index', 'search')" />
        ///     </form>
        /// </example>
        public async void OnPostSearchAsync()
        {
           sCxItems = SCxSearchParams.SCxSearch(await _context.SCxItems.ToListAsync());
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetNew() - Create New Document Handler (data entry)
        ///     Search Handler. GET End point for '/Index?handler=new'
        ///     
        /// </summary>
        /// <remarks>
        /// 
        ///     This handler is invoked by the Create button ('+') in Index.cshtml.
        ///     Returns the modal _PopupNew.cshtml for user input
        ///     
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="new" method="get">
        ///         <input type="hidden" />
        ///         <input id="modal" class="create" type="submit" value="create" data-toggle="modal" data-target="#popup-modal" data-url="@Url.Page('Index', 'new')" />
        ///     </form>
        /// </example>
        public PartialViewResult OnGetNew()
        {
            sCxItem = new();
            sCxItem.Initialise(sCxItem);
            return Partial("_PopupNew", sCxItem);
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetCreateAsync() Create New Document Handler (database update)
        ///     Create handler - Get end point for '/Index?hanler=Create'
        /// 
        /// </summary>
        /// <remarks>
        /// 
        ///     The view uses a '+' button to invoke a JavaScript function which loads the popup
        ///     (_PopupNew.cshtml) and prompts the user to enter document data. 
        ///     
        ///     the popup specifies a Create button with the method type set to 'get'
        ///     and the asp-page-handler set to 'create'. When clicked this OnGet'Create'Async handler
        ///     is invoked (injects 'Create' into OnGetAsync).
        /// 
        ///     This method then creates a new document in SCxItems and returns a partialView
        ///     to the modal, ready for the next create.
        /// 
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="create" method="get">
        ///         <input type="hidden" />
        ///         <input class="create" type="submit" value="create" data-toggle="modal" data-target="#popup-modal" data-url="@Url.Page('Index', 'create')" />
        ///     </form>
        /// </example>
        public async Task<IActionResult> OnGetCreateAsync()
        {
            if (!ModelState.IsValid) return BadRequest();
            else if (sCxItem.Type is null || sCxItem.Type == "") return NotFound();
            else
            {
                try
                {
                    sCxItem.Id = Guid.NewGuid();
                    _context.SCxItems.Add(sCxItem);
                    var rtn = await _context.SaveChangesAsync();
                    return Partial("_PopupNew", sCxItem);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetEdit() - Edit Handler (data entry)
        ///     Edit Handler. GET End point for '/Index?hanler=edit'
        ///     
        /// </summary>
        /// <remarks>
        /// 
        ///     This handler is invoked by clicking a row in Index.cshtml.
        ///     
        ///     This handler then returns the modal _PopupSearch.cshtml which 
        ///     displays the details of the document clicked on, for user amendment.
        ///     
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="edit" asp-route-id="@item.Id" method="get">
        ///         <input type="hidden" />
        ///         <input id="modal" class="edit" type="submit" value="edit" data-toggle="modal" data-target="#popup-modal" data-url="@Url.Page('Index', 'edit')" />
        ///     </form>
        /// </example>
        /// <param name="id">Document Id</param>
        public async Task<IActionResult> OnGetEditAsync(Guid? id)
        {
            if (!ModelState.IsValid) return BadRequest();
            else if (!_context.SCxItems.Any(e => e.Id == id)) return NotFound();
            else
            {
                sCxItem = await _context.SCxItems.FirstOrDefaultAsync(m => m.Id == id);
                if (sCxItem is null) return NotFound();
                else
                {
                    return Partial("_PopupEdit", sCxItem);
                }
            }
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetNextAsync() - Get Nex/Prev Document
        ///     Next Handler. GET End point for '/Index?hanler=next'
        ///     
        /// </summary>
        /// <remarks>
        /// 
        ///     This handler is invoked by the Previous or Next document button in _popupEdit.cshtml.
        ///     It uses the cmd parameter to set sCxItem to the next or previous document index of sCxItems. 
        ///     
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="next" asp-route-id="@item.Id" asp-route-cmd="next" method="get">
        ///         <input type="hidden" />
        ///         <input id="GetNewModalContent" type="submit" value=">"  />
        ///     </form>
        /// </example>
        /// <param name="id">Document Id</param>
        /// <param name="cmd">Next or Prev Document</param>
        public async Task<IActionResult> OnGetNextAsync(Guid id, string cmd)
        {
            if (!ModelState.IsValid) return BadRequest();
            else if (!_context.SCxItems.Any(e => e.Id == id)) return NotFound();
            else
            {
                sCxItem = _context.SCxItems.FirstOrDefault(m => m.Id == id);
                var AllsCxItems = await _context.SCxItems.OrderByDescending(item => item.TimeStamp).ToListAsync();
                if (AllsCxItems.Any())
                {
                    if (cmd is not null && cmd != string.Empty)
                    {
                        var sCxIndex = AllsCxItems.IndexOf(sCxItem);
                        if (cmd == "prev")
                        {
                            if (sCxIndex == 0) sCxIndex = AllsCxItems.Count - 1;
                            else --sCxIndex;
                        }
                        else
                        {
                            if (sCxIndex + 1 >= AllsCxItems.Count) sCxIndex = 0;
                            else ++sCxIndex;
                        }
                        sCxItem = AllsCxItems.ElementAt(sCxIndex);
                    }
                }
                return Partial("_PopupEdit", sCxItem);
            }
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetSaveAsync() - Update Handler (database update)
        ///     Edit handler - POST end point for '/Index?hanler=Edit'
        /// 
        /// </summary>
        /// <remarks>
        /// 
        ///     The user clicks a row in the view to invoke a JavaScript function which 
        ///     loads a popup (_popupEdit.cshtml) and prompts the user to Amend the document. 
        ///     
        ///     the popup specifies a Save button with the method type set to 'get'
        ///     When clicked, JavaScript invokes this handler, passing the Model (sCxItem) as 'data:'.
        /// 
        ///     This handler then updates the changed document data in SCxItems and returns 
        ///     the same document back to the JavaScript as a partialView.
        /// 
        /// </remarks>
        /// <example>
        ///     <form asp-page-handler="save" asp-route-id="@item" method="get">
        ///         <input type="hidden" />
        ///         <input id="GetNewModalContent" type="submit" value="Save" />
        ///     </form>
        /// </example>
        public async Task<IActionResult> OnGetSaveAsync()
        {
            if (!ModelState.IsValid) return BadRequest();
            else if (!_context.SCxItems.Any(e => e.Id == sCxItem.Id)) return NotFound();
            else
            {
                _context.Attach(sCxItem).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
            }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return Partial("_popupEdit", sCxItem);
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetDeleteAsync() - Document Delete Handler (database update)
        ///     Delete handler - GET end point for '/Index?hanler=Delete'
        /// 
        /// </summary>
        /// <remarks>
        /// 
        ///     The user clicks a row in the view to invoke a JavaScript function which loads a popup 
        ///     (_popupEdit.cshtml) prompts the user to enter Amend a document. 
        ///     
        ///     the popup specifies a Delete button with the method type set to 'get'
        ///     and the asp-page-handler set to 'delete'. When clicked this OnGet'Delete'Async handler
        ///     is invoked (injects 'Delete' into OnGetAsync).
        /// 
        ///     This method then deletes the current document data in SCxItems, and returns the 
        ///     next record to the modal as a partialView for display.
        /// 
        /// </remarks>
        /// <example>
        ///     <button asp-page-handler="delete" asp-route-id="@Model.Id" method="get">
        ///         <input id="GetNewModalContent" type="submit" value="delete" />
        ///     </button>
        /// </example>
        /// <param name="id">Document Id</param>
        public async Task<IActionResult> OnGetDeleteAsync(Guid id)
        {
            if (!ModelState.IsValid)  return BadRequest();
            else if (!_context.SCxItems.Any(e => e.Id == id)) return NotFound();
            else
            {
                sCxItem = _context.SCxItems.FirstOrDefault(m => m.Id == id);
                _context.Attach(sCxItem).State = EntityState.Deleted;

                sCxItems = await _context.SCxItems.OrderByDescending(item => item.TimeStamp).ToListAsync();
                var sCxIndex = sCxItems.IndexOf(sCxItem);
                if (sCxIndex + 1 >= sCxItems.Count) sCxIndex = 0;
                //else ++sCxIndex;

                _context.SCxItems.Remove(sCxItem);
                await _context.SaveChangesAsync();

                sCxItems = await _context.SCxItems.OrderByDescending(item => item.TimeStamp).ToListAsync();
                sCxItem = sCxItems.ElementAt(sCxIndex);
            }
            return Partial("_PopupEdit", sCxItem);
        }

        /// <summary>
        /// 
        ///     DbWebAPI.IndexModel.OnGetCopyAsync() - Copy Document Handler (database update)
        ///     Copy handler - GET end point for '/Index?hanler=Copy'
        /// 
        /// </summary>
        /// <remarks>
        /// 
        ///     The view uses a Edit button to invoke a JavaScript function which loads a popup
        ///     (_popupEdit.cshtml) and prompts the user to enter Amend a document. 
        ///     
        ///     the popup specifies a Copy button with the method type set to 'get'
        ///     and the asp-page-handler set to 'copy'. When clicked this OnGet'Copy'Async handler
        ///     is invoked (injects 'Copy' into OnGetAsync).
        /// 
        ///     This method then copies the document data with a new Guid and TimeStamp  
        ///     and creates a new document in SCxItems. The new document partial view is 
        ///     returned to the modal for display.
        /// 
        /// </remarks>
        /// <example>
        ///     <button asp-page-handler="copy" method="get" asp-route-id="@Model.Id" >
        ///         <input type="hidden" />
        ///         <input id="GetNewModalContent" value="copy" />
        ///     </button>
        /// </example>
        /// <param name="id">Document Id</param>
        public async Task<IActionResult> OnGetCopyAsync(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest();
            else if (!_context.SCxItems.Any(e => e.Id == id)) return NotFound();
            else
            {
                sCxItem = _context.SCxItems.FirstOrDefault(m => m.Id == id);
                sCxItem.Id = Guid.NewGuid();
                sCxItem.TimeStamp = DateTime.Now;
                _context.SCxItems.Add(sCxItem);
                await _context.SaveChangesAsync();
                sCxItems = await _context.SCxItems.OrderByDescending(item => item.TimeStamp).ToListAsync();
            }
            return Partial("_PopupEdit", sCxItem);
        }
    }
}

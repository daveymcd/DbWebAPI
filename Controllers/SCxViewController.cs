using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbWebAPI.Models;
using System.Net.Mime;

namespace DbWebAPI.Controllers
{
    /// <summary>
    /// 
    ///     DbWebAPI.Controllers.SCxViewController - MVC View Controller
    /// 
    ///     This controller exposes the View pages CRUD endpoints for SCx Document Views. 
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
    /// </remarks>>
    [Route("Home/[controller]/[action]")]           // Attribute routing
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]         // Hide View endpoints from swagger
    public class SCxViewController : Controller
    {
        private readonly SCxItemContext _context;

        /// <summary>
        ///     This method gets called by the runtime and configures the Database context.
        /// </summary>
        public SCxViewController(SCxItemContext context)
        {
            _context = context;
            if (!_context.SCxItems.Any())
            {                                       // If no data - setup test data for the last 31 days.
                foreach (SCxItem sCxItem in SCxItem.AddThisMonthsSCxData(0)) { _context.SCxItems.Add(sCxItem); }
                _context.SaveChangesAsync();
            }
        }

        //*** VIEW PROCESSING... ***//

        /// <summary>
        ///     GET: SCxItems - Get all. 
        ///     This method returns all SCx Document data to the Index View.
        /// </summary>
        //[Route("/SCxView")]
        //[Route("~/SCxView/Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.SCxItems.OrderByDescending(item => item.TimeStamp).ToListAsync());
        }

        /// <summary>
        ///     GET: SCxItems/Details/5 - Get by Id.
        ///     This method returns a single SCx Document to the View.
        /// </summary>
        /// <param name="id">Document Id</param>
        //[Route("~/SCxView/Details")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (!_context.SCxItems.Any()) {return NotFound("NotFound:Details - No Data.");}
            else if (id == Guid.Empty) {return View(await _context.SCxItems.FirstOrDefaultAsync());}
            else {return View(await _context.SCxItems.FirstOrDefaultAsync(m => m.Id == id));}
        }

        //[Route("~/SCxView/Create")]
        /// <summary>
        ///     GET: SCxItems/Create - Create new Document.
        ///     This method returns an initialised SCx Document shell to Create View
        /// </summary>
        public IActionResult Create()
        {
            var sCxItem = new SCxItem { };
            sCxItem.Initialise(sCxItem);
            return View(sCxItem);
        }

        /// <summary>
        ///     POST: SCxItems/Create - Create new Document.
        ///     This method creates a new SCx Document.
        /// </summary>
        /// <param name="sCxItem">Document</param>
        [HttpPost, ActionName("CreateConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed([FromForm, Bind("Id,TimeStamp,Type,Dept,Food,Supplier,CheckUBD,Temperature,Comment,SignOff")] SCxItem sCxItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (SCxItemExists(sCxItem.Id)) { return BadRequest("BadRequest:CreateConfirmed - id Already Exists."); }
                    else
                    {
                        _context.Add(sCxItem);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sCxItem);
        }

        /// <summary>
        ///     GET: SCxItems/Edit/5 - Update Document.
        ///     This method returns the Document to the Edit View.
        /// </summary>
        /// <param name="id">Document Id</param>
        //[Route("~/SCxView/Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!_context.SCxItems.Any()) { return NotFound("NotFound:Edit - No Data."); }
            else if (id == Guid.Empty) { return View(await _context.SCxItems.FirstOrDefaultAsync()); }
            else { return View(await _context.SCxItems.FindAsync(id)); } 
        }

        /// <summary>
        ///     POST: SCxItems/EditUpdate/5 - Update Document.
        ///     This method updates a single SCx Document.
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <param name="sCxItem">Document</param>
        [HttpPost, ActionName("EditUpdate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUpdate(Guid id, [FromForm, Bind("Id,TimeStamp,Type,Dept,Food,Supplier,CheckUBD,Temperature,Comment,SignOff")] SCxItem sCxItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id != sCxItem.Id) { return BadRequest("BadRequest:EditUpdate - id Mismatch."); }
                    else if (!SCxItemExists(sCxItem.Id)) { return NotFound("NotFound:EditUpdate - id Not Found."); }
                    else
                    {
                        _context.Update(sCxItem);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sCxItem);
        }


        //[Route("~/SCxView/Delete")]
        /// <summary>
        ///     GET: SCxItems/Delete/5 - Delete Document.
        ///     This method returns the Document to the Delete View.
        /// </summary>
        /// <param name="id">Document Id</param>
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty) { return BadRequest("BadRequest:Delete - id Missing."); }
            else if (!SCxItemExists(id)) { return NotFound("NotFound:Delete - id Not Found."); }
            else { return View(await _context.SCxItems.FirstOrDefaultAsync(m => m.Id == id)); }
        }

        /// <summary>
        ///     POST: SCxItems/DeleteConfirmed/5 - Delete Document
        ///     This method deletes a single SCx Document.
        /// </summary>
        /// <param name="id">Document Id</param>
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == Guid.Empty) { return BadRequest("BadRequest:DeleteConfirmed - id Missing."); }
                    else if (!SCxItemExists(id)) { return NotFound("NotFound:DeleteConfirmed - id Not Found."); }
                    else
                    {
                        _context.SCxItems.Remove(await _context.SCxItems.FindAsync(id));
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // Check by id if Document exists
        private bool SCxItemExists(Guid id)
        {
            return _context.SCxItems.Any(e => e.Id == id);
        }

    }
}

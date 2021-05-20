using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DbWebAPI.Models;

namespace DbWebAPI
{
    public class EditModel : PageModel
    {
        private readonly DbWebAPI.Models.SCxItemContext _context;

        public EditModel(DbWebAPI.Models.SCxItemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SCxItem sCxItem { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id is null) { return NotFound(); }
            else if (!_context.SCxItems.Any(e => e.Id == id)) { return BadRequest(); }
            else
            {
                sCxItem = await _context.SCxItems.FirstOrDefaultAsync(m => m.Id == id);
                if (sCxItem is null) { return NotFound(); }
                else { return Page(); }
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(sCxItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SCxItemExists(sCxItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SCxItemExists(Guid id)
        {
            return _context.SCxItems.Any(e => e.Id == id);
        }
    }
}

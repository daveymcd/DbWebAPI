using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DbWebAPI.Models;

namespace DbWebAPI
{
    public class DeleteModel : PageModel
    {
        private readonly DbWebAPI.Models.SCxItemContext _context;

        public DeleteModel(DbWebAPI.Models.SCxItemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SCxItem sCxItem { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            sCxItem = await _context.SCxItems.FirstOrDefaultAsync(m => m.Id == id);

            if (sCxItem == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            sCxItem = await _context.SCxItems.FindAsync(id);

            if (sCxItem != null)
            {
                _context.SCxItems.Remove(sCxItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

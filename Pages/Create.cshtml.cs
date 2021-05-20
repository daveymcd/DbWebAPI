using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DbWebAPI.Models;

namespace DbWebAPI
{
    public class CreateModel : PageModel
    {
        private readonly DbWebAPI.Models.SCxItemContext _context;
        
        public CreateModel(DbWebAPI.Models.SCxItemContext context)
        {
            _context = context;
        }


        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty(SupportsGet = true)]
        public SCxItem sCxItem { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {   //[FromForm, Bind("sCxItem.Id,sCxItem.TimeStamp,sCxItem.Type,sCxItem.Dept,sCxItem.Food,sCxItem.Supplier,sCxItem.CheckUBD,sCxItem.Temperature,sCxItem.Comment,sCxItem.SignOff")] SCxItem? sCxItem
            //sCxItem.Type = Request.Form["sCxItem.Type"].ToString();
            //sCxItem.Dept = Request.Form["Dept"].ToString();
            //sCxItem.Food = Request.Form["sCxItem.Food"].ToString();
            //sCxItem.Supplier = Request.Form["sCxItem.Supplier"];
            //sCxItem.CheckUBD = Convert.ToInt32(Request.Form["sCxItem.CheckUBD"]);
            //sCxItem.Temperature = Convert.ToInt32(Request.Form["sCxItem.Temperature"]);
            //sCxItem.Comment = Request.Form["sCxItem.Comment"].ToString();
            //sCxItem.SignOff = Request.Form["sCxItem.SignOff"].ToString();

            if (!ModelState.IsValid) { return Page(); }
            else
            {
                _context.SCxItems.Add(sCxItem);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
        }
    }
}

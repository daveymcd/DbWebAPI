/// <summary>
/// 
///     DbWebAPI.Models._Search - SCx Documents Search - Partial view
///     
///     Modal popup body used to gather the search criteria used by Index to display a subset of 
///     SCx Documents. 
///     
/// </summary>
/// <remarks>
/// 
///     This partial view is loaded into the (aria-hidden) 'Modal-Body' section of Index.cshtml by a 
///     JavaScipt wired to the search button. The Modal has a Master Detail relationship with Index.cshtml. 
///     When the Search button is clicked, the popup appears, the search criteria is entered and 
///     used to fetch a subset of documents into SCxItems, which is displayed by Index.cshtml.
///     
/// </remarks>>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DbWebAPI.Pages
{
    public class _SearchModel : PageModel
    {
        public async Task<PartialViewResult> OnGetAsync()
        {
            return Partial("_PopupSearch");
        }
    }
}

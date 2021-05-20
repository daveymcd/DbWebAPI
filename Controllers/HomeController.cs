using DbWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Controllers
{
    /// <summary>
    /// 
    ///     DbWebAPI.Controllers.HomeController - http://localhost:5001;http://localhost:5001/Home
    ///     
    ///     Home Page. Allows exploration of DbWebApi services via Swagger/Views/Pages etc.
    ///     
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]         // Hide Home page endpoint from swagger
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        /// <summary>Home View Logging</summary>
        /// <param name="logger"></param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>API Landing Page</summary>
        [Route("~/")]
        [Route("/Home")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>API Privacy Page</summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>API Error Page</summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using fyp.Models;
using Microsoft.EntityFrameworkCore;


//add the required namespaces regarding EF Core and models

namespace fyp.Controllers
{
    public class HomeController : Controller
    {
        //add dependency injection to this controller
        private AppDbContext _dbContext;
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Report()
        {
           

            return View();
        }
    }
}
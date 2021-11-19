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

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Report()
        {
            // uncomment code below to pass the list of mug orders
            // , shirt orders and users to the SalesReport view

            //DbSet<MugOrder> dbsMug = _dbContext.MugOrder;
            //DbSet<ShirtOrder> dbsShirt = _dbContext.ShirtOrder;
            //DbSet<AppUser> dbsUser = _dbContext.AppUser;

            //ViewData["mugOrders"] = dbsMug.ToList<MugOrder>();
            //ViewData["shirtOrders"] = dbsShirt.ToList<ShirtOrder>();
            //ViewData["users"] = dbsUser.ToList<AppUser>();

            return View();
        }
    }
}
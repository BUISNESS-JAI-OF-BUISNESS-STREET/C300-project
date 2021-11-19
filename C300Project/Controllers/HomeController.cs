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

            DbSet<Question> dbsQues= _dbContext.Question;
            DbSet<Result> dbsRes = _dbContext.Result;
            DbSet<Account> dbsAcc = _dbContext.Account;

            ViewData["questions"] = dbsQues.ToList<Question>();
            ViewData["results"] = dbsRes.ToList<Result>();
            ViewData["account"] = dbsAcc.ToList<Account>();

            return View();
        }
    }
}
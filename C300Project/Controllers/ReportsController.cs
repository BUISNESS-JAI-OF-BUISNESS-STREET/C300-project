using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using fyp.Models;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace fyp.Controllers.Controllers
{

    public class ReportsController : Controller
    {
        private AppDbContext _dbContext;

        public ReportsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.ToList();
            return View(model);
        }

        
        [AllowAnonymous]
        public IActionResult ViewReport(int id)
        {
            DbSet<Result> dbs = _dbContext.Result;
            List<Result> model = dbs.ToList();

            var viewer = model
                        .Where(m => m.QuizId == id)
                        .ToList<Result>();
            return View(model);
        }


        
    }
}
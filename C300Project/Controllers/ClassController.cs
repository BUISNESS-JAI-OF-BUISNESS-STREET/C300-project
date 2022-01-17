using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fyp.Models;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace fyp.Controllers
{
    public class ClassController : Controller
    {
        private AppDbContext _dbContext;
        public ClassController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Main page that will show teachers and their details
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Class> dbs = _dbContext.Class;
            //List<Class> model = dbs.Include(m => m.AddedByNavigation).ToList();

            return View();
        }
    }
}


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

namespace fyp.Controllers
{
    public class QuestionsController : Controller
    {
        private AppDbContext _dbContext;

        public QuestionsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            return View(model);
        }
    }
}

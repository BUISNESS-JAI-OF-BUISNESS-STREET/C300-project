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
    public class QuizController : Controller
    {
        private AppDbContext _dbContext;

        public QuizController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.ToList();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.IsInRole("Admin"))
                model = model.Where(so => so.CreatedBy == userId).ToList();
            return View(model);
        }
    }
}

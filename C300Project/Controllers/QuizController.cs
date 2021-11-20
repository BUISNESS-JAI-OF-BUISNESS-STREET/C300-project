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

        [AllowAnonymous]
        public IActionResult Index()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.ToList();
            return View(model);
        }

        /*
        [Authorize]
        public IActionResult Create() //for users to attempt the quiz
        {
            //TODO: require attention

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(ShirtOrder shirtOrder)
        {
            //TODO: require attention

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            //TODO: require attention
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(ShirtOrder shirtOrder)
        {
            //TODO: require attention
            return RedirectToAction("Index");
        }
        */

    }
}

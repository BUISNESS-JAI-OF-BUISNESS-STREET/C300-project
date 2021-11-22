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

        
        [AllowAnonymous]
        public IActionResult Attempt() //for users to attempt the quiz
        {
            //TODO: require attention
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Attempt(Result result)
        {
            //TODO: require attention
            if (ModelState.IsValid)
            {
                DbSet<Result> dbs = _dbContext.Result;
                dbs.Add(result);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "Quiz Attempted";
                else
                    TempData["Msg"] = "Attempt failed!";
            }
            else
            {
                TempData["Msg"] = "Other error has occured";
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult CalculateResult() //for users to attempt the quiz
        {
            //TODO: require attention
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            return View(model);
        }

        /*
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

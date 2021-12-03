using fyp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

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


        [Authorize(Roles = "User")]
        public IActionResult Attempt(int id) //for users to attempt the quiz
        {
            //TODO: require attention
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            DbSet<Quiz> dbs2 = _dbContext.Quiz;
            List<Quiz> model2 = dbs2.ToList();
            ViewData["quizid"] = id;
            ViewData["title"] = model2[id].Title;
            ViewData["topic"] = model2[id].Topic;
            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Attempt(IFormCollection form)
        {
            int quizid = Int32.Parse(form["QuizId"].ToString());
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            var quizCount = model.Count();
            var y = 0;
            
            for (var x = 0; x < quizCount; x++ )
            {
                var z = x + 1;
                var answer = form["Answer" +z];
                var corrAns = model[x].CorrectAns;
                if (answer == corrAns)
                    y++;
                else
                    y+= 0;
            }

            Result newResult = new Result();
            newResult.QuizId = quizid;
            newResult.Score = y;
            newResult.AccountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            newResult.Name = "test";
            newResult.Title = "test";
            newResult.Topic = "test";
            newResult.Attempt = true;
            newResult.Dt = DateTime.Now;


            //TODO: require attention
            if (ModelState.IsValid)
            {

                DbSet<Result> dbsresult = _dbContext.Result;
                dbsresult.Add(newResult);

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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                DbSet<Quiz> dbs = _dbContext.Quiz;
                dbs.Add(quiz);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "New quiz added!";
                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }


        [Authorize]
        public IActionResult Update(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            Quiz quiz = dbs.Where(mo => mo.QuizId == id).FirstOrDefault();


            if (quiz != null)
            {
                DbSet<Quiz> dbsQuiz = _dbContext.Quiz;
                var lstQuiz = dbsQuiz.ToList();
                ViewData["quiz"] = new SelectList(lstQuiz, "QuizId", "Title");

                return View(quiz);
            }
            else
            {
                TempData["Msg"] = "Quiz not found!";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                DbSet<Quiz> dbs = _dbContext.Quiz;
                Quiz tOrder = dbs.Where(mo => mo.QuizId == quiz.QuizId).FirstOrDefault();

                if (tOrder != null)
                {
                    tOrder.Title = quiz.Title;
                    tOrder.Topic = quiz.Topic;
                    tOrder.Sec = quiz.Sec;
                    tOrder.Dt = quiz.Dt;


                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Quiz updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Quiz not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }


        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;

            Quiz tOrder = dbs.Where(mo => mo.QuizId == id)
                                     .FirstOrDefault();

            if (tOrder != null)
            {
                dbs.Remove(tOrder);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "Quiz deleted!";
                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Quiz not found!";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ViewQuestions(/*int id*/) //for users to attempt the quiz
        {
            //TODO: require attention
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            return View(model);
        }
    }
}

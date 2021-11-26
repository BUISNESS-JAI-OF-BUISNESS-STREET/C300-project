﻿using System;
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


        [Authorize(Roles = "User")]
        public IActionResult Attempt() //for users to attempt the quiz
        {
            //TODO: require attention
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> model = dbs.ToList();
            return View(model);
        }

        [Authorize(Roles = "User")]
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
                _dbContext.Quiz.Add(quiz);
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

        /*
        [Authorize]
        public IActionResult Update(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            Quiz quiz = dbs.Where(mo => mo.QuizId == id).FirstOrDefault();
            

            if (quiz != null)
            {
                DbSet<Pokedex> dbsPokes = _dbContext.Pokedex;
                var lstPokes = dbsPokes.ToList();
                ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");

                return View(quiz);
            }
            else
            {
                TempData["Msg"] = "Shirt order not found!";
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
                    tOrder.= quiz.Name;
                    tOrder.Color = quiz.Color;
                    tOrder.PokedexId = quiz.PokedexId;
                    tOrder.Qty = quiz.Qty;
                    tOrder.Price = quiz.Price;
                    tOrder.FrontPosition = quiz.FrontPosition;

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
        }*/


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
        

    }
}

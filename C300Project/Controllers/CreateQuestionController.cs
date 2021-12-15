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

//
    public class CreateQuestionsController : Controller
    {
        private AppDbContext _dbContext;

        public CreateQuestionsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    public IActionResult Index()
    {
        DbSet<Question> dbs = _dbContext.Question;
        List<Question> model = dbs.ToList();
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    //To create question 
    [HttpPost]
    [Authorize(Roles = "Admin")]
    
    public IActionResult Create(Question question)
    {
        if (ModelState.IsValid)
        {
            DbSet<Question> dbs = _dbContext.Question;
            dbs.Add(question);
            if (_dbContext.SaveChanges() == 1)
                TempData["Msg"] = "New quiz question is added!";
            else
                TempData["Msg"] = "The quiz question has been failed update database!";
        }
        else
        {
            TempData["Msg"] = "No quiz question is being entered in the database";
        }
        return RedirectToAction("Index");
    }

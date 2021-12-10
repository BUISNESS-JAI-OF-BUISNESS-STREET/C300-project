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
            if (User.IsInRole("Admin"))
                model = dbs.Include(mo => mo.UserCodeNavigation)
                            .ToList();
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = dbs.Where(so => so.UserCode == userId).ToList();
            }
            return View(model);

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            var lstQuiz = dbs.ToList();
            ViewData["Quiz"] = new SelectList(lstQuiz, "QuizId", "Title");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                DbSet<Question> dbs = _dbContext.Question;
                dbs.Add(question);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "New question added!";
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
            DbSet<Question> dbs = _dbContext.Question;
            Question question = dbs.Where(mo => mo.QuestionId == id).FirstOrDefault();


            if (question != null)
            {
                DbSet<Question> dbsQuestion = _dbContext.Question;
                var lstQuestion = dbsQuestion.ToList();
                ViewData["queston"] = new SelectList(lstQuestion, "QuestionId", "Questions");
                DbSet<Quiz> dbs2 = _dbContext.Quiz;
                var lstQuiz = dbs2.ToList();
                ViewData["Quiz"] = new SelectList(lstQuiz, "QuizId", "Title");


                return View(question);
            }
            else
            {
                TempData["Msg"] = "Question not found!";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(Question question)
        {
            if (ModelState.IsValid)
            {
                DbSet<Question> dbs = _dbContext.Question;
                Question tOrder = dbs.Where(mo => mo.QuestionId == question.QuestionId).FirstOrDefault();

                if (tOrder != null)
                {
                    tOrder.QuizId = question.QuizId;
                    tOrder.Questions = question.Questions;
                    tOrder.FirstOption = question.FirstOption;
                    tOrder.SecondOption = question.SecondOption;
                    tOrder.ThirdOption = question.ThirdOption;
                    tOrder.FourthOption = question.FourthOption;
                    tOrder.Topic = question.Topic;
                    tOrder.CorrectAns = question.CorrectAns;


                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Question updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Question not found!";
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
            DbSet<Question> dbs = _dbContext.Question;

            Question tOrder = dbs.Where(mo => mo.QuestionId== id)
                                     .FirstOrDefault();

            if (tOrder != null)
            {
                dbs.Remove(tOrder);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "Question deleted!";
                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Question not found!";
            }
            return RedirectToAction("Index");
        }
    }
}

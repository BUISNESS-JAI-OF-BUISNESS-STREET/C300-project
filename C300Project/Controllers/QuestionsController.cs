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
using Microsoft.AspNetCore.Http;

namespace fyp.Controllers
{
    public class QuestionsController : Controller
    {
        private AppDbContext _dbContext;

        public QuestionsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Index Action
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
        #endregion

        #region Create Get Action
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            
            DbSet<Quiz> dbs = _dbContext.Quiz;
            var lstQuiz = dbs.ToList();
            ViewData["Quiz"] = new SelectList(lstQuiz, "QuizId", "Title");
            ViewData["Test"] = lstQuiz;

            
            DbSet<Segment> dbs2 = _dbContext.Segment;
            var lstSegment = dbs2.ToList();
            ViewData["Segment"] = new SelectList(lstSegment, "SegmentId", "Name");
            return View();
        }
        #endregion

        #region Create Post Action
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Question question, IFormCollection form)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<Quiz> dbs2 = _dbContext.Quiz;
            DbSet<QuizQuestionBindDb> dbs3= _dbContext.QuizQuestionBindDb;
            
            
            
            List<Quiz> lstQuiz = dbs2.ToList<Quiz>();
            var dbcount = lstQuiz.Count();
            question.UserCode = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {   
                
                dbs.Add(question);
                _dbContext.SaveChanges();
                for (var x = 0; x < dbcount; x++)
                    {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];
                        
                    if (radiocheck.Equals("True"))
                    {
                        if (ModelState.IsValid)
                            {
                            quizQuestionBind.QuestionId = question.QuestionId;
                            quizQuestionBind.QuizId = y;
                            dbs3.Add(quizQuestionBind);
                            
                            }
                    }
                        
                    }
                if (_dbContext.SaveChanges() >= 1)
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
        #endregion

        #region Update Get Action
        [Authorize]
        public IActionResult Update(int id)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            DbSet<Quiz> dbs3 = _dbContext.Quiz;
            DbSet<Segment> dbs4 = _dbContext.Segment;

            List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == id).ToList();
            List<Question> lstdb = dbs.ToList();
            var lstSegment = dbs4.ToList();

            List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
            List<int> lstdb3 = dbs3.Select(mo => mo.QuizId).ToList();
            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();
            
            Question question = dbs.Where(mo => mo.QuestionId == id).FirstOrDefault();

            
            

            if (question != null)
            {
                DbSet<Quiz> dbsQuiz = _dbContext.Quiz;
                var lstQuiz = dbsQuiz.ToList();
                ViewData["Segment"] = new SelectList(lstSegment, "SegmentId", "Name");
                ViewData["Test"] = lstQuiz;
                ViewData["common"] = commonlist;
                return View(question);
            }
            else
            {
                TempData["Msg"] = "Question not found!";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Update Post Action
        [Authorize]
        [HttpPost]
        public IActionResult Update(Question question, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DbSet<Question> dbs = _dbContext.Question;
                DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
                DbSet<Quiz> dbs3 = _dbContext.Quiz;

                Question tOrder = dbs.Where(mo => mo.QuestionId == question.QuestionId).FirstOrDefault();

                
                List<Quiz> lstQuiz = dbs3.ToList<Quiz>();
                List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == question.QuestionId).ToList();
                var dbcount = lstQuiz.Count();

                List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
                List<int> lstdb3 = dbs2.Select(mo => mo.QuizId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if(commonlist.Contains(y) && radiocheck.Equals("False"))
                    {
                        dbs2.Remove(dbs2.Where(mo =>mo.QuestionId == question.QuestionId).Where(mo => mo.QuizId == y).FirstOrDefault());
                        _dbContext.SaveChanges();
                    }else if (commonlist.Contains(y) && radiocheck.Equals("True"))
                    {
                        continue;
                    }
                    else if (radiocheck.Equals("True"))
                    {
                        if (ModelState.IsValid)
                        {
                            quizQuestionBind.QuestionId = question.QuestionId;
                            quizQuestionBind.QuizId = y;
                            dbs2.Add(quizQuestionBind);

                        }
                    }

                }

                if (tOrder != null)
                {
                    tOrder.Questions = question.Questions;
                    tOrder.FirstOption = question.FirstOption;
                    tOrder.SecondOption = question.SecondOption;
                    tOrder.ThirdOption = question.ThirdOption;
                    tOrder.FourthOption = question.FourthOption;
                    tOrder.Topic = question.Topic;
                    tOrder.CorrectAns = question.CorrectAns;

                    if (_dbContext.SaveChanges() >= 1)
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
        #endregion

        #region Delete Action
        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;

            dbs2.RemoveRange(dbs2.Where(mo => mo.QuestionId == id).ToList());
            _dbContext.SaveChanges();

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
        #endregion
    }
}

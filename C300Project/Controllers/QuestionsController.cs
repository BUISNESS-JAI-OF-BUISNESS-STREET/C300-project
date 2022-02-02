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
using System.Web;

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
            List<Question> model = dbs
                .Include(mo => mo.SegmentNavigation)
                .Include(mo => mo.TopicNavigation)
                .ToList();

            if (User.IsInRole("Admin"))
                model = dbs.Include(mo => mo.UserCodeNavigation)
                            .ToList();
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = dbs.Where(so => so.UserCode == userId).ToList();
            }

            DbSet<Topic> dbs3 = _dbContext.Topic;
            var lstTopic = dbs3.ToList();
            ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");

            return View(model);

        }
        #endregion

        #region Create Get Action
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            
            DbSet<Quiz> dbs = _dbContext.Quiz;
            var lstQuiz = dbs
                .Include(mo => mo.TopicNavigation)
                .ToList();
            
            DbSet<Segment> dbs2 = _dbContext.Segment;
            var lstSegment = dbs2.ToList();
            ViewData["Segment"] = new SelectList(lstSegment, "SegmentId", "Name");

            DbSet<Topic> dbs3 = _dbContext.Topic;
            var lstTopic = dbs3.ToList();
            ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");
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

            List<Quiz> lstQuiz = dbs2.ToList();
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
                    var radiocheck = Convert.ToInt32(form["Add" + y]);
                        
                    if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                            {
                            quizQuestionBind.QuestionId = question.QuestionId;
                            quizQuestionBind.QuizId = radiocheck;
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
            DbSet<Topic> dbs5 = _dbContext.Topic;

            var lstTopic = dbs5.ToList();
            

            List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == id).ToList();
            List<Question> lstdb = dbs.ToList();
            var lstSegment = dbs4.ToList();

            List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
            List<int> lstdb3 = dbs3.Select(mo => mo.QuizId).ToList();
            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();
            
            Question question = dbs
                .Where(mo => mo.QuestionId == id)
                .Include(mo => mo.SegmentNavigation)
                .Include(mo => mo.TopicNavigation).FirstOrDefault();

            
            

            if (question != null)
            {
                DbSet<Quiz> dbsQuiz = _dbContext.Quiz;
                var lstQuiz = dbsQuiz.ToList();
                ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");
                ViewData["Segment"] = new SelectList(lstSegment, "SegmentId", "Name");
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

                
                List<Quiz> lstQuiz = dbs3.ToList();
                List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == question.QuestionId).ToList();
                var dbcount = lstQuiz.Count();

                List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
                List<int> lstdb3 = dbs2.Select(mo => mo.QuizId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = Convert.ToInt32(form["Add" + y]);

                    if(commonlist.Contains(y) && radiocheck == -1)
                    {
                        dbs2.Remove(dbs2.Where(mo =>mo.QuestionId == question.QuestionId && mo.QuizId == radiocheck).FirstOrDefault());
                    }else if (commonlist.Contains(y) && radiocheck > 0)
                    {
                        continue;
                    }
                    else if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            quizQuestionBind.QuestionId = question.QuestionId;
                            quizQuestionBind.QuizId = radiocheck;
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
                    tOrder.Segment = question.Segment;
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

        #region GetQtns
        public IActionResult GetQtns(int moduleId)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<Topic> dbs2 = _dbContext.Topic;

            List<Question> question;

            if(moduleId == -1)
            {
                question = dbs
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.SegmentNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                question = dbs
                    .Where(q => q.Topic == moduleId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.SegmentNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QtnTable", question);
        }

        #endregion

        #region GetAddQtns
        public IActionResult GetAddQtns(int moduleId)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<Topic> dbs2 = _dbContext.Topic;

            List<Question> question;

            if (moduleId == -1)
            {
                question = dbs
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.SegmentNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                question = dbs
                    .Where(q => q.Topic == moduleId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.SegmentNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QtnTableAdd", question);
        }

        #endregion

        #region GetQtnTableUpdate
        public IActionResult GetQuizTableUpdate(int questionId, int quizId)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<Topic> dbs2 = _dbContext.Topic;
            DbSet<Question> dbs3 = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs4 = _dbContext.QuizQuestionBindDb;
            DbSet<Segment> dbs5 = _dbContext.Segment;
            DbSet<Topic> dbs6 = _dbContext.Topic;

            var lstTopic = dbs5.ToList();


            List<QuizQuestionBindDb> lstdb4 = dbs4.Where(mo => mo.QuizId == quizId).ToList();
            List<Quiz> lstdb = dbs.ToList();
            var lstSegment = dbs4.ToList();

            List<int> lstoverlap = lstdb4.Select(mo => mo.QuestionId).ToList();
            List<int> lstdb3 = dbs3.Select(mo => mo.QuestionId).ToList();
            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            ViewData["common"] = commonlist;

            List<Question> question;

            if (questionId == -1)
            {
                question = dbs3
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.SegmentNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                question = dbs3
                    .Where(q => q.Topic == questionId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.SegmentNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QtnTableUpdate", question);
        }

        #endregion
    }
}

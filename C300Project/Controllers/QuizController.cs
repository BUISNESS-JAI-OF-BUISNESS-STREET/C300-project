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
using System.Dynamic;
using System.Data.SqlClient;
using Rotativa.AspNetCore;

namespace fyp.Controllers
{
    public class QuizController : Controller
    {
        private AppDbContext _dbContext;

        public QuizController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Index Action
        [Authorize(Roles = "Student,Admin,Teacher,PartTimeTeacher")]
        public IActionResult Index()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.ToList();
            model = dbs.Include(mo => mo.UserCodeNavigation)
                        .ToList();

            DbSet<Topic> dbs3 = _dbContext.Topic;
            var lstTopic = dbs3.ToList();
            ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");

            return View(model);
        }
        #endregion

        #region Instruction Action
        [Authorize(Roles = "Student")]
        public IActionResult Instruction(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs
                .Where(mo => mo.QuizId == id)
                .Include(mo =>mo.TopicNavigation)
                .ToList();

         return View(model);
        }
        #endregion

        #region Attempt Get Action
        [Authorize(Roles = "Student")]
        public IActionResult Attempt(int id) 
        {
            DbSet<QuizQuestionBindDb> dbs = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> model = dbs.Where(mo => mo.QuizId == id).ToList();
            List<int> overlapList = model.Select(mo => mo.QuestionId).ToList();

            DbSet<Question> dbs3 = _dbContext.Question;
            List<Question> model3 = dbs3.ToList();
            List<int> overlappedquestno = model3.Select(mo => mo.QuestionId).ToList().Intersect(overlapList).ToList();
            List<Question> questionlist = dbs3.Where(r => overlappedquestno.Contains(r.QuestionId))
                .Include(mo => mo.TopicNavigation)
                .Include(mo => mo.SegmentNavigation)
                .ToList();

            DbSet<Quiz> dbs2 = _dbContext.Quiz;
            List<Quiz> model2 = dbs2.ToList();
            ViewData["quizid"] = id;
            var countdowntimer = model2.Where(o => o.QuizId == id).ToList();
            //ViewData["Timer"] = model2[id].Duration;


            if (model == null && model2 == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                
                return View(questionlist);
            }

        }
        #endregion

        #region Attempt Post Action
        [Authorize(Roles = "Student")]
        [HttpPost]
        public IActionResult Attempt(IFormCollection form)
        {
            int quizid = Int32.Parse(form["QuizId"].ToString());
            DbSet<QuizQuestionBindDb> dbs = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> model = dbs.Where(mo => mo.QuizId == quizid).ToList();
            List<int> overlapList = model.Select(mo => mo.QuestionId).ToList();

            DbSet<Question> dbs3 = _dbContext.Question;
            List<Question> model3 = dbs3.ToList();
            List<int> overlappedquestno = model3.Select(mo => mo.QuestionId).ToList().Intersect(overlapList).ToList();
            List<Question> questionlist = dbs3.Where(r => overlappedquestno.Contains(r.QuestionId)).ToList();
            var quizCount = model.Count();
            var y = 0;

            for (var x = 0; x < quizCount; x++)
            {
                var z = x + 1;
                var answer = form["Answer" + z];
                var corrAns = questionlist[x].CorrectAns;
                if (answer == corrAns)
                    y++;
                else
                    y += 0;
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
        #endregion

        #region Create Quiz Action
        [Authorize(Roles = "Admin, Teacher")]
        public IActionResult Create()
        {
            DbSet<Question> dbs = _dbContext.Question;
            var lstQuestion = dbs.ToList();
            ViewData["Test"] = lstQuestion;

            DbSet<Topic> dbs3 = _dbContext.Topic;
            var lstTopic = dbs3.ToList();
            ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");

            DbSet<Class> dbs5 = _dbContext.Class;
            var lstClass = dbs5.ToList();
            ViewData["listClass"] = lstClass;
            return View();
        }
        #endregion

        #region Create Quiz Post Action
        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        public IActionResult Create(Quiz quiz, IFormCollection form)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<Question> dbs2 = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs3 = _dbContext.QuizQuestionBindDb;
            DbSet<Class> dbs4 = _dbContext.Class;
            DbSet<QuizClassBindDb> dbs5 = _dbContext.QuizClassBindDb;


            List<Question> lstQuestion = dbs2.ToList();
            List<Class> lstClass = dbs4.ToList();
            

            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            quiz.UserCode = userId;
            quiz.Duration = 60;

            var dbcount = lstQuestion.Count();
            var dbcount2 = lstClass.Count();

            if (ModelState.IsValid)
            {

                dbs.Add(quiz);
                _dbContext.SaveChanges();
                
                #region Quiz Class Add
                for (var b = 0; b < dbcount2; b++)
                {

                    QuizClassBindDb quizClassBindDb = new QuizClassBindDb();
                    var a = b + 1;
                    var radiocheck = Convert.ToInt32(form["AddClass" + a]);

                    if (radiocheck.Equals("True"))
                    {
                        if (ModelState.IsValid)
                        {
                            quizClassBindDb.ClassId = radiocheck;
                            quizClassBindDb.QuizId = quiz.QuizId;
                            dbs5.Add(quizClassBindDb);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                #endregion

                #region Quiz Question Bind Add
                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = Convert.ToInt32(form["Add" + y]);

                    if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            quizQuestionBind.QuestionId = radiocheck;
                            quizQuestionBind.QuizId = quiz.QuizId;
                            dbs3.Add(quizQuestionBind);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                #endregion

                

                if (_dbContext.SaveChanges() >= 1)
                    TempData["Msg"] = "New quiz added!";
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }
        #endregion Create Quiz Post Action

        #region Update Quiz Action
        [Authorize]
        [HttpGet]
        public IActionResult Update(int id)
        {
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            DbSet<QuizClassBindDb> dbs4 = _dbContext.QuizClassBindDb;
            DbSet<Class> dbs5 = _dbContext.Class;
            DbSet<Quiz> dbs3 = _dbContext.Quiz;
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<Topic> dbs6 = _dbContext.Topic;
            var lstTopic = dbs6.ToList();
            

            List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuizId == id).ToList();
            List<QuizClassBindDb> lstdb4 = dbs4.Where(mo => mo.QuizId == id).ToList();

            List<int> lstoverlap = lstdb2.Select(mo => mo.QuestionId).ToList();
            List<int> lstdb3 = dbs3.Select(mo => mo.QuizId).ToList();
            List<int> lstdb = dbs.Select(mo => mo.QuestionId).ToList();

            List<int> lstoverlap2 = lstdb4.Select(mo => mo.ClassId).ToList();
            List<int> lstdb5 = dbs5.Select(mo => mo.ClassId).ToList();

            List<int> commonlist = lstoverlap.Intersect(lstdb).ToList();
            List<int> commonClasslist = lstoverlap2.Intersect(lstdb5).ToList();

            Quiz quiz = dbs3.Where(mo => mo.QuizId == id).FirstOrDefault();


            if (quiz != null)
            {
                var lstQuestion = dbs.ToList();
                var lstClass = dbs5.ToList();
                ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");
                ViewData["listClass"] = lstClass;
                ViewData["Test"] = lstQuestion;
                ViewData["common"] = commonlist;
                ViewData["commonClass"] = commonClasslist;


                return View(quiz);
            }
            else
            {
                TempData["Msg"] = "Quiz not found!";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Update Post Action
        [Authorize]
        [HttpPost]
        public IActionResult Update(Quiz quiz, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DbSet<Quiz> dbs = _dbContext.Quiz;
                DbSet<Question> dbs2 = _dbContext.Question;
                DbSet<QuizQuestionBindDb> dbs3 = _dbContext.QuizQuestionBindDb;
                DbSet<Class> dbs4 = _dbContext.Class;
                DbSet<QuizClassBindDb> dbs5 = _dbContext.QuizClassBindDb;

                Quiz tOrder = dbs.Where(mo => mo.QuizId == quiz.QuizId).FirstOrDefault();

                List<Question> lstQuestion = dbs2.ToList();
                List<QuizQuestionBindDb> lstdb2 = dbs3.Where(mo => mo.QuizId == quiz.QuizId).ToList();
                List<QuizClassBindDb> lstdb3 = dbs5.Where(mo => mo.QuizId == quiz.QuizId).ToList();
                List<Class> lstClass = dbs4.ToList();

                List<int> lstoverlap = lstdb2.Select(mo => mo.QuestionId).ToList();
                List<int> lstdb = dbs2.Select(mo => mo.QuestionId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb).ToList();

                List<int> lstoverlap2 = lstdb3.Select(mo => mo.ClassId).ToList();
                List<int> lstdb4 = dbs4.Select(mo => mo.ClassId).ToList();
                List<int> commonlist2 = lstoverlap2.Intersect(lstdb4).ToList();

                
                
                var dbcount = lstQuestion.Count();
                var dbcount2 = lstClass.Count();

                #region QuizQuestionBind Update Check Code
                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var questcheck = Convert.ToInt32(form["currAddId" + y]);
                    var radiocheck = Convert.ToInt32(form["Add" + y]);

                    if (commonlist.Contains(questcheck) && radiocheck == -1)
                    {
                        dbs3.Remove(dbs3.Where(mo => mo.QuestionId == radiocheck && mo.QuizId == quiz.QuizId).FirstOrDefault());
                        _dbContext.SaveChanges();
                    }
                    else if (commonlist.Contains(questcheck) && radiocheck > 0)
                    {
                        continue;
                    }
                    else if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            quizQuestionBind.QuestionId = radiocheck;
                            quizQuestionBind.QuizId = quiz.QuizId;
                            dbs3.Add(quizQuestionBind);

                        }
                    }

                }
                #endregion

                #region QuizClassBind Update Check Code
                for (var x = 0; x < dbcount2; x++)
                {

                    QuizClassBindDb quizClassBindDb = new QuizClassBindDb();
                    var y = x + 1;
                    var classcheck = Convert.ToInt32(form["currAddId" + y]);
                    var radiocheck = Convert.ToInt32(form["AddClass" + y]);

                    if (commonlist2.Contains(classcheck) && radiocheck == -1)
                    {
                        dbs5.Remove(dbs5.Where(mo => mo.ClassId == radiocheck && mo.QuizId == quiz.QuizId).FirstOrDefault());
                    }
                    else if (commonlist2.Contains(classcheck) && radiocheck > 0)
                    {
                        continue;
                    }
                    else if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            quizClassBindDb.ClassId = radiocheck;
                            quizClassBindDb.QuizId = quiz.QuizId;
                            dbs5.Add(quizClassBindDb);

                        }
                    }

                }
                #endregion

                if (tOrder != null)
                {
                    tOrder.Title = quiz.Title;
                    tOrder.Topic = quiz.Topic;
                    tOrder.Sec = quiz.Sec;
                    tOrder.StartDt = quiz.StartDt;
                    tOrder.EndDt = quiz.EndDt;


                    if (_dbContext.SaveChanges() >= 1)
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
        #endregion

        #region Delete Quiz
        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            DbSet<QuizClassBindDb> dbs3 = _dbContext.QuizClassBindDb;

            dbs2.RemoveRange(dbs2.Where(mo => mo.QuizId == id).ToList());
            dbs3.RemoveRange(dbs3.Where(mo => mo.QuizId == id).ToList());
            _dbContext.SaveChanges();

            

            Quiz tQuiz = dbs.Where(mo => mo.QuizId == id)
                                     .FirstOrDefault();

            if (tQuiz != null)
            {
                dbs.Remove(tQuiz);
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
        #endregion Delete Quiz

        #region View Questions Action
        [Authorize(Roles = "Admin,Teacher,PartTimeTeacher")]
        public IActionResult ViewQuestions(int id)
        {
            DbSet<QuizQuestionBindDb> dbs = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> model = dbs.Where(mo => mo.QuizId == id).ToList();
            List<int> overlapList = model.Select(mo => mo.QuestionId).ToList();

            ViewData["QuizId"] = id;

            DbSet<Question> dbs3 = _dbContext.Question;
            List<Question> model3 = dbs3.ToList();
            List<int> overlappedquestno = model3.Select(mo => mo.QuestionId).ToList().Intersect(overlapList).ToList();

            List<Question> questionlist = dbs3
                .Where(r => overlappedquestno.Contains(r.QuestionId))
                .Include(mo => mo.UserCodeNavigation)
                .Include(mo => mo.TopicNavigation)
                .Include(mo => mo.SegmentNavigation)
                .ToList();
            return View(questionlist);
        }
        #endregion

        #region Create Question From Quiz Action
        [Authorize(Roles = "Admin, Teacher")]
        public IActionResult CreateQuestions(int id)
        {
            ViewData["QuizId"] = id;
            DbSet<Quiz> dbs = _dbContext.Quiz;
            var lstQuiz = dbs.ToList();
            ViewData["Quiz"] = lstQuiz.ToList();
            ViewData["Test"] = lstQuiz.Select(mo => mo.QuizId).ToList();

            DbSet<Segment> dbs2 = _dbContext.Segment;
            var lstSegment = dbs2.ToList();
            ViewData["Segment"] = new SelectList(lstSegment, "SegmentId", "Name");

            DbSet<Topic> dbs3 = _dbContext.Topic;
            var lstTopic = dbs3.ToList();
            ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");

            return View();

        }
        #endregion Create Question From Quiz Action

        #region Create Question From Quiz Post
        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        public IActionResult CreateQuestions(Question question, IFormCollection form)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<Quiz> dbs2 = _dbContext.Quiz;
            DbSet<QuizQuestionBindDb> dbs3 = _dbContext.QuizQuestionBindDb;

            var quizid = form["QuizIdInput"];
            question.UserCode = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Quiz> lstQuiz = dbs2.ToList<Quiz>();
            var dbcount = lstQuiz.Count();
            

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
            return RedirectToAction("ViewQuestions", new { id = quizid });
        }
        #endregion

        #region Update Questions From Quiz Action
        [Authorize]
        public IActionResult UpdateQuestions(int id)
        {
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == id).ToList();
            List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
            DbSet<Quiz> dbs3 = _dbContext.Quiz;
            List<int> lstdb3 = dbs3.Select(mo => mo.QuizId).ToList();
            DbSet<Question> dbs = _dbContext.Question;
            List<Question> lstdb = dbs.ToList();

            Question question = dbs
                .Where(mo => mo.QuestionId == id)
                .Include(mo => mo.TopicNavigation)
                .Include(mo => mo.SegmentNavigation)
                .FirstOrDefault();

            DbSet<Topic> dbs4 = _dbContext.Topic;
            var lstTopic = dbs4.ToList();

            DbSet<Segment> dbs5 = _dbContext.Segment;
            var lstSegment = dbs5.ToList();
            


            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            if (question != null)
            {
                ViewData["Segment"] = new SelectList(lstSegment, "SegmentId", "Name");
                ViewData["Topic"] = new SelectList(lstTopic, "TopicId", "Name");
                DbSet<Quiz> dbsQuiz = _dbContext.Quiz;
                var lstQuiz = dbsQuiz.ToList();
                ViewData["Test"] = lstQuiz;
                ViewData["common"] = commonlist;
                return View(question);
            }
            else
            {
                TempData["Msg"] = "Question not found!";
                return RedirectToAction("ViewQuestions");
            }
        }
        #endregion

        #region Update Questions from quiz Action
        [Authorize]
        [HttpPost]
        public IActionResult UpdateQuestions(Question question, IFormCollection form)
        {
            var quizid = form["QuizIdInput"];
            if (ModelState.IsValid)
            {
                DbSet<Question> dbs = _dbContext.Question;
                DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
                DbSet<Quiz> dbs3 = _dbContext.Quiz;

                Question tOrder = dbs.Where(mo => mo.QuestionId == question.QuestionId).FirstOrDefault();

                List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == question.QuestionId).ToList();
                List<Quiz> lstQuiz = dbs3.ToList();
                var dbcount = lstQuiz.Count();
                List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
                List<int> lstdb3 = dbs2.Select(mo => mo.QuizId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var quizcheck = Convert.ToInt32(form["currAddId" + y]);
                    var radiocheck = Convert.ToInt32(form["Add" + y]);

                    if (commonlist.Contains(quizcheck) && radiocheck == -1)
                    {
                        dbs2.Remove(dbs2
                            .Where(mo => mo.QuizId == quizcheck && mo.QuestionId == question.QuestionId  )
                            .FirstOrDefault());
                        _dbContext.SaveChanges();
                    }
                    else if (commonlist.Contains(quizcheck) && radiocheck > 0)
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
                            _dbContext.SaveChanges();
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
                    tOrder.Segment = question.Segment;

                    if (_dbContext.SaveChanges() >= 1)
                        TempData["Msg"] = "Question updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Question not found!";
                    return RedirectToAction("ViewQuestions");
                }
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("ViewQuestions");
        }
        #endregion

        #region Delete Question Actions
        [Authorize]
        public IActionResult DeleteQuestions(int id, IFormCollection form)
        {
            DbSet<Question> dbs = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;

            dbs2.RemoveRange(dbs2.Where(mo => mo.QuestionId == id).ToList());
            _dbContext.SaveChanges();

            Question tOrder = dbs.Where(mo => mo.QuestionId == id)
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
            return RedirectToAction("ViewQuestions");
        }
        #endregion Delete Questions

        #region Preview Quiz Action
        [Authorize]
        [HttpGet]
        public IActionResult PreviewQuiz(int id)

        {
            DbSet<QuizQuestionBindDb> dbs = _dbContext.QuizQuestionBindDb;
            DbSet<Quiz> dbs2 = _dbContext.Quiz;
            DbSet<Question> dbs3 = _dbContext.Question;

            List<int> quizidlist = dbs.Where(m => m.QuizId == id).Select(m => m.QuestionId).ToList();
            List<Question> questions = dbs3
                .Where(m => quizidlist.Contains(m.QuestionId))
                .Include(mo => mo.SegmentNavigation)
                .Include(mo => mo.TopicNavigation)
                .ToList();

            dynamic previewmodel = new ExpandoObject();


            previewmodel.Quiz = dbs2.Where(mo => mo.QuizId == id).Include(mo => mo.UserCodeNavigation).ToList();
            previewmodel.Question = questions;

            if (previewmodel != null)
                return new ViewAsPdf(previewmodel)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait

                };
            else
            {
                TempData["Msg"] = "Quiz not found!";
                return RedirectToAction("Index");
            }



        }
        #endregion Preview Quiz Action

        #region GetQuiz
        public IActionResult GetQuiz(int moduleId)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<Topic> dbs2 = _dbContext.Topic;

            List<Quiz> Quiz;

            if (moduleId == -1)
            {
                Quiz = dbs
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                Quiz = dbs
                    .Where(q => q.Topic == moduleId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QuizTable", Quiz);
        }

        #endregion

        #region GetQuizTableAdd
        public IActionResult GetQuizTableAdd(int quizId)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<Topic> dbs2 = _dbContext.Topic;

            List<Quiz> Quiz;

            if (quizId == -1)
            {
                Quiz = dbs
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                Quiz = dbs
                    .Where(q => q.Topic == quizId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QuizTableAdd", Quiz);
        }

        #endregion

        #region GetQQTableUpdate
        public IActionResult GetQQTableUpdate(int quizId, int questionId)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<Topic> dbs2 = _dbContext.Topic;
            DbSet<Question> dbs3 = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs4 = _dbContext.QuizQuestionBindDb;
            DbSet<Segment> dbs5 = _dbContext.Segment;
            DbSet<Topic> dbs6 = _dbContext.Topic;

            var lstTopic = dbs5.ToList();


            List<QuizQuestionBindDb> lstdb4 = dbs4.Where(mo => mo.QuestionId == questionId).ToList();
            List<Question> lstdb = dbs3.ToList();
            var lstSegment = dbs4.ToList();

            List<int> lstoverlap = lstdb4.Select(mo => mo.QuizId).ToList();
            List<int> lstdb3 = dbs.Select(mo => mo.QuizId).ToList();
            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            ViewData["common"] = commonlist;

            List<Quiz> Quiz;

            if (quizId == -1)
            {
                Quiz = dbs
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                Quiz = dbs
                    .Where(q => q.Topic == quizId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QQTableUpdate", Quiz);
        }

        #endregion

        #region GetQuizTableUpdate
        public IActionResult GetQuizTableUpdate(int quizId, int questionId)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            DbSet<Topic> dbs2 = _dbContext.Topic;
            DbSet<Question> dbs3 = _dbContext.Question;
            DbSet<QuizQuestionBindDb> dbs4 = _dbContext.QuizQuestionBindDb;
            DbSet<Segment> dbs5 = _dbContext.Segment;
            DbSet<Topic> dbs6 = _dbContext.Topic;

            var lstTopic = dbs5.ToList();


            List<QuizQuestionBindDb> lstdb4 = dbs4.Where(mo => mo.QuestionId == questionId).ToList();
            List<Question> lstdb = dbs3.ToList();
            var lstSegment = dbs4.ToList();

            List<int> lstoverlap = lstdb4.Select(mo => mo.QuizId).ToList();
            List<int> lstdb3 = dbs.Select(mo => mo.QuizId).ToList();
            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            ViewData["common"] = commonlist;

            List<Quiz> Quiz;

            if (quizId == -1)
            {
                Quiz = dbs
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }
            else
            {
                Quiz = dbs
                    .Where(q => q.Topic == quizId)
                    .Include(q => q.TopicNavigation)
                    .Include(q => q.UserCodeNavigation)
                    .ToList();
            }


            return PartialView("_QuizTableUpdate", Quiz);
        }

        #endregion
    }
}

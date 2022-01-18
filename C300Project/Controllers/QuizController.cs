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

        [AllowAnonymous]
        public IActionResult Index()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.ToList();
            //if (User.IsInRole("Admin"))
            model = dbs.Include(mo => mo.UserCodeNavigation)
                        .ToList();
            /* else
             {
                 var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                 model = dbs.Where(so => so.UserCode == userId).ToList();
             }*/
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Instruction(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.Where(mo => mo.QuizId == id).ToList();

            return View(model);
        }


        [Authorize(Roles = "Student")]
        public IActionResult Attempt(int id) //for users to attempt the quiz
        {
            //TODO: require attention
            DbSet<QuizQuestionBindDb> dbs = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> model = dbs.Where(mo => mo.QuizId == id).ToList();
            List<int> overlapList = model.Select(mo => mo.QuestionId).ToList();

            DbSet<Question> dbs3 = _dbContext.Question;
            List<Question> model3 = dbs3.ToList();
            List<int> overlappedquestno = model3.Select(mo => mo.QuestionId).ToList().Intersect(overlapList).ToList();
            List<Question> questionlist = dbs3.Where(r => overlappedquestno.Contains(r.QuestionId)).ToList();

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
                //ViewData["title"] = model2[id].Title;
                //ViewData["topic"] = model2[id].Topic;
                return View(questionlist);
            }






        }

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
            DbSet<Question> dbs = _dbContext.Question;
            var lstQuestion = dbs.ToList();
            ViewData["Test"] = lstQuestion;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Quiz quiz, IFormCollection form)
        {
            DbSet<Question> dbs2 = _dbContext.Question;
            List<Question> lstQuestion = dbs2.ToList<Question>();
            var dbcount = lstQuestion.Count();
            DbSet<QuizQuestionBindDb> dbs3 = _dbContext.QuizQuestionBindDb;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            quiz.UserCode = userId;
            DbSet<Quiz> dbs = _dbContext.Quiz;

            if (ModelState.IsValid)
            {

                dbs.Add(quiz);
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
                            quizQuestionBind.QuestionId = y;
                            quizQuestionBind.QuizId = quiz.QuizId;
                            dbs3.Add(quizQuestionBind);
                        }
                    }
                }

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "New quiz added!";
            }

            else if (ModelState.IsValid)
            {

                dbs.Add(quiz);
                _dbContext.SaveChanges();


                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if (radiocheck.Equals("False"))
                    {
                        if (ModelState.IsValid)
                        {
                            quizQuestionBind.QuestionId = y;
                            quizQuestionBind.QuizId = quiz.QuizId;
                            dbs3.Add(quizQuestionBind);
                        }
                    }
                }
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "New quiz added!";
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
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuizId == id).ToList();
            List<int> lstoverlap = lstdb2.Select(mo => mo.QuestionId).ToList();
            DbSet<Quiz> dbs3 = _dbContext.Quiz;
            List<int> lstdb3 = dbs3.Select(mo => mo.QuizId).ToList();
            DbSet<Question> dbs = _dbContext.Question;
            List<int> lstdb = dbs.Select(mo => mo.QuestionId).ToList();

            List<int> commonlist = lstoverlap.Intersect(lstdb).ToList();

            Quiz quiz = dbs3.Where(mo => mo.QuizId == id).FirstOrDefault();


            if (quiz != null)
            {
                var lstQuestion = dbs.ToList();
                ViewData["Test"] = lstQuestion;
                ViewData["common"] = commonlist;

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
        public IActionResult Update(Quiz quiz, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DbSet<Quiz> dbs = _dbContext.Quiz;
                Quiz tOrder = dbs.Where(mo => mo.QuizId == quiz.QuizId).FirstOrDefault();

                DbSet<Question> dbs2 = _dbContext.Question;
                List<Question> lstQuestion = dbs2.ToList();
                var dbcount = lstQuestion.Count();

                DbSet<QuizQuestionBindDb> dbs3 = _dbContext.QuizQuestionBindDb;
                List<QuizQuestionBindDb> lstdb2 = dbs3.Where(mo => mo.QuizId == quiz.QuizId).ToList();

                List<int> lstoverlap = lstdb2.Select(mo => mo.QuestionId).ToList();

                List<int> lstdb = dbs2.Select(mo => mo.QuestionId).ToList();

                List<int> commonlist = lstoverlap.Intersect(lstdb).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if (commonlist.Contains(y) && radiocheck.Equals("False"))
                    {
                        dbs3.Remove(dbs3.Where(mo => mo.QuestionId == y).Where(mo => mo.QuizId == quiz.QuizId).FirstOrDefault());
                        _dbContext.SaveChanges();
                    }
                    else if (commonlist.Contains(y) && radiocheck.Equals("True"))
                    {
                        continue;
                    }
                    else if (radiocheck.Equals("True"))
                    {
                        if (ModelState.IsValid)
                        {
                            quizQuestionBind.QuestionId = y;
                            quizQuestionBind.QuizId = quiz.QuizId;
                            dbs3.Add(quizQuestionBind);

                        }
                    }

                }

                if (tOrder != null)
                {
                    tOrder.Title = quiz.Title;
                    tOrder.Topic = quiz.Topic;
                    tOrder.Sec = quiz.Sec;
                    tOrder.StartDt = quiz.StartDt;


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
            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            dbs2.RemoveRange(dbs2.Where(mo => mo.QuizId == id).ToList());
            _dbContext.SaveChanges();

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
        public IActionResult ViewQuestions(int id) //for viewing questions in the specific quiz
        {
            DbSet<QuizQuestionBindDb> dbs = _dbContext.QuizQuestionBindDb;
            List<QuizQuestionBindDb> model = dbs.Where(mo => mo.QuizId == id).ToList();
            List<int> overlapList = model.Select(mo => mo.QuestionId).ToList();

            ViewData["QuizId"] = id;

            DbSet<Question> dbs3 = _dbContext.Question;
            List<Question> model3 = dbs3.ToList();
            List<int> overlappedquestno = model3.Select(mo => mo.QuestionId).ToList().Intersect(overlapList).ToList();
            List<Question> questionlist = dbs3.Where(r => overlappedquestno.Contains(r.QuestionId)).Include(mo => mo.UserCodeNavigation).ToList();
            return View(questionlist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateQuestions(int id)
        {
            ViewData["QuizId"] = id;
            DbSet<Quiz> dbs = _dbContext.Quiz;
            var lstQuiz = dbs.ToList();
            ViewData["Quiz"] = lstQuiz.ToList();
            ViewData["Test"] = lstQuiz.Select(mo => mo.QuizId).ToList();
            return View();

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateQuestions(IFormCollection form)
        {
            var quizid = form["QuizIdInput"];
            DbSet<Question> dbs = _dbContext.Question;
            Question question = new Question();

            question.Questions = form["Questions"];
            question.Topic = form["Topic"];
            question.FirstOption = form["FirstOption"];
            question.SecondOption = form["SecondOption"];
            question.ThirdOption = form["ThirdOption"];
            question.FourthOption = form["FourthOption"];
            question.CorrectAns = form["CorrectAns"];
            question.CorrectAns = form["Segment"];
            question.UserCode = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            DbSet<Quiz> dbs2 = _dbContext.Quiz;
            List<Quiz> lstQuiz = dbs2.ToList<Quiz>();
            var dbcount = lstQuiz.Count();
            DbSet<QuizQuestionBindDb> dbs3 = _dbContext.QuizQuestionBindDb;

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
                if (_dbContext.SaveChanges() == 1)
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
            Question question = dbs.Where(mo => mo.QuestionId == id).FirstOrDefault();

            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            if (question != null)
            {
                DbSet<Quiz> dbsQuiz = _dbContext.Quiz;
                var lstQuiz = dbsQuiz.ToList();
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

        [Authorize]
        [HttpPost]
        public IActionResult UpdateQuestions(Question question, IFormCollection form)
        {
            var quizid = form["QuizIdInput"];
            if (ModelState.IsValid)
            {
                DbSet<Question> dbs = _dbContext.Question;
                Question tOrder = dbs.Where(mo => mo.QuestionId == question.QuestionId).FirstOrDefault();

                DbSet<Quiz> dbs3 = _dbContext.Quiz;
                List<Quiz> lstQuiz = dbs3.ToList<Quiz>();
                var dbcount = lstQuiz.Count();

                DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
                List<QuizQuestionBindDb> lstdb2 = dbs2.Where(mo => mo.QuestionId == question.QuestionId).ToList();

                List<int> lstoverlap = lstdb2.Select(mo => mo.QuizId).ToList();
                List<int> lstdb3 = dbs2.Select(mo => mo.QuizId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    QuizQuestionBindDb quizQuestionBind = new QuizQuestionBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if (commonlist.Contains(y) && radiocheck.Equals("False"))
                    {
                        dbs2.Remove(dbs2.Where(mo => mo.QuestionId == question.QuestionId).Where(mo => mo.QuizId == y).FirstOrDefault());
                        _dbContext.SaveChanges();
                    }
                    else if (commonlist.Contains(y) && radiocheck.Equals("True"))
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
                    tOrder.Segment = question.Segment;



                    _dbContext.SaveChanges();

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
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult PreviewQuiz(int id)
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            Quiz model = dbs.Where(mo => mo.QuizId == id)
                .Include(mo => mo.UserCodeNavigation)
                .Include(mo => mo.QuizQuestionBindDb)
                .FirstOrDefault();

            DbSet<QuizQuestionBindDb> dbs2 = _dbContext.QuizQuestionBindDb;
            List<int> qtnlist = dbs2.ToList().Where(m => m.QuizId == id).Select(m => m.QuestionId).ToList();


            DbSet<Question> dbs3 = _dbContext.Question;
            List<Question> qtnlist1 = dbs3.Where(m => qtnlist.Contains(m.QuestionId)).ToList();
            ViewData["QtnPull"] = qtnlist1;





            if (dbs3 != null)
                return new ViewAsPdf(model)
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


    }
}

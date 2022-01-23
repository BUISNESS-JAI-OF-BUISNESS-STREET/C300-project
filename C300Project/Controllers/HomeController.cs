using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using fyp.Models;
using Microsoft.EntityFrameworkCore;



namespace fyp.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _dbContext;
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Homepage Index Action
        [Authorize]
        public IActionResult Index()
        {
            DbSet<Class> dbs = _dbContext.Class;
            var model = dbs.Include(m => m.Announcement).ToList();
            return View(model);
        }
        #endregion


        [HttpGet]
        public IActionResult GetRemarks(int classId, int id)
        {
            DbSet<Announcement> dbs = _dbContext.Announcement;
            var announcement = dbs.Where
                (l => l.ClassId == classId && l.Id == id).
                Include(l => l.Class).FirstOrDefault();
            return PartialView("_Remarks", announcement);
        }

        [HttpPost]
        public IActionResult SaveRemarks(Announcement uannouncement)
        {
            if (ModelState.IsValid)
            {
                DbSet<Announcement> dbs = _dbContext.Announcement;
                var tAnnouncement = dbs.Where(l => l.ClassId == uannouncement.ClassId && l.Id == uannouncement.Id).FirstOrDefault();

                if (tAnnouncement != null)
                {
                    tAnnouncement.Remarks = uannouncement.Remarks;
                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Remarks saved!";
                    else
                        TempData["Msg"] = "Failed to save Remarks!";
                }
                else
                    TempData["Msg"] = "Remarks not found!";
            }
            else
                TempData["Msg"] = "Invalid data entry!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Report()
        {


            DbSet<Question> dbsQues = _dbContext.Question;
            DbSet<Result> dbsRes = _dbContext.Result;
            DbSet<Account> dbsAcc = _dbContext.Account;

            ViewData["questions"] = dbsQues.ToList<Question>();
            ViewData["results"] = dbsRes.ToList<Result>();
            ViewData["account"] = dbsAcc.ToList<Account>();

            return View();
        }
    }
}
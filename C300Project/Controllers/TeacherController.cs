using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fyp.Models;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace fyp.Controllers
{
    public class TeacherController : Controller
    {
        private AppDbContext _dbContext;
        public TeacherController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Main page that will show teachers and their details
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Teacher> dbs = _dbContext.Teacher;
            List<Teacher> model = dbs.Include(m=> m.AddedByNavigation).ToList();
            
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            DbSet<Class> dbs = _dbContext.Class;
            var lstClass = dbs.ToList();
            ViewData["Class"] = lstClass;
            return View();
        }

        //HttpPost Action to create a new teacher object and send to Teacher database
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(IFormCollection form)
        {
            DbSet<Teacher> dbs = _dbContext.Teacher;
            Teacher teacher = new Teacher();

            DbSet<Class> dbs2 = _dbContext.Class;
            List<Class> lstClass = dbs2.ToList();
            var dbcount = lstClass.Count();

            DbSet<TeacherClassBindDb> dbs3 = _dbContext.TeacherClassBindDb; 
            DbSet<TeacherStudentBindDb> dbs4 = _dbContext.TeacherStudentBindDb;

            if (ModelState.IsValid)
            {

                dbs.Add(teacher);
                _dbContext.SaveChanges();
                for (var x = 0; x < dbcount; x++)
                {

                    TeacherClassBindDb teacherClassBindDb = new TeacherClassBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if (radiocheck.Equals("True"))
                    {
                        if (ModelState.IsValid)
                        {
                            teacherClassBindDb.TeacherId = teacher.TeacherId;
                            teacherClassBindDb.ClassId = y;
                            dbs3.Add(teacherClassBindDb);

                        }
                    }

                }
                if (_dbContext.SaveChanges() >= 1)
                    TempData["Msg"] = "New teacher added!";

                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }
    }
}

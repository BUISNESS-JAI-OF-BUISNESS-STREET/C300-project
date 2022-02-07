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
    public class ClassController : Controller
    {
        private AppDbContext _dbContext;
        public ClassController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Index Page Action
        //Main page that will show teachers and their details
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Class> dbs = _dbContext.Class;
            List<Class> model = dbs.Include(m => m.AddedByNavigation).ToList();

            return View(model);
        }
        #endregion Index Page Action

        #region HttpGet Create Class Action
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            DbSet<Teacher> dbs = _dbContext.Teacher;
            var lstTeacher = dbs.ToList();
            ViewData["Teachers"] = lstTeacher;

            /*
            DbSet<Student> dbs2 = _dbContext.Student;
            var lstStudent = dbs2.ToList();
            ViewData["Students"] = lstStudent;
            */
            return View();
        }
        #endregion

        #region HttpPost Create Class Action
        //HttpPost Action to create a new teacher object and send to Teacher database
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Class classAdd, IFormCollection form)
        {
            DbSet<Teacher> dbsteach = _dbContext.Teacher;
            DbSet<Class> dbsclass = _dbContext.Class;
        DbSet<Student> dbsstudent = _dbContext.Student;

            DbSet<TeacherClassBindDb> dbsTchCls = _dbContext.TeacherClassBindDb;
            DbSet<StudentClassBindDb> dbsStdCls = _dbContext.StudentClassBindDb;
            DbSet<TeacherStudentBindDb> dbsTchStd = _dbContext.TeacherStudentBindDb;
            List<Teacher> lstTeach = dbsteach.ToList();
            var dbcount = lstTeach.Count();

            

            if (ModelState.IsValid)
            {
                classAdd.AddedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                dbsclass.Add(classAdd);
                _dbContext.SaveChanges();
                for (var x = 0; x < dbcount; x++)
                {

                    TeacherClassBindDb teacherClassBindDb = new TeacherClassBindDb();
                    var y = x + 1;
                    var radiocheck = Convert.ToInt32(form["Add" + y]);

                    if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            teacherClassBindDb.TeacherId = radiocheck;
                            teacherClassBindDb.ClassId = classAdd.ClassId;

                            dbsTchCls.Add(teacherClassBindDb);

                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                if (_dbContext.SaveChanges() >= 1)
                    TempData["Msg"] = "New class added!";

                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }
        #endregion HttpPost Create Class Action

        #region HttpGet Update Teacher Action
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            DbSet<TeacherClassBindDb> dbs2 = _dbContext.TeacherClassBindDb;
            DbSet<Teacher> dbs = _dbContext.Teacher;
            DbSet<Class> dbs3 = _dbContext.Class;

            List<TeacherClassBindDb> lstdb2 = dbs2.Where(mo => mo.ClassId == id).ToList();
            List<int> lstoverlap = lstdb2.Select(mo => mo.TeacherId).ToList();
            List<int> lstdbs = dbs.Select(mo => mo.TeacherId).ToList();
            List<Teacher> lstdb = dbs.ToList();
            Class tClass = dbs3.Where(mo => mo.ClassId == id).FirstOrDefault();

            List<int> commonlist = lstoverlap.Intersect(lstdbs).ToList();

            if (tClass != null)
            {
                var lstTeach = dbs.ToList();
                ViewData["Teachers"] = lstTeach;
                ViewData["common"] = commonlist;
                return View(tClass);
            }
            else
            {
                TempData["Msg"] = "Class not found!";
                return RedirectToAction("Index");
            }
        }
        #endregion HttpPost Create Teacher Action

        #region HttpPost Update Teacher Action
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(Class classUpdate, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DbSet<Teacher> dbs = _dbContext.Teacher;
                List<Teacher> lstTch = dbs.ToList();
                var dbcount = lstTch.Count();
                

                DbSet<Class> dbs3 = _dbContext.Class;
                Class tClass = dbs3.Where(mo => mo.ClassId == classUpdate.ClassId).FirstOrDefault();
                

                DbSet<TeacherClassBindDb> dbs2 = _dbContext.TeacherClassBindDb;
                List<TeacherClassBindDb> lstdb2 = dbs2.Where(mo => mo.ClassId == classUpdate.ClassId).ToList();

                List<int> lstoverlap = lstdb2.Select(mo => mo.ClassId).ToList();
                List<int> lstdb3 = dbs2.Select(mo => mo.ClassId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    TeacherClassBindDb teacherClassBindDb = new TeacherClassBindDb();
                    var y = x + 1;
                    var classcheck = Convert.ToInt32(form["currAddId" + y]);
                    var radiocheck = Convert.ToInt32(form["Add" + y]);

                    if (commonlist.Contains(classcheck) && radiocheck == -1)
                    {
                        dbs2.Remove(dbs2.Where(mo => mo.ClassId == classUpdate.ClassId && mo.TeacherId == radiocheck).FirstOrDefault());
                        _dbContext.SaveChanges();
                    }
                    else if (commonlist.Contains(classcheck) && radiocheck > 0)
                    {
                        continue;
                    }
                    else if (radiocheck > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            teacherClassBindDb.TeacherId = radiocheck;
                            teacherClassBindDb.ClassId = classUpdate.ClassId;
                            dbs2.Add(teacherClassBindDb);

                        }
                    }

                }

                if (tClass != null)
                {
                    tClass.Name = classUpdate.Name;

                    if (_dbContext.SaveChanges() >= 1)
                        TempData["Msg"] = "Class updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Class not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }
        #endregion HttpPost Update Teacher Action

        #region HttpPost Delete Teacher Action
        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<Class> dbs = _dbContext.Class;
            DbSet<TeacherClassBindDb> dbs2 = _dbContext.TeacherClassBindDb;
            DbSet<StudentClassBindDb> dbs3 = _dbContext.StudentClassBindDb;

            dbs3.RemoveRange(dbs3.Where(mo => mo.ClassId == id).ToList());
            dbs2.RemoveRange(dbs2.Where(mo => mo.ClassId == id).ToList());
            _dbContext.SaveChanges();

            Class tClass = dbs.Where(mo => mo.ClassId == id)
                                     .FirstOrDefault();

            if (tClass != null)
            {
                dbs.Remove(tClass);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "Class deleted!";
                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Class not found!";
            }
            return RedirectToAction("Index");
        }
        #endregion HttpPost Delete Teacher Action

    }
}

//19046587 Alfie Farhan
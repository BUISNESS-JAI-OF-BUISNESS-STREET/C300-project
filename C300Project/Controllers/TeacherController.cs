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

        #region Teacher Index
        //Main page that will show teachers and their details
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Teacher> dbs = _dbContext.Teacher;
            List<Teacher> model = dbs.Include(m=> m.AddedByNavigation).ToList();
            
            return View(model);
        }
        #endregion Teacher Index

        #region HttpGet Create Teacher Action
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            DbSet<Class> dbs = _dbContext.Class;
            var lstClass = dbs.ToList();
            ViewData["Class"] = lstClass;
            return View();
        }
        #endregion

        #region HttpPost Create Teacher Action
        //HttpPost Action to create a new teacher object and send to Teacher database
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Teacher teacher,IFormCollection form)
        {
            DbSet<Teacher> dbsteach = _dbContext.Teacher;
            

            DbSet<Class> dbsclass = _dbContext.Class;
            List<Class> lstClass = dbsclass.ToList();
            var dbcount = lstClass.Count();

            DbSet<Student> dbsstudent = _dbContext.Student;

            DbSet<TeacherClassBindDb> dbsTchCls = _dbContext.TeacherClassBindDb;
            DbSet<StudentClassBindDb> dbsStdCls = _dbContext.StudentClassBindDb;
            DbSet<TeacherStudentBindDb> dbsTchStd = _dbContext.TeacherStudentBindDb;

            if (ModelState.IsValid)
            {
                teacher.AddedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                dbsteach.Add(teacher);
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


                            List<StudentClassBindDb> lstStdcls = 
                                dbsStdCls.
                                Where(m => m.ClassId == y)
                                .ToList();

                            List<Student> lstStd = dbsstudent
                                .Where(m => lstStdcls
                                .Select(m => m.StudentId)
                                .Contains(m.StudentId))
                                .ToList();

                            #region Add entry to TeacherStudentBindDb  
                            if (lstStd != null)
                            {
                                foreach(var item in lstStd)
                                {
                                    TeacherStudentBindDb dbsTchStdAdd = new TeacherStudentBindDb();
                                    dbsTchStdAdd.StudentId = item.StudentId;
                                    dbsTchStdAdd.TeacherId = teacher.TeacherId;

                                    dbsTchStd.Add(dbsTchStdAdd);
                                    
                                }
                                _dbContext.SaveChanges();
                            }
                            else
                            {
                                continue;
                            }
                                
# endregion Add entry to TeacherStudentBindDb  

                            dbsTchCls.Add(teacherClassBindDb);

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
        #endregion HttpPost Create Teacher Action

        #region HttpGet Update Teacher Action
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            DbSet<TeacherClassBindDb> dbs2 = _dbContext.TeacherClassBindDb;
            List<TeacherClassBindDb> lstdb2 = dbs2.Where(mo => mo.TeacherId == id).ToList();
            List<int> lstoverlap = lstdb2.Select(mo => mo.ClassId).ToList();
            DbSet<Class> dbs3 = _dbContext.Class;
            List<int> lstdb3 = dbs3.Select(mo => mo.ClassId).ToList();
            DbSet<Teacher> dbs = _dbContext.Teacher;
            List<Teacher> lstdb = dbs.ToList();
            Teacher teacher = dbs.Where(mo => mo.TeacherId == id).FirstOrDefault();

            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            if (teacher != null)
            {
                var lstClass = dbs3.ToList();
                ViewData["Test"] = lstClass;
                ViewData["common"] = commonlist;
                return View(teacher);
            }
            else
            {
                TempData["Msg"] = "Question not found!";
                return RedirectToAction("Index");
            }
        }
        #endregion HttpPost Create Teacher Action

        #region HttpPost Update Teacher Action
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(Teacher teacher, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DbSet<Teacher> dbs = _dbContext.Teacher;
                Teacher tTeach = dbs.Where(mo => mo.TeacherId == teacher.TeacherId).FirstOrDefault();

                DbSet<Class> dbs3 = _dbContext.Class;
                List<Class> lstCls = dbs3.ToList();
                var dbcount = lstCls.Count();

                DbSet<TeacherClassBindDb> dbs2 = _dbContext.TeacherClassBindDb;
                List<TeacherClassBindDb> lstdb2 = dbs2.Where(mo => mo.TeacherId == teacher.TeacherId).ToList();

                List<int> lstoverlap = lstdb2.Select(mo => mo.ClassId).ToList();
                List<int> lstdb3 = dbs2.Select(mo => mo.ClassId).ToList();
                List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

                for (var x = 0; x < dbcount; x++)
                {

                    TeacherClassBindDb teacherClassBindDb = new TeacherClassBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if (commonlist.Contains(y) && radiocheck.Equals("False"))
                    {
                        dbs2.Remove(dbs2.Where(mo => mo.TeacherId == teacher.TeacherId && mo.ClassId == y).FirstOrDefault());
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
                            teacherClassBindDb.TeacherId = teacher.TeacherId;
                            teacherClassBindDb.ClassId = y;
                            dbs2.Add(teacherClassBindDb);

                        }
                    }

                }

                if (tTeach != null)
                {
                    tTeach.Name = teacher.Name;
                    tTeach.MobileNo = teacher.MobileNo;
                    tTeach.Email = teacher.Email;
                    tTeach.Role = teacher.Role;

                    if (_dbContext.SaveChanges() >= 1)
                        TempData["Msg"] = "Teacher updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Teacher not found!";
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
            DbSet<Teacher> dbs = _dbContext.Teacher;
            DbSet<TeacherClassBindDb> dbs2 = _dbContext.TeacherClassBindDb;
            DbSet<TeacherStudentBindDb> dbs3 = _dbContext.TeacherStudentBindDb;

            dbs3.RemoveRange(dbs3.Where(mo => mo.TeacherId == id).ToList());
            _dbContext.SaveChanges();

            dbs2.RemoveRange(dbs2.Where(mo => mo.TeacherId == id).ToList());
            _dbContext.SaveChanges();

            Teacher tTeach = dbs.Where(mo => mo.TeacherId == id)
                                     .FirstOrDefault();

            if (tTeach != null)
            {
                dbs.Remove(tTeach);
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
        #endregion HttpPost Update Teacher Action
    }
}
//19046587 Alfie Farhan
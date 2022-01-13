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
        [HttpGet]
        public IActionResult Create()
        {
            DbSet<Class> dbs = _dbContext.Class;
            var lstClass = dbs.ToList();
            ViewData["Class"] = lstClass;
            return View();
        }

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
    }
}

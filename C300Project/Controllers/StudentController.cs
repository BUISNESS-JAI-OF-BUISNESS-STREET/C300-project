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
    public class StudentController : Controller
    {
        private AppDbContext _dbContext;
        public StudentController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Student> dbs = _dbContext.Student;
            List<Student> model = dbs.Include(m => m.AddedByNavigation).ToList();

            return View(model);
        }

        #region HttpGet Add Student Action
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

        #region HttpPost Add Student Action
        //HttpPost Action to create a new teacher object and send to Teacher database
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Student student, IFormCollection form)
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
                student.AddedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                dbsstudent.Add(student);
                _dbContext.SaveChanges();
                for (var x = 0; x < dbcount; x++)
                {

                    StudentClassBindDb studentClassBindDb = new StudentClassBindDb();
                    var y = x + 1;
                    var radiocheck = form["Add" + y];

                    if (radiocheck.Equals("True"))
                    {
                        if (ModelState.IsValid)
                        {
                            studentClassBindDb.StudentId = student.StudentId;
                            studentClassBindDb.ClassId = y;


                            List<TeacherClassBindDb> lstTchCls =
                                dbsTchCls.
                                Where(m => m.ClassId == y)
                                .ToList();

                            List<Teacher> lstTch = dbsteach
                                .Where(m => lstTchCls
                                .Select(m => m.TeacherId)
                                .Contains(m.TeacherId))
                                .ToList();

                            #region Add entry to TeacherStudentBindDb  
                            if (lstTch != null)
                            {
                                foreach (var item in lstTch)
                                {
                                    TeacherStudentBindDb dbsTchStdAdd = new TeacherStudentBindDb();
                                    dbsTchStdAdd.StudentId = student.StudentId;
                                    dbsTchStdAdd.TeacherId = item.TeacherId;

                                    dbsTchStd.Add(dbsTchStdAdd);

                                }
                                _dbContext.SaveChanges();
                            }
                            else
                            {
                                continue;
                            }

                            #endregion Add entry to TeacherStudentBindDb  

                            dbsStdCls.Add(studentClassBindDb);

                        }
                    }

                }
                if (_dbContext.SaveChanges() >= 1)
                    TempData["Msg"] = "New student added!";

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

        #region HttpGet Update Student Action
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            DbSet<StudentClassBindDb> dbs2 = _dbContext.StudentClassBindDb;
            List<StudentClassBindDb> lstdb2 = dbs2.Where(mo => mo.StudentId == id).ToList();
            List<int> lstoverlap = lstdb2.Select(mo => mo.ClassId).ToList();
            DbSet<Class> dbs3 = _dbContext.Class;
            List<int> lstdb3 = dbs3.Select(mo => mo.ClassId).ToList();
            DbSet<Student> dbs = _dbContext.Student;
            List<Student> lstdb = dbs.ToList();
            Student student = dbs.Where(mo => mo.StudentId == id).FirstOrDefault();

            List<int> commonlist = lstoverlap.Intersect(lstdb3).ToList();

            if (student != null)
            {
                var lstClass = dbs3.ToList();
                ViewData["Test"] = lstClass;
                ViewData["common"] = commonlist;
                return View(student);
            }
            else
            {
                TempData["Msg"] = "Question not found!";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region HttpPost Student Action
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
                            teacherClassBindDb.TeacherId = teacherClassBindDb.TeacherId;
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
        #endregion

        #region Delete Student Action
        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<Student> dbs = _dbContext.Student;
            DbSet<StudentClassBindDb> dbs2 = _dbContext.StudentClassBindDb;
            DbSet<TeacherStudentBindDb> dbs3 = _dbContext.TeacherStudentBindDb;

            dbs3.RemoveRange(dbs3.Where(mo => mo.StudentId == id).ToList());
            _dbContext.SaveChanges();

            dbs2.RemoveRange(dbs2.Where(mo => mo.StudentId == id).ToList());
            _dbContext.SaveChanges();

            Student tStdt = dbs.Where(mo => mo.StudentId == id)
                                     .FirstOrDefault();

            if (tStdt != null)
            {
                dbs.Remove(tStdt);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "Student deleted!";
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

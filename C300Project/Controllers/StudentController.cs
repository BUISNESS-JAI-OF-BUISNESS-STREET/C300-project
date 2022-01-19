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

        #region Index
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Student> dbs = _dbContext.Student;
            List<Student> model = dbs.Include(m => m.AddedByNavigation).ToList();

            return View(model);
        }
        #endregion

        #region HttpGet Add Student Action
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            DbSet<Class> dbs = _dbContext.Class;
            var lstClass = dbs.ToList();
            ViewData["Class"] = new SelectList(lstClass, "Name", "Name");
            return View();
        }
        #endregion

        #region HttpPost Add Student Action
        //HttpPost Action to create a new teacher object and send to Teacher database
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Student student, IFormCollection form)
        {
            DbSet<Class> dbsclass = _dbContext.Class;
            DbSet<Student> dbsstudent = _dbContext.Student;
            DbSet<Teacher> dbsteach = _dbContext.Teacher;
            DbSet<TeacherClassBindDb> dbsTchCls = _dbContext.TeacherClassBindDb;
            DbSet<StudentClassBindDb> dbsStdCls = _dbContext.StudentClassBindDb;
            DbSet<TeacherStudentBindDb> dbsTchStd = _dbContext.TeacherStudentBindDb;

            List<Class> lstClass = dbsclass.ToList();
            var classid = dbsclass.Where(mo => mo.Name == student.Class).Select(mo => mo.ClassId).FirstOrDefault();
            

            if (ModelState.IsValid)
            {
                student.AddedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                dbsstudent.Add(student);
                _dbContext.SaveChanges();

                    StudentClassBindDb studentClassBindDb = new StudentClassBindDb();

                        if (ModelState.IsValid)
                        {
                            studentClassBindDb.StudentId = student.StudentId;
                            studentClassBindDb.ClassId = classid;


                            List<TeacherClassBindDb> lstTchCls =
                                dbsTchCls.
                                Where(m => m.ClassId == classid)
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

                            #endregion Add entry to TeacherStudentBindDb  

                            dbsStdCls.Add(studentClassBindDb);

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
            DbSet<Class> dbs3 = _dbContext.Class;
            var lstClass = dbs3.ToList();
            
            DbSet<Student> dbs = _dbContext.Student;
            List<Student> lstdb = dbs.ToList();
            Student student = dbs.Where(mo => mo.StudentId == id).FirstOrDefault();

            if (student != null)
            {
                ViewData["Class"] = new SelectList(lstClass, "ClassId", "Name");
                return View(student);
            }
            else
            {
                TempData["Msg"] = "Student not found!";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region HttpPost Update Student Action
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(Student student, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DbSet<Student> dbs = _dbContext.Student;
                Student tStud = dbs.Where(mo => mo.StudentId == student.StudentId).FirstOrDefault();

                DbSet<Class> dbs3 = _dbContext.Class;
                List<Class> lstCls = dbs3.ToList();

                if (tStud != null)
                {
                    tStud.Name = student.Name;
                    tStud.MobileNo = student.MobileNo;
                    tStud.Country = student.Country;
                    tStud.Foreigner = student.Foreigner;
                    tStud.SchLvl = student.SchLvl;
                    tStud.Email = student.Email;
                    tStud.Class = student.Class;

                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Student updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Student not found!";
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

//19046587 Alfie Farhan
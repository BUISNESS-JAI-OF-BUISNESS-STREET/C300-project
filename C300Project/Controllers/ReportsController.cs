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
using System.IO;
using System.Data;
using ClosedXML.Excel;
using System.Data.SqlClient;

namespace fyp.Controllers.Controllers
{

    public class ReportsController : Controller
    {
        private AppDbContext _dbContext;

        public ReportsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            DbSet<Quiz> dbs = _dbContext.Quiz;
            List<Quiz> model = dbs.ToList();
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ViewReport(int id)
        {
            DbSet<Result> dbs = _dbContext.Result;
            List<Result> model = dbs.Where(m => m.QuizId == id).ToList();
            ViewData["QuizId"] = id;
            return View(model);
        }

        public IActionResult exportExcel(int id)
        {
            using(var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Result");

                ws.Cell(1, 1).Value = "ResultId";
                ws.Cell(1, 2).Value = "QuizId";
                ws.Cell(1, 3).Value = "AccountId";
                ws.Cell(1, 4).Value = "Name";
                ws.Cell(1, 5).Value = "Title";
                ws.Cell(1, 6).Value = "Topic";
                ws.Cell(1, 7).Value = "Score";
                ws.Cell(1, 8).Value = "Attempt";
                ws.Cell(1, 9).Value = "Dt";

                System.Data.DataTable dt = new System.Data.DataTable();
                SqlConnection con = new SqlConnection(@"Data Source = (localdb)\ProjectsV13; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
                SqlDataAdapter ed = new SqlDataAdapter("Select * from fypdb.dbo.Result where QuizId =" + id,con);
                ed.Fill(dt);

                if(dt != null)
                {
                    int i = 2;
                foreach(System.Data.DataRow row in dt.Rows)
                {
                    ws.Cell(i, 1).Value = row[0].ToString();
                    ws.Cell(i, 2).Value = row[1].ToString();
                    ws.Cell(i, 3).Value = row[2].ToString();
                    ws.Cell(i, 4).Value = row[3].ToString();
                    ws.Cell(i, 5).Value = row[4].ToString();
                    ws.Cell(i, 6).Value = row[5].ToString();
                    ws.Cell(i, 7).Value = row[6].ToString();
                    ws.Cell(i, 8).Value = row[7].ToString();
                    ws.Cell(i, 9).Value = row[8].ToString();
                    i++;
                }
                
                using(var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument-spreadsheetml.sheet", "Results.xlsx");
                }

                }
                else
                {
                    return RedirectToAction("ViewReport", new { id = id });
                }
                
            }
        }

    }
}
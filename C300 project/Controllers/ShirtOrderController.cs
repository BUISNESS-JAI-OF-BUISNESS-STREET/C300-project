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

namespace fyp.Controllers
{
    public class ShirtOrderController : Controller
    {
        private AppDbContext _dbContext;

        public ShirtOrderController(AppDbContext dbContext)
        {
           _dbContext = dbContext;
        }
        
	[Authorize]
        public IActionResult Index()
        {
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
            List<ShirtOrder> model = dbs.ToList();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.IsInRole("Admin"))
                model = model.Where(so => so.CreatedBy == userId).ToList();
            return View(model);
        }

	[Authorize]
        public IActionResult Create()
        {
            var dbs = _dbContext.Pokedex;
            var lstPokes = dbs.ToList<Pokedex>();
            ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");

            return View();
        }

	[Authorize]
        [HttpPost]
        public IActionResult Create(ShirtOrder shirtOrder)
        {
            if (ModelState.IsValid)
            {
				shirtOrder.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _dbContext.ShirtOrder.Add(shirtOrder);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "New order added!";
                else
                    TempData["Msg"] = "Failed to update database!";
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }

            return RedirectToAction("Index");
        }

        // Update actions yet to be implemented
        
        [Authorize]
        [HttpGet]
        public IActionResult Update(int id)
        {
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
            ShirtOrder tOrder = dbs.Where(mo => mo.Id == id).FirstOrDefault();


            if (tOrder != null)
            {
                var dbsPokes = _dbContext.Pokedex;
                var lstPokes = dbsPokes.ToList<Pokedex>();
                ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");
                return View(tOrder);
            }
            else
            {
                TempData["Msg"] = "Shirt order not found!";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(ShirtOrder shirtOrder)
        {
            if (ModelState.IsValid)
            {
                DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
                ShirtOrder tOrder = dbs.Where(mo => mo.Id == shirtOrder.Id).FirstOrDefault(); ;

                if (tOrder != null)
                {
                    tOrder.Name = shirtOrder.Name;
                    tOrder.Color = shirtOrder.Color;
                    tOrder.PokedexId = shirtOrder.PokedexId;
                    tOrder.Qty = shirtOrder.Qty;
                    tOrder.FrontPosition = shirtOrder.FrontPosition;
                    tOrder.Price = shirtOrder.Price;
                    tOrder.Name = shirtOrder.Name;
                    tOrder.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;


                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Shirt Order Updated";
                    else
                        TempData["Msg"] = "Failed to add Shirt order in database";

                }
                else
                {
                    TempData["Msg"] = "Shirt order not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }
    }
}

//18043018 khai




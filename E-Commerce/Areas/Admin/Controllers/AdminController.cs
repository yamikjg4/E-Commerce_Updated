using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class AdminController : Controller
    {
        private readonly Eshopcontext _db;
     
        public AdminController(Eshopcontext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        //get
        [HttpGet]
        [Route("addcategory")]
        public IActionResult AddCategory()
        {
            return View();
        }

        //post
        [HttpPost]
        [Route("addcategory")]
        public async Task<IActionResult> AddCategory(Category obj)
        {
            if (ModelState.IsValid)
            {
                 await _db.tblcategory.AddAsync(obj);
               await _db.SaveChangesAsync();
               return RedirectToAction("ViewCategory", "Admin", new { area = "Admin" });
            }
            else
            {
                return Content("Not Working");
            }
            
        }




        [Route("viewcategory")]
        public IActionResult ViewCategory()
        {
            try
            {
                var stdList = _db.tblcategory.ToList();
                return View(stdList);
            }
            catch (Exception)
            {

                return View();
            }
            
        }

        [Route("addproduct")]
        public IActionResult AddProduct()
        {
            return View();
        }

        [Route("viewproduct")]
        public IActionResult ViewProduct()
        {
            return View();
        }

        [Route("adduser")]
        public IActionResult AddUser()
        {
            return View();
        }

        [Route("viewuser")]
        public IActionResult ViewUser()
        {
            return View();
        }


        [Route("editadmin")]
        public IActionResult EditAdmin()
        {
            return View();
        }
      
        [Route("adminlogout")]
       public IActionResult AdminLogout()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }

    }
}

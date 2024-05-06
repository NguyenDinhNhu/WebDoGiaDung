using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Controllers
{
    public class ModuleController : Controller
    {
        private AppDbContext db = new AppDbContext();   
        // GET: Module
        public ActionResult Header()
        {
            var list = db.CategoryEntities.Where(m => m.Status != 0).ToList();
            return View("_Header", list);
        }
        
        public ActionResult Footer()
        {
            return View("_Footer");
        }

        public ActionResult Login()
        {
            return View("_Login");
        }
        public ActionResult ICart()
        {
            return View("_ICart");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class DashBoardController : BaseController
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/DashBoard
        public ActionResult Index()
        {
            ViewBag.CountOrderSuccess = db.OrdersEntities.Where(m => m.Status == 3).Count();
            ViewBag.CountOrderCancel = db.OrdersEntities.Where(m => m.Status == 1).Count();
            ViewBag.CountContactDoneReply = db.ContactEntities.Where(m => m.Flag == 0).Count();
            ViewBag.CountUser = db.UsersEntities.Where(m => m.Status != 0).Count();
            return View();
        }
    }
}
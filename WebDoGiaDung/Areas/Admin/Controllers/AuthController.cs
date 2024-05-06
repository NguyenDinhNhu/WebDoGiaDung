using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/Auth
        public ActionResult Login()
        {
            if (Session["Admin_Name"] != null)
            {
                Response.Redirect("~/Admin");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Login(String User, String Pass)
        {
            int count_username = db.UsersEntities.Where(m => m.Status == 1 && (m.Email == User || m.UserName == User) && m.Role != 0).Count();
            if (count_username == 0)
                return Json(new { s = 1 });
            else
            {
                String Password = XString.ToMD5(Pass);
                //String Password = Pass;
                var user_account = db.UsersEntities
                .Where(m => m.Status == 1 && (m.Email == User || m.UserName == User) && m.Role != 0 && m.Password == Password);
                if (user_account.Count() == 0)
                    return Json(new { s = 2 });
                else
                {
                    var user = user_account.First();
                    Session["Admin_Name"] = user.FullName;
                    Session["Admin_ID"] = user.Id;
                    Session["Admin_Images"] = user.Image;
                    Session["Admin_Address"] = user.Address;
                    Session["Admin_Email"] = user.Email;
                    Session["Admin_CreatedDate"] = user.CreatedDate;
                    return Json(new { s = 0 });
                }
            }
        }

        public ActionResult Logout()
        {
            if (Session["Admin_Name"] != null)
            {
                Session["Admin_Name"] = null;
                Session["Admin_ID"] = null;
                Session["Admin_Images"] = null;
                Session["Admin_Address"] = null;
                Session["Admin_Email"] = null;
                Session["Admin_CreatedDate"] = null;
            }
            return Redirect("~/Admin/Login");
        }
        public ActionResult Profiles()
        {
            if (Session["Admin_Name"] == null)
            {
                return Redirect("~/Admin/Login");
            }
            if (Session["Admin_Name"] == null)
            {
                return HttpNotFound();
            }


            return View("Profiles");

        }
        //public ActionResult Check()
        //{
        //    if (Session["Admin_Name"] == null)
        //    {
        //        return Redirect("~/Admin/Login");
        //    }
        //    if (Session["Admin_Name"] == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var list = db.Users.Where(m => m.Status == 1).ToList();
        //    return View("_Test", list);

        //}
    }
}
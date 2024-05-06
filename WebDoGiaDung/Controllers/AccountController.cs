using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext db = new AppDbContext();
        [HttpPost]
        public JsonResult UserLogin(string user, string password)
        {
            int count_username = db.UsersEntities.Where(m => m.Status == 1 && (m.Email == user || m.UserName == user) && m.Role == 0).Count();
            if (count_username == 0)
            {

                return Json(new { s = 1 });
            }
            else
            {
                password = XString.ToMD5(password);
                //Password = Password;
                var user_acount = db.UsersEntities.Where(m => m.Status == 1 && (m.Email == user || m.UserName == user) && m.Password == password);
                if (user_acount.Count() == 0)
                {
                    return Json(new { s = 2 });
                }
                else
                {
                    var users = user_acount.First();
                    Session["User_Name"] = users.FullName;
                    Session["User_ID"] = users.Id;
                }
            }
            return Json(new { s = 0 });
        }

        [HttpPost]
        public JsonResult Register(UsersEntity user)
        {
            try
            {
                var checkPM = db.UsersEntities.Any(m => m.Phone == user.Phone && m.Email.ToLower().Equals(user.Email.ToLower()));
                if (checkPM)
                {
                    return Json(new { Code = 1, Message = "Số điện thoại hoặc Email đã được sử dụng." });
                }
                user.Image = "";
                user.Role = 0;
                user.Status = 1;
                user.Password = XString.ToMD5(user.Password);
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = 1;
                user.UpdatedDate = DateTime.Now;
                user.UpdatedBy = 1;

                db.UsersEntities.Add(user);
                db.SaveChanges();

                return Json(new { Code = 0, Message = "Đăng ký thành công!" });
            }
            catch (Exception e)
            {
                return Json(new { Code = 1, Message = "Đăng ký thất bại!" });
                throw e;
            }
        }
        public ActionResult NotFound()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        public ActionResult UserLogout(string url)
        {
            Session["User_Name"] = null;
            Session["User_ID"] = null;
            return Redirect("~/" + url);
        }
        
        public ActionResult UserProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersEntity user = db.UsersEntities.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UsersEntity user)
        {
            if (ModelState.IsValid)
            {
                user.UpdatedDate = DateTime.Now;
                user.UpdatedBy = int.Parse(Session["User_ID"].ToString());
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Cập nhật thành công!", "success");
                return RedirectToAction("UserProfile");
            }
            return View(user);
        }

        public ActionResult Order()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var list = db.OrdersEntities.Where(m => m.UserId == userid).OrderByDescending(m => m.CreatedDate).ToList();
            ViewBag.itemOrder = db.OrderDetailEntities.ToList();
            ViewBag.productOrder = db.ProductEntities.ToList();
            return View(list);
        }
        public ActionResult OrderDetails(int id)
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var checkO = db.OrdersEntities.Where(m => m.UserId == userid && m.Id == id);
            if (checkO.Count() == 0)
            {
                return this.NotFound();
            }

            var id_order = db.OrdersEntities.Where(m => m.UserId == userid && m.Id == id).FirstOrDefault();
            ViewBag.Order = id_order;
            var itemOrder = db.OrderDetailEntities.Where(m => m.OrderId == id_order.Id).ToList();
            ViewBag.productOrder = db.ProductEntities.ToList();
            return View(itemOrder);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            ViewBag.countTrash = db.UsersEntities.Where(m => m.Status == 0).Count();
            var query = db.UsersEntities.Where(m => m.Status != 0).ToList();
            return View(query);
        }
        public ActionResult Trash()
        {
            var query = db.UsersEntities.Where(m => m.Status == 0).ToList();
            return View(query);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsersEntity user)
        {
            if (ModelState.IsValid)
            {
                String avatar = XString.ToAscii(user.FullName);
                user.Password = XString.ToMD5(user.Password);
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
                user.UpdatedDate = DateTime.Now;
                user.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = avatar + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    user.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/user/"), filename);
                    file.SaveAs(Strpath);
                }

                db.UsersEntities.Add(user);
                db.SaveChanges();
                Notification.set_flash("User đã được thêm!", "success");
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult DelTrash(int id)
        {
            UsersEntity user = db.UsersEntities.Find(id);
            if (user == null)
            {
                Notification.set_flash("Không tồn tại User trên!", "warning");
                return RedirectToAction("Index");
            }

            user.Status = 0;

            user.CreatedDate = DateTime.Now;
            //user.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Đã xóa vào thùng rác!" + " ID = " + id, "warning");
            return RedirectToAction("Index");
        }

        // Delete to trash - Restore
        public ActionResult ReTrash(int? id)
        {
            UsersEntity user = db.UsersEntities.Find(id);
            if (user == null)
            {
                Notification.set_flash("Không tồn tại User!", "warning");
                return RedirectToAction("Trash", "User");
            }
            user.Status = 2;

            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "User");
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(int? id)
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

        public ActionResult Edit(int? id)
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
        public ActionResult Edit(UsersEntity user)
        {
            if (ModelState.IsValid)
            {

                String avatar = XString.ToAscii(user.FullName);
                user.Password = XString.ToMD5(user.Password);
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
                user.UpdatedDate = DateTime.Now;
                user.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = avatar + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    user.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/user/"), filename);
                    file.SaveAs(Strpath);
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Cập nhật thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/User/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsersEntity user = db.UsersEntities.Find(id);
            //string fileName = Server.MapPath("~/Public/img/user/" + user.Image);
            //if (fileName != null)
            //{
            //    System.IO.File.Delete(fileName);
            //}
            db.UsersEntities.Remove(user);
            db.SaveChanges();
            Notification.set_flash("Đã xóa hoàn User!", "danger");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            UsersEntity user = db.UsersEntities.Find(id);
            user.Status = (user.Status == 1) ? 2 : 1;

            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = user.Status
            });
        }
    }
}
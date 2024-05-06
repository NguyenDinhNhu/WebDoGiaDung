using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class NewsController : BaseController
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/News
        public ActionResult Index()
        {
            ViewBag.countTrash = db.NewsEntities.Where(m => m.Status == 0).Count();
            var query = db.NewsEntities.Where(m => m.Status != 0).ToList();
            return View(query);
        }

        public ActionResult Trash()
        {
            var query = db.NewsEntities.Where(m => m.Status == 0).ToList();
            return View(query);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewsEntity news)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(news.Name);
                news.Slug = strSlug;
                news.CreatedDate = DateTime.Now;
                news.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
                news.UpdatedDate = DateTime.Now;
                news.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
                var file = Request.Files["Image"];

                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    news.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/news/"), filename);
                    file.SaveAs(Strpath);
                }
                db.NewsEntities.Add(news);
                db.SaveChanges();
                Notification.set_flash("Đã thêm bài viết mới!", "success");
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // Edit
        public ActionResult Edit(int? id)
        {
            NewsEntity news = db.NewsEntities.Find(id);
            if (news == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "News");
            }
            return View(news);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewsEntity news)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(news.Name);
                news.Slug = strSlug;
                news.UpdatedDate = DateTime.Now;
                news.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    news.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/news/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Đã cập nhật lại bài viết!", "success");
                return RedirectToAction("Index");
            }
            return View(news);
        }

        public ActionResult DelTrash(int? id)
        {
            NewsEntity news = db.NewsEntities.Find(id);
            news.Status = 0;

            news.UpdatedDate = DateTime.Now;
            news.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Đã chuyển vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }

        public ActionResult Undo(int? id)
        {
            NewsEntity news = db.NewsEntities.Find(id);
            news.Status = 2;

            news.UpdatedDate = DateTime.Now;
            news.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            NewsEntity news = db.NewsEntities.Find(id);
            news.Status = (news.Status == 1) ? 2 : 1;

            news.UpdatedDate = DateTime.Now;
            news.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = news.Status });
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            NewsEntity news = db.NewsEntities.Find(id);
            if (news == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(news);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            NewsEntity news = db.NewsEntities.Find(id);
            if (news == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(news);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsEntity news = db.NewsEntities.Find(id);
            string fileName = Server.MapPath("~/Public/img/news/" + news.Image);
            if (fileName != null)
            {
                System.IO.File.Delete(fileName);
            }
            db.NewsEntities.Remove(news);
            db.SaveChanges();
            Notification.set_flash("Đã xóa vĩnh viễn", "danger");
            return RedirectToAction("Trash");
        }
    }
}
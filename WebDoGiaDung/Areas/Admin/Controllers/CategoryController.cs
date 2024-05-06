using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private AppDbContext db = new AppDbContext(); 
        // GET: Admin/Category
        public ActionResult Index()
        {
            ViewBag.count_trash = db.CategoryEntities.Where(m => m.Status == 0).Count();
            var query = db.CategoryEntities.Where(m => m.Status != 0).ToList();
            return View(query);
        }

        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            CategoryEntity category = db.CategoryEntities.Find(id);
            if (category == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryEntity cate)
        {
            if (ModelState.IsValid)
            {
                String Slug = XString.ToAscii(cate.Name);
                CheckSlug check = new CheckSlug();

                if (!check.KiemTraSlug("Category", Slug, null))
                {
                    Notification.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Category");
                }

                cate.Slug = Slug;
                cate.CreatedDate = DateTime.Now;
                cate.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
                cate.UpdatedDate = DateTime.Now;
                cate.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = Slug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    cate.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/category/"), filename);
                    file.SaveAs(Strpath);
                }

                db.CategoryEntities.Add(cate);
                db.SaveChanges();
                Notification.set_flash("Danh mục đã được thêm!", "success");
                return RedirectToAction("Index", "Category");
            }
            Notification.set_flash("Có lỗi xảy ra khi thêm danh mục!", "warning");
            return View(cate);
        }

        public ActionResult Edit(int? id)
        {
            CategoryEntity category = db.CategoryEntities.Find(id);
            if (category == null)
            {
                Notification.set_flash("404!", "warning");
                return RedirectToAction("Index", "Category");
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryEntity category)
        {
            if (ModelState.IsValid)
            {
                String Slug = XString.ToAscii(category.Name);
                int ID = category.Id;
                if (db.CategoryEntities.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.NewsEntities.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong Tin tức, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.ProductEntities.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong Sản phẩm, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }

                category.Slug = Slug;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = Slug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    category.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/category/"), filename);
                    file.SaveAs(Strpath);
                }
                category.UpdatedDate = DateTime.Now;
                category.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Cập nhật thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //trash
        public ActionResult Trash()
        {
            var list = db.CategoryEntities.Where(m => m.Status == 0).ToList();

            return View(list);
        }

        public ActionResult DelTrash(int id)
        {
            CategoryEntity category = db.CategoryEntities.Find(id);
            if (category == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa vĩnh viễn!", "warning");
                return RedirectToAction("Index");
            }
            category.Status = 0;

            category.CreatedDate = DateTime.Now;
            category.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            category.UpdatedDate = DateTime.Now;
            category.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "warning");
            return RedirectToAction("Index");
        }

        public ActionResult ReTrash(int? id)
        {
            CategoryEntity category = db.CategoryEntities.Find(id);
            if (category == null)
            {
                Notification.set_flash("Không tồn tại danh mục!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            category.Status = 2;

            category.UpdatedDate = DateTime.Now;
            category.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Category");
        }

        //Change Status
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            CategoryEntity category = db.CategoryEntities.Find(id);
            category.Status = (category.Status == 1) ? 2 : 1;

            category.UpdatedDate = DateTime.Now;
            category.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = category.Status
            });
        }

        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            CategoryEntity category = db.CategoryEntities.Find(id);
            if (category == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryEntity category = db.CategoryEntities.Find(id);
            string fileName = Server.MapPath("~/Public/img/category/" + category.Image);
            if (fileName != null)
            {
                System.IO.File.Delete(fileName);
            }
            db.CategoryEntities.Remove(category);
            db.SaveChanges();
            Notification.set_flash("Đã xóa hoàn toàn danh mục!", "danger");
            return RedirectToAction("Trash", "Category");
        }
    }
}
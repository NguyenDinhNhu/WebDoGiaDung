using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;
using WebDoGiaDung.Models;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/Product
        public ActionResult Index()
        {
            ViewBag.countTrash = db.ProductEntities.Where(m => m.Status == 0).Count();
            var query = from p in db.ProductEntities
                        join c in db.CategoryEntities on p.CategoryId equals c.Id
                        where p.Status != 0 && p.CategoryId == c.Id
                        orderby p.CreatedDate descending
                        select new ProductCategory
                        {
                            ProductId = p.Id,
                            ProductImage = p.Image,
                            ProductName = p.Name,
                            ProductStatus = p.Status,
                            ProductDiscount = p.Discount,
                            IsHot = p.IsHot,
                            CategoryName = c.Name,
                        };
            return View(query.ToList());
        }

        public ActionResult Trash()
        {
            var query = from p in db.ProductEntities
                       join c in db.CategoryEntities on p.CategoryId equals c.Id
                       where p.Status == 0 && p.CategoryId == c.Id
                       orderby p.CreatedDate descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImage = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           IsHot = p.IsHot,
                           CategoryName = c.Name
                       };
            return View(query.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            ProductEntity product = db.ProductEntities.Find(id);
            if (product == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(product);
        }


        public ActionResult Create()
        {
            CategoryEntity category = new CategoryEntity();
            ViewBag.ListCat = new SelectList(db.CategoryEntities.Where(m => m.Status != 0), "ID", "Name", 0);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductEntity product)
        {
            ViewBag.ListCat = new SelectList(db.CategoryEntities.Where(m => m.Status != 0), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                product.Price = product.Price;
                product.PriceSale = product.PriceSale;

                String strSlug = XString.ToAscii(product.Name);
                product.Slug = strSlug;
                product.CreatedDate = DateTime.Now;
                product.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
                product.UpdatedDate = DateTime.Now;
                product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

                // Upload file
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    product.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/product/"), filename);
                    file.SaveAs(Strpath);
                }

                db.ProductEntities.Add(product);
                db.SaveChanges();
                Notification.set_flash("Thêm mới sản phẩm thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.ListCat = new SelectList(db.CategoryEntities.ToList(), "ID", "Name", 0);
            ProductEntity product = db.ProductEntities.Find(id);
            if (product == null)
            {
                Notification.set_flash("404!", "warning");
                return RedirectToAction("Index", "Product");
            }
            return View(product);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductEntity product)
        {
            ViewBag.ListCat = new SelectList(db.CategoryEntities.ToList(), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(product.Name);
                product.Slug = strSlug;

                product.UpdatedDate = DateTime.Now;
                product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

                // Upload file
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    product.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/img/product/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Đã cập nhật lại thông tin sản phẩm!", "success");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult DelTrash(int? id)
        {
            ProductEntity product = db.ProductEntities.Find(id);
            product.Status = 0;

            product.UpdatedDate = DateTime.Now;
            product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }

        public ActionResult Undo(int? id)
        {
            ProductEntity product = db.ProductEntities.Find(id);
            product.Status = 2;

            product.UpdatedDate = DateTime.Now;
            product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash");
            }
            ProductEntity product = db.ProductEntities.Find(id);
            if (product == null)
            {
                Notification.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash");
            }
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductEntity product = db.ProductEntities.Find(id);
            string fileName = Server.MapPath("~/Public/img/category/" + product.Image);
            if (fileName != null)
            {
                System.IO.File.Delete(fileName);
            }
            db.ProductEntities.Remove(product);
            db.SaveChanges();
            Notification.set_flash("Đã xóa vĩnh viễn sản phẩm!", "danger");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            ProductEntity product = db.ProductEntities.Find(id);
            product.Status = (product.Status == 1) ? 2 : 1;

            product.UpdatedDate = DateTime.Now;
            product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = product.Status });
        }
        [HttpPost]
        public JsonResult changeDiscount(int id)
        {
            ProductEntity product = db.ProductEntities.Find(id);
            product.Discount = (product.Discount == 1) ? 2 : 1;

            product.UpdatedDate = DateTime.Now;
            product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Discount = product.Discount });
        }

        [HttpPost]
        public JsonResult changeHot(int id)
        {
            ProductEntity product = db.ProductEntities.Find(id);
            product.IsHot = (product.IsHot == 1) ? 2 : 1;

            product.UpdatedDate = DateTime.Now;
            product.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { IsHot = product.IsHot });
        }
    }
}
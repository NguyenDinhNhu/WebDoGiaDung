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
    public class ProductImageController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/ProductImage
        public ActionResult Index(int id)
        {
            ViewBag.ListImage = db.ProductImageEntities.Where(m => m.ProductId == id).ToList();
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public JsonResult CreateListImage(ImageFile imagefile)
        {
            if (imagefile != null)
            {
                int index = 0;
                string nameImage;
                var strSlug = db.ProductEntities.Where(m => m.Id == imagefile.ProductId).FirstOrDefault();
                if(strSlug != null)
                {
                    nameImage = strSlug.Slug;
                }
                else
                {
                    return Json(false);
                }
                int countImg = db.ProductImageEntities.Where(pi => pi.ProductId == imagefile.ProductId).Count();
                if (countImg != 0)
                {
                    string maxName = db.ProductImageEntities.Where(pi => pi.ProductId == imagefile.ProductId).OrderByDescending(pi => pi.Id).Take(1).First().Image;
                    string[] lstCharac = maxName.Split('-');
                    string[] lastCharac = (lstCharac[lstCharac.Length - 1]).Split('.');
                    index = Int16.Parse(lastCharac[0]);
                }
                else
                {
                    index = 0;
                }
                ProductImageEntity productImage = new ProductImageEntity();
                foreach (HttpPostedFileBase file in imagefile.Files)
                {
                    productImage.ProductId = imagefile.ProductId;
                    productImage.CreatedDate = DateTime.Now;
                    productImage.CreatedBy = int.Parse(Session["Admin_ID"].ToString());
                    productImage.UpdatedDate = DateTime.Now;
                    productImage.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
                    productImage.Status = 1;
                    if (file != null && file.ContentLength > 0)
                    {
                        string filename = nameImage + '-' + (index + 1) + file.FileName.Substring(file.FileName.LastIndexOf("."));
                        productImage.Image = filename;
                        string strPath = Path.Combine(Server.MapPath("~/Public/img/product-img/"), filename);
                        file.SaveAs(strPath);
                    }
                    db.ProductImageEntities.Add(productImage);
                    db.SaveChanges();
                    index++;
                }
                Notification.set_flash("Thêm mới sản phẩm thành công!", "success");
            }
            return Json(true);
        }

        //[HttpGet]
        //public JsonResult EditImage(int id)
        //{
        //    ProductImageEntity productImage = db.ProductImageEntities.Find(id);
        //    productImage.Id = id;
        //    productImage.UpdatedDate = DateTime.Now;
        //    productImage.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());

        //    var file = Request.Files["Files"];
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        string fileName = db.ProductImageEntities.Where(m => m.Id == id).First().Image;
        //        string strPath = Path.Combine(Server.MapPath("~/Public/img/product-img/"), fileName);
        //        file.SaveAs(strPath);
        //    }
        //    db.Entry(productImage).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult Delete(int id)
        {
            ProductImageEntity productImage = db.ProductImageEntities.Find(id);
            string fileName = Server.MapPath("~/Public/img/product-img/" + productImage.Image);
            if (fileName != null)
            {
                System.IO.File.Delete(fileName);
            }
            db.ProductImageEntities.Remove(productImage);
            db.SaveChanges();
            Notification.set_flash("Xóa ảnh thành công!", "danger");
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            ProductImageEntity productImage = db.ProductImageEntities.Find(id);
            productImage.Status = (productImage.Status == 1) ? 2 : 1;

            productImage.UpdatedDate = DateTime.Now;
            productImage.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(productImage).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = productImage.Status });
        }
    }
}
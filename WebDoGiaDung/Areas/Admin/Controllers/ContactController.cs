using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;
using WebDoGiaDung.Library;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class ContactController : BaseController
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/Contact
        public ActionResult Index()
        {
            ViewBag.countTrash = db.ContactEntities.Where(m => m.Status == 0).Count();
            var query = db.ContactEntities.Where(m => m.Status != 0).ToList();
            return View(query);
        }

        public ActionResult Trash()
        {
            var query = db.ContactEntities.Where(m => m.Status == 0).ToList();
            return View(query);
        }

        public ActionResult Reply(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại liên hệ từ khách hàng!", "danger");
                return RedirectToAction("Index", "Contact");
            }
            ContactEntity contact = db.ContactEntities.Find(id);
            if (contact == null)
            {
                Notification.set_flash("Không tồn tại liên hệ từ khách hàng!", "danger");
                return RedirectToAction("Index", "Contact");
            }
            return View(contact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Reply(ContactEntity contact)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        contact.Status = 2;
        //        contact.RepliedDate = DateTime.Now;
        //        contact.RepliedBy = int.Parse(Session["Admin_ID"].ToString());

        //        String content = System.IO.File.ReadAllText(Server.MapPath("~/Areas/Admin/Views/Mail/G_Mail.html"));
        //        content = content.Replace("{{FullName}}", contact.FullName);
        //        content = content.Replace("{{Reply}}", contact.Reply);
        //        content = content.Replace("{{RQ}}", contact.Detail);
        //        content = content.Replace("{{AdminName}}", Session["Admin_Name"].ToString());
        //        String subject = "Phản hồi liên hệ từ Giadunghiendai.com";
        //        new MailHelper().SendMail(contact.Email, subject, content);

        //        db.Entry(contact).State = EntityState.Modified;
        //        db.SaveChanges();
        //        Notification.set_flash("Đã trả lời liên hệ!", "success");
        //        return RedirectToAction("Index");
        //    }
        //    return View(contact);
        //}
        public ActionResult Reply(ContactEntity contact)
        {
            if (ModelState.IsValid)
            {
                contact.Flag = 1;
                contact.RepliedDate = DateTime.Now;
                contact.RepliedBy = int.Parse(Session["Admin_ID"].ToString());

                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Đã trả lời liên hệ!", "success");
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        public ActionResult DelTrash(int id)
        {
            ContactEntity contact = db.ContactEntities.Find(id);
            if (contact == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            contact.Status = 0;
            contact.RepliedDate = DateTime.Now;
            contact.RepliedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(contact).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }

        public ActionResult ReTrash(int? id)
        {
            ContactEntity contact = db.ContactEntities.Find(id);
            if (contact == null)
            {
                Notification.set_flash("Không tồn tại danh mục!", "warning");
                return RedirectToAction("Trash", "Contact");
            }
            contact.Status = 1;
            contact.RepliedDate = DateTime.Now;
            contact.RepliedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(contact).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Contact");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            ContactEntity contact = db.ContactEntities.Find(id);
            if (contact == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactEntity contact = db.ContactEntities.Find(id);
            db.ContactEntities.Remove(contact);
            db.SaveChanges();
            Notification.set_flash("Đã xóa vĩnh viễn liên hệ!", "danger");
            return RedirectToAction("Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;
using WebDoGiaDung.Models;

namespace WebDoGiaDung.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private AppDbContext db = new AppDbContext();
        // GET: Admin/Order
        public ActionResult Index()
        {
            ViewBag.countTrash = db.OrdersEntities.Where(m => m.Trash == 1).Count();
            var query = from od in db.OrderDetailEntities
                        join o in db.OrdersEntities on od.OrderId equals o.Id
                        where o.Trash != 1
                        group od by new { od.OrderId, o } into groupb
                        orderby groupb.Key.o.CreatedDate descending
                        select new ListOrder
                        {
                            Id = groupb.Key.OrderId,
                            SAmount = groupb.Sum(m => m.Amount),
                            CustomerName = groupb.Key.o.CustomerName,
                            Status = groupb.Key.o.Status,
                            CreatedDate = groupb.Key.o.CreatedDate,
                            ExportDate = groupb.Key.o.ExportDate,
                        };
            return View(query.ToList());
        }

        public ActionResult Trash()
        {
            ViewBag.countTrash = db.OrdersEntities.Where(m => m.Status == 0).Count();
            var query = from od in db.OrderDetailEntities
                        join o in db.OrdersEntities on od.OrderId equals o.Id
                        where o.Trash == 1
                        group od by new { od.OrderId, o } into groupb
                        orderby groupb.Key.o.CreatedDate descending
                        select new ListOrder
                        {
                            Id = groupb.Key.OrderId,
                            SAmount = groupb.Sum(m => m.Amount),
                            CustomerName = groupb.Key.o.CustomerName,
                            Status = groupb.Key.o.Status,
                            CreatedDate = groupb.Key.o.CreatedDate,
                            ExportDate = groupb.Key.o.ExportDate,
                        };

            return View(query.ToList());
        }

        public ActionResult DelTrash(int? id)
        {
            OrdersEntity order = db.OrdersEntities.Find(id);
            order.Trash = 1;

            order.UpdatedDate = DateTime.Now;
            order.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Đã hủy đơn hàng!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }

        public ActionResult Undo(int? id)
        {
            OrdersEntity order = db.OrdersEntities.Find(id);
            order.Trash = 0;

            order.UpdatedDate = DateTime.Now;
            order.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại đơn hàng!", "warning");
                return RedirectToAction("Index", "Order");
            }
            OrdersEntity order = db.OrdersEntities.Find(id);
            if (order == null)
            {
                Notification.set_flash("Không tồn tại  đơn hàng!", "warning");
                return RedirectToAction("Index", "Order");
            }
            ViewBag.orderDetails = db.OrderDetailEntities.Where(m => m.OrderId == id).ToList();
            ViewBag.productOrder = db.ProductEntities.ToList();
            return View(order);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại đơn hàng!", "warning");
                return RedirectToAction("Trash", "Order");
            }
            OrdersEntity order = db.OrdersEntities.Find(id);
            if (order == null)
            {
                Notification.set_flash("Không tồn tại đơn hàng!", "warning");
                return RedirectToAction("Trash", "Order");
            }
            ViewBag.orderDetails = db.OrderDetailEntities.Where(m => m.OrderId == id).ToList();
            ViewBag.productOrder = db.ProductEntities.ToList();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrdersEntity order = db.OrdersEntities.Find(id);
            db.OrdersEntities.Remove(order);
            db.SaveChanges();
            Notification.set_flash("Đã xóa đơn hàng!", "success");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id, int op)
        {
            OrdersEntity order = db.OrdersEntities.Find(id);
            if (op == 1) 
            { 
                order.Status = 1; 
            } 
            else if (op == 2) 
            { 
                order.Status = 2; 
            } 
            else 
            { 
                order.Status = 3; 
                var orderDetail = db.OrderDetailEntities.Where(od => od.OrderId == order.Id).ToList();
                if (orderDetail != null)
                {
                    foreach(var od in orderDetail)
                    {
                        var product = db.ProductEntities.Where(p => p.Id == od.ProductId).FirstOrDefault();
                        if (product != null)
                        {
                            product.Quantity -= od.Quantity;
                        }
                    }
                }
            }

            order.ExportDate = DateTime.Now;
            order.UpdatedDate = DateTime.Now;
            order.UpdatedBy = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { s = order.Status, t = order.UpdatedDate.ToString() });
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;
using WebDoGiaDung.Models;

namespace WebDoGiaDung.Controllers
{
    public class CartController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: Cart
        public ActionResult Index()
        {
            if(Session["User_Name"] != null && Session["Cart"] != null)
            {
                int user_id = Convert.ToInt32(Session["User_ID"]);
                ViewBag.Info = db.UsersEntities.Where(m => m.Id == user_id).First();
            }
            return View();
        }
        public ActionResult Add(int pid, int qty)
        {
            var p = db.ProductEntities.Where(m => m.Status == 1 && m.Id == pid).First();

            var cart = Session["Cart"];
            if (cart == null)
            {
                var item = new ModelCart();
                item.ProductID = p.Id;
                item.Name = p.Name;
                item.Slug = p.Slug;
                item.Image = p.Image;
                item.Quantity = qty;
                item.Price = p.PriceSale;
                var list = new List<ModelCart>();
                list.Add(item);

                Session["Cart"] = list;
                return Json(new { result = 1 });
            }
            else
            {
                var list = (List<ModelCart>)cart;

                if (list.Exists(m => m.ProductID == pid))
                {
                    foreach (var item in list)
                    {
                        if (item.ProductID == pid)
                            item.Quantity += qty;
                        return Json(new { result = 2 });
                    }
                }
                else
                {
                    var item = new ModelCart();
                    item.ProductID = p.Id;
                    item.Name = p.Name;
                    item.Slug = p.Slug;
                    item.Image = p.Image;
                    item.Quantity = qty;
                    item.Price = p.Price;
                    list.Add(item);
                    return Json(new { result = 1 });
                }
            }
            return Json(new { result = 0 });
            
        }

        public JsonResult Update(int pid, String option)
        {
            var sCart = (List<ModelCart>)Session["Cart"];
            var getQty = db.ProductEntities.Where(m => m.Status == 1 && m.Id == pid).First();
            ModelCart c = sCart.First(m => m.ProductID == pid);
            if (c != null)
            {
                switch (option)
                {
                    case "add":
                        if(c.Quantity < getQty.Quantity)
                        {
                            c.Quantity++;
                        }
                        return Json(1);
                    case "minus":
                        if(c.Quantity > 1)
                        {
                            c.Quantity--;
                        }
                        return Json(2);
                    case "remove":
                        sCart.Remove(c);
                        if (sCart.Count() == 0)
                            Session.Remove("Cart");
                        return Json(3);
                    default:
                        break;
                }
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult Payment(String FullName, String Phone, String Email, String Address)
        {
            var order = new OrdersEntity();
            int user_id = Convert.ToInt32(Session["User_ID"]);
            order.Code = DateTime.Now.ToString("yyyyMMddhhMMss"); // yyyy-MM-dd hh:MM:ss
            order.UserId = user_id;
            order.CreatedDate = DateTime.Now;
            order.CustomerName = FullName;
            order.CustomerPhone = Phone;
            order.CustomerEmail = Email;
            order.CustomerAddress = Address;
            order.Status = 1;
            db.OrdersEntities.Add(order);
            db.SaveChanges();

            var OrderID = order.Id;

            foreach (var c in (List<ModelCart>)Session["Cart"])
            {
                var orderdetails = new OrderDetailEntity();
                orderdetails.OrderId = OrderID;
                orderdetails.ProductId = c.ProductID;
                orderdetails.Price = c.Price;
                orderdetails.Quantity = c.Quantity;
                orderdetails.Amount = c.Price * c.Quantity;
                db.OrderDetailEntities.Add(orderdetails);
            }
            db.SaveChanges();

            Session.Remove("Cart");
            Notification.set_flash("Bạn đã đặt hàng thành công!", "success");
            return Json(true);

        }
        public JsonResult CheckAuth()
        {
            if (Session["User_Name"] != null)
            {
                return Json(1);
            }
            return Json(0);
        }
    }
}
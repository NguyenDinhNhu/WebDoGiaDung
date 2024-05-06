using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext(); 
        public ActionResult Index()
        {
            ViewBag.HotProduct = db.ProductEntities.Where(m => m.Status == 1 && m.IsHot == 1).OrderByDescending(m => m.CreatedDate).Take(12).ToList();
            ViewBag.NewProduct = db.ProductEntities.Where(m => m.Status == 1).OrderByDescending(m => m.CreatedDate).Take(12).ToList();
            ViewBag.NewsList = db.NewsEntities.Where(m => m.Status == 1).OrderByDescending(m => m.CreatedDate).Take(4).ToList();
            ViewBag.CateProduct = db.CategoryEntities.Where(m => m.Status == 1).ToList();
            var query = db.ProductEntities.Where(m => m.Status == 1).ToList();
            return View("Index", query);
        }

        public ActionResult ProductNotFound()
        {
            return View();
        }

        public ActionResult ProductCategory(string slug, string sortOrder, string typeSort, int? page)
        {
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.Sort = "Sắp xếp";
            ViewBag.sortOrder = sortOrder;
            ViewBag.typeSort = typeSort;
            ViewBag.slug = slug;
            var getC = db.CategoryEntities.Where(m => m.Slug == slug && m.Status == 1).First();
            ViewBag.CateName = getC.Name;
            var listProduct = db.ProductEntities.Where(m => m.Status == 1 && m.CategoryId == getC.Id).ToList();
            if (listProduct.Count() == 0)
            {
                return RedirectToAction("ProductNotFound");
            }
            else
            {
                if (typeSort == "price" && sortOrder == "desc")
                {
                    ViewBag.Sort = "Giá cao đến thấp";
                    listProduct = db.ProductEntities.Where(m => m.Status == 1 && m.CategoryId == getC.Id)
                        .OrderByDescending(p => p.PriceSale).ToList();
                }
                else if (typeSort == "price" && sortOrder == "asc")
                {
                    ViewBag.Sort = "Giá thấp đến cao";
                    listProduct = db.ProductEntities.Where(m => m.Status == 1 && m.CategoryId == getC.Id)
                        .OrderBy(p => p.PriceSale).ToList();
                }
            }
            return View("ProductCategory", listProduct.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ProductDetail(string slug)
        {
            var getP = db.ProductEntities.Where(m => m.Slug == slug && m.Status == 1).First();
            ViewBag.ListOther = db.ProductEntities.Where(m => m.Status == 1 && m.CategoryId == getP.CategoryId && m.Id != getP.Id).OrderByDescending(m => m.CreatedDate).ToList();
            ViewBag.ImageList = db.ProductImageEntities.Where(m => m.Status == 1 && m.ProductId == getP.Id).ToList();
            return View("ProductDetail", getP);
        }

        public ActionResult News(int? page)
        {
            int pageSize = 6;
            int pageNumber = page ?? 1;
            var news = db.NewsEntities.Where(m => m.Status == 1).OrderByDescending(m => m.CreatedDate).ToList();
            ViewBag.ListNews = db.NewsEntities.Where(m => m.Status == 1).OrderByDescending(m => m.CreatedDate).Take(5).ToList();
            return View(news.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult NewsDetail(string slug)
        {
            var getN = db.NewsEntities.Where(m => m.Slug == slug && m.Status == 1).First();
            return View("NewsDetail", getN);
        }

        public ActionResult Introduction()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitContact(ContactEntity contact)
        {
            contact.FullName = Request.Form["Fullname"];
            contact.Email = Request.Form["Email"];
            contact.Phone = Convert.ToInt32(Request.Form["Phone"]);
            contact.Title = Request.Form["Title"];
            contact.Detail = Request.Form["Detail"];
            contact.Status = 1;
            contact.CreatedDate = DateTime.Now;
            contact.RepliedDate = DateTime.Now;

            db.ContactEntities.Add(contact);
            db.SaveChanges();
            Notification.set_flash("Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. Xin cảm ơn!", "success");
            return RedirectToAction("Contact", "Home");
        }

        public ActionResult Search(string key, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;
            var query = db.ProductEntities.Where(m => m.Status == 1).ToList();
            if (String.IsNullOrEmpty(key.Trim()))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                query = db.ProductEntities.Where(m => m.Status == 1 && m.Name.Contains(key)).ToList();
            }
            ViewBag.Count = query.Count();
            Session["keyword"] = key;
            return View(query.ToPagedList(pageNumber, pageSize));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebDoGiaDung
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
           
            routes.MapRoute(
                name: "ProductCategory",
                url: "danh-muc/{slug}",
                defaults: new { controller = "Home", action = "ProductCategory", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "AllNews",
               url: "tin-tuc",
               defaults: new { controller = "Home", action = "News", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "NewsDetail",
               url: "tin-tuc/{slug}",
               defaults: new { controller = "Home", action = "NewsDetail", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Introduction",
              url: "gioi-thieu",
              defaults: new { controller = "Home", action = "Introduction", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Cart",
               url: "gio-hang",
               defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //   name: "Checkout",
            //   url: "thanh-toan",
            //   defaults: new { controller = "Cart", action = "Checkout", id = UrlParameter.Optional }
            //);
            routes.MapRoute(
               name: "Search",
               url: "search",
               defaults: new { controller = "Home", action = "Search", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Contact",
              url: "lien-he",
              defaults: new { controller = "Home", action = "Contact", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "ProductDetail",
               url: "{slug}",
               defaults: new { controller = "Home", action = "ProductDetail", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

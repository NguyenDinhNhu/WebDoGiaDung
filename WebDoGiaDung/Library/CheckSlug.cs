using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDoGiaDung.Entities;

namespace WebDoGiaDung
{
    public class CheckSlug
    {
        AppDbContext db = new AppDbContext();
        public bool KiemTraSlug(String Table, String Slug, int? id)
        {
            switch (Table)
            {
                case "Category":
                    if (id != null)
                    {
                        if (db.CategoryEntities.Where(m => m.Slug == Slug && m.Id != id).Count() > 0)
                            return false;
                    }
                    else
                    {
                        if (db.CategoryEntities.Where(m => m.Slug == Slug).Count() > 0)
                            return false;
                    }
                    break;
                case "News":
                    break;
                case "Product":
                    break;
            }
            return true;


        }
    }
}
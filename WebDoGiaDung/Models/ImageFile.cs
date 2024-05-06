using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Models
{
    public class ImageFile
    {
        public int ProductId { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
    }
}
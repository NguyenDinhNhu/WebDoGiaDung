using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Models
{
    public class ProductCategory
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public int ProductStatus { get; set; }
        public int ProductDiscount { get; set; }
        public int IsHot { get; set; }
    }
}
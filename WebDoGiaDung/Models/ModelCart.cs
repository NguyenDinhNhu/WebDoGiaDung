﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Models
{
    public class ModelCart
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Models
{
    public class ListOrder
    {
        public int Id { get; set; }
        public String CustomerName { get; set; }
        public decimal SAmount { get; set; }
        public int? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExportDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Entities
{
    [Table("Contact")]
    public class ContactEntity
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string Detail { get; set; }
        public string Title { get; set; }
        public string Reply { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RepliedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? RepliedBy { get; set; }
        public int Flag { get; set; }
        public int Status { get; set; }
    }
}
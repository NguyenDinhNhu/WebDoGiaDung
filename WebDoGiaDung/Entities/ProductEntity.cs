using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Entities
{
    [Table("Product")]
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn nhập tên sản phẩm")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int CategoryId { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string ShortDesc { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập chi tiết sản phẩm")]
        public string DetailDesc { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string MetaKey { get; set; }
        public string MetaDesc { get; set; }
        public string MetaTitle { get; set; }
        public int Discount { get; set; }
        public int Status { get; set; }
        public int IsHot { get; set; }

    }
}
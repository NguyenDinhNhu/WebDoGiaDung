using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebDoGiaDung.Entities
{
    public class AppDbContext:DbContext
    {
        public AppDbContext() : base("name=StrConn") { }

        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<ProductEntity> ProductEntities { get; set; }
        public DbSet<ContactEntity> ContactEntities { get; set; }
        public DbSet<NewsEntity> NewsEntities { get; set; }
        public DbSet<ProductImageEntity> ProductImageEntities { get; set; }
        public DbSet<OrderDetailEntity> OrderDetailEntities { get; set; }
        public DbSet<OrdersEntity> OrdersEntities { get; set; }
        public DbSet<UsersEntity> UsersEntities { get; set; }
    }
}
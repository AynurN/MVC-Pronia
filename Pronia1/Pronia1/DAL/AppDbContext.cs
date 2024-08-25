﻿using Microsoft.EntityFrameworkCore;
using Pronia1.Models;

namespace Pronia1.DAL
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options){}
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slide> Slides { get; set; }
       public DbSet<Product> Products { get; set; }
       public DbSet<Category> Categories { get; set; }
       public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

    }
}

using ManshiBhatnagarPortfolio.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ManshiBhatnagarPortfolio.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
    }
}

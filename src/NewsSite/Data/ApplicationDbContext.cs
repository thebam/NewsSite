using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsSite.Models;

namespace NewsSite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Article> Article { get; set; }
        public DbSet<MediaKitFile> MediaKitFile { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Owner> Owner { get; set; }
        public DbSet<ArticleMediaKitFile> ArticleMediaKitFile { get; set; }
        public DbSet<ArticleTag> ArticleTag { get; set; }

        public DbSet<MediaKitFileTag> MediaKitFileTag { get; set; }
    }
}

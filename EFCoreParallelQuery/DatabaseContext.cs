using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCoreParallelQuery
{
    public class DatabaseContext : DbContext
    {
        public static DatabaseContext CreateSqliteInstance()
        {
            var options = new DbContextOptionsBuilder().UseSqlite("Data Source=:memory:").Options;
            var dbContext = new DatabaseContext(options);

            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        public static DatabaseContext CreateSqlServerInstance()
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<DatabaseContext>().Build();
            var connectionStrings = configuration.GetConnectionString("SqlServer");
            var options = new DbContextOptionsBuilder().UseSqlServer(connectionStrings).Options;

            var dbContext = new DatabaseContext(options);

            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }


        public virtual DbSet<BlogPost> BlogPost { get; set; }
        public virtual DbSet<BlogComment> BlogComment { get; set; }

        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.ToTable("BlobPost");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Title).IsRequired().IsUnicode();
            });

            modelBuilder.Entity<BlogComment>(entity =>
            {
                entity.ToTable("BlogComment");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Comment).IsRequired().IsUnicode();
            });
        }
    }

    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class BlogComment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string Comment { get; set; }
    }
}
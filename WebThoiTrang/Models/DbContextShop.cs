using Data;
using Microsoft.EntityFrameworkCore;

namespace WebThoiTrang.Models
{
    public class DbContextShop : DbContext
    {
        public DbContextShop(DbContextOptions<DbContextShop> options) : base(options) { }
        public DbContextShop() { }
        public DbSet<Category> categories { get; set; }
        public DbSet<Admin> admins { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }    
        public DbSet<User> users { get; set; }  
        public DbSet<Review> reviews { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);
        }
    }
    }


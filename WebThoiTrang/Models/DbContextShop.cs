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
<<<<<<< HEAD
=======
        
>>>>>>> aeefa36c15e904858c8700698f6022722429480b
        public DbSet<Review> reviews { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

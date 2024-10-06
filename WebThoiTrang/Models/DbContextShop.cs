using Data;
using Microsoft.EntityFrameworkCore;

namespace WebThoiTrang.Models
{
    public class DbContextShop : DbContext
    {
        public DbContextShop(DbContextOptions<DbContextShop> options) : base(options) { }
        public DbContextShop() { }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillDetail> BillDetail { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartDetail> CartDetail { get; set; }
        public DbSet<Color> Color { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<ProductDetail> ProductDetail { get; set; }
        public DbSet<ProductDetailColor> ProductDetailColor { get; set; }
        public DbSet<ProductDetailSize> ProductDetailSize { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<User> revUseriews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bảng Product Detail (không có quan hệ với Manufacturer)
            modelBuilder.Entity<ProductDetail>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductDetails)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            // Bảng Product Detail Color
            modelBuilder.Entity<ProductDetailColor>()
                .HasOne(p => p.ProductDetail)
                .WithMany(p => p.ProductDetailColors)
                .HasForeignKey(p => p.ProductDetailId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            modelBuilder.Entity<ProductDetailColor>()
                .HasOne(p => p.Color)
                .WithMany(p => p.ProductDetailColors)
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            // Bảng Product Detail Size
            modelBuilder.Entity<ProductDetailSize>()
                .HasOne(p => p.ProductDetail)
                .WithMany(p => p.ProductDetailSizes)
                .HasForeignKey(p => p.ProductDetailId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            modelBuilder.Entity<ProductDetailSize>()
                .HasOne(p => p.Size)
                .WithMany(p => p.ProductDetailSizes)
                .HasForeignKey(p => p.SizeId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            // Bảng Cart Detail
            modelBuilder.Entity<CartDetail>()
                .HasOne(p => p.ProductDetail)
                .WithMany(p => p.CartDetails)
                .HasForeignKey(p => p.ProductDetailId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            modelBuilder.Entity<CartDetail>()
                .HasOne(p => p.Cart)
                .WithMany(p => p.CartDetails)
                .HasForeignKey(p => p.CartId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            // Bảng Bill Detail
            modelBuilder.Entity<BillDetail>()
                .HasOne(p => p.ProductDetail)
                .WithMany(p => p.BillDetails)
                .HasForeignKey(p => p.ProductDetailId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            modelBuilder.Entity<BillDetail>()
                .HasOne(p => p.Bill)
                .WithMany(p => p.BillDetails)
                .HasForeignKey(p => p.BillId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            // Bảng Voucher
            modelBuilder.Entity<Voucher>()
                .HasMany(p => p.Bills)
                .WithOne(p => p.Voucher)
                .HasForeignKey(p => p.VoucherId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            // Bảng User
            modelBuilder.Entity<User>()
                .HasMany(p => p.Bills)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            //// Bảng Customer
            //modelBuilder.Entity<Customer>()
            //    .HasMany(p => p.Carts)
            //    .WithOne(p => p.Customer)
            //    .HasForeignKey(p => p.CustomerId)
            //    .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            modelBuilder.Entity<Customer>()
                .HasMany(p => p.Bills)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Manufacturer) // Quan hệ với Manufacturer
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                 .HasOne(p => p.ProductType)  // Một Product có một ProductType
                 .WithMany(pt => pt.Products)
                 .HasForeignKey(p => p.ProductTypeId)
                 .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ 1-1 giữa Product và Material
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Material)     // Product có một Material
                .WithOne(m => m.Product)     // Material có một Product
                .HasForeignKey<Product>(p => p.MaterialId);

            // Cấu hình mối quan hệ 1-1 giữa Customer và Cart
            modelBuilder.Entity<Cart>()
                .HasOne(p => p.Customers)    
                .WithOne(m => m.Carts)     
                .HasForeignKey<Cart>(p => p.CustomerId);
        }

    }
}


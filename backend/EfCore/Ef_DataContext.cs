using backend.EfCore;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.EfCore
{
    public class EF_DataContext : DbContext
    {
        public EF_DataContext(DbContextOptions<EF_DataContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        //public async Task<List<Order>> GetOrdersAsync()
        //{
        //    return await Orders
        //        .Include(o => o.OrderProducts)
        //            .ThenInclude(op => op.Product)
        //        .ToListAsync();
        //}

        public DbSet<Order_Product> OrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order_Product>()
                .HasKey(op => new { op.orderId, op.productId });

            modelBuilder.Entity<Order_Product>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.orderId);

            modelBuilder.Entity<Order_Product>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.productId);
        }




    }
}
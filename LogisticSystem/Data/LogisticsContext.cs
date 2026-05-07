using LogisticSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Data
{
    public class LogisticsContext: DbContext
    {
        public LogisticsContext() : base("name=LogisticsDB") { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductWarehouse> Stocks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Связь один один к user и клиента
            modelBuilder.Entity<Client>()
                .HasOptional(c => c.User)
                .WithOptionalPrincipal(u => u.Client)
                .WillCascadeOnDelete(false);

            // Составной ключ для OrderProduct (многие-ко-многим Order - Product)
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            // Настройка внешних ключей OrderProduct
            modelBuilder.Entity<OrderProduct>()
                .HasRequired(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasRequired(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId);

            // Составной ключ для ProductWarehouse (многие-ко-многим Product - Warehouse)
            modelBuilder.Entity<ProductWarehouse>()
                .HasKey(s => new { s.ProductId, s.WarehouseId });

            // Настройка внешних ключей Stock
            modelBuilder.Entity<ProductWarehouse>()
                .HasRequired(s => s.Product)
                .WithMany(p => p.ProductWarehouse)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<ProductWarehouse>()
                .HasRequired(s => s.Warehouse)
                .WithMany(w => w.ProductWarehouse)
                .HasForeignKey(s => s.WarehouseId);

            // Дополнительные ограничения (например, уникальность логина пользователя)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}

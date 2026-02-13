using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class MyDbContext : DbContext
    {
        //DbSet<ClassName> TableName
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<City> Cities { get; set; }

        
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"");
        //}

        private readonly string _connectionString;
        public MyDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //some base data for the database
            //modelBuilder.Entity<Category>().HasData(
            //    new Category { Id = 1, Name = "Övrigt" },
            //    new Category { Id = 2, Name = "Skor" },
            //    new Category { Id = 4, Name = "Tröjor" },
            //    new Category { Id = 3, Name = "Byxor" },
            //    new Category { Id = 5, Name = "Kjolar" },
            //    new Category { Id = 6, Name = "Klänningar" },
            //    new Category { Id = 7, Name = "Accessoarer" }
            //);
            
            //modelBuilder.Entity<DeliveryMethod>().HasData(
            //    new DeliveryMethod { Id = 1, Name = "Default/Bortagen" },
            //    new DeliveryMethod { Id = 2, Name = "Hämta i affär" },
            //    new DeliveryMethod { Id = 3, Name = "DHL" },
            //    new DeliveryMethod { Id = 4, Name = "Post nord" }
            //);

            //modelBuilder.Entity<PaymentMethod>().HasData(
            //    new PaymentMethod { Id = 1, Name = "Default/Bortagen" },
            //    new PaymentMethod { Id = 2, Name = "Klarna" },
            //    new PaymentMethod { Id = 3, Name = "Kort" },
            //    new PaymentMethod { Id = 4, Name = "Crypto" }
            //);
            //modelBuilder.Entity<Supplier>().HasData(
            //    new Supplier
            //    {
            //        Id = 1,
            //        Name = "Okänd/Borttagen Leverantör",
            //        Email = "default@supplier.se",
            //        PhoneNumber = "+46 70 000 00 00"
            //    },
            //    new Supplier
            //    {
            //        Id = 2,
            //        Name = "Stockholm Streetwear AB",
            //        Email = "sales@stockholmstreetwear.se",
            //        PhoneNumber = "+46 70 111 11 11"
            //    },
            //    new Supplier
            //    {
            //        Id = 3,
            //        Name = "Lindström Textil AB",
            //        Email = "support@lindstromtextil.se",
            //        PhoneNumber = "+46 70 111 22 33"
            //    },
            //    new Supplier
            //    {
            //        Id = 4,
            //        Name = "Fjäll & Stad Kläder AB",
            //        Email = "info@fjallstadklader.se",
            //        PhoneNumber = "+46 70 999 99 99"
            //    }
            //);


            modelBuilder.Entity<Order>()//Limit Cost to 10 total digits with 2 decimal places
                .Property(o => o.Cost)
                .HasPrecision(10, 2);       

            modelBuilder.Entity<Product>()
                .Property(o => o.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()//Setting up one to many relation because both ShippingAddress and BillingAddress lead to the Address table
                .HasOne(o => o.ShippingAddress)
                .WithMany(a => a.ShippingOrders)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>() 
                .HasOne(o => o.BillingAddress)
                .WithMany(a => a.BillingOrders)
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()//Sets up one to many relation and prevents cascading deletion of product upon category deletion 
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()//Automatically set date to the current date/time when the product is first inserted into the database
                .Property(p => p.DateAdded)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Order>()
                .Property(p => p.OrderDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

        }

        //How to set up a Database with Entity Framework, code first:
        //Tools -> NuGet Package Manager -> Package Manager Console      
        //Add-migration First
        //Update-database
    }
}

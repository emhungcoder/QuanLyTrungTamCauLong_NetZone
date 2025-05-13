using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Models;

namespace NetZone_BackEnd.Data
{
    public class NetZoneDbContext : IdentityDbContext<ApplicationUser>
    {
        public NetZoneDbContext(DbContextOptions<NetZoneDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProgressTracking> ProgressTrackings { get; set; }
        public DbSet<TrainingMaterial> TrainingMaterials { get; set; }

        public DbSet<UserPhone> UserPhones { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<CourseRegistration> CourseRegistrations { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<CourseCoach> CourseCoaches { get; set; }
        public DbSet<Coach> Coaches { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<OrderCoupon> OrderCoupons { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Court> Courts { get; set; }
        public DbSet<CourtBooking> CourtBookings { get; set; }

        public DbSet<SupportTicket> SupportTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================
            // Course - CourseCoach many-to-many
            // ============================
            modelBuilder.Entity<CourseCoach>()
                .HasKey(cc => new { cc.CourseId, cc.CoachId });

            modelBuilder.Entity<CourseCoach>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.CourseCoaches)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseCoach>()
                .HasOne(cc => cc.Coach)
                .WithMany()
                .HasForeignKey(cc => cc.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // Course - Lesson
            // ============================
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // Course - CourseRegistration
            // ============================
            modelBuilder.Entity<CourseRegistration>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.CourseRegistrations)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseRegistration>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.CourseRegistrations)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // CourseReview - Course & User
            // ============================
            modelBuilder.Entity<CourseReview>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseReview>()
                .HasOne(cr => cr.User)
                .WithMany()
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // ProductCategory many-to-many
            // ============================
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // OrderCoupon many-to-many
            // ============================
            modelBuilder.Entity<OrderCoupon>()
                .HasKey(oc => new { oc.OrderId, oc.CouponId });

            modelBuilder.Entity<OrderCoupon>()
                .HasOne(oc => oc.Order)
                .WithMany(o => o.Coupons)
                .HasForeignKey(oc => oc.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderCoupon>()
                .HasOne(oc => oc.Coupon)
                .WithMany()
                .HasForeignKey(oc => oc.CouponId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
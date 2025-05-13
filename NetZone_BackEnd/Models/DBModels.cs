using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NetZone_BackEnd.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string? IsActive { get; set; } = "On"; 
        public DateTime DateOfBirth { get; set; }



        public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new HashSet<CourseRegistration>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<CourtBooking> CourtBookings { get; set; } = new HashSet<CourtBooking>();
        public virtual Cart Cart { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
        public virtual ICollection<UserPhone> Phones { get; set; } = new List<UserPhone>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
    public class UserPhone
    {
        [Key]
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsDefault { get; set; } = false;

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        public string Address { get; set; }

        public bool IsDefault { get; set; } = false;

        // Liên kết với ApplicationUser
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required]
        public string CoachId { get; set; }
        public ApplicationUser Coach { get; set; }

        public int CourseCategoryId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public decimal Fee { get; set; }

        [Required]
        public int MaxStudents { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } = new HashSet<Lesson>();
        public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new HashSet<CourseRegistration>();
        public virtual ICollection<CourseReview> Reviews { get; set; } = new HashSet<CourseReview>();
        public virtual ICollection<CourseCoach> CourseCoaches { get; set; } = new HashSet<CourseCoach>();
        public virtual CourseCategory CourseCategory { get; set; }

    }

    public class CourseCategory
    {
        [Key]
        public int CourseCategoryId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }


    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        // Khóa ngoại đến buổi học (Lesson)
        [Required]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        // Khóa ngoại đến người dùng (student)
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Trạng thái điểm danh: Present, Absent, Late…
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Present";

        // Thời gian điểm danh
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class CourseRegistration
    {
        [Key]
        public int RegistrationId { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public virtual Certificate Certificate { get; set; }
    }

    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }

        [Required]
        public int RegistrationId { get; set; }
        public virtual CourseRegistration Registration { get; set; }

        [Required, MaxLength(500)]
        public string FileUrl { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.Now;
    }

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }
        [Required]
        public string IsActive { get; set; } = "On";

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public virtual ICollection<ProductReview> Reviews { get; set; } = new HashSet<ProductReview>();
        public virtual ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new HashSet<ProductCategory>();
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string ShippingAddress { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
        public virtual ICollection<OrderCoupon> Coupons { get; set; } = new HashSet<OrderCoupon>();
        public virtual ICollection<PaymentTransaction> Transactions { get; set; } = new HashSet<PaymentTransaction>();
    }

    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }

    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<CartItem> Items { get; set; } = new HashSet<CartItem>();
    }

    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class PaymentTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        [Required, MaxLength(50)]
        public string Provider { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string Status { get; set; }

        [MaxLength(200)]
        public string ResponseMessage { get; set; }
    }

    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class OrderCoupon
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int CouponId { get; set; }
        public virtual Coupon Coupon { get; set; }
    }

    public class CourseReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class ProductReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new HashSet<ProductCategory>();
    }

    public class ProductCategory
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; } = false;
    }

    public class Coach
    {
        [Key]
        public int CoachId { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public string Bio { get; set; }
    }

    public class CourseCoach
    {

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public int CoachId { get; set; }
        public virtual Coach Coach { get; set; }
    }

    public class Court
    {
        [Key]
        public int CourtId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        public virtual ICollection<CourtBooking> CourtBookings { get; set; } = new HashSet<CourtBooking>();
    }

    public class CourtBooking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int CourtId { get; set; }
        public virtual Court Court { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }

    // Optional: Chat or Ticket for support
    public class SupportTicket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; }

        [MaxLength(2000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsResolved { get; set; } = false;
    }
    public class ProgressTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; } // Ghi chú, nhận xét

        [MaxLength(1000)]
        public string Evaluation { get; set; } // Đánh giá

        [MaxLength(1000)]
        public string Suggestion { get; set; } // Đề xuất cải tiến

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class TrainingMaterial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Đường dẫn tới file/video
        [Required]
        public string Url { get; set; }

        // Loại tài liệu: Video, PDF, Slide, etc.
        [Required]
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }



}

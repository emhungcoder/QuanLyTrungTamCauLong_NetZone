//// DataSeeder.cs
//using System;
//using System.Linq;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using NetZone_BackEnd.Data;
//using NetZone_BackEnd.Models;

//namespace NetZone_BackEnd
//{
//    public static class DataSeeder
//    {
//        public static void Seed(IServiceProvider serviceProvider)
//        {
//            // 1. Tạo scope và lấy context + managers
//            using var scope = serviceProvider.CreateScope();
//            var context = scope.ServiceProvider.GetRequiredService<NetZoneDbContext>();
//            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//            // 2. Chạy migrations
//            context.Database.Migrate();

//            // ===== Part 0: Seed Roles =====
//            string[] roles = { "Admin", "Student", "Staff", "Coach" };
//            foreach (var role in roles)
//            {
//                if (!roleManager.RoleExistsAsync(role).Result)
//                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
//            }

//            // ===== Part 1: Seed Users & Assign Roles =====
//            if (userManager.FindByEmailAsync("admin@netzone.com").Result == null)
//            {
//                var admin = new ApplicationUser
//                {
//                    UserName = "admin@netzone.com",
//                    Email = "admin@netzone.com",
//                    FullName = "Admin User",
//                    IsActive = "On",
//                    DateOfBirth = new DateTime(1990, 1, 1),
//                    EmailConfirmed = true
//                };
//                userManager.CreateAsync(admin, "P@ssw0rd1").Wait();
//                userManager.AddToRoleAsync(admin, "Admin").Wait();
//            }

//            if (userManager.FindByEmailAsync("student@netzone.com").Result == null)
//            {
//                var student = new ApplicationUser
//                {
//                    UserName = "student@netzone.com",
//                    Email = "student@netzone.com",
//                    FullName = "Student One",
//                    IsActive = "On",
//                    DateOfBirth = new DateTime(2000, 5, 20),
//                    EmailConfirmed = true
//                };
//                userManager.CreateAsync(student, "P@ssword2").Wait();
//                userManager.AddToRoleAsync(student, "Student").Wait();

//                // seed phone & address for student
//                context.UserPhones.Add(new UserPhone
//                {
//                    PhoneNumber = "0123456789",
//                    IsDefault = true,
//                    UserId = student.Id
//                });
//                context.UserAddresses.Add(new UserAddress
//                {
//                    Address = "123 Main St, Bangkok",
//                    IsDefault = true,
//                    UserId = student.Id
//                });
//                context.SaveChanges();
//            }

//            // ===== Part 2: Seed Core Lookup Tables =====
//            if (!context.CourseCategories.Any())
//            {
//                context.CourseCategories.AddRange(
//                    new CourseCategory { Name = "Beginner" },
//                    new CourseCategory { Name = "Intermediate" },
//                    new CourseCategory { Name = "Advanced" }
//                );
//                context.SaveChanges();
//            }

//            if (!context.Categories.Any())
//            {
//                context.Categories.AddRange(
//                    new Category { Name = "Rackets" },
//                    new Category { Name = "Shoes" },
//                    new Category { Name = "Accessories" }
//                );
//                context.SaveChanges();
//            }

//            // ===== Part 3: Seed Coaches, Courses, Lessons =====
//            var adminUser = userManager.FindByEmailAsync("admin@netzone.com").Result;
//            if (!context.Coaches.Any())
//            {
//                context.Coaches.Add(new Coach
//                {
//                    UserId = adminUser.Id,
//                    Bio = "Certified badminton coach with 10 years experience"
//                });
//                context.SaveChanges();
//            }

//            if (!context.Courses.Any())
//            {
//                var coachId = context.Coaches.First().CoachId;
//                var catBegin = context.CourseCategories.First(c => c.Name == "Beginner").CourseCategoryId;
//                var catAdv = context.CourseCategories.First(c => c.Name == "Advanced").CourseCategoryId;

//                context.Courses.AddRange(
//                    new Course
//                    {
//                        Title = "Badminton for Beginners",
//                        Description = "Learn basic strokes and rules.",
//                        Fee = 500,
//                        MaxStudents = 20,
//                        CoachId = adminUser.Id,
//                        CourseCategoryId = catBegin
//                    },
//                    new Course
//                    {
//                        Title = "Advanced Smash Techniques",
//                        Description = "Master offensive plays.",
//                        Fee = 800,
//                        MaxStudents = 15,
//                        CoachId = adminUser.Id,
//                        CourseCategoryId = catAdv
//                    }
//                );
//                context.SaveChanges();

//                // liên kết CourseCoaches (M:N)
//                foreach (var course in context.Courses)
//                {
//                    context.CourseCoaches.Add(new CourseCoach
//                    {
//                        CourseId = course.CourseId,
//                        CoachId = context.Coaches.First().CoachId
//                    });
//                }
//                context.SaveChanges();
//            }

//            if (!context.Lessons.Any())
//            {
//                var c1 = context.Courses.First(c => c.Title.Contains("Beginners")).CourseId;
//                var c2 = context.Courses.First(c => c.Title.Contains("Advanced")).CourseId;

//                context.Lessons.AddRange(
//                    new Lesson
//                    {
//                        CourseId = c1,
//                        StartTime = DateTime.Now.AddDays(1).Date.AddHours(9),
//                        EndTime = DateTime.Now.AddDays(1).Date.AddHours(11),
//                        Location = "Court A"
//                    },
//                    new Lesson
//                    {
//                        CourseId = c2,
//                        StartTime = DateTime.Now.AddDays(2).Date.AddHours(14),
//                        EndTime = DateTime.Now.AddDays(2).Date.AddHours(16),
//                        Location = "Court B"
//                    }
//                );
//                context.SaveChanges();
//            }

//            // ===== Part 4: Seed Orders, Products, Reviews, etc. =====
//            // (tương tự, bạn có thể thêm phần seed cho Product, Cart, Order… không cần ID cố định)

//            // Ví dụ nhanh Product + Image + Review
//            if (!context.Products.Any())
//            {
//                var studentUser = userManager.FindByEmailAsync("student@netzone.com").Result;
//                context.Products.Add(new Product
//                {
//                    Name = "Yonex Racket",
//                    Description = "High quality racket.",
//                    Price = 1200,
//                    Stock = 10,
//                    IsActive = "On"
//                });
//                context.SaveChanges();

//                var p = context.Products.First();
//                context.ProductImages.Add(new ProductImage
//                {
//                    ProductId = p.ProductId,
//                    ImageUrl = "/images/racket1.jpg",
//                    IsMain = true
//                });
//                context.ProductReviews.Add(new ProductReview
//                {
//                    ProductId = p.ProductId,
//                    UserId = studentUser.Id,
//                    Rating = 4,
//                    Comment = "Good balance and feel."
//                });
//                context.SaveChanges();
//            }

//            // … tiếp các bảng còn lại theo nhu cầu của bạn
//        }
//    }
//}
// DataSeeder.cs
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;

namespace NetZone_BackEnd
{
    public static class DataSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            // 1. Tạo scope và lấy context + managers
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NetZoneDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 2. Chạy migrations
            context.Database.Migrate();

            // ===== Part 0: Seed Roles =====
            string[] roles = { "Admin", "Student", "Staff", "Coach" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }

            // ===== Part 1: Seed Users & Assign Roles =====
            ApplicationUser admin = userManager.FindByEmailAsync("admin@netzone.com").Result;
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@netzone.com",
                    Email = "admin@netzone.com",
                    FullName = "Admin User",
                    IsActive = "On",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    EmailConfirmed = true
                };
                userManager.CreateAsync(admin, "P@ssw0rd1").Wait();
            }
            if (!userManager.IsInRoleAsync(admin, "Admin").Result)
                userManager.AddToRoleAsync(admin, "Admin").Wait();

            ApplicationUser student = userManager.FindByEmailAsync("student@netzone.com").Result;
            if (student == null)
            {
                student = new ApplicationUser
                {
                    UserName = "student@netzone.com",
                    Email = "student@netzone.com",
                    FullName = "Student One",
                    IsActive = "On",
                    DateOfBirth = new DateTime(2000, 5, 20),
                    EmailConfirmed = true
                };
                userManager.CreateAsync(student, "P@ssword2").Wait();
            }
            if (!userManager.IsInRoleAsync(student, "Student").Result)
                userManager.AddToRoleAsync(student, "Student").Wait();

            // Phones & Addresses
            if (!context.UserPhones.Any(u => u.UserId == student.Id))
            {
                context.UserPhones.Add(new UserPhone
                {
                    PhoneNumber = "0123456789",
                    IsDefault = true,
                    UserId = student.Id
                });
                context.UserAddresses.Add(new UserAddress
                {
                    Address = "123 Main St, Bangkok",
                    IsDefault = true,
                    UserId = student.Id
                });
                context.SaveChanges();
            }

            // ===== Part 2: Seed Lookup Tables =====
            if (!context.CourseCategories.Any())
            {
                context.CourseCategories.AddRange(
                    new CourseCategory { Name = "Beginner" },
                    new CourseCategory { Name = "Intermediate" },
                    new CourseCategory { Name = "Advanced" }
                );
                context.SaveChanges();
            }
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Rackets" },
                    new Category { Name = "Shoes" },
                    new Category { Name = "Accessories" }
                );
                context.SaveChanges();
            }

            // ===== Part 3: Seed Coaches, Courses, Lessons =====
            if (!context.Coaches.Any())
            {
                context.Coaches.Add(new Coach
                {
                    UserId = admin.Id,
                    Bio = "Certified badminton coach with 10 years experience"
                });
                context.SaveChanges();
            }
            var coachEntity = context.Coaches.First();

            if (!context.Courses.Any())
            {
                var catBegin = context.CourseCategories.First(c => c.Name == "Beginner").CourseCategoryId;
                var catAdv = context.CourseCategories.First(c => c.Name == "Advanced").CourseCategoryId;

                context.Courses.AddRange(
                    new Course
                    {
                        Title = "Badminton for Beginners",
                        Description = "Learn basic strokes and rules.",
                        Fee = 500,
                        MaxStudents = 20,
                        CoachId = admin.Id,
                        CourseCategoryId = catBegin
                    },
                    new Course
                    {
                        Title = "Advanced Smash Techniques",
                        Description = "Master offensive plays.",
                        Fee = 800,
                        MaxStudents = 15,
                        CoachId = admin.Id,
                        CourseCategoryId = catAdv
                    }
                );
                context.SaveChanges();

                foreach (var course in context.Courses)
                {
                    context.CourseCoaches.Add(new CourseCoach
                    {
                        CourseId = course.CourseId,
                        CoachId = coachEntity.CoachId
                    });
                }
                context.SaveChanges();
            }

            if (!context.Lessons.Any())
            {
                var c1 = context.Courses.First(c => c.Title.Contains("Beginners")).CourseId;
                var c2 = context.Courses.First(c => c.Title.Contains("Advanced")).CourseId;

                context.Lessons.AddRange(
                    new Lesson
                    {
                        CourseId = c1,
                        StartTime = DateTime.Now.AddDays(1).Date.AddHours(9),
                        EndTime = DateTime.Now.AddDays(1).Date.AddHours(11),
                        Location = "Court A"
                    },
                    new Lesson
                    {
                        CourseId = c2,
                        StartTime = DateTime.Now.AddDays(2).Date.AddHours(14),
                        EndTime = DateTime.Now.AddDays(2).Date.AddHours(16),
                        Location = "Court B"
                    }
                );
                context.SaveChanges();
            }

            // ===== Part 4: Seed Registrations & Certificates =====
            if (!context.CourseRegistrations.Any())
            {
                var firstCourse = context.Courses.First().CourseId;
                context.CourseRegistrations.Add(new CourseRegistration
                {
                    UserId = student.Id,
                    CourseId = firstCourse,
                    RegistrationDate = DateTime.Now
                });
                context.SaveChanges();

                var reg = context.CourseRegistrations.First();
                context.Certificates.Add(new Certificate
                {
                    RegistrationId = reg.RegistrationId,
                    FileUrl = "/certs/cert1.pdf",
                    IssuedAt = DateTime.Now
                });
                context.SaveChanges();
            }

            // ===== Part 5: Seed Reviews =====
            if (!context.CourseReviews.Any())
            {
                var regCourse = context.CourseRegistrations.First().CourseId;
                context.CourseReviews.Add(new CourseReview
                {
                    CourseId = regCourse,
                    UserId = student.Id,
                    Rating = 5,
                    Comment = "Great intro course!",
                    CreatedAt = DateTime.Now
                });
                context.SaveChanges();
            }

            // ===== Part 6: Seed Products, Images & Reviews =====
            if (!context.Products.Any())
            {
                context.Products.Add(new Product
                {
                    Name = "Yonex Racket",
                    Description = "High quality racket.",
                    Price = 1200,
                    Stock = 10,
                    IsActive = "On"
                });
                context.SaveChanges();

                var prod = context.Products.First();
                context.ProductImages.Add(new ProductImage
                {
                    ProductId = prod.ProductId,
                    ImageUrl = "/images/racket1.jpg",
                    IsMain = true
                });
                context.ProductReviews.Add(new ProductReview
                {
                    ProductId = prod.ProductId,
                    UserId = student.Id,
                    Rating = 4,
                    Comment = "Good balance and feel.",
                    CreatedAt = DateTime.Now
                });
                context.SaveChanges();
            }

            // ===== Part 7: Seed Cart & Items =====
            if (!context.Carts.Any())
            {
                context.Carts.Add(new Cart
                {
                    UserId = student.Id
                });
                context.SaveChanges();

                var cart = context.Carts.First();
                var prod = context.Products.First();
                context.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = prod.ProductId,
                    Quantity = 1
                });
                context.SaveChanges();
            }

            // ===== Part 8: Seed Coupons, Orders & Transactions =====
            if (!context.Coupons.Any())
            {
                context.Coupons.Add(new Coupon
                {
                    Code = "WELCOME10",
                    DiscountPercent = 10,
                    ValidFrom = DateTime.Now.AddDays(-10),
                    ValidUntil = DateTime.Now.AddMonths(1),
                    IsActive = true
                });
                context.SaveChanges();
            }
            if (!context.Orders.Any())
            {
                var coupon = context.Coupons.First();
                var prod = context.Products.First();
                context.Orders.Add(new Order
                {
                    UserId = student.Id,
                    ShippingAddress = "123 Main St, Bangkok",
                    OrderDate = DateTime.Now,
                    TotalAmount = prod.Price
                });
                context.SaveChanges();

                var order = context.Orders.First();
                context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = prod.ProductId,
                    Quantity = 1,
                    UnitPrice = prod.Price
                });
              
                context.OrderCoupons.Add(new OrderCoupon
                {
                    OrderId = order.OrderId,
                    CouponId = coupon.CouponId
                });
                context.SaveChanges();
            }

            // ===== Part 9: Seed Notifications =====
            if (!context.Notifications.Any())
            {
                context.Notifications.Add(new Notification
                {
                    UserId = student.Id,
                    Title = "Welcome",
                    Message = "Thanks for joining!",
                    CreatedAt = DateTime.Now
                });
                context.SaveChanges();
            }

            // ===== Part 10: Seed Courts & Bookings =====
            if (!context.Courts.Any())
            {
                context.Courts.AddRange(
                    new Court { Name = "Court A", Location = "First Floor" },
                    new Court { Name = "Court B", Location = "Second Floor" }
                );
                context.SaveChanges();

                var court = context.Courts.First();
                context.CourtBookings.Add(new CourtBooking
                {
                    CourtId = court.CourtId,
                    UserId = student.Id,
                    BookingDate = DateTime.Now.Date.AddDays(1),
                    StartTime = DateTime.Now.Date.AddDays(1).AddHours(9),
                    EndTime = DateTime.Now.Date.AddDays(1).AddHours(10)
                });
                context.SaveChanges();
            }

            // ===== Part 11: Seed Support Tickets =====
            if (!context.SupportTickets.Any())
            {
                context.SupportTickets.Add(new SupportTicket
                {
                    UserId = student.Id,
                    Subject = "Account issue",
                    Content = "I can't login.",
                    CreatedAt = DateTime.Now,
                    IsResolved = false
                });
                context.SaveChanges();
            }
        }
    }
}

﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NetZone_BackEnd.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ProductDto Product { get; set; }
    }

    public class ProductDto
    {
        public string ProductName { get; set; }
    }

    [NotMapped]
    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
    [NotMapped]
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    [NotMapped]
    public class ProductCreateModel
    {
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile ProductImage { get; set; } = null!;
        public int CategoryID { get; set; }
    }
    [NotMapped]
    public class ProductUpdateModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile? ProductImage { get; set; }
    }
    [NotMapped]
    public class UpdateProfileModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
    [NotMapped]
    public class RegisterModel
    {
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
     }
[NotMapped]
public class EmployeeDto
{
    public string? Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? TinhTrangHoatDong { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
}
[NotMapped]
public class AuthResponse
{
    public string Token { get; set; }
}
    public class UserProfileModel
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string RoleName { get; set; } = "";
    }
    public class ProgressTrackingDto
    {
        public int LessonId { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }
        public string Evaluation { get; set; }
        public string Suggestion { get; set; }
    }
    public class TrainingMaterialDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }

}

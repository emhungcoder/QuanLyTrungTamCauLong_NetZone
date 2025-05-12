using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetZone_BackEnd.Data;
using NetZone_BackEnd.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetZone_BackEnd.Controllers.Admin
{
    [Route("api/admin/promotions")]
    [ApiController]
    public class AdminPromotionController : ControllerBase
    {
        private readonly NetZoneDbContext _context;

        public AdminPromotionController(NetZoneDbContext context)
        {
            _context = context;
        }

        // POST: api/admin/promotions
        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] Coupon coupon)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPromotion), new { id = coupon.CouponId }, coupon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/admin/promotions
        [HttpGet]
        public async Task<IActionResult> GetAllPromotions()
        {
            var promotions = await _context.Coupons.ToListAsync();
            return Ok(promotions);
        }

        // GET: api/admin/promotions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotion(int id)
        {
            var promotion = await _context.Coupons.FindAsync(id);
            if (promotion == null) return NotFound("Promotion not found");

            return Ok(promotion);
        }

        // PUT: api/admin/promotions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] Coupon coupon)
        {
            if (id != coupon.CouponId) return BadRequest("ID mismatch");

            var existingCoupon = await _context.Coupons.FindAsync(id);
            if (existingCoupon == null) return NotFound("Promotion not found");

            try
            {
                existingCoupon.Code = coupon.Code;
                existingCoupon.DiscountAmount = coupon.DiscountAmount;
                existingCoupon.DiscountPercent = coupon.DiscountPercent;
                existingCoupon.ValidFrom = coupon.ValidFrom;
                existingCoupon.ValidUntil = coupon.ValidUntil;
                existingCoupon.IsActive = coupon.IsActive;

                await _context.SaveChangesAsync();
                return Ok(existingCoupon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // DELETE: api/admin/promotions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.Coupons.FindAsync(id);
            if (promotion == null) return NotFound("Promotion not found");

            try
            {
                _context.Coupons.Remove(promotion);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/admin/promotions/check/{code}
        [HttpGet("check/{code}")]
        public async Task<IActionResult> CheckPromotionUsage(string code)
        {
            var coupon = await _context.Coupons
                .Where(c => c.Code == code && c.IsActive &&
                            (!c.ValidUntil.HasValue || c.ValidUntil >= DateTime.Now))
                .FirstOrDefaultAsync();

            if (coupon == null) return NotFound("Promotion code is invalid or expired");

            return Ok(new
            {
                coupon.CouponId,
                coupon.Code,
                coupon.DiscountAmount,
                coupon.DiscountPercent,
                coupon.ValidFrom,
                coupon.ValidUntil,
                coupon.IsActive
            });
        }
    }
}

using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace Microservices.Services.CouponAPI.Services
{
    public interface ICouponService
    {
        List<Coupon> GetAllCoupons();
        Coupon GetCoupon(Guid id);
        Coupon CreateCoupon(CouponDto couponDto);
        Coupon UpdateCoupon(Guid id, CouponDto couponDto);
        bool DeleteCoupon(Guid id);
        bool UpdateCoupon(Guid id, JsonPatchDocument<CouponDto> patchDto);
    }
}

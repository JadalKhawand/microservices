using AutoMapper;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models.Dto;
using Microservices.Services.CouponAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Azure;

namespace Microservices.Services.CouponAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CouponService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<Coupon> GetAllCoupons()
        {
                List<Coupon> objList = _db.Coupons.ToList();
                return objList;
        }
        
        public Coupon GetCoupon(Guid id)
        {
            var coupon = _db.Coupons.FirstOrDefault(u => u.CouponId == id);
            if (coupon == null)
                throw new Exception("Coupon isn't found");

            return coupon;
        }

        public Coupon CreateCoupon(CouponDto couponDto)
        {
           
                var coupon = _mapper.Map<Coupon>(couponDto);
                coupon.CouponId = Guid.NewGuid();
                _db.Coupons.Add(coupon);
                _db.SaveChanges();
                return coupon;
            
        }

        public Coupon UpdateCoupon(Guid id, CouponDto couponDto)
        {
            
                var existingCoupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
            if (existingCoupon == null)
                return null;

                var updatedCoupon = _mapper.Map(couponDto, existingCoupon);
                _db.Coupons.Update(updatedCoupon);
                _db.SaveChanges();
                return updatedCoupon;
            
        }

        public bool DeleteCoupon(Guid id)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(u => u.CouponId == id);
                if (coupon == null)
                    return false;

                _db.Coupons.Remove(coupon);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCoupon(Guid id, JsonPatchDocument<CouponDto> patchDto)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                    return false;

                var couponDto = _mapper.Map<CouponDto>(coupon);
                patchDto.ApplyTo(couponDto);
                _mapper.Map(couponDto, coupon);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
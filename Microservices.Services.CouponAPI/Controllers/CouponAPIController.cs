using AutoMapper;
using Azure;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupons")]

    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _response;
        private IMapper _mapper;
        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDTO();
        }
        // get all 
        [HttpGet]
        public ResponseDTO GetAllCoupons()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = objList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                
            }
            return _response;
        }
        // get a single coupon by id
        [HttpGet("{id:Guid}")]
        public ResponseDTO GetCoupon(Guid id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        
        // create a new coupon
        [HttpPost]
        public ResponseDTO CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                obj.CouponId = Guid.NewGuid();
                _db.Coupons.Add(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.Message = "Coupon created successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                _response.InnerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
            return _response;
        }
        // update an existing coupon
        [HttpPut]
        public ResponseDTO UpsertCoupon([FromQuery] Guid id, [FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                obj.CouponId = id;
                _db.Coupons.Update(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        // Delete an existing coupon
        [HttpDelete("{id:Guid}")]
       
        public ResponseDTO DeleteCoupon(Guid id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u=>u.CouponId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();
                _response.Result = obj;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPatch("{id:Guid}")]
        public ResponseDTO UpdateCoupon(Guid id, JsonPatchDocument<CouponDto> patchDto)
        {
            try
            {
                if (patchDto == null)
                {
                    _response.IsSuccess = false;
                    return _response;
                }

                var coupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    return _response;
                }

                var couponDto = _mapper.Map<CouponDto>(coupon);
                patchDto.ApplyTo(couponDto, ModelState);

                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    return _response;
                }

                _mapper.Map(couponDto, coupon);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(coupon);
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return _response;
            }
        }


    }
}

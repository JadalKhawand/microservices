using AutoMapper;
using Azure;
using FakeItEasy;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;
using Microservices.Services.CouponAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupons")]

    public class CouponAPIController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private ResponseDTO _response;
        public CouponAPIController(ICouponService couponService)
        {
            _couponService = couponService;
            _response = new ResponseDTO();
        }
        // get all 
        [HttpGet]
        public IActionResult GetAllCoupons()
        {
            try
            {
                var coupons = _couponService.GetAllCoupons();
                _response.Result = coupons;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(ex.Message);
            }
            return Ok(_response.Result);
        }
        // get a single coupon by id
        [HttpGet("{id:Guid}")]
        public IActionResult GetCoupon(Guid id)
        {
            try
            {
                Coupon obj = _couponService.GetCoupon(id);
                _response.Result = obj;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(ex.Message);
            }
            return Ok(_response.Result);
        }
        
        // create a new coupon
        [HttpPost]
        public ResponseDTO CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _couponService.CreateCoupon(couponDto);
                _response.Result = coupon;
                _response.Message = "Coupon created successfully";
                _response.IsSuccess = true;
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
                var resUpsert = _couponService.UpdateCoupon(id, couponDto);
                if(resUpsert == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon wasn't found";
                }
                _response.IsSuccess = true;
                _response.Message = "Coupon Updated successfully";
                _response.Result = resUpsert;
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
                var resDelete = _couponService.DeleteCoupon(id);
                if(resDelete == false)
                {
                    _response.IsSuccess = false;
                }
                _response.Message = "Coupon Deleted Successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPatch("{id:Guid}")]
        public IActionResult UpdateCoupon(Guid id, JsonPatchDocument<CouponDto> patchDto)
        {
            try
            {
                if (patchDto == null)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                var coupon = _couponService.UpdateCoupon(id, patchDto);
                if (coupon == false)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }


                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                return Ok("coupon updated successfully");

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest();
            }
        }
       
    }
}

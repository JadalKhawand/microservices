using AutoMapper;
using Azure;
using FakeItEasy;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;
using Microservices.Services.CouponAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

                if (coupons == null)
                    return NotFound(); 

                _response.Result = coupons;
                return Ok(coupons); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database"); 
            }
        }

        // get a single coupon by id
        [HttpGet("{id:Guid}")]
        public IActionResult GetCoupon(Guid id)
        {
            try
            {
                var coupon = _couponService.GetCoupon(id);

                if (coupon == null)
                    return NotFound(); 

                return Ok(coupon); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving coupon"); 
            }
        }


        // create a new coupon
        [HttpPost]
        public  ActionResult<Coupon> CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                if (couponDto == null)
                    return BadRequest(); 

                var createdCoupon = _couponService.CreateCoupon(couponDto);

                if (createdCoupon == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error creating new coupon"); 

                return CreatedAtAction(nameof(CreateCoupon), createdCoupon); // 201 Created
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new coupon"); 
            }
        }

        // update an existing coupon
        [HttpPut("{id:Guid}")]
        public ActionResult<Coupon> UpdateEmployee(Guid id, CouponDto couponDto)
        {
            try
            {
                var couponToUpdate =  _couponService.GetCoupon(id);

                if (couponToUpdate == null)
                    return NotFound($"Coupon with Id = {id} not found");

                var updatedCoupon =  _couponService.UpdateCoupon(id, couponDto);

                if (updatedCoupon == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error updating data");

                return Ok(updatedCoupon);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data"); 
            }
        }


        // Delete an existing coupon
        [HttpDelete("{id:Guid}")]
        public ActionResult<Coupon> DeleteCoupon(Guid id)
        {
            try
            {
                var couponToDelete = _couponService.GetCoupon(id);

                if (couponToDelete == null)
                {
                    return NotFound($"Coupon with Id = {id} not found"); 
                }

                var deleteResult = _couponService.DeleteCoupon(id);

                if (!deleteResult)
                {
                    return BadRequest("Error deleting coupon"); 
                }

                return Ok(couponToDelete); 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting coupon");
            }
        }




        [HttpPatch("{id:Guid}")]
        public IActionResult UpdateCoupon(Guid id, JsonPatchDocument<CouponDto> patchDto)
        {
            try
            {
                var couponToUpdate = _couponService.GetCoupon(id);

                if (couponToUpdate == null)
                    return NotFound($"Employee with Id = {id} not found");

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

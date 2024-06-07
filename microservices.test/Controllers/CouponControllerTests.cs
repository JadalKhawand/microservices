using Microservices.Services.CouponAPI.Controllers;
using Microservices.Services.CouponAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using AutoMapper;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microservices.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Microservices.Services.CouponAPI.Services;
using AutoFixture.AutoNSubstitute;
using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;

namespace microservices.test.Controllers
{
    public class CouponControllerTests
    {
        [Fact]
        public void CouponController_GetAllCoupons_ReturnsOK()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var fakeCoupons = fixture.CreateMany<Coupon>(2).ToList();


            var couponService = Substitute.For<ICouponService>();
            couponService.GetAllCoupons().Returns(fakeCoupons);

            var apiController = new CouponAPIController(couponService);

            // Act
            var result = apiController.GetAllCoupons();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var coupons = Assert.IsAssignableFrom<List<Coupon>>(okResult.Value); 
            var count = coupons.Count;
            Assert.Equal(2, count);
        }



        [Fact]
        public void CouponController_CreateCoupon_ReturnsResponseDTO()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var couponDto = fixture.Create<CouponDto>();
            var createdCoupon = fixture.Create<Coupon>();
            var couponService = Substitute.For<ICouponService>();

            couponService.CreateCoupon(couponDto).Returns(createdCoupon);

            var controller = new CouponAPIController(couponService);

            // Act
            var result = controller.CreateCoupon(couponDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Coupon created successfully", result.Message);
            Assert.Equal(createdCoupon, result.Result);
        }

        
        [Fact]
        public void CouponController_UpsertCoupon_ReturnsResponseDTO()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var id = new Guid();
            var couponDto = fixture.Create<CouponDto>();
            var updatedCoupon = fixture.Create<Coupon>();
            var couponService = Substitute.For<ICouponService>();

            couponService.UpdateCoupon(id, couponDto).Returns(updatedCoupon);

            var controller = new CouponAPIController(couponService);

            // Act
            var response = controller.UpsertCoupon(id, couponDto);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("Coupon Updated successfully", response.Message);
            Assert.Equal(updatedCoupon, response.Result);
        }
        
        [Fact]
        public void CouponController_DeleteCoupon_ReturnsResponseDTO()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var coupon = fixture.Create<Coupon>();
            var couponService = Substitute.For<ICouponService>();

            couponService.DeleteCoupon(coupon.CouponId).Returns(true);

            var controller = new CouponAPIController(couponService);

            // Act
            var response = controller.DeleteCoupon(coupon.CouponId);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("Coupon Deleted Successfully", response.Message);
        }
        /*
        [Fact]
        public void CouponController_GetCoupon_ReturnsCoupon()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "FakeDb")
                .Options;

            var fakeDb = new AppDbContext(dbContextOptions);
            var fakeMapper = A.Fake<IMapper>();

            var controller = new CouponAPIController(fakeDb, fakeMapper);

            var fakeCoupon = new Coupon
            {
                CouponId = Guid.NewGuid(),
                CouponCode = "CODE",
                DiscountAmount = 10,
            };
            fakeDb.Coupons.Add(fakeCoupon);
            fakeDb.SaveChanges();

            // Act
            var response = controller.GetCoupon(fakeCoupon.CouponId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var couponDto = Assert.IsAssignableFrom<CouponDto>(okResult.Value); 
        }
        [Fact]
        public void CouponController_UpdateCoupon_ReturnsOkResult_WithValidPatch()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "FakeDb")
                .Options;

            using (var fakeDb = new AppDbContext(dbContextOptions))
            {
                var fakeMapper = A.Fake<IMapper>();

                var controller = new CouponAPIController(fakeDb, fakeMapper);

                var fakeCoupon = new Coupon
                {
                    CouponId = Guid.NewGuid(),
                    CouponCode = "CODE",
                    DiscountAmount = 10,
                    MinAmount = 10
                };
                fakeDb.Coupons.Add(fakeCoupon);
                fakeDb.SaveChanges();

                var patchDoc = new JsonPatchDocument<CouponDto>();
                patchDoc.Replace(c => c.MinAmount, 100);

                // Act
                var response = controller.UpdateCoupon(fakeCoupon.CouponId, patchDoc);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(response);
                var updatedCoupon = Assert.IsType<Coupon>(okResult.Value);
                Assert.Equal(100, updatedCoupon.MinAmount);
            }
        }


        */
    }

}

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
        public void CouponController_CreateCoupon_ReturnsActionResult()
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
            Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdAtActionResult = (CreatedAtActionResult)result.Result;
            Assert.Equal(nameof(CouponAPIController.CreateCoupon), createdAtActionResult.ActionName);
        }



        [Fact]
        public void CouponController_UpdateCoupon_ReturnsActionResult()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var couponDto = fixture.Create<CouponDto>();
            var couponToUpdate = fixture.Create<Coupon>();
            var updatedCoupon = fixture.Create<Coupon>();
            var couponService = Substitute.For<ICouponService>();

            couponService.GetCoupon(id).Returns(couponToUpdate);
            couponService.UpdateCoupon(id, couponDto).Returns(updatedCoupon);

            var controller = new CouponAPIController(couponService);

            // Act
            var result = controller.UpdateEmployee(id, couponDto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okObjectResult = (OkObjectResult)result.Result;
            Assert.Equal(updatedCoupon, okObjectResult.Value);
        }

        [Fact]
        public void CouponController_DeleteCoupon_ExistingCoupon_ReturnsOk()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var couponToDelete = fixture.Create<Coupon>();
            var couponService = Substitute.For<ICouponService>();

            couponService.GetCoupon(id).Returns(couponToDelete);
            couponService.DeleteCoupon(id).Returns(true); 

            var controller = new CouponAPIController(couponService);

            // Act
            var result = controller.DeleteCoupon(id);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okObjectResult = (OkObjectResult)result.Result;
            Assert.Equal(couponToDelete, okObjectResult.Value);
        }

        [Fact]
        public void CouponController_DeleteCoupon_NonExistingCoupon_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var couponService = Substitute.For<ICouponService>();

            couponService.GetCoupon(id).Returns((Coupon)null); // Simulate non-existing coupon

            var controller = new CouponAPIController(couponService);

            // Act
            var result = controller.DeleteCoupon(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void CouponController_DeleteCoupon_FailureToDelete_ReturnsBadRequest()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var couponToDelete = fixture.Create<Coupon>();
            var couponService = Substitute.For<ICouponService>();

            couponService.GetCoupon(id).Returns(couponToDelete);
            couponService.DeleteCoupon(id).Returns(false); // Simulate deletion failure

            var controller = new CouponAPIController(couponService);

            // Act
            var result = controller.DeleteCoupon(id);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
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

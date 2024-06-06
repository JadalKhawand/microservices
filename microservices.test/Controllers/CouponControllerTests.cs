﻿using Microservices.Services.CouponAPI.Controllers;
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

namespace microservices.test.Controllers
{
    public class CouponControllerTests
    {
        [Fact]
        public void CouponController_GetAllCoupons_ReturnsOK()
        {
            // Arrange

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDb")
            .Options;

            var fakeDb = new AppDbContext(dbContextOptions);
            var fakeMapper = A.Fake<IMapper>();

            var fakeCoupons = new List<Coupon>
            {
                new Coupon { CouponId = Guid.NewGuid(), CouponCode = "CODE1", DiscountAmount = 10, MinAmount = 20 },
                new Coupon { CouponId = Guid.NewGuid(), CouponCode = "CODE2", DiscountAmount = 20, MinAmount = 40 }
            };

            fakeDb.Coupons.AddRange(fakeCoupons);
            fakeDb.SaveChanges();

            var controller = new CouponAPIController(fakeDb, fakeMapper);

            // Act

            var results = controller.GetAllCoupons();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(results);
            var coupons = Assert.IsAssignableFrom<IEnumerable<Coupon>>(okResult.Value);
            var count = coupons.Count();
            Assert.NotNull(results);
            Assert.Equal(2, count);

        }

        [Fact]
        public void CouponController_CreateCoupon_ReturnsResponseDTO()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "FakeDb")
                .Options;

            var fakeDb = new AppDbContext(dbContextOptions);
            var fakeMapper = A.Fake<IMapper>();

            var controller = new CouponAPIController(fakeDb, fakeMapper);

            var fakeCouponDto = new CouponDto
            {
                CouponCode = "CODE",
                DiscountAmount = 10,
                MinAmount = 20
            };

            var fakeCoupon = A.Fake<Coupon>();
            A.CallTo(() => fakeMapper.Map<Coupon>(fakeCouponDto)).Returns(fakeCoupon);

            // Act
            var response = controller.CreateCoupon(fakeCouponDto);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<ResponseDTO>(response);

            if (response.IsSuccess)
            {
                Assert.Equal("Coupon created successfully", response.Message);
            }
            else
            {
                Assert.NotEmpty(response.Message);
            }

        }

    }

}

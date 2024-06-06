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

namespace microservices.test.Controllers
{
    public class CouponControllerTests
    {
        [Fact]
        public void CouponController_GetAllCoupons_ReturnsAllCoupons()
        {
            // Arrange

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDb")
            .Options;

            var fakeDb = new AppDbContext(dbContextOptions);
            var fakeMapper = A.Fake<IMapper>();
            
            var fakeCoupons = A.CollectionOfFake<Coupon>(10);
            var fakeCouponsList = A.Fake<List<Coupon>>();

            A.CallTo(() => fakeMapper.Map<List<Coupon>>(fakeCoupons)).Returns(fakeCouponsList);

            var controller = new CouponAPIController(fakeDb, fakeMapper);

            // Act

            var results = controller.GetAllCoupons();

            // Assert

            Assert.NotNull(results);
            Assert.IsType<OkObjectResult>(results);

        }
    }
}

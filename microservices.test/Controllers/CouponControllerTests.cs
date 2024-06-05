using Microservices.Services.CouponAPI.Controllers;
using Microservices.Services.CouponAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;

namespace microservices.test.Controllers
{
    public class CouponControllerTests
    {
        [Fact]
        public void CouponController_GetAllCoupons_ReturnsAllCoupons()
        {
            // Arrange
            var dataStore = A.Fake<ICoupon>();
            var controller = new CouponAPIController();  
        }
    }
}

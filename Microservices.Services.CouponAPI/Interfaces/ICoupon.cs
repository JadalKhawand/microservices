namespace Microservices.Services.CouponAPI.Interfaces
{
    public class ICoupon
    {
        Guid CouponId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        double Discount { get; set; }


    }
}

namespace Microservices.Services.CouponAPI.Models.Dto
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string InnerExceptionMessage { get; set; } = string.Empty;
    }
}

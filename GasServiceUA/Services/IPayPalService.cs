namespace GasServiceUA.Services
{
    public interface IPayPalService
    {
        public string Mode { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        Task<CreateOrderResponse> CreateOrder(string value, string currency, string reference);
        Task<CaptureOrderResponse> CaptureOrder(string orderId);
    }
}

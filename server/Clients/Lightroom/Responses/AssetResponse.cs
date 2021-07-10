namespace ldam.co.za.server.Clients.Lightroom
{
    public class AssetResponse : BaseResponse
    {
        public Asset Asset { get; set; }
        public dynamic Payload { get; set; }
    }
}
namespace ldam.co.za.lib.Lightroom
{
    public class AssetResponse : BaseResponse
    {
        public Asset Asset { get; set; }
        public dynamic Payload { get; set; }
    }
}
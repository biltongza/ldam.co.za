using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ldam.co.za.client.Services
{
    public interface IViewportService
    {
        Task<decimal> GetViewportDensity();
    }

    public class ViewportService : IViewportService
    {
        private readonly IJSRuntime jsRuntime;

        public ViewportService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<decimal> GetViewportDensity()
        {
            var pixelRatio = await this.jsRuntime.InvokeAsync<decimal>("getViewportDensity");
            return pixelRatio;
        }
    }
}
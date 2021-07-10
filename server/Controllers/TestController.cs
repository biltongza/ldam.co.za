using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ldam.co.za.server.Services;
using System.Text.Json;

namespace ldam.co.za.server
{
    [ApiController]
    [Authorize]
    [Route("{controller}")]
    public class TestController : Controller
    {
        private readonly LightroomService _lightroomClient;
        public TestController(LightroomService lightroomClient)
        {
            _lightroomClient = lightroomClient;
        }

        [HttpGet("albums")]
        public async Task<IActionResult> GetAlbums()
        {
            var result = _lightroomClient.GetAlbums();
            return Json(result);
        }

        [HttpGet("albums/{albumId}")]
        public async Task<IActionResult> GetAlbumImageInfo(string albumId)
        {
            var result = _lightroomClient.GetImageList(albumId);

            return Json(result);
        }
    }
}
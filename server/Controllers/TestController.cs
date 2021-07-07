using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ldam.co.za.server.Services;

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
            var result = await _lightroomClient.GetAlbums().ToDictionaryAsync(k => k.Key, k => k.Value);
            return Ok(result);
        }

        [HttpGet("albums/{albumId}")]
        public async Task<IActionResult> GetAlbumImageInfo(string albumId)
        {
            var result = await _lightroomClient.GetImageList(albumId).ToListAsync();
            return Ok(result);
        }
    }
}
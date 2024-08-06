using Application.Service.Interface.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers.Cache
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : Controller
    {
        private readonly ICacheApplication _cacheApplication;
        public CacheController(ICacheApplication cacheApplication) 
        {
            _cacheApplication = cacheApplication;   
        }
        [HttpGet("{key}")]
        public async Task<IActionResult> GetCache(string key)
        {
            var value = await _cacheApplication.GetCacheAsync<string>(key);
            if (value is null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> SetCache([FromQuery] string key, [FromBody] string value)
        {
            await _cacheApplication.SetCacheAsync(key, value, TimeSpan.FromMinutes(5));
            return Ok();    
        }
    }
}

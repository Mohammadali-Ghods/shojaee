using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace Shojaee.SlideShowModule
{

    public class SlideShowController : ControllerBase
    {
        [HttpGet("GetSlideShows")]
        public async Task<List<SlideShow>> GetSlideShows()
        {
            return await DB.Find<SlideShow>().ExecuteAsync();
        }
    }
}

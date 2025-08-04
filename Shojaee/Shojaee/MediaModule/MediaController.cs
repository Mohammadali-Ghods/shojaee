using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shojaee.MediaModule
{

    public class MediaController : ControllerBase
    {
        [HttpPost]
        [Route("Upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            // Generate a unique ID
            var generatedId = Guid.NewGuid().ToString();

            // Determine file extension
            string extension = Path.GetExtension(file.FileName).ToLower();

            // Path to save the original file
            string originalFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/media",
                generatedId + extension
            );

            // 1. Save the original file
            using (var stream = new FileStream(originalFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok($"/media/{generatedId}{extension}");
        }
    }
}

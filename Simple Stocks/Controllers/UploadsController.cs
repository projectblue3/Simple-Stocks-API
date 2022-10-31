using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple_Stocks.Services;
using System.Net.Http.Headers;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IRefreshTokenRepo _refreshTokenRepo;

        public UploadsController(IRefreshTokenRepo refreshTokenRepo)
        {
            _refreshTokenRepo = refreshTokenRepo;
        }

        [HttpPost("u/{username}/profile"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadProfilePic(string username, [FromQuery] string? uploadType)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            if(tokenUser.ToLower() != username.ToLower())
            {
                return StatusCode(403);
            }

            var file = Request.Form.Files[0];
            var folderName = Path.Combine("Media", "Users", "Profile", username);

            Directory.CreateDirectory(folderName);

            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (file.Length > 0)
            {
                var uploadName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileType = Path.GetExtension(uploadName);

                if (fileType != ".jpg" && fileType != ".jpeg" && fileType != ".png")
                {
                    return BadRequest(new { messages = new List<string>() { "Invalid file type. Must be .jpeg, .jpg, or .png" } });
                }

                var fileName = $"profile{fileType}";

                if (uploadType == "banner")
                {
                    fileName = $"Banner{fileType}";
                }

                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { messages = new List<string>() { dbPath } });
            }
            else
            {
                return BadRequest(new { messages = new List<string>() { "File not found." } });
            }
        }

        [HttpPost("u/{username}/post"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPostContent(string username)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (tokenUser.ToLower() != username.ToLower())
            {
                return StatusCode(403);
            }

            var folderName = Path.Combine("Media", "Posts", username);

            Directory.CreateDirectory(folderName);

            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            ICollection<string> contentPaths = new List<string>();

            foreach (var formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    var fileType = Path.GetExtension(fileName);

                    if (fileType != ".jpg" && fileType != ".jpeg" && fileType != ".png" && fileType != ".mp3" && fileType != ".mp4" && fileType != ".gif")
                    {
                        return BadRequest(new { messages = new List<string>() { "Invalid file type. Must be .jpeg, .jpg, .png, .mp3, .mp4, .gif" } });
                    }                    

                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var inputStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(inputStream);                        
                    }

                    contentPaths.Add(dbPath);
                }
                else
                {
                    return BadRequest(new { messages = new List<string>() { "File not found." } });
                }
            }

            return Ok(new { messages = contentPaths });
        }
    }
}

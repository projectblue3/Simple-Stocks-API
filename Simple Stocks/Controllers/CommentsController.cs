using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Simple_Stocks.Dtos;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Dtos.PrivacyDtos;
using Simple_Stocks.Dtos.UserUpdateDtos;
using Simple_Stocks.Models;
using Simple_Stocks.Services;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepo _commentRepo;
        private readonly IUserRepo _userRepo;
        private readonly IPostRepo _postRepo;
        private readonly IUserBlockRepo _userBlockRepo;
        private readonly ILikedCommentRepo _likedCommentRepo;
        private readonly IMapper _mapper;

        public CommentsController(ICommentRepo commentRepo, IMapper mapper, IUserRepo userRepo, ILikedCommentRepo likedCommentRepo, IPostRepo postRepo, IUserBlockRepo userBlockRepo)
        {
            _commentRepo = commentRepo;
            _mapper = mapper;
            _userRepo = userRepo;
            _likedCommentRepo = likedCommentRepo;
            _postRepo = postRepo;
            _userBlockRepo = userBlockRepo;
        }

        //Get req for all comments
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            ICollection<Comment> comments = await _commentRepo.GetAllComments();

            if (comments.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(comments);
        }

        //Get req to get comment by id
        [HttpGet("i/{id}", Name = "GetCommentById")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var desiredComment = await _commentRepo.GetCommentById(id);

            if (desiredComment == null)
            {
                return NotFound();
            }

            return Ok(desiredComment);
        }

        //Get req to get comment likes
        [HttpGet("i/{id}/likes")]
        public async Task<IActionResult> GetCommentLikes(int id)
        {
            ICollection<User> likes = await _commentRepo.GetLikesOfComment(id);

            if (likes.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(likes);
        }

        //Post req to create a comment
        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment commentPassedIn)
        {
            User commentUser = await _userRepo.GetUserById(commentPassedIn.UserID);
            Post commentPost = await _postRepo.GetPostById(commentPassedIn.PostId);

            if (commentUser == null)
            {
                return StatusCode(404, new { messages = new List<string>() { $"Author of comment not valid" } });
            }

            if (commentPost == null)
            {
                return StatusCode(404, new { messages = new List<string>() { $"Commented on an invalid post" } });
            }

            UserBlock blockedUser = await _userBlockRepo.SearchForBlockedUser(commentPost.UserID, commentUser.Id);

            if (blockedUser != null)
            {
                return StatusCode(400, new { messages = new List<string>() { $"You are blocked" } });
            }

            if (commentUser.AccountIsEnabled == false)
            {
                ModelState.AddModelError("BannedAuthorError", "The Comments's author is banned.");
                return StatusCode(400, new { messages = new List<string>() { "The Comments's author is banned." } });
            }

            Comment commentToCreate = new Comment()
            {
                Text = commentPassedIn.Text,
                CreatedAt = DateTimeOffset.Now,
                CommentIsHidden = false,
                UserID = commentPassedIn.UserID,
                PostId = commentPassedIn.PostId,
            };

            if (commentToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _commentRepo.AddComment(commentToCreate);

            return Ok();
        }

        //Delete req to remove comments
        [HttpDelete("i/{id}")]
        public async Task<IActionResult> DeleteCommentById(int id)
        {
            var desiredComment = await _commentRepo.GetCommentById(id);
            ICollection<LikedComment> likedComments = await _likedCommentRepo.SearchByCommentId(id);

            if (desiredComment == null)
            {
                return NotFound();
            }

            foreach (LikedComment likedComment in likedComments)
            {
                await _likedCommentRepo.DeleteLikedComment(likedComment);
            }

            await _commentRepo.DeleteComment(desiredComment);

            return NoContent();

        }

        //Patch req to edit comment text
        [HttpPatch("i/{id}/update")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] JsonPatchDocument<CommentUpdateDto> patchPassedIn)
        {
            var commentInDb = await _commentRepo.GetCommentById(id);

            if (commentInDb == null)
            {
                return NotFound();
            }

            var updatedComment = _mapper.Map<CommentUpdateDto>(commentInDb);

            patchPassedIn.ApplyTo(updatedComment, ModelState);

            _mapper.Map(updatedComment, commentInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _commentRepo.UpdateComment(commentInDb);

            return NoContent();
        }

        //Patch req for comment moderation
        [HttpPatch("i/{id}/hide")]
        public async Task<IActionResult> HideComment(int id, [FromBody] JsonPatchDocument<CommentVisabilityDto> patchPassedIn)
        {
            var commentInDb = await _commentRepo.GetCommentById(id);

            if (commentInDb == null)
            {
                return NotFound();
            }

            var updatedComment = _mapper.Map<CommentVisabilityDto>(commentInDb);

            patchPassedIn.ApplyTo(updatedComment, ModelState);

            _mapper.Map(updatedComment, commentInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _commentRepo.UpdateComment(commentInDb);

            return NoContent();
        }
    }
}

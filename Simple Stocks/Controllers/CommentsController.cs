using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepo _commentRepo;
        private readonly IUserRepo _userRepo;
        private readonly IPostRepo _postRepo;
        private readonly IUserBlockRepo _userBlockRepo;
        private readonly ILikedCommentRepo _likedCommentRepo;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepo _refreshTokenRepo;

        public CommentsController(ICommentRepo commentRepo, IMapper mapper, IUserRepo userRepo, ILikedCommentRepo likedCommentRepo, IPostRepo postRepo, IUserBlockRepo userBlockRepo, IRefreshTokenRepo refreshTokenRepo)
        {
            _commentRepo = commentRepo;
            _mapper = mapper;
            _userRepo = userRepo;
            _likedCommentRepo = likedCommentRepo;
            _postRepo = postRepo;
            _userBlockRepo = userBlockRepo;
            _refreshTokenRepo = refreshTokenRepo;
        }

        //Get req for all comments
        [HttpGet, Authorize(Roles = "Mod,Admin")]
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

            ICollection<GetUserDto> userDtos = new List<GetUserDto>();

            foreach (var user in likes)
            {
                userDtos.Add(new GetUserDto
                {
                    Id = user.Id,
                    AvatarLink = user.AvatarLink,
                    BannerLink = user.BannerLink,
                    Username = user.Username,
                    Bio = user.Bio,
                    AccountIsEnabled = user.AccountIsEnabled,
                    AccountIsHidden = user.AccountIsHidden,
                    AccountIsPrivate = user.AccountIsPrivate,
                    LikesArePrivate = user.LikesArePrivate,
                    FollowsArePrivate = user.FollowsArePrivate,
                    RoleId = user.RoleId,
                    CreatedAt = user.CreatedAt
                });
            }

            return Ok(userDtos);
        }

        //Post req to create a comment
        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment commentPassedIn)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();
            User commentUser = await _userRepo.GetUserByUsername(tokenUser);
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
                UserID = commentUser.Id,
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

        //Post req to like a comment
        [HttpPost("i/{commentId}/like")]
        public async Task<IActionResult> LikePost(int commentId)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            var userLiking = await _userRepo.GetUserByUsername(tokenUser);

            if (userLiking == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "User not found" } });
            }

            var commentLiked = await _commentRepo.GetCommentById(commentId);

            if (commentLiked == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "comment not found" } });
            }

            var commentLike = new LikedComment
            {
                CommentId = commentId,
                UserId = userLiking.Id
            };

            if (commentId == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error Liking" } });
            }

            await _likedCommentRepo.LikeComment(commentLike);

            return Ok();
        }

        //Delete req to unlike a post
        [HttpDelete("i/{commentId}/unlike")]
        public async Task<IActionResult> UnlikePost(int commentId)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            var userUnliking = await _userRepo.GetUserByUsername(tokenUser);

            if (userUnliking == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "User not found" } });
            }

            var commentUnliked = await _commentRepo.GetCommentById(commentId);

            if (commentUnliked == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Comment not found" } });
            }

            var commentLike = await _likedCommentRepo.SearchByUserAndCommentIds(userUnliking.Id, commentId);

            if (commentLike == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error unliking" } });
            }

            await _likedCommentRepo.DeleteLikedComment(commentLike);

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

            var tokenUser = _refreshTokenRepo.ReadToken();

            var commentAuthor = await _userRepo.GetUserById(desiredComment.UserID);

            if (commentAuthor.Username != tokenUser)
            {
                return StatusCode(403);
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

            var tokenUser = _refreshTokenRepo.ReadToken();

            var commentAuthor = await _userRepo.GetUserById(commentInDb.UserID);

            if (commentAuthor.Username != tokenUser)
            {
                return StatusCode(403);
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
        [HttpPatch("i/{id}/hide"), Authorize(Roles = "Mod,Admin")]
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

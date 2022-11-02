using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Simple_Stocks.Dtos;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Dtos.PrivacyDtos;
using Simple_Stocks.Models;
using Simple_Stocks.Services;
using System.Data;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepo _postRepo;
        private readonly IUserRepo _userRepo;
        private readonly ICommentRepo _commentRepo;
        private readonly ILikedPostRepo _likedPostRepo;
        private readonly ITagRepo _tagRepo;
        private readonly IPostTagRepo _postTagRepo;
        private readonly IRefreshTokenRepo _refreshTokenRepo;

        public PostsController(IMapper mapper, IPostRepo postRepo, IUserRepo userRepo, ICommentRepo commentRepo, ITagRepo tagRepo, IPostTagRepo postTagRepo, ILikedPostRepo likedPostRepo, IRefreshTokenRepo refreshTokenRepo)
        {
            _mapper = mapper;
            _postRepo = postRepo;
            _userRepo = userRepo;
            _commentRepo = commentRepo;
            _tagRepo = tagRepo;
            _postTagRepo = postTagRepo;
            _likedPostRepo = likedPostRepo;
            _refreshTokenRepo = refreshTokenRepo;   
        }


        //Get req to get all posts for mods
        [HttpGet("modview"), Authorize(Roles = "Mod,Admin")]
        public async Task<IActionResult> ModsGetPosts()
        {
            ICollection<Post> foundPosts = new List<Post>();

            foundPosts = await _postRepo.GetAllPostsModView();

            if (foundPosts.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            

            return Ok(foundPosts.OrderBy(p => p.CreatedAt).ToList());
        }

        //Get req to get all posts for users
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts()
        {
            ICollection<Post> foundPosts = new List<Post>();

            foundPosts = await _postRepo.GetAllPostsUserView();

            if (foundPosts.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundPosts.OrderBy(p => p.CreatedAt).ToList());
        }

        //Get req to get a post by id
        [HttpGet("i/{id}", Name = "GetPostById")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var desiredPost = await _postRepo.GetPostById(id);

            if (desiredPost == null)
            {
                return NotFound();
            }

            if(desiredPost.PostIsPrivate == true)
            {
                return StatusCode(403, new { messages = new List<string>() { "Post is private." } });
            }

            return Ok(desiredPost);
        }

        //Get req to search for post by title
        [HttpGet("search/t")]
        public async Task<IActionResult> TitleSearch([FromBody] JSONString input)
        {
            ICollection<Post> foundPosts = new List<Post>();

            foundPosts = await _postRepo.FindPostsByTitle(input.Text);

            if (foundPosts.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundPosts.OrderBy(p => p.CreatedAt).ToList());
        }

        //Get req to search for post by content
        [HttpGet("search/c")]
        public async Task<IActionResult> ContentSearch([FromBody] JSONString input)
        {
            ICollection<Post> foundPosts = await _postRepo.FindPostsByContent(input.Text);

            if (foundPosts.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundPosts.OrderBy(p => p.CreatedAt).ToList());
        }

        //Get req to get tags of post
        [HttpGet("i/{id}/tags", Name = "GetTagsOfPost")]
        public async Task<IActionResult> GetTagsOfPost(int id)
        {
            ICollection<Tag> tags = await _postRepo.GetAllTagsOfPost(id);

            if (tags.Count <= 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(tags);
        }

        //Get req to get comments of post
        [HttpGet("i/{id}/comments", Name = "GetCommentsOfPost")]
        public async Task<IActionResult> GetCommentsOfPost(int id)
        {
            ICollection<Comment> comments = await _postRepo.GetAllCommentsOfPost(id);

            if (comments.Count <= 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(comments);
        }

        //Get req to get post likes
        [HttpGet("i/{id}/likes")]
        public async Task<IActionResult> GetPostLikes(int id)
        {
            ICollection<User> likes = await _postRepo.GetLikesOfPost(id);

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

        //Post req to create a post
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromQuery] List<int> tagId, [FromBody] Post postPassedIn)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();
            var userFound = await _userRepo.GetUserByUsername(tokenUser);

            if (postPassedIn == null)
            {
                ModelState.AddModelError("MissingPostError", "Post details are missing.");
                return StatusCode(400, new { messages = new List<string>() { "Post details are missing." } });
            }
            
            if (userFound == null)
            {
                ModelState.AddModelError("PostAuthorError", "The Post's author is not valid.");
                return StatusCode(400, new { messages = new List<string>() { "The Post's author is not valid." } });
            }

            if (userFound.AccountIsEnabled == false)
            {
                ModelState.AddModelError("BannedAuthorError", "The Post's author is banned.");
                return StatusCode(400, new { messages = new List<string>() { "The Post's author is banned." } });
            }

            if (tagId.Count > 0)
            {
                foreach (var id in tagId)
                {
                    var tagFound = await _tagRepo.GetTagById(id);

                    if (tagFound == null)
                    {
                        ModelState.AddModelError("TagNotFoundError", $"Tag with ID {id} was not found");
                        return StatusCode(404, new { messages = new List<string>() { $"Tag with ID {id} was not found" } });
                    }
                }
            }

            Post postToCreate = new Post()
            {
                Title = postPassedIn.Title,
                Text = postPassedIn.Text,
                PostIsHidden = false,
                PostIsPrivate = postPassedIn.PostIsPrivate,
                UserID = userFound.Id,
                Author = userFound.Username,
                CreatedAt = DateTimeOffset.Now
            };

            if (postToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _postRepo.AddPost(postToCreate, tagId);

            return Ok();
        }

        //Post req to like a post
        [HttpPost("i/{postId}/like")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            var userLiking = await _userRepo.GetUserByUsername(tokenUser);

            if (userLiking == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "User not found" } });
            }

            var postLiked = await _postRepo.GetPostById(postId);

            if (postLiked == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Post not found" } });
            }

            var postLike = new LikedPost
            {
                PostId = postId,
                UserId = userLiking.Id
            };

            if (postLike == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error Liking" } });
            }

            await _likedPostRepo.LikePost(postLike);

            return Ok();
        }

        //Delete req to unlike a post
        [HttpDelete("i/{postId}/unlike")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            var userUnliking = await _userRepo.GetUserByUsername(tokenUser);

            if (userUnliking == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "User not found" } });
            }

            var postUnliked = await _postRepo.GetPostById(postId);

            if (postUnliked == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Post not found" } });
            }

            var postLike = await _likedPostRepo.SearchByUserAndPostIds(userUnliking.Id, postId);

            if (postLike == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error unliking" } });
            }

            await _likedPostRepo.DeleteLikedPost(postLike);

            return Ok();
        }

        //Delete req to remove a post
        [HttpDelete("i/{id}")]
        public async Task<IActionResult> DeletePostById(int id)
        {
            var desiredPost = await _postRepo.GetPostById(id);

            if (desiredPost == null)
            {
                return NotFound();
            }

            var tokenUser = _refreshTokenRepo.ReadToken();
            
            var postAuthor = await _userRepo.GetUserById(desiredPost.UserID);

            if (postAuthor.Username != tokenUser)
            {
                return StatusCode(403);
            }

            //delete post likes too
            ICollection<PostTag> postTags = await _postTagRepo.SearchByPostId(id);
            ICollection<Comment> comments = await _postRepo.GetAllCommentsOfPost(id);            

            foreach (Comment comment in comments)
            {
                await _commentRepo.DeleteComment(comment);
            }

            foreach (PostTag postTag in postTags)
            {
                await _postTagRepo.DeletePostTag(postTag);
            }

            await _postRepo.DeletePost(desiredPost);

            return NoContent();
        }

        //Patch req to update post
        [HttpPatch("i/{id}/update")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] JsonPatchDocument<PostUpdateDto> patchPassedIn)
        {
            var postInDb = await _postRepo.GetPostById(id);

            if (postInDb == null)
            {
                return NotFound();
            }

            var tokenUser = _refreshTokenRepo.ReadToken();

            var postAuthor = await _userRepo.GetUserById(postInDb.UserID);

            if (postAuthor.Username != tokenUser)
            {
                return StatusCode(403);
            }

            var updatedPost = _mapper.Map<PostUpdateDto>(postInDb);

            patchPassedIn.ApplyTo(updatedPost, ModelState);

            _mapper.Map(updatedPost, postInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _postRepo.UpdatePost(postInDb);

            return NoContent();
        }

        [HttpPost("i/{postId}/addtags")]
        public async Task<IActionResult> AddPostTag(int postId, [FromQuery] int tagId)
        {
            var tagToAdd = await _tagRepo.GetTagById(tagId);
            var post = await _postRepo.GetPostById(postId);

            PostTag postTag = new PostTag()
            {
                PostId = postId,
                TagId = tagId,
            };

            if (post == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "post was not found." } });
            }

            if (tagToAdd == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "tag was not found." } });
            }

            if (postTag == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error adding tag" } });
            }

            var tokenUser = _refreshTokenRepo.ReadToken();

            var postAuthor = await _userRepo.GetUserById(post.UserID);

            if (postAuthor.Username != tokenUser)
            {
                return StatusCode(403);
            }

            await _postTagRepo.AddPostTag(postTag);

            return NoContent();
        }

        //Put req to update post's tags
        [HttpPut("i/{id}/updatetags")]
        public async Task<IActionResult> UpdatePostTags(int id, [FromQuery] int oldTagId, [FromQuery] int newTagId)
        {
            var postTagToChange = await _postTagRepo.SearchByPostAndTagIds(id, oldTagId);
            var checkNewTag = await _tagRepo.GetTagById(newTagId);
            var post = await _postRepo.GetPostById(id);

            if (post == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "post was not found." } });
            }

            if (postTagToChange == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Old tag was not found." } });
            }

            if (checkNewTag == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "New tag was not found." } });
            }

            var tokenUser = _refreshTokenRepo.ReadToken();

            var postAuthor = await _userRepo.GetUserById(post.UserID);

            if (postAuthor.Username != tokenUser)
            {
                return StatusCode(403);
            }

            postTagToChange.TagId = newTagId;

            await _postTagRepo.UpdatePostTag(postTagToChange);

            return NoContent();
        }

        //Put req remove post's tags
        [HttpDelete("i/{id}/removetags")]
        public async Task<IActionResult> RemovePostTags(int id, [FromQuery] int tagId)
        {
            var tagToDelete = await _postTagRepo.SearchByPostAndTagIds(id, tagId);
            var post = await _postRepo.GetPostById(id);

            if (post == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "post was not found." } });
            }

            if (tagToDelete == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Tag was not found." } });
            }

            var tokenUser = _refreshTokenRepo.ReadToken();

            var postAuthor = await _userRepo.GetUserById(post.UserID);

            if (postAuthor.Username != tokenUser)
            {
                return StatusCode(403);
            }

            await _postTagRepo.DeletePostTag(tagToDelete);

            return NoContent();
        }

        //Patch req for post moderation
        [HttpPatch("i/{id}/hide"), Authorize(Roles = "Mod,Admin")]
        public async Task<IActionResult> HidePost(int id, [FromBody] JsonPatchDocument<PostVisabilityDto> patchPassedIn)
        {
            var postInDb = await _postRepo.GetPostById(id);

            if (postInDb == null)
            {
                return NotFound();
            }

            var updatedPost = _mapper.Map<PostVisabilityDto>(postInDb);

            patchPassedIn.ApplyTo(updatedPost, ModelState);

            _mapper.Map(updatedPost, postInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _postRepo.UpdatePost(postInDb);

            return NoContent();
        }

        //Patch req for post privacy
        [HttpPatch("i/{id}/private")]
        public async Task<IActionResult> PrivatePost(int id, [FromBody] JsonPatchDocument<PostPrivacyDto> patchPassedIn)
        {
            var postInDb = await _postRepo.GetPostById(id);

            if (postInDb == null)
            {
                return NotFound();
            }

            var tokenUser = _refreshTokenRepo.ReadToken();

            var postAuthor = await _userRepo.GetUserById(postInDb.UserID);

            if (postAuthor.Username != tokenUser)
            {
                return StatusCode(403);
            }

            var updatedPost = _mapper.Map<PostPrivacyDto>(postInDb);

            patchPassedIn.ApplyTo(updatedPost, ModelState);

            _mapper.Map(updatedPost, postInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _postRepo.UpdatePost(postInDb);

            return NoContent();
        }
    }
}

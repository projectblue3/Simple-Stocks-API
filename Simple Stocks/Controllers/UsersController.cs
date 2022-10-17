using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Dtos.PrivacyDtos;
using Simple_Stocks.Dtos.UserUpdateDtos;
using Simple_Stocks.Models;
using Simple_Stocks.Services;
using System.Collections.Generic;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepo userRepo, IRoleRepo roleRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _mapper = mapper;
        }

        //Get req where any admin can get all users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            ICollection<User> foundUsers = new List<User>();

            foundUsers = await _userRepo.GetAllUsers();

            if (foundUsers.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundUsers.OrderBy(u => u.Username).ToList());
        }

        //Get req where any user can search for a specific account
        [HttpGet("search")]
        public async Task<IActionResult> GetUserSearch([FromBody] JSONString input)
        {
            ICollection<User> foundUsers = new List<User>();

            foundUsers = await _userRepo.GetSearchedUsers(input.Text);

            if (foundUsers.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            //return Ok(foundUsers.OrderBy(u => u.Followers.Count).ToList());
            return Ok(foundUsers);
        }

        //Get req to access a user profile
        [HttpGet("u/{username}", Name="GetUserByUsername")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var desiredUser = await _userRepo.GetUserByUsername(username);

            if(desiredUser == null)
            {
                return NotFound();
            }

            return Ok(desiredUser);
        }

        //Get req to access a user profile by Id
        [HttpGet("i/{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            return Ok(desiredUser);
        }

        //Get req to get role of user
        [HttpGet("i/{id}/role")]
        public async Task<IActionResult> GetRoleOfUser(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            var userRole = await _roleRepo.GetRoleById(desiredUser.RoleId);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (userRole == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Role not found." } });
            }

            return Ok(userRole);
        }

        //Get req to access a users's rights
        [HttpGet("i/{id}/rights", Name = "GetUserRights")]
        public async Task<IActionResult> GetUserRights(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            ICollection<Right> foundRights = await _userRepo.GetUserRights(desiredUser.RoleId);

            if (foundRights.Count <= 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundRights);
        }

        //GET req for posts of user
        [HttpGet("i/{id}/posts")]
        public async Task<IActionResult> GetPostsOfUser(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<Post> userPosts = await _userRepo.GetAllPosts(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if(desiredUser.AccountIsPrivate == true)
            {
                return StatusCode(403, new { messages = new List<string>() { "User is private." } });
            }

            if (userPosts == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Posts not found." } });
            }

            return Ok(userPosts);
        }

        //Get req for user's own posts
        [HttpGet("i/{id}/myposts")]
        public async Task<IActionResult> GetOwnPosts(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<Post> userPosts = await _userRepo.GetPersonalPosts(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (userPosts == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Posts not found." } });
            }

            return Ok(userPosts);
        }

        //GET req for comments of user
        [HttpGet("i/{id}/mycomments")]
        public async Task<IActionResult> GetOwnComments(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<Comment> userComments = await _userRepo.GetAllComments(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (userComments == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Comments not found." } });
            }

            return Ok(userComments);
        }

        //GET req for followers
        [HttpGet("i/{id}/followers")]
        public async Task<IActionResult> GetFollowers(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<User> followers = await _userRepo.GetAllFollowers(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (followers == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Followers not found." } });
            }

            return Ok(followers);
        }
        //GET req for users you followed
        [HttpGet("i/{id}/following")]
        public async Task<IActionResult> GetFollowed(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<User> followed = await _userRepo.GetAllFollowedUsers(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (followed == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Followed users not found." } });
            }

            return Ok(followed);
        }

        //GET req for blocked users
        [HttpGet("i/{id}/blocked")]
        public async Task<IActionResult> GetBlocked(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<User> blocked = await _userRepo.GetAllBlockedUsers(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (blocked == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Blocked users not found." } });
            }

            return Ok(blocked);
        }

        //GET req for liked posts
        [HttpGet("i/{id}/likedposts")]
        public async Task<IActionResult> GetLikes(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<Post> likedPosts = await _userRepo.GetAllLikedPosts(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (likedPosts == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Posts not found." } });
            }

            return Ok(likedPosts);
        }

        //GET req for liked comments
        [HttpGet("i/{id}/likedcomments")]
        public async Task<IActionResult> GetLikedComments(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            ICollection<Comment> likedComments = await _userRepo.GetLikedComments(desiredUser.Id);

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (likedComments == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Comments not found." } });
            }

            return Ok(likedComments);
        }

        //Post req to create a user
        [HttpPost]
        public async Task<IActionResult> CreateUserAccount(User userPassedIn)
        {
            var userRole = await _roleRepo.GetRoleById(userPassedIn.RoleId);

            string dup = await _userRepo.IsDuplicate(userPassedIn.Username, userPassedIn.Email, userPassedIn.PhoneNumber);

            if (userRole == null)
            {
                return StatusCode(404, new { messages = new List<string>() { $"Role with ID {userPassedIn.RoleId} was not found" } });
            }

            if(dup != String.Empty)
            {
                return StatusCode(400, new { messages = new List<string>() { $"{dup} is in use." } });
            }

            User userToCreate = new User()
            {
                AvatarLink = "test/test.png",
                FirstName = userPassedIn.FirstName,
                LastName = userPassedIn.LastName,
                Username = userPassedIn.Username,
                Password = userPassedIn.Password,
                Email = userPassedIn.Email,
                DateOfBirth = userPassedIn.DateOfBirth,
                PhoneNumber = userPassedIn.PhoneNumber,
                Bio = userPassedIn.Bio,
                AccountIsEnabled = userPassedIn.AccountIsEnabled,
                AccountIsHidden = userPassedIn.AccountIsHidden,
                AccountIsPrivate = userPassedIn.AccountIsPrivate,
                LikesArePrivate = userPassedIn.LikesArePrivate,
                FollowsArePrivate = userPassedIn.FollowsArePrivate,
                RoleId = userPassedIn.RoleId,
                CreatedAt = DateTimeOffset.Now
            };

            if (userToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRepo.AddUser(userToCreate);

            return Ok();
            //return CreatedAtRoute(nameof(GetUserById), new { userId = userToCreate.Id }, userToCreate);
        }

        //Delete req to delete a user
        [HttpDelete("i/{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);

            if(desiredUser == null)
            {
                return NotFound();
            }

            await _userRepo.DeleteUserAsync(desiredUser);

            return NoContent();
        }

        //Patch req to update user's real name, username, and bio
        [HttpPatch("i/{id}/profileupdate")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody]JsonPatchDocument<ProfileUpdateDto> patchPassedIn)
        {
            var userInDb = await _userRepo.GetUserById(id);


            if(userInDb == null)
            {
                return NotFound();
            }

            var updatedUser = _mapper.Map<ProfileUpdateDto>(userInDb);

            patchPassedIn.ApplyTo(updatedUser, ModelState);

            string dup = await _userRepo.IsDuplicate(updatedUser.Username, String.Empty, String.Empty);

            if (dup != String.Empty)
            {
                return StatusCode(400, new { messages = new List<string>() { $"{dup} is in use." } });
            }

            _mapper.Map(updatedUser, userInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRepo.UpdateUserAsync(userInDb);

            return NoContent();
        }

        //Patch req to update user's privacy settings
        [HttpPatch("i/{id}/privacyupdate")]
        public async Task<IActionResult> UpdateUserPrivacy(int id, [FromBody] JsonPatchDocument<UserPrivacyDto> patchPassedIn)
        {
            var userInDb = await _userRepo.GetUserById(id);


            if (userInDb == null)
            {
                return NotFound();
            }

            var updatedUser = _mapper.Map<UserPrivacyDto>(userInDb);

            patchPassedIn.ApplyTo(updatedUser, ModelState);

            _mapper.Map(updatedUser, userInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRepo.UpdateUserAsync(userInDb);

            return NoContent();
        }

        //Put req to update a user's role
        [HttpPut("i/{id}/changerole")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromQuery] int newRoleId)
        {
            
            var checkNewRole = await _roleRepo.GetRoleById(newRoleId);
            var userToUpdate = await _userRepo.GetUserById(id);
            var roleToChange = await _roleRepo.GetRoleById(userToUpdate.RoleId);

            if (roleToChange == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Old role was not found." } });
            }

            if (checkNewRole == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "New role was not found." } });
            }

            if (userToUpdate == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "User was not found." } });
            }

            userToUpdate.RoleId = newRoleId;

            await _userRepo.UpdateUserAsync(userToUpdate);

            return NoContent();
        }

        //Patch req to hide user
        [HttpPatch("i/{id}/hideuser")]
        public async Task<IActionResult> HideUser(int id, [FromBody] JsonPatchDocument<UserVisabilityDto> patchPassedIn)
        {
            var userInDb = await _userRepo.GetUserById(id);


            if (userInDb == null)
            {
                return NotFound();
            }

            var updatedUser = _mapper.Map<UserVisabilityDto>(userInDb);

            patchPassedIn.ApplyTo(updatedUser, ModelState);

            _mapper.Map(updatedUser, userInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRepo.UpdateUserAsync(userInDb);

            return NoContent();
        }

        //add blocked users to get req
        //user feed
        //main page posts vs mod page posts
        //like, follow, block routes (may require jwt)
    }
}

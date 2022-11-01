using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Simple_Stocks.Dtos;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Dtos.PrivacyDtos;
using Simple_Stocks.Dtos.UserUpdateDtos;
using Simple_Stocks.Migrations;
using Simple_Stocks.Models;
using Simple_Stocks.Services;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly IUserFollowRepo _userFollowRepo;
        private readonly IUserBlockRepo _userBlockRepo;
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepo userRepo, IRoleRepo roleRepo, IMapper mapper, IConfiguration configuration, IRefreshTokenRepo refreshTokenRepo, IUserFollowRepo userFollowRepo, IUserBlockRepo userBlockRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _mapper = mapper;
            _configuration = configuration;
            _refreshTokenRepo = refreshTokenRepo;
            _userFollowRepo = userFollowRepo;
            _userBlockRepo = userBlockRepo;
        }

        //Get req where any admin can get all users
        [HttpGet, Authorize(Roles = "Admin")]
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

            ICollection<GetUserDto> userDtos = new List<GetUserDto>();

            foreach (var user in foundUsers)
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

            //return Ok(foundUsers.OrderBy(u => u.Followers.Count).ToList());
            return Ok(userDtos);
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

            var userDto = new GetUserDto()
            {
                Id = desiredUser.Id,
                AvatarLink = desiredUser.AvatarLink,
                BannerLink = desiredUser.BannerLink,
                Username = desiredUser.Username,
                Bio = desiredUser.Bio,
                AccountIsEnabled = desiredUser.AccountIsEnabled,
                AccountIsHidden = desiredUser.AccountIsHidden,
                AccountIsPrivate = desiredUser.AccountIsPrivate,
                LikesArePrivate = desiredUser.LikesArePrivate,
                FollowsArePrivate = desiredUser.FollowsArePrivate,
                RoleId = desiredUser.RoleId,
                CreatedAt = desiredUser.CreatedAt
            };

            return Ok(userDto);
        }

        //Get req to access a user profile by Id
        [HttpGet("i/{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);

            var tokenUser = _refreshTokenRepo.ReadToken();

            if (desiredUser == null)
            {
                return NotFound();
            }

            if(desiredUser.Username != tokenUser)
            {
                return StatusCode(403);
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
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (desiredUser.Username != tokenUser)
            {
                return StatusCode(403);
            }

            ICollection<Post> userPosts = await _userRepo.GetPersonalPosts(desiredUser.Id);

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
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (desiredUser.Username != tokenUser)
            {
                return StatusCode(403);
            }

            ICollection<Comment> userComments = await _userRepo.GetAllComments(desiredUser.Id);

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

            ICollection<GetUserDto> userDtos = new List<GetUserDto>();

            foreach (var user in followers)
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

            ICollection<GetUserDto> userDtos = new List<GetUserDto>();

            foreach (var user in followed)
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

        //GET req for blocked users
        [HttpGet("i/{id}/blocked")]
        public async Task<IActionResult> GetBlocked(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (desiredUser.Username != tokenUser)
            {
                return StatusCode(403);
            }

            ICollection<User> blocked = await _userRepo.GetAllBlockedUsers(desiredUser.Id);

            if (blocked == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Blocked users not found." } });
            }

            ICollection<GetUserDto> userDtos = new List<GetUserDto>();

            foreach (var user in blocked)
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

        //GET req for liked posts
        [HttpGet("i/{id}/likedposts")]
        public async Task<IActionResult> GetLikes(int id)
        {
            var desiredUser = await _userRepo.GetUserById(id);
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (desiredUser.Username != tokenUser)
            {
                return StatusCode(403);
            }

            ICollection<Post> likedPosts = await _userRepo.GetAllLikedPosts(desiredUser.Id);

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
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (desiredUser == null)
            {
                return NotFound();
            }

            if (desiredUser.Username != tokenUser)
            {
                return StatusCode(403);
            }

            ICollection<Comment> likedComments = await _userRepo.GetLikedComments(desiredUser.Id);

            if (likedComments == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Comments not found." } });
            }

            return Ok(likedComments);
        }

        //Post req to create a user
        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> CreateUserAccount(RegisterDto userPassedIn)
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

            CreatePasswordHash(userPassedIn.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var jwt = CreateJWT(userPassedIn.Username, userRole.Title, "token");
            var rjwt = CreateJWT(userPassedIn.Username, userRole.Title, "refreshToken");

            User userToCreate = new User()
            {
                AvatarLink = userPassedIn.AvatarLink,
                BannerLink = userPassedIn.BannerLink,
                FirstName = userPassedIn.FirstName,
                LastName = userPassedIn.LastName,
                Username = userPassedIn.Username,
                //Password = userPassedIn.Password,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = userPassedIn.Email,
                DateOfBirth = userPassedIn.DateOfBirth,
                PhoneNumber = userPassedIn.PhoneNumber,
                Bio = userPassedIn.Bio,
                AccountIsEnabled = true,
                AccountIsHidden = false,
                AccountIsPrivate = false,
                LikesArePrivate = false,
                FollowsArePrivate = false,
                RoleId = userPassedIn.RoleId,
                CreatedAt = DateTimeOffset.Now,
                RefreshToken = rjwt
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

            return StatusCode(200, new { token = jwt, refreshToken = rjwt });
            //return CreatedAtRoute(nameof(GetUserById), new { userId = userToCreate.Id }, userToCreate);
        }

        //Post req to allow a user to login
        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto login)
        {
            User user = await _userRepo.GetUserByUsername(login.Username);

            if(user == null)
            {
                return StatusCode(400, new { messages = new List<string>() { $"Username or password is incorrect" } });
            }

            if (!VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return StatusCode(400, new { messages = new List<string>() { $"Username or password is incorrect" } });
            }

            Role role = await _roleRepo.GetRoleById(user.RoleId);

            var jwt = CreateJWT(user.Username, role.Title, "token");
            var rjwt = CreateJWT(user.Username, role.Title, "refreshToken");

            user.RefreshToken = rjwt;

            await _userRepo.UpdateUserAsync(user);

            return StatusCode(200, new { token = jwt, refreshToken = rjwt });
        }

        //Post req to follow another user
        [HttpPost("u/{userToFollow}/follow")]
        public async Task<IActionResult> FollowUser(string userToFollow)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (tokenUser == null)
            {
                return NotFound();
            }

            var userFollowing = await _userRepo.GetUserByUsername(tokenUser);

            if (userFollowing == null)
            {
                return NotFound();
            }

            var userFollowed = await _userRepo.GetUserByUsername(userToFollow);

            if (userFollowed == null)
            {
                return NotFound();
            }

            var userFollow = new UserFollow
            {
                SourceUserId = userFollowing.Id,
                FollowedUserId = userFollowed.Id,
            };

            if (userFollow == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error Following" } });
            }

            await _userFollowRepo.FollowUser(userFollow);

            return Ok();
        }

        //Post req to block another user
        [HttpPost("u/{userToBlock}/block")]
        public async Task<IActionResult> BlockUser(string userToBlock)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (tokenUser == null)
            {
                return NotFound();
            }

            var userBlocking = await _userRepo.GetUserByUsername(tokenUser);

            if (userBlocking == null)
            {
                return NotFound();
            }

            var userBlocked = await _userRepo.GetUserByUsername(userToBlock);

            if (userBlocked == null)
            {
                return NotFound();
            }

            var userBlock = new UserBlock
            {
                SourceUserId = userBlocking.Id,
                BlockedUserId = userBlocked.Id,
            };

            if (userBlock == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error Blocking User" } });
            }

            await _userBlockRepo.BlockUser(userBlock);

            return Ok();
        }

        //Delete req to unfollow a user
        [HttpDelete("u/{userToUnFollow}/unfollow")]
        public async Task<IActionResult> UnFollowUser(string userToUnFollow)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (tokenUser == null)
            {
                return NotFound();
            }

            var userFollowing = await _userRepo.GetUserByUsername(tokenUser);

            if (userFollowing == null)
            {
                return NotFound();
            }

            var userUnFollowed = await _userRepo.GetUserByUsername(userToUnFollow);

            if (userUnFollowed == null)
            {
                return NotFound();
            }

            var userUnFollow = await _userFollowRepo.SearchForFollowedUser(userFollowing.Id, userUnFollowed.Id);

            if (userUnFollow == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error Following" } });
            }

            await _userFollowRepo.DeleteUserFollow(userUnFollow);

            return Ok();
        }

        //Delete req to unblock a user
        [HttpDelete("u/{userToUnBlock}/unblock")]
        public async Task<IActionResult> UnBlockUser(string userToUnBlock)
        {
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (tokenUser == null)
            {
                return NotFound();
            }

            var userUnBlocking = await _userRepo.GetUserByUsername(tokenUser);

            if (userUnBlocking == null)
            {
                return NotFound();
            }

            var userUnBlocked = await _userRepo.GetUserByUsername(userToUnBlock);

            if (userUnBlocked == null)
            {
                return NotFound();
            }

            var userUnBlock = await _userBlockRepo.SearchForBlockedUser(userUnBlocking.Id, userUnBlocked.Id);

            if (userUnBlock == null)
            {
                return StatusCode(400, new { messages = new List<string>() { "Error Unblocking" } });
            }

            await _userBlockRepo.DeleteUserBlock(userUnBlock);

            return Ok();
        }

        //Delete req to logout
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var tokenUser = _refreshTokenRepo.ReadToken();            

            var user = await _userRepo.GetUserByUsername(tokenUser);

            if (user == null)
            {
                return NotFound();
            }

            user.RefreshToken = String.Empty;

            await _userRepo.UpdateUserAsync(user);

            return NoContent();
        }


        //Delete req to delete a user
        [HttpDelete("i/{id}"), Authorize(Roles = "Admin")]
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

        //Put req to update a user's jwt
        [HttpPut("u/{username}/refresh"), AllowAnonymous]
        public async Task<IActionResult> RefreshJwt(string username)
        {
            string rjwt = Request.Headers["RefreshToken"];

            if (rjwt == null)
            {
                return StatusCode(404, new { messages = new List<string>() { $"Refresh token not found" } });
            }

            var user = await _userRepo.GetUserByToken(rjwt);

            if (username != user.Username)
            {
                return StatusCode(403);
            }

            Role role = await _roleRepo.GetRoleById(user.RoleId);

            var newJwt = CreateJWT(user.Username, role.Title, "token");

            return StatusCode(200, new { token = newJwt });
        }

        //Patch req to update user's real name, username, and bio
        [HttpPatch("i/{id}/profileupdate")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody]JsonPatchDocument<ProfileUpdateDto> patchPassedIn)
        {
            var userInDb = await _userRepo.GetUserById(id);
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (userInDb == null)
            {
                return NotFound();
            }

            if (userInDb.Username != tokenUser)
            {
                return StatusCode(403);
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
            var tokenUser = _refreshTokenRepo.ReadToken();

            if (userInDb == null)
            {
                return NotFound();
            }

            if (userInDb.Username != tokenUser)
            {
                return StatusCode(403);
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
        [HttpPut("i/{id}/changerole"), Authorize(Roles = "Admin")]
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
        [HttpPatch("i/{id}/hideuser"), Authorize(Roles = "Mod,Admin")]
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

        //make these async and maybe move to another file

        //Salt and hashes passwords for the database
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        //Returns true of false based of the validity of the password entered
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        //Creates a JWT
        private string CreateJWT(string username, string role, string type)
        {
            var timeLength = DateTime.Now.AddMinutes(15);

            if(type == "refresh")
            {
                timeLength = DateTime.Now.AddDays(7);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWT:Secret").Value)
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: timeLength,
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        //add blocked users to get req
        //user feed
        //main page posts vs mod page posts
        //exception handler
        //get my media route
    }
}

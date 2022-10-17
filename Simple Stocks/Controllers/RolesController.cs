using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Simple_Stocks.Dtos;
using Simple_Stocks.Models;
using Simple_Stocks.Services;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepo _roleRepo;
        private readonly IRightRepo _rightRepo;
        private readonly IRoleRightRepo _roleRightRepo;
        private readonly IMapper _mapper;

        public RolesController(IRoleRepo roleRepo, IRightRepo rightRepo,IRoleRightRepo roleRightRepo, IMapper mapper)
        {
            _roleRepo = roleRepo;
            _rightRepo = rightRepo;
            _mapper = mapper;
            _roleRightRepo = roleRightRepo;
        }

        //Get req to access a list of all roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            ICollection<Role> foundRoles = new List<Role>();

            foundRoles = await _roleRepo.GetAllRoles();

            if (foundRoles.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundRoles.OrderBy(r => r.Title).ToList());
        }

        //Get req to access a role by id
        [HttpGet("i/{id}", Name = "GetRoleById")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var desiredRole = await _roleRepo.GetRoleById(id);

            if (desiredRole == null)
            {
                return NotFound();
            }

            return Ok(desiredRole);
        }

        //Get req to access a role's rights
        [HttpGet("i/{id}/rights", Name = "GetRoleRights")]
        public async Task<IActionResult> GetRoleRights(int id)
        {
            ICollection<Right> foundRights = await _roleRepo.GetRightsOfRole(id);

            if (foundRights.Count <= 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundRights);
        }

        //Get req to access a role by title
        [HttpGet("title", Name = "GetRoleByTitle")]
        public async Task<IActionResult> GetRoleByTitle([FromBody] JSONString input)
        {
            var desiredRole = await _roleRepo.GetRoleByTitle(input.Text);

            if (desiredRole == null)
            {
                return NotFound();
            }

            return Ok(desiredRole);
        }

        //Get req to get users with role
        [HttpGet("i/{id}/users", Name = "GetUsersWithRole")]
        public async Task<IActionResult> GetUsersWithRole(int id)
        {
            ICollection<User> users = await _roleRepo.GetAllUsersWithRole(id);

            if (users.Count <= 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(users);
        }

        //Post to create a role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromQuery] List<int> rightId, [FromBody]Role rolePassedIn)
        {
            if (rolePassedIn == null || rightId.Count <= 0)
            {
                ModelState.AddModelError("MissingRequirementError", "A role or right is missing.");
                return StatusCode(400, new { messages = new List<string>() { "A role or right is missing." } });
            }

            if (await _roleRepo.IsDuplicate(rolePassedIn.Title))
            {
                ModelState.AddModelError("DuplicateRoleError", "This role already exists.");
                return StatusCode(400, new { messages = new List<string>() { "This role already exists." } });
            }

            foreach (var id in rightId)
            {
                var rightFound = await _rightRepo.GetRightById(id);

                if (rightFound == null)
                {
                    ModelState.AddModelError("RightNotFoundError", $"Right with ID {id} was not found");
                    return StatusCode(404, new { messages = new List<string>() { $"Right with ID {id} was not found" } });
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _roleRepo.AddRole(rolePassedIn, rightId);

            return Ok();
        }

        //Delete req to delete a role
        [HttpDelete("i/{id}")]
        public async Task<IActionResult> DeleteRoleById(int id)
        {
            var desiredRole = await _roleRepo.GetRoleById(id);

            ICollection<RoleRight> roleRights = await _roleRightRepo.SearchByRoleId(id);
            ICollection<User> users = await _roleRepo.GetAllUsersWithRole(id);

            if (desiredRole == null)
            {
                return NotFound();
            }

           foreach (RoleRight roleRight in roleRights)
            {
                await _roleRightRepo.DeleteRoleRight(roleRight);
            }

            if (users.Any())
            {
                return StatusCode(400, new { messages = new List<string>() { "Cannot Delete, there are users depending on this role." } });
            }

            await _roleRepo.DeleteRole(desiredRole);

            return NoContent();
        }

        //Patch req to update role
        [HttpPatch("i/{id}")]
        public async Task<IActionResult> UpdateRoleById(int id, [FromBody]JsonPatchDocument<RoleUpdateDto> patchPassedIn)
        {
            var roleInDb = await _roleRepo.GetRoleById(id);

            if(roleInDb == null)
            {
                return NotFound();
            }

            var updatedRole = _mapper.Map<RoleUpdateDto>(roleInDb);

            patchPassedIn.ApplyTo(updatedRole, ModelState);

            if (await _roleRepo.IsDuplicate(updatedRole.Title))
            {
                ModelState.AddModelError("DuplicateRoleError", "This role already exists.");
                return StatusCode(400, new { messages = new List<string>() { "This role already exists." } });
            }

            _mapper.Map(updatedRole, roleInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _roleRepo.UpdateRole(roleInDb);

            return NoContent();
        }

        [HttpPost("i/{roleId}/addright")]
        public async Task<IActionResult> AddRoleRight(int roleId, [FromQuery] int rightId)
        {
            var rightToAdd = await _rightRepo.GetRightById(rightId);
            var role = await _roleRepo.GetRoleById(roleId);

            RoleRight roleRight = new RoleRight()
            {
                RoleId = roleId,
                RightId = rightId,
            };

            if (role == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "role was not found." } });
            }

            if (rightToAdd == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "right was not found." } });
            }

            if (roleRight == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Error adding right" } });
            }

            await _roleRightRepo.AddRoleRight(roleRight);

            return NoContent();
        }

        //Put req to update a role's right
        [HttpPut("i/{roleId}/updateright")]
        public async Task<IActionResult> UpdateRoleRights(int roleId, [FromQuery] int oldRightId, [FromQuery] int newRightId)
        {
            var roleRightToChange = await _roleRightRepo.SearchByRoleAndRightIds(roleId, oldRightId);
            var checkNewRight = await _rightRepo.GetRightById(newRightId);
            var role = await _roleRepo.GetRoleById(roleId);

            if (role == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "role was not found." } });
            }

            if (roleRightToChange == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "Old right was not found." } });
            }

            if(checkNewRight == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "New right was not found." } });
            }

            roleRightToChange.RightId = newRightId;

            await _roleRightRepo.UpdateRoleRight(roleRightToChange);

            return NoContent();
        }

        //Delete req to remove a role's right
        [HttpDelete("i/{roleId}/removeright")]
        public async Task<IActionResult> RemoveRoleRights(int roleId, [FromQuery] int rightId)
        {
            var roleRightToRemove = await _roleRightRepo.SearchByRoleAndRightIds(roleId, rightId);
            var role = await _roleRepo.GetRoleById(roleId);

            if (role == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "role was not found." } });
            }

            if (roleRightToRemove == null)
            {
                return StatusCode(404, new { messages = new List<string>() { "right was not found." } });
            }

            await _roleRightRepo.DeleteRoleRight(roleRightToRemove);

            return NoContent();
        }
    }
}

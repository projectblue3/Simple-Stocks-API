using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Simple_Stocks.Dtos;
using Simple_Stocks.Models;
using Simple_Stocks.Services;
using System.Collections.Generic;

namespace Simple_Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RightsController : ControllerBase
    {
        private readonly IRightRepo _rightRepo;
        private readonly IRoleRightRepo _roleRightRepo;
        private readonly IMapper _mapper;

        public RightsController(IRightRepo rightRepo,IRoleRightRepo roleRightRepo, IMapper mapper)
        {
            _rightRepo = rightRepo;
            _roleRightRepo = roleRightRepo;
            _mapper = mapper;
        }

        //Get req to access a list of all rights
        [HttpGet]
        public async Task<IActionResult> GetRights()
        {
            ICollection<Right> foundRights = new List<Right>();

            foundRights = await _rightRepo.GetAllRights();

            if (foundRights.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundRights.OrderBy(r => r.Title).ToList());
        }

        //Get req to access a right by id
        [HttpGet("i/{id}", Name ="GetRightById")]
        public async Task<IActionResult> GetRightById(int id)
        {
            var desiredRight = await _rightRepo.GetRightById(id);

            if(desiredRight == null)
            {
                return NotFound();
            }

            return Ok(desiredRight);
        }

        //Get req to access a right by title
        [HttpGet("title", Name = "GetRightByTitle")]
        public async Task<IActionResult> GetRightByTitle([FromBody]JSONString input)
        {
            var desiredRight = await _rightRepo.GetRightByTitle(input.Text);

            if (desiredRight == null)
            {
                return NotFound();
            }

            return Ok(desiredRight);
        }

        [HttpGet("i/{id}/roles")]
        public async Task<IActionResult> GetRolesWithRight(int id)
        {
            ICollection<Role> Roles = await _rightRepo.GetAllRolesWithRight(id);

            if (Roles.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(Roles.OrderBy(r => r.Title).ToList());
        }

        //Post req to create a right
        [HttpPost]
        public async Task<IActionResult> CreateRight(Right rightPassedIn)
        {
            Right rightToCreate = new Right()
            {
                Title = rightPassedIn.Title,
                Description = rightPassedIn.Description,
            };

            if(rightToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (await _rightRepo.IsDuplicate(rightPassedIn.Title))
            {
                ModelState.AddModelError("DuplicateRightError", "This right already exists.");
                return StatusCode(400, new { messages = new List<string>() { "This right already exists." } });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _rightRepo.AddRight(rightToCreate);

            return Ok();
        }

        //Delete req to delete a right
        [HttpDelete("i/{id}")]
        public async Task<IActionResult> DeleteRightById(int id)
        {
            var desiredRight = await _rightRepo.GetRightById(id);
            ICollection<RoleRight> roleRights = await _roleRightRepo.SearchByRightId(id);

            if (desiredRight == null)
            {
                return NotFound();
            }

            if (roleRights.Any())
            {
               return StatusCode(400, new { messages = new List<string>() { "Cannot Delete, there are roles depending on this right." } });
            }

            await _rightRepo.DeleteRight(desiredRight);

            return NoContent();
        }

        //Patch req to edit a right
        [HttpPatch("i/{id}")]
        public async Task<IActionResult> UpdateRightById(int id, [FromBody]JsonPatchDocument<RightUpdateDto> patchPassedIn)
        {
            var rightInDb = await _rightRepo.GetRightById(id);

            if (rightInDb == null)
            {
                return NotFound();
            }

            var updatedRight = _mapper.Map<RightUpdateDto>(rightInDb);

            patchPassedIn.ApplyTo(updatedRight, ModelState);

            if (await _rightRepo.IsDuplicate(updatedRight.Title))
            {
                ModelState.AddModelError("DuplicateRightError", "This right already exists.");
                return StatusCode(400, new { messages = new List<string>() { "This right already exists." } });
            }

            _mapper.Map(updatedRight, rightInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _rightRepo.UpdateRight(rightInDb);

            return NoContent();
        }
    }
}

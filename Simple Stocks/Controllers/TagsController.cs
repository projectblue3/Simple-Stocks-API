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
    public class TagsController : ControllerBase
    {
        private readonly ITagRepo _tagRepo;
        private readonly IPostTagRepo _postTagRepo;
        private readonly IPostRepo _postRepo;
        private readonly IMapper _mapper;

        public TagsController(ITagRepo tagRepo, IPostTagRepo postTagRepo, IPostRepo postRepo, IMapper mapper)
        {
            _tagRepo = tagRepo;
            _postTagRepo = postTagRepo;
            _postRepo = postRepo;
            _mapper = mapper;
        }

        //Get req to get all tags
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            ICollection<Tag> foundTags = new List<Tag>();

            foundTags = await _tagRepo.GetAllTags();

            if (foundTags.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundTags.OrderBy(t => t.Text).ToList());
        }

        //Get req to get a tag by id
        [HttpGet("i/{id}", Name = "GetTagById")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var desiredTag = await _tagRepo.GetTagById(id);

            if (desiredTag == null)
            {
                return NotFound();
            }

            return Ok(desiredTag);
        }

        //Get req to get a tag by name
        [HttpGet("n/{name}", Name = "GetTagByTitle")]
        public async Task<IActionResult> GetTagByTitle(string name)
        {
            var desiredTag = await _tagRepo.GetTagByTitle(name);

            if (desiredTag == null)
            {
                return NotFound();
            }

            return Ok(desiredTag);
        }

        //Get req to search for tags
        [HttpGet("search")]
        public async Task<IActionResult> TagSearch([FromBody] JSONString input)
        {
            ICollection<Tag> foundTags = new List<Tag>();

            foundTags = await _tagRepo.SearchTags(input.Text);

            if (foundTags.Count == 0)
            {
                List<string> noItems = new List<string>();
                return Ok(noItems);
            }

            return Ok(foundTags.OrderBy(t => t.Text).ToList());
        }

        //Post req to create a tag
        [HttpPost]
        public async Task<IActionResult> CreateTag(Tag tagPassedIn)
        {
            Tag tagToCreate = new Tag()
            {
                Text = tagPassedIn.Text,
            };

            if (tagToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (await _tagRepo.IsDuplicate(tagPassedIn.Text))
            {
                ModelState.AddModelError("DuplicateTagError", "This tag already exists.");
                return StatusCode(400, new { messages = new List<string>() { "This tag already exists." } });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _tagRepo.AddTag(tagToCreate);

            return Ok();
        }

        //Delete req to delete a tag
        [HttpDelete("i/{id}")]
        public async Task<IActionResult> DeleteTagById(int id)
        {
            var desiredTag = await _tagRepo.GetTagById(id);
            ICollection<PostTag> postTags = await _postTagRepo.SearchByTagId(id);

            if (desiredTag == null)
            {
                return NotFound();
            }

            if (postTags.Any())
            {
                return StatusCode(400, new { messages = new List<string>() { "Cannot Delete, there are posts depending on this tag." } });
            }
            

            await _tagRepo.DeleteTag(desiredTag);

            return NoContent();
        }

        //Patch req to edit a tag
        [HttpPatch("i/{id}")]
        public async Task<IActionResult> UpdateTagById(int id, [FromBody] JsonPatchDocument<TagUpdateDto> patchPassedIn)
        {
            var tagInDb = await _tagRepo.GetTagById(id);

            if (tagInDb == null)
            {
                return NotFound();
            }

            var updatedTag = _mapper.Map<TagUpdateDto>(tagInDb);

            patchPassedIn.ApplyTo(updatedTag, ModelState);

            if (await _tagRepo.IsDuplicate(updatedTag.Text))
            {
                ModelState.AddModelError("DuplicateTagError", "This tag already exists.");
                return StatusCode(400, new { messages = new List<string>() { "This tag already exists." } });
            }

            _mapper.Map(updatedTag, tagInDb);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _tagRepo.UpdateTag(tagInDb);

            return NoContent();
        }
    }
}

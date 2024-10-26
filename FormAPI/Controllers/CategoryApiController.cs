using Microsoft.AspNetCore.Mvc;
using Repositories;
using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FormAPI.DTO;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace FormAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryApiController> _logger;

        public CategoryApiController(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryApiController> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategories();
                return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting all categories");
                return StatusCode(500, "An error occured while processing your request");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<CategoryDto>(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with id {CategoryId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddCategory([FromBody] CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = _mapper.Map<Category>(categoryDto);
                await _categoryRepository.AddCategory(category);

                var createdCategory = _mapper.Map<CategoryDto>(category);
                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = category.CategoryId },
                    category);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new category");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateCategory(int id, CategoryDto categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingCategory = await _categoryRepository.GetCategoryById(id);
                if (existingCategory == null)
                {
                    return NotFound($"Category with ID {id} not found");
                }

                var category = _mapper.Map<Category>(categoryDto);
                await _categoryRepository.UpdateCategory(category);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with id {CategoryId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found");
                }
                await _categoryRepository.DeleteCategory(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with id {CategoryId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}

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
    [Route("api/forms")]
    [ApiController]
    public class FormApiController : ControllerBase
    {
        private readonly IFormRepository _formRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FormApiController> _logger;

        public FormApiController(IFormRepository formRepository, IMapper mapper, ILogger<FormApiController> logger)
        {
            _formRepository = formRepository ?? throw new ArgumentNullException(nameof(formRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<FormDto>>> GetAllForms()
        {
            try
            {
                var forms = await _formRepository.GetAllForms();
                return Ok(_mapper.Map<IEnumerable<FormDto>>(forms));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all forms");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("get-by-student")]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<FormDto>>> GetFormsByStudentId(int studentId)
        {
            try
            {
                var forms = await _formRepository.GetFormsByStudentId(studentId);
                return Ok(_mapper.Map<IEnumerable<FormDto>>(forms));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all forms");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FormDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<FormDto>> GetFormById(int id)
        {
            try
            {
                var form = await _formRepository.GetFormById(id);
                if (form == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<FormDto>(form));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting form with id {FormId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(FormDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddForm([FromBody] FormDto formDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var form = _mapper.Map<Form>(formDto);
                await _formRepository.AddForm(form);

                var createdForm = _mapper.Map<FormDto>(form);
                return CreatedAtAction(
                    nameof(GetFormById),
                    new { id = form.FormId },
                    createdForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new form");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Student")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateForm(int id, [FromBody] FormDto formDto)
        {
            try
            {
                if (id != formDto.FormId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingForm = await _formRepository.GetFormById(id);
                if (existingForm == null)
                {
                    return NotFound($"Form with ID {id} not found");
                }

                _mapper.Map(formDto, existingForm);
                await _formRepository.UpdateForm(existingForm);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating form with id {FormId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id:int}")]
        //[Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteForm(int id)
        {
            try
            {
                var form = await _formRepository.GetFormById(id);
                if (form == null)
                {
                    return NotFound($"Form with ID {id} not found");
                }
                await _formRepository.DeleteForm(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting form with id {FormId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}

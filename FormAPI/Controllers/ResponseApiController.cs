using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FormAPI.DTO;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using DataAccess;

namespace FormAPI.Controllers
{
    [Route("api/responses")]
    [ApiController]
    public class ResponseApiController : ControllerBase
    {
        private readonly ResponseService _responseService;
        private readonly FormService _formService;
        private readonly IMapper _mapper;
        private readonly ILogger<ResponseApiController> _logger;

        public ResponseApiController(ResponseService responseService, IMapper mapper, ILogger<ResponseApiController> logger, FormService formService)
        {
            _responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _formService = formService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ResponseDto>>> GetAllResponses()
        {
            try
            {
                var responses = await _responseService.GetAllResponses();
                return Ok(_mapper.Map<IEnumerable<ResponseDto>>(responses));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all responses");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ResponseDto>> GetResponseById(int id)
        {
            try
            {
                var response = await _responseService.GetResponseById(id);
                if (response == null)
                {
                    return NotFound($"Response with ID {id} not found");
                }
                return Ok(_mapper.Map<ResponseDto>(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting response with id {ResponseId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddResponse([FromBody] ResponseDto responseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = _mapper.Map<Response>(responseDto);
                await _responseService.AddResponse(response);
                var createdResponse = _mapper.Map<ResponseDto>(response);

                return CreatedAtAction(
                    nameof(GetResponseById),
                    new { id = createdResponse.ResponseId },
                    createdResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new response");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateResponse(int id, [FromBody] ResponseDto responseDto)
        {
            try
            {
                if (id != responseDto.ResponseId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingResponse = await _responseService.GetResponseById(id);
                if (existingResponse == null)
                {
                    return NotFound($"Response with ID {id} not found");
                }

                var response = _mapper.Map<Response>(responseDto);
                await _responseService.UpdateResponse(response);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating response with id {ResponseId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteResponse(int id)
        {
            try
            {
                var response = await _responseService.GetResponseById(id);
                if (response == null)
                {
                    return NotFound($"Response with ID {id} not found");
                }
                await _responseService.DeleteResponse(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting response with id {ResponseId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("byForm")]
        [ProducesResponseType(typeof(IEnumerable<ResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ResponseDto>>> GetResponseByFormId([FromQuery] int formId)
        {
            try
            {
                var response = await _formService.GetResponseByFormId(formId);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<ResponseDto>>(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting response by form ID");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}

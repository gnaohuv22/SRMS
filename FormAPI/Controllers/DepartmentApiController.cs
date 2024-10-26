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
    [Route("api/departments")]
    [ApiController]
    public class DepartmentApiController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentApiController> _logger;

        public DepartmentApiController(IDepartmentRepository departmentRepository, IMapper mapper, ILogger<DepartmentApiController> logger)
        {
            _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
        {
            try
            {
                var departments = await _departmentRepository.GetAllDepartments();
                return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all departments");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DepartmentDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<DepartmentDto>(department));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department with id {DepartmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(DepartmentDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddDepartment([FromBody] DepartmentDto departmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var department = _mapper.Map<Department>(departmentDto);
                await _departmentRepository.AddDepartment(department);

                var createdDepartment = _mapper.Map<DepartmentDto>(department);
                return CreatedAtAction(
                    nameof(GetDepartmentById),
                    new { id = department.DepartmentId },
                    createdDepartment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new department");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateDepartment(int id, [FromBody] DepartmentDto departmentDto)
        {
            try
            {
                if (id != departmentDto.DepartmentId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingDepartment = await _departmentRepository.GetDepartmentById(id);
                if (existingDepartment == null)
                {
                    return NotFound($"Department with ID {id} not found");
                }

                var department = _mapper.Map<Department>(departmentDto);
                await _departmentRepository.UpdateDepartment(department);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department with id {DepartmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    return NotFound($"Department with ID {id} not found");
                }

                await _departmentRepository.DeleteDepartment(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department with id {DepartmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}

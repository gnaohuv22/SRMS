using Microsoft.AspNetCore.Mvc;
using Repositories;
using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormAPI.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using FormAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace FormAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserApiController> _logger;
        private readonly IAuthentication _authentication;

        public UserApiController(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserApiController> logger, 
            IAuthentication authentication)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsers();
                return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("get-by-role")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(int role)
        {
            try
            {
                var users = await _userRepository.GetAllUsersByRole(role);
                return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with id {UserId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required");
            }

            try
            {
                var user = await _authentication.Login(request.Email);
                if (user != null)
                {
                    return Ok(_mapper.Map<UserDto>(user));
                }
                else
                {
                    _logger.LogWarning("User with email {Email} not found", request.Email);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email {Email}", request.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Logout()
        {
            return Ok("Logged out successfully.");
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<UserDto>> AddUser([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _mapper.Map<User>(userDto);
                await _userRepository.AddUser(user);

                var createdUser = _mapper.Map<UserDto>(user);
                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = createdUser.UserId },
                    createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                
                if (id != userDto.UserId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingUser = await _userRepository.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }
                userDto.CreatedAt = existingUser.CreatedAt;
                var currentUserId = Request.Headers["UserId"].ToString();
                if (int.TryParse(currentUserId, out var userId))
                {
                    if (existingUser.UserId != userId)
                    {
                        return Forbid();
                    }
                }

                _mapper.Map(userDto, existingUser);
                await _userRepository.UpdateUser(existingUser);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with id {UserId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                await _userRepository.DeleteUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with id {UserId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = Request.Headers["UserId"].ToString();

                if (int.TryParse(userId, out int id))
                {
                    var user = await _userRepository.GetUserById(id);
                    if (user == null)
                    {
                        _logger.LogWarning("User with id {Id} not found", id);
                        return NotFound();
                    }

                    return Ok(_mapper.Map<UserDto>(user));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving profile for user {Email}", User.FindFirst(ClaimTypes.Name)?.Value);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }

    public class LoginRequestDto
    {
        public string Email { get; set; }
    }
}
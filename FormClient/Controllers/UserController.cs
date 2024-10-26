using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using FormAPI.DTO;
using FormAPI.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net;

[Authorize]
public class UserController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserController> _logger;
    private readonly string _apiBaseUrl;

    public UserController(HttpClient httpClient, IConfiguration configuration, ILogger<UserController> logger)
    {
        _httpClient = httpClient;
        _apiBaseUrl = configuration["ApiBaseUrl"];
        _logger = logger;
    }

    // GET: User
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
                return View(users);
            }

            _logger.LogError("Failed to retrieve users. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve users.";
            return View(Enumerable.Empty<UserDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving users");
            TempData["Error"] = "An error occurred while retrieving users.";
            return View(Enumerable.Empty<UserDto>());
        }
    }

    // GET: User/Details/5
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        if (!User.IsInRole("Admin") && id.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        {
            return Forbid();
        }
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return View(user);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve user details. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve user details.";
            return RedirectToAction("Index", "User");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user details for ID {UserId}", id);
            TempData["Error"] = "An error occurred while retrieving user details.";
            return RedirectToAction("Index", "User");
        }
    }

    // GET: User/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(UserDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return View(userDto);
        }

        try
        {
            userDto.CreatedAt = DateTime.Now;
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/users", userDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User created successfully.";
                return RedirectToAction("Index", "User");
            }

            _logger.LogError("Failed to create user. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to create user.");
            return View(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            ModelState.AddModelError("", "An error occurred while creating the user.");
            return View(userDto);
        }
    }

    // GET: User/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return View(user);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve user for editing. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve user for editing.";
            return RedirectToAction("Index", "User");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user for editing for ID {UserId}", id);
            TempData["Error"] = "An error occurred while retrieving user for editing.";
            return RedirectToAction("Index", "User");
        }
    }

    // POST: User/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, UserDto userDto)
    {
        if (id != userDto.UserId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(userDto);
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/users/{id}", userDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction("Index", "User");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to update user. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to update user.");
            return View(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
            ModelState.AddModelError("", "An error occurred while updating the user.");
            return View(userDto);
        }
    }

    // GET: User/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return View(user);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve user for deletion. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve user for deletion.";
            return RedirectToAction("Index", "User");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user for deletion for ID {UserId}", id);
            TempData["Error"] = "An error occurred while retrieving user for deletion.";
            return RedirectToAction("Index", "User");
        }
    }

    // POST: User/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/users/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction("Index", "Home");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to delete user. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to delete user.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
            TempData["Error"] = "An error occurred while deleting the user.";
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: User/Login
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    // POST: User/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Email))
        {
            ModelState.AddModelError("Email", "Email is required.");
            return View(loginRequest);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/users/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var userDto = await response.Content.ReadFromJsonAsync<UserDto>();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userDto.Email),
                    new Claim(ClaimTypes.Role, userDto.Role.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userDto.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(15)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                TempData["Success"] = "Logged in successfully.";
                return RedirectToAction("Index", "Home");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError("", "User not found.");
                return View(loginRequest);
            }

            _logger.LogError("Failed to login. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to login.");
            return View(loginRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in user with email {Email}", loginRequest.Email);
            ModelState.AddModelError("", "An error occurred while logging in.");
            return View(loginRequest);
        }
    }

    // POST: User/Logout
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _httpClient.PostAsync($"{_apiBaseUrl}/users/logout", null);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging out");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }

    //GET: User/Profile
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _httpClient.DefaultRequestHeaders.Add("UserId", userId);
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users/profile");

            if (response.IsSuccessStatusCode)
            {
                var userProfile = await response.Content.ReadFromJsonAsync<UserDto>();
                return View(userProfile);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Failed to retrieve your profile.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user profile");
            TempData["Error"] = "An error occurred while retrieving your profile.";
            return RedirectToAction("Index", "Home");
        }
    }
}
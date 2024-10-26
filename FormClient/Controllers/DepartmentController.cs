using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FormAPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DepartmentController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DepartmentController> _logger;
    private readonly string _apiBaseUrl;

    public DepartmentController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DepartmentController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7066/api";
        _logger = logger;
    }

    // GET: Department
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/departments");
            if (response.IsSuccessStatusCode)
            {
                var departments = await response.Content.ReadFromJsonAsync<IEnumerable<DepartmentDto>>();
                return View(departments);
            }

            _logger.LogError("Failed to retrieve departments. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve departments.";
            return View(Enumerable.Empty<DepartmentDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving departments.");
            TempData["Error"] = "An error occurred while retrieving departments.";
            return View(Enumerable.Empty<DepartmentDto>());
        }
    }

    // GET: Department/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/departments/{id}");
            if (response.IsSuccessStatusCode)
            {
                var department = await response.Content.ReadFromJsonAsync<DepartmentDto>();
                return View(department);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve department details. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve department details.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving department details for ID {DepartmentId}", id);
            TempData["Error"] = "An error occurred while retrieving department details.";
            return RedirectToAction("Index");
        }
    }

    // GET: Department/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Department/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(DepartmentDto departmentDto)
    {
        if (!ModelState.IsValid)
        {
            return View(departmentDto);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/departments", departmentDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Department created successfully.";
                return RedirectToAction("Index");
            }

            _logger.LogError("Failed to create department. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to create department.");
            return View(departmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating department");
            ModelState.AddModelError("", "An error occurred while creating the department.");
            return View(departmentDto);
        }
    }

    // GET: Department/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/departments/{id}");
            if (response.IsSuccessStatusCode)
            {
                var department = await response.Content.ReadFromJsonAsync<DepartmentDto>();
                return View(department);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve department for editing. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve department for editing.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving department for editing for ID {DepartmentId}", id);
            TempData["Error"] = "An error occurred while retrieving department for editing.";
            return RedirectToAction("Index");
        }
    }

    // POST: Department/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, DepartmentDto departmentDto)
    {
        if (id != departmentDto.DepartmentId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(departmentDto);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/departments/{id}", departmentDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Department updated successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to update department. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to update department.");
            return View(departmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating department with ID {DepartmentId}", id);
            ModelState.AddModelError("", "An error occurred while updating the department.");
            return View(departmentDto);
        }
    }

    // GET: Department/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/departments/{id}");
            if (response.IsSuccessStatusCode)
            {
                var department = await response.Content.ReadFromJsonAsync<DepartmentDto>();
                return View(department);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve department for deletion. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve department for deletion.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving department for deletion for ID {DepartmentId}", id);
            TempData["Error"] = "An error occurred while retrieving department for deletion.";
            return RedirectToAction("Index");
        }
    }

    // POST: Department/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/departments/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Department deleted successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to delete department. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to delete department.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting department with ID {DepartmentId}", id);
            TempData["Error"] = "An error occurred while deleting the department.";
            return RedirectToAction("Index");
        }
    }
}

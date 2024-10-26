using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using FormAPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;

public class FormController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FormController> _logger;
    private readonly string _apiBaseUrl;

    public FormController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FormController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7066/api";
        _logger = logger;
    }

    // GET: Form
    public async Task<IActionResult> Index()
    {
        try
        {
            IEnumerable<FormDto> forms;
            if (User.IsInRole("Admin"))
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/forms");
                if (response.IsSuccessStatusCode)
                {
                    forms = await response.Content.ReadFromJsonAsync<IEnumerable<FormDto>>();
                }
                else
                {
                    _logger.BeginScope("Failed to retrieve forms. Status code: {StatusCode}", response.StatusCode);
                    TempData["Error"] = "Failed to retrieve forms.";
                    forms = Enumerable.Empty<FormDto>();
                }
            }
            //else if (User.IsInRole("Department"))
            //{
            //    //TODO: Implement Department logic later
            //}
            else 
            {
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/forms/get-by-student?studentId={studentId}");
                if (response.IsSuccessStatusCode)
                {
                    forms = await response.Content.ReadFromJsonAsync<IEnumerable<FormDto>>();
                }
                else
                {
                    _logger.BeginScope("Failed to retrieve forms for student ID {StudentId}. Status code: {StatusCode}", studentId, response.StatusCode);
                    TempData["Error"] = "Failed to retrieve forms.";
                    forms = Enumerable.Empty<FormDto>();
                }
            }
            return View(forms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving forms");
            TempData["Error"] = "An error occurred while retrieving forms.";
            return View(Enumerable.Empty<FormDto>());
        }
    }

    // GET: Form/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/forms/{id}");
            if (response.IsSuccessStatusCode)
            {
                var form = await response.Content.ReadFromJsonAsync<FormDto>();
                return View(form);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve form details. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve form details.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving form details for ID {FormId}", id);
            TempData["Error"] = "An error occurred while retrieving form details.";
            return RedirectToAction("Index");
        }
    }

    // GET: Form/Create
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> Create()
    {
        var formDto = new FormDto();
        if (User.IsInRole("Student"))
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(studentIdClaim, out int studentId))
            {
                formDto.StudentId = studentId;
            }
        }
        var categories = await GetCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

        if (User.IsInRole("Admin"))
        {
            var students = await GetStudentsAsync();
            ViewBag.Students = new SelectList(students, "UserId", "Email");
        }
        return View(formDto);
    }

    // POST: Form/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> Create(FormDto formDto)
    {
        ModelState.Remove(nameof(formDto.CategoryName));
        ModelState.Remove(nameof(formDto.StudentEmail));

        if (!ModelState.IsValid)
        {
            var categories = await GetCategoriesAsync();
            var students = await GetStudentsAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Students = new SelectList(students, "UserId", "Email");
            return View(formDto);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/forms", formDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Form created successfully.";
                return RedirectToAction("Index");
            }

            _logger.LogError("Failed to create form. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to create form.");
            var categories = await GetCategoriesAsync();
            var students = await GetStudentsAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Students = new SelectList(students, "UserId", "Email");
            return View(formDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating form");
            ModelState.AddModelError("", "An error occurred while creating the form.");
            var categories = await GetCategoriesAsync();
            var students = await GetStudentsAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Students = new SelectList(students, "UserId", "Email");
            return View(formDto);
        }
    }

    // GET: Form/Edit/5
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/forms/{id}");
            if (response.IsSuccessStatusCode)
            {
                var form = await response.Content.ReadFromJsonAsync<FormDto>();
                if (form.Status != BusinessObject.FormStatus.Pending)
                {
                    TempData["Error"] = "Form can only be edited in Pending status.";
                    return RedirectToAction("Index");
                }
                var categories = await GetCategoriesAsync();
                var students = await GetStudentsAsync();

                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
                ViewBag.Students = new SelectList(students, "UserId", "Email");
                return View(form);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve form for editing. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve form for editing.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving form for editing for ID {FormId}", id);
            TempData["Error"] = "An error occurred while retrieving form for editing.";
            return RedirectToAction("Index");
        }
    }

    // POST: Form/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Edit(int id, FormDto formDto)
    {
        if (id != formDto.FormId)
        {
            return BadRequest();
        }

        ModelState.Remove(nameof(formDto.CategoryName));
        ModelState.Remove(nameof(formDto.StudentEmail));

        if (!ModelState.IsValid)
        {
            var categories = await GetCategoriesAsync();
            var students = await GetStudentsAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Students = new SelectList(students, "UserId", "Email");
            return View(formDto);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/forms/{id}", formDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Form updated successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to update form. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to update form.");
            var categories = await GetCategoriesAsync();
            var students = await GetStudentsAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Students = new SelectList(students, "UserId", "Email");
            return View(formDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating form with ID {FormId}", id);
            ModelState.AddModelError("", "An error occurred while updating the form.");
            var categories = await GetCategoriesAsync();
            var students = await GetStudentsAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Students = new SelectList(students, "UserId", "Email");
            return View(formDto);
        }
    }

    // GET: Form/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/forms/{id}");
            if (response.IsSuccessStatusCode)
            {
                var form = await response.Content.ReadFromJsonAsync<FormDto>();
                if (form.Status != BusinessObject.FormStatus.Pending)
                {
                    TempData["Error"] = "Form can only be deleted in Pending status.";
                    return RedirectToAction("Index");
                }
                return View(form);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve form for deletion. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve form for deletion.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving form for deletion for ID {FormId}", id);
            TempData["Error"] = "An error occurred while retrieving form for deletion.";
            return RedirectToAction("Index");
        }
    }

    // POST: Form/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/forms/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Form deleted successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to delete form. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to delete form.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting form with ID {FormId}", id);
            TempData["Error"] = "An error occurred while deleting the form.";
            return RedirectToAction("Index");
        }
    }

    private async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
        }
        _logger.LogError("Failed to retrieve categories. Status code: {StatusCode}", response.StatusCode);
        return Enumerable.Empty<CategoryDto>();
    }

    private async Task<IEnumerable<UserDto>> GetStudentsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users/0");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        }
        _logger.LogError("Failed to retrieve users. Status code: {StatusCode}", response.StatusCode);
        return Enumerable.Empty<UserDto>();
    }
}
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

public class CategoryController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoryController> _logger;
    private readonly string _apiBaseUrl;

    public CategoryController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CategoryController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7066/api";
        _logger = logger;
    }

    // GET: Category
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
                return View(categories);
            }

            _logger.LogError("Failed to retrieve categories. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve categories.";
            return View(Enumerable.Empty<CategoryDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving categories.");
            TempData["Error"] = "An error occurred while retrieving categories.";
            return View(Enumerable.Empty<CategoryDto>());
        }
    }

    // GET: Category/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
                return View(category);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve category details. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve category details.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category details for ID {CategoryId}", id);
            TempData["Error"] = "An error occurred while retrieving category details.";
            return RedirectToAction("Index");
        }
    }

    // GET: Category/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return View(categoryDto);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/categories", categoryDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }

            _logger.LogError("Failed to create category. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to create category.");
            return View(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category");
            ModelState.AddModelError("", "An error occurred while creating the category.");
            return View(categoryDto);
        }
    }

    // GET: Category/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
                return View(category);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve category for editing. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve category for editing.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category for editing for ID {CategoryId}", id);
            TempData["Error"] = "An error occurred while retrieving category for editing.";
            return RedirectToAction("Index");
        }
    }

    // POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, CategoryDto categoryDto)
    {
        if (id != categoryDto.CategoryId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(categoryDto);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/categories/{id}", categoryDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to update category. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to update category.");
            return View(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category with ID {CategoryId}", id);
            ModelState.AddModelError("", "An error occurred while updating the category.");
            return View(categoryDto);
        }
    }

    // GET: Category/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
                return View(category);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve category for deletion. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve category for deletion.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category for deletion for ID {CategoryId}", id);
            TempData["Error"] = "An error occurred while retrieving category for deletion.";
            return RedirectToAction("Index");
        }
    }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to delete category. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to delete category.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category with ID {CategoryId}", id);
            TempData["Error"] = "An error occurred while deleting the category.";
            return RedirectToAction("Index");
        }
    }
}

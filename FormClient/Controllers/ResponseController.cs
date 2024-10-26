using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FormAPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject;

public class ResponseController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResponseController> _logger;
    private readonly string _apiBaseUrl;

    public ResponseController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ResponseController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7066/api";
        _logger = logger;
    }

    // GET: Response
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/responses");
            if (response.IsSuccessStatusCode)
            {
                var responses = await response.Content.ReadFromJsonAsync<IEnumerable<ResponseDto>>();
                return View(responses);
            }

            _logger.LogError("Failed to retrieve responses. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve responses.";
            return View(Enumerable.Empty<ResponseDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving responses.");
            TempData["Error"] = "An error occurred while retrieving responses.";
            return View(Enumerable.Empty<ResponseDto>());
        }
    }

    // GET: Response/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/responses/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
                return View(responseDto);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve response details. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve response details.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving response details for ID {ResponseId}", id);
            TempData["Error"] = "An error occurred while retrieving response details.";
            return RedirectToAction("Index");
        }
    }

    // GET: Response/Reply/5
    [Authorize(Roles = "Department")]
    public async Task<IActionResult> Reply(int formId)
    {
        var formResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/forms/{formId}");
        if (formResponse.IsSuccessStatusCode)
        {
            var formDto = await formResponse.Content.ReadFromJsonAsync<FormDto>();
            var responseDto = new ResponseDto
            {
                FormId = formDto.FormId,
                FormSubject = formDto.Subject,
                FormContent = formDto.Content,
                CreatedAt = DateTime.Now
            };
            return View(responseDto);
        }
        else return NotFound();
    }

    // POST: Response/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Department")]
    public async Task<IActionResult> Reply(ResponseDto responseDto)
    {
        if (!ModelState.IsValid)
        {
            return View(responseDto);
        }

        try
        {
            var formResponse = await _httpClient.GetFromJsonAsync<FormDto>($"{_apiBaseUrl}/forms/{responseDto.FormId}");
            if (formResponse == null)
            {
                TempData["Error"] = "Form not found.";
                return NotFound();
            }

            formResponse.Status = FormStatus.Processing;
            await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/forms/{responseDto.FormId}", formResponse);

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/responses", responseDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Response created successfully.";
                return RedirectToAction("Index", "Form");
            }

            _logger.LogError("Failed to create response. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to create response.");
            return View(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating response");
            ModelState.AddModelError("", "An error occurred while creating the response.");
            return View(responseDto);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Department")]
    public async Task<IActionResult> UpdateStatus(int id, string status, string reason)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/responses/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Response not found.";
                return NotFound();
            }

            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            responseDto.Content = $"Update at: {DateTime.Now}\n{reason}\n\n{responseDto.Content}";

            var formResponse = await _httpClient.GetFromJsonAsync<FormDto>($"{_apiBaseUrl}/forms/{responseDto.FormId}");
            if (formResponse == null)
            {
                TempData["Error"] = "Form not found.";
                return NotFound();
            }
            formResponse.Status = status == "Accepted" ? FormStatus.Accepted : FormStatus.Rejected;

            var updateResponse = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/responses/{id}", responseDto);
            var updateForm = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/forms/{responseDto.FormId}", formResponse);

            if (updateResponse.IsSuccessStatusCode && updateForm.IsSuccessStatusCode)
            {
                TempData["Success"] = "Response updated successfully.";
                return RedirectToAction("Index");
            }

            _logger.LogError("Failed to update response. Status code: {StatusCode}", updateResponse.StatusCode);
            TempData["Error"] = "Failed to update response.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating response with ID {ResponseId}", id);
            TempData["Error"] = "An error occurred while updating the response.";
            return RedirectToAction("Index");
        }
    }

    // GET: Response/Edit/5
    [Authorize(Roles = "Department")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/responses/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
                return View(responseDto);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve response for editing. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve response for editing.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving response for editing for ID {ResponseId}", id);
            TempData["Error"] = "An error occurred while retrieving response for editing.";
            return RedirectToAction("Index");
        }
    }

    // POST: Response/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, ResponseDto responseDto)
    {
        if (id != responseDto.ResponseId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(responseDto);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/responses/{id}", responseDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Response updated successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to update response. Status code: {StatusCode}", response.StatusCode);
            ModelState.AddModelError("", "Failed to update response.");
            return View(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating response with ID {ResponseId}", id);
            ModelState.AddModelError("", "An error occurred while updating the response.");
            return View(responseDto);
        }
    }

    // GET: Response/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/responses/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
                return View(responseDto);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to retrieve response for deletion. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to retrieve response for deletion.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving response for deletion for ID {ResponseId}", id);
            TempData["Error"] = "An error occurred while retrieving response for deletion.";
            return RedirectToAction("Index");
        }
    }

    // POST: Response/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/responses/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Response deleted successfully.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            _logger.LogError("Failed to delete response. Status code: {StatusCode}", response.StatusCode);
            TempData["Error"] = "Failed to delete response.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting response with ID {ResponseId}", id);
            TempData["Error"] = "An error occurred while deleting the response.";
            return RedirectToAction("Index");
        }
    }
}

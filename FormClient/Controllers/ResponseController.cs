using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FormAPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormClient.Controllers
{
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
        [Authorize]
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
        [Authorize]
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
            try
            {
                var formResponse = await _httpClient.GetFromJsonAsync<FormDto>($"{_apiBaseUrl}/forms/{formId}");
                if (formResponse == null)
                {
                    TempData["Error"] = "Form not found.";
                    return NotFound();
                }
                var exitstingResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/responses/byForm?formId={formId}");
                if (exitstingResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "This form has already been replied to.";
                    return RedirectToAction("Index", "Form");
                }

                var viewModel = new ReplyViewModel
                {
                    FormId = formResponse.FormId,
                    FormSubject = formResponse.Subject,
                    FormContent = formResponse.Content
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing reply for form ID {FormId}", formId);
                TempData["Error"] = "An error occurred while preparing the reply.";
                return RedirectToAction("Index", "Form");
            }
            
        }

        // POST: Response/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Department")]
        public async Task<IActionResult> Reply(ReplyViewModel replyViewModel)
        {
            var departmentEmail = User.FindFirst(ClaimTypes.Name).Value;

            if (!ModelState.IsValid)
            {
                return View(replyViewModel);
            }

            var responseDto = new ResponseDto
            {
                FormId = replyViewModel.FormId,
                StaffEmail = departmentEmail,
                Content = replyViewModel.Content,
                CreatedAt = DateTime.Now,
                StaffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                FormSubject = replyViewModel.FormSubject,
                FormContent = replyViewModel.FormContent
            };

            try
            {
                var formResponse = await _httpClient.GetFromJsonAsync<FormDto>($"{_apiBaseUrl}/forms/{responseDto.FormId}");
                if (formResponse == null)
                {
                    TempData["Error"] = "Form not found.";
                    return NotFound();
                }

                // Update form status based on radio selection
                formResponse.Status = (FormStatus)replyViewModel.Status;
                await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/forms/{responseDto.FormId}", formResponse);

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/responses", responseDto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Response created successfully.";
                    return RedirectToAction("Index", "Form");
                }

                _logger.LogError("Failed to create response. Status code: {StatusCode}", response.StatusCode);
                ModelState.AddModelError("", "Failed to create response.");
                return View(replyViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating response");
                ModelState.AddModelError("", "An error occurred while creating the response.");
                return View(replyViewModel);
            }
        }



        [HttpPost]
        [Authorize(Roles = "Department")]
        public async Task<IActionResult> UpdateStatus(int id, string status, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    TempData["Error"] = "Reason is required for status update.";
                    return RedirectToAction("Update", new { id });
                }
                var (form, response) = await GetFormAndResponseData(id);
                if (form == null || response == null)
                {
                    return RedirectToAction("Index", "Form");
                }

                if (form.Status != FormStatus.Processing)
                {
                    TempData["Error"] = "Only forms in Processing status can be updated.";
                    return RedirectToAction("Index");
                }

                response.Content = FormatUpdateContent(reason, response.Content);
                form.Status = ParseFormStatus(status);

                await Task.WhenAll(
                    UpdateResponse(response),
                    UpdateForm(form)
                );

                TempData["Success"] = "Response updated successfully. Form status also updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating response with ID {ResponseId}", id);
                TempData["Error"] = "An error occurred while updating the response.";
                return RedirectToAction("Index");
            }
        }

        private async Task UpdateForm(FormDto form)
        {
            var result = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/forms/{form.FormId}", form);
            result.EnsureSuccessStatusCode();
        }

        private async Task UpdateResponse(ResponseDto response)
        {
            var result = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/responses/{response.ResponseId}", response);
            result.EnsureSuccessStatusCode();
        }

        private FormStatus ParseFormStatus(string status) =>
            status.ToLower() == "accepted" ? FormStatus.Accepted : FormStatus.Rejected;
        

        private string FormatUpdateContent(string reason, string content)
        {
            return $"Update at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nReason: {reason}\n\n{content}";
        }

        // GET: Response/Update/5
        [Authorize(Roles = "Department")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var (form, response) = await GetFormAndResponseData(id);
                if (form == null || response == null)
                {
                    return RedirectToAction("Index", "Form");
                }

                if (form.Status != FormStatus.Processing)
                {
                    TempData["Error"] = "Only forms in Processing status can be updated.";
                    return RedirectToAction("Index", "Form");
                }

                var viewModel = new UpdateResponseViewModel
                {
                    Form = form,
                    Response = response
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving update data for response {ResponseId}", id);
                TempData["Error"] = "An error occurred while retrieving the response data.";
                return RedirectToAction("Index", "Form");
            }
        }

        private async Task<(FormDto form, ResponseDto response)> GetFormAndResponseData(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ResponseDto>($"{_apiBaseUrl}/responses/byForm?formId={id}");
            if (response == null)
            {
                TempData["Error"] = "Response not found.";
                return (null, null);
            }

            var form = await _httpClient.GetFromJsonAsync<FormDto>($"{_apiBaseUrl}/forms/{response.FormId}");
            if (form == null)
            {
                TempData["Error"] = "Associated form not found.";
                return (null, null);
            }
            return (form, response);
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

    public class UpdateResponseViewModel
    {
        public FormDto Form { get; set; }
        public ResponseDto Response { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }

    public class ReplyViewModel
    {
        public string Content { get; set; }
        public int FormId { get; set; }
        public int Status { get; set; }
        public string FormSubject { get; set; }
        public string FormContent { get; set; }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WWWatering.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public string Message { get; private set; }

        [BindProperty]
        public int SliderValue { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostButtonClickAsync()
        {
            try 
            {
                var httpClient = _clientFactory.CreateClient();
                var json = new { SliderValue = SliderValue, User = User.Identity?.Name };
                var content = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("http://127.0.0.1:5555/api/water", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Process the response content as needed
                    Message = $"Button clicked! Slider value: {SliderValue}. API call successful." + response.Content.ToString();
                }
                else
                {
                    Message = $"Button clicked! Slider value: {SliderValue}. API call failed.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Button clicked! Slider value: {SliderValue}. API call failed. Exception: {ex.Message}";
            }

            return Page();
        }
    }
}
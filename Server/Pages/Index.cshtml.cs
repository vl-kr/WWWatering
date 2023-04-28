using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
                var response = await httpClient.GetAsync("http://127.0.0.1:5555/api/get-humidity");
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
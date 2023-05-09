using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WWWatering.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly PlantInfo _plantInfo;

        public int WaterVolumePerSecond = 20;
        public string? ErrorInfo => _plantInfo.ErrorInfo;
        public string PostRequestWateringMessage { get; private set; }

        private string GetTimeElapsedMessage(DateTime lastEvent)
        {
            string timeElapsedMessage;
            if ((DateTime.UtcNow - lastEvent).TotalDays > 1)
            {
                double days = (DateTime.UtcNow - lastEvent).TotalDays;
                timeElapsedMessage = $"{Math.Round(days, 2)} days ago";
            }
            else if ((DateTime.UtcNow - lastEvent).TotalHours > 1)
            {
                double hours = (DateTime.UtcNow - lastEvent).TotalHours;
                timeElapsedMessage = $"{Math.Round(hours, 2)} hours ago";
            }
            else if ((DateTime.UtcNow - lastEvent).TotalMinutes > 1)
            {
                double minutes = (DateTime.UtcNow - lastEvent).TotalMinutes;
                timeElapsedMessage = $"{Math.Round(minutes, 2)} minutes ago";
            }
            else
            {
                timeElapsedMessage = $"{(int)(DateTime.UtcNow - lastEvent).TotalSeconds} seconds ago";
            }
            return timeElapsedMessage;
        }

        public string HumdityValueMessage
        {
            get
            {
                if (_plantInfo.Humidity == null)
                {
                    return "Unknown";
                }
                else
                {
                    return _plantInfo.Humidity.Value.ToString() + '%';
                }
            }
        }

        public string HumidityLastUpdatedMessage 
        { 
            get
            {
                if (_plantInfo.LastChecked == null)
                {
                    return "Last updated: never";
                }
                else
                {
                    string timeElapsedMessage = GetTimeElapsedMessage(_plantInfo.LastChecked.Value);
                    return $"Last updated: {_plantInfo.LastChecked.Value.ToLongTimeString()} ({timeElapsedMessage})";
                }
            }
        }

        public string LastWateringMessage
        {
            get
            {
                if (_plantInfo.LastWatering == null)
                {
                    return "Never";
                }
                else
                {
                    string timeElapsedMessage = GetTimeElapsedMessage(_plantInfo.LastWatering.Value);
                    
                    return $"{_plantInfo.LastWatering.Value.ToLongTimeString()} ({timeElapsedMessage})";
                }
            }
        }

        [BindProperty]
        public int SliderValue { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, PlantInfo plantInfo)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("TunneledClient");
            this._plantInfo = plantInfo;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostButtonClickAsync()
        {
            try 
            {
                var json = new { SliderValue = SliderValue.ToString(), User = User.Identity?.Name };
                var content = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/water", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Process the response content as needed
                    _plantInfo.LastWatering = DateTime.UtcNow;
                    PostRequestWateringMessage = $"Request to water for {SliderValue} seconds sent successfully!";
                }
                else
                {
                    PostRequestWateringMessage = $"Request to water for {SliderValue} seconds failed with code {response.StatusCode}.";
                }
            }
            catch (Exception ex)
            {
                PostRequestWateringMessage = $"Request to water for {SliderValue} seconds failed with exception: {ex.Message}";
            }

            return Page();
        }
    }
}
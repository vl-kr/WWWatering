using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WWWatering;
public class AutoHumidityUpdaterBackgroundService : IHostedService, IDisposable
{
    private readonly PlantInfo _plantInfo;
    private readonly HttpClient _httpClient;
    private Timer _timer;

    public AutoHumidityUpdaterBackgroundService(PlantInfo plantInfo, IHttpClientFactory httpClientFactory)
    {
        _plantInfo = plantInfo;
        _httpClient = httpClientFactory.CreateClient("TunneledClient");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(RequestHumidity, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    private async void RequestHumidity(object? state)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/get-humidity");
            response.EnsureSuccessStatusCode();
            var responseDict = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            _plantInfo.Humidity = responseDict["Humidity"];
            _plantInfo.ErrorInfo = null;
        }
        catch (Exception ex)
        {
            _plantInfo.ErrorInfo = ex.Message;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

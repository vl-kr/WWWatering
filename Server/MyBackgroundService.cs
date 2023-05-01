using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWWatering;
public class MyBackgroundService : IHostedService, IDisposable
{
    private readonly MyService _myService;
    private Timer _timer;

    public MyBackgroundService(MyService myService)
    {
        _myService = myService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5)); // Adjust the interval as needed
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        _myService.GetMessage();
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

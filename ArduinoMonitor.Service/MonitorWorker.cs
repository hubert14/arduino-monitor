using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using ArduinoMonitor.Common.Controllers;

namespace ArduinoMonitor.Service
{
    public class MonitorWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MainController.Start();
        }
    }
}
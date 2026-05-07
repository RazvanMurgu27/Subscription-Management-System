using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProiectAbonamente.Logica;
using ProiectAbonamente.Servicii;

namespace ProiectAbonamente;

class Program
{
    static async Task Main(string[] args)
    {
        
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IServiciuStocare, ServiciuFisiere>();
                services.AddSingleton<SistemCentral>();
                services.AddTransient<AplicatieConsola>();
            })
            .ConfigureLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        var app = host.Services.GetRequiredService<AplicatieConsola>();
        await app.Ruleaza();
    }
}
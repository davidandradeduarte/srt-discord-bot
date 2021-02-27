using System.Threading.Tasks;
using SimpleRandomTeams.Services;

namespace SimpleRandomTeams
{
    internal static class Program
    {
        private static async Task Main()
        {
            LoggingService.AddLogging();
            await new BotService().InitAsync();
        }
    }
}
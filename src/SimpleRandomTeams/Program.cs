using System.Threading.Tasks;
using SimpleRandomTeams.Services;

namespace SimpleRandomTeams
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            LoggerService.AddLogging();
            await new BotService().InitAsync();
        }
    }
}
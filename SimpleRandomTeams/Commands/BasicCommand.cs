using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SimpleRandomTeams.Commands
{
    public class BasicCommands : IModule
    {
        [Command("yo")]
        [Description("Test if the bot is running.")]
        public async Task Alive(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("yo.");
        }
        
        [Command("reset")]
        [Description("Reset in memory database.")]
        public async Task Reset(CommandContext ctx)
        {
            InMemoryDatabase.Reset(); 
            await ctx.RespondAsync("Everything good.");
        }
    }
}
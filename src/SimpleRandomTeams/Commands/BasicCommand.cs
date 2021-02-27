using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Serilog;

namespace SimpleRandomTeams.Commands
{
    public class BasicCommands : IModule
    {
        [Command("yo")]
        [Description("Test if the bot is running.")]
        public async Task Alive(CommandContext ctx)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordGame{Name = "teste"});
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("yo.");
        }

        [Command("commands")]
        [Description("List available commands.")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            
            var embed = new DiscordEmbedBuilder
                {
                    Title = "Simple Team Generator",
                    Timestamp = DateTimeOffset.Now,
                    Color = new DiscordColor(0xFF6133)
                };
            
            embed.AddField("!map", "Generate a random map to play from the csgo scrim map pool.");
            embed.AddField("!teams", "Generate random teams with members in the current voice channel.");
            embed.AddField("!veto", "Picks one random member from each team to start a veto process.");
            embed.AddField("!ban", "Bans a map from the veto available maps.");
            embed.AddField("!split", "Split the generated teams to their individual team voice channels.");
            embed.AddField("!end", "Move team members to the original voice channel.");
            embed.AddField("!reset", "Reset in memory database.");
            embed.AddField("!yo", "Test if the bot is running.");
            
            await ctx.RespondAsync(embed: embed);
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
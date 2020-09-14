using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Serilog;

namespace SimpleRandomTeams.Commands
{
    public class MapCommand : IModule
    {
        [Command("map")]
        [Description("Generate a random map to play from the csgo scrim map pool.")]
        public async Task Map(CommandContext ctx)
        {
            try
            {
                // TODO: move to a custom "auth" attribute
                if (ctx.Member.Roles.FirstOrDefault(x => x.Name == "Ducks") == null)
                {
                    Log.Warning($"User {ctx.Member.DisplayName} has no access to execute this command.");
                    return;
                }
            
                Log.Information("Generating a random map to play from the csgo scrim map pool.");
            
                if (ctx.Member.VoiceState == null)
                {
                    Log.Warning($"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }
                
                await ctx.TriggerTypingAsync();

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Simple Team Generator",
                    Timestamp = DateTimeOffset.Now,
                    Color = new DiscordColor(0xFF6133)
                };

                string[] maps = {"Inferno", "Train", "Mirage", "Nuke", "Overpass", "DustII", "Vertigo", "Cache"};

                var random = new Random();

                var map = maps[random.Next(0,maps.Length)];    
                    
                embed.AddField($"Map {DiscordEmoji.FromName(ctx.Client, ":arrow_down:")}\n", map);

                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"{map}, what a good choice! {DiscordEmoji.FromName(ctx.Client, ":muscle:")}"
                };

                embed.Fields.ToList().ForEach(x => Log.Information($"\n{x.Name}\n{x.Value}"));
                await ctx.Message.Channel.SendMessageAsync(embed: embed);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
    }
}
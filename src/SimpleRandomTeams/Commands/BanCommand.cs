using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SimpleRandomTeams.Commands.Interfaces;
using SimpleRandomTeams.Services;

namespace SimpleRandomTeams.Commands
{
    public class BanCommand : BaseCommandModule, IModule
    {
        [Command("ban")]
        [Description("Bans a map from the veto available maps.")]
        public async Task Ban(CommandContext ctx, string map)
        {
            try
            {
                map = map?.ToLower();
                
                var db = InMemoryDatabase.Instance;
                
                if (!new List<DiscordMember>{db.VetoPlayerTeam1, db.VetoPlayerTeam2}.Contains(ctx.Member))
                {
                    LoggerService.LogWarning(ctx.Client, $"User {ctx.Member.DisplayName} has no access to execute this command.");
                    return;
                }
            
                LoggerService.LogInformation(ctx.Client, $"{ctx.Member.DisplayName} voting to ban {map}.");
            
                await ctx.TriggerTypingAsync();

                if (ctx.Member.VoiceState == null)
                {
                    LoggerService.LogWarning(ctx.Client, $"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }

                if (string.IsNullOrEmpty(map))
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} please type the map name after the `!ban` command.");
                    
                    var embedDefaultMaps = new DiscordEmbedBuilder
                    {
                        Title = "Simple Team Generator",
                        Timestamp = DateTimeOffset.Now,
                        Color = new DiscordColor(0xFF6133)
                    };
                    
                    embedDefaultMaps.AddField($"Available maps {DiscordEmoji.FromName(ctx.Client, ":map:")}",
                        string.Join('\n', db.DefaultMaps.Select(x => $"- {x}")));
                    
                    await ctx.RespondAsync(embed: embedDefaultMaps);
                    return;
                }

                if (db.LastVetoPlayer == ctx.Member)
                {
                    await ctx.RespondAsync($"");
                    var emb = new DiscordEmbedBuilder
                    {
                        Title = "Simple Team Generator",
                        Timestamp = DateTimeOffset.Now,
                        Color = new DiscordColor(0xFF6133)
                    };

                    emb.AddField("",
                        $"{ctx.Member.Mention} let your opponent choose first. {DiscordEmoji.FromName(ctx.Client, ":wink:")}");
                    
                    await ctx.RespondAsync(embed: emb);
                    return;
                }
                
                if(!db.VetoMaps.Contains(map))
                {
                    var emb = new DiscordEmbedBuilder
                    {
                        Title = "Simple Team Generator",
                        Timestamp = DateTimeOffset.Now,
                        Color = new DiscordColor(0xFF6133)
                    };

                    emb.AddField("",
                        $"{ctx.Member.Mention} map is not available or doesn't exist. {DiscordEmoji.FromName(ctx.Client, ":confused:")}");
                    
                    await ctx.RespondAsync(embed: emb);
                    return;
                }

                db.VetoMaps.Remove(map);
                db.LastVetoPlayer = ctx.Member;
                
                if (db.VetoMaps.Count == 1)
                {
                    var embedVetoMap = new DiscordEmbedBuilder
                    {
                        Title = "Simple Team Generator",
                        Timestamp = DateTimeOffset.Now,
                        Color = new DiscordColor(0xFF6133)
                    };
                    
                    embedVetoMap.AddField($"We have a map {DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")}",
                        string.Join('\n', db.VetoMaps.Select(x => $"- {x}")));

                    embedVetoMap.Fields.ToList().ForEach(x => LoggerService.LogInformation(ctx.Client, $"\n{x.Name}\n{x.Value}"));
                    await ctx.RespondAsync(embed: embedVetoMap);

                    db.VetoPlayerTeam1 = default;
                    db.VetoPlayerTeam2 = default;
                    db.LastVetoPlayer = default;
                    db.VetoMaps = new List<string>();
                    return;
                }

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Simple Team Generator",
                    Timestamp = DateTimeOffset.Now,
                    Color = new DiscordColor(0xFF6133)
                };
                    
                embed.AddField($"Veto maps {DiscordEmoji.FromName(ctx.Client, ":map:")}",
                    string.Join('\n', db.VetoMaps.Select(x => $"- {x}")));

                embed.Fields.ToList().ForEach(x => LoggerService.LogInformation(ctx.Client, $"\n{x.Name}\n{x.Value}"));
                await ctx.RespondAsync(embed: embed);
            }
            catch (Exception e)
            {
                LoggerService.LogError(ctx.Client, e, e.Message);
            }
        }
    }
}
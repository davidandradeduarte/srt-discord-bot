using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Serilog;
using SimpleRandomTeams.Commands.Interfaces;
using SimpleRandomTeams.Services;

namespace SimpleRandomTeams.Commands
{
    public class TeamsCommand : BaseCommandModule, IModule
    {
        [Command("teams")]
        [Description("Generate random teams with members in the current voice channel.")]
        public async Task Teams(CommandContext ctx)
        {
            try
            {
                // if (ctx.Member.Roles.FirstOrDefault(x => x.Name == "Ducks") == null)
                // {
                //     LoggingService.LogWarning($"User {ctx.Member.DisplayName} has no access to execute this command.");
                //     return;
                // }
            
                LoggerService.LogInformation(ctx.Client, "Generating random teams with members in the current voice channel.");
            
                await ctx.TriggerTypingAsync();

                if (ctx.Member.VoiceState == null)
                {
                    LoggerService.LogWarning(ctx.Client, $"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }

                var db = InMemoryDatabase.Instance;

                db.OriginChannel = ctx.Member.VoiceState.Channel;
                var connectedMembers = ctx.Guild.Members
                    .Where(member => member.Value.VoiceState?.Channel == db.OriginChannel)
                    .OrderBy(a => Guid.NewGuid())
                    .Distinct()
                    .Select(x => x.Value)
                    .ToList();

                if (connectedMembers.Count < 2)
                {
                    await ctx.RespondAsync($"There aren't enough players to generate random teams.");
                    return;
                }

                db.Team1 = connectedMembers.Take(connectedMembers.Count / 2).ToList();
                db.Team2 = connectedMembers.Skip(connectedMembers.Count / 2).ToList();

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Simple Team Generator",
                    Timestamp = DateTimeOffset.Now,
                    Color = new DiscordColor(0xFF6133)
                };

                if (db.Team1.Any())
                {
                    embed.AddField($"Team {DiscordEmoji.FromName(ctx.Client, ":point_up:")}",
                        string.Join('\n', db.Team1.Select(x => $"- {x.Mention}")));
                }

                if (db.Team1.Any())
                {
                    embed.AddField($"Team {DiscordEmoji.FromName(ctx.Client, ":v:")}",
                        string.Join('\n', db.Team2.Select(x => $"- {x.Mention}")));
                }

                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Good Luck & Have Fun! {DiscordEmoji.FromName(ctx.Client, ":wink:")}"
                };

                LoggerService.LogInformation(ctx.Client, "Team1:");
                db.Team1.ForEach(x => LoggerService.LogInformation(ctx.Client, $"{x.DisplayName}"));
                LoggerService.LogInformation(ctx.Client, "Team2:");
                db.Team2.ForEach(x => LoggerService.LogInformation(ctx.Client, $"{x.DisplayName}"));
                await ctx.RespondAsync(embed: embed);
            }
            catch (Exception e)
            {
                LoggerService.LogError(ctx.Client, e, e.Message);
            }
        }
    }
}
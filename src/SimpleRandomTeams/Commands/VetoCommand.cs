using System;
using System.Globalization;
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
    public class VetoCommand : BaseCommandModule, IModule
    {
        [Command("veto")]
        [Description("Picks one random member from each team to start a veto process.")]
        public async Task Veto(CommandContext ctx, params string[] args)
        {
            try
            {
                // if (ctx.Member.Roles.FirstOrDefault(x => x.Name == "Ducks") == null)
                // {
                //     LoggingService.LogWarning($"User {ctx.Member.DisplayName} has no access to execute this command.");
                //     return;
                // }

                if (args.Any())
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} !veto command doesn't take any arguments.");
                    return;
                }
            
                LoggerService.LogInformation(ctx.Client, "Picking one random member from each team to start a veto process.");
            
                await ctx.TriggerTypingAsync();

                if (ctx.Member.VoiceState == null)
                {
                    LoggerService.LogWarning(ctx.Client, $"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }

                var db = InMemoryDatabase.Instance;

                if (!db.Team1.Any() || !db.Team2.Any())
                {
                    LoggerService.LogWarning(ctx.Client, "There aren't enough players to start a veto process.");
                    await ctx.RespondAsync("There aren't enough players to start a veto process.");
                    return;
                }
                
                var random = new Random();

                var player1 = db.Team1[random.Next(0, db.Team1.Count)];
                var player2 = db.Team2[random.Next(0, db.Team2.Count)];

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Simple Team Generator",
                    Timestamp = DateTimeOffset.Now,
                    Color = new DiscordColor(0xFF6133)
                };
                
                DiscordMember player;
                if (random.Next() > random.Next() / 2)
                {
                    player = player1;
                    db.LastVetoPlayer = player2;
                }
                else
                {
                    player = player2;
                    db.LastVetoPlayer = player1;
                }

                embed.AddField($"Leader for team {DiscordEmoji.FromName(ctx.Client, ":point_up:")}", player1.Mention);

                embed.AddField($"Leader for team {DiscordEmoji.FromName(ctx.Client, ":v:")}", player2.Mention);
                
                embed.AddField("First player to ban a map is:", $"{player.Mention}");
                
                embed.AddField($"Available maps {DiscordEmoji.FromName(ctx.Client, ":map:")}",
                    string.Join('\n', db.DefaultMaps.Select(x => $"- {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x)}")));

                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Start banning maps with `!ban <map-name>` {DiscordEmoji.FromName(ctx.Client, ":no_entry:")}"
                };

                db.VetoPlayerTeam1 = player1;
                db.VetoPlayerTeam2 = player2;
                db.VetoMaps = db.DefaultMaps;
                
                await ctx.RespondAsync(embed: embed);
            }
            catch (Exception e)
            {
                LoggerService.LogError(ctx.Client, e, e.Message);
            }
        }
    }
}
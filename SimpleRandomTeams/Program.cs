using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Serilog;

namespace SimpleRandomTeams
{
    class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("simplerandomteams.log")
                .WriteTo.Console()
                .CreateLogger();
            
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "<token>",
                TokenType = TokenType.Bot
            });
            
            var team1 = new List<DiscordMember>();
            var team2 = new List<DiscordMember>();
            DiscordChannel originChannel = default;

            discord.Ready += async eventArgs =>
            {
            };

            discord.MessageCreated += async message =>
            {
                if (message.Message.Content.Equals("!teams"))
                {
                    Log.Information("Creating teams...");
                    
                    originChannel = ((DiscordMember) message.Author).VoiceState.Channel;
                    var channel = originChannel;
                    var connectedMembers = discord.Guilds.FirstOrDefault().Value.Members
                        .Where(member => member.VoiceState?.Channel == channel)
                        .OrderBy(a => Guid.NewGuid())
                        .Distinct()
                        .ToList();

                    team1 = connectedMembers.Take(connectedMembers.Count / 2).ToList();
                    team2 = connectedMembers.Skip(connectedMembers.Count / 2).ToList();

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Simple Team Generator",
                        Timestamp = DateTimeOffset.Now,
                        Color = new DiscordColor(0xFF6133)
                    };

                    if (team1.Any())
                    {
                        embed.AddField($"Team {DiscordEmoji.FromName(discord, ":point_up:")}",
                            string.Join('\n', team1.Select(x => $"- {x.DisplayName}")));
                    }
                    
                    if (team2.Any())
                    {
                        embed.AddField($"Team {DiscordEmoji.FromName(discord, ":v:")}",
                            string.Join('\n', team2.Select(x => $"- {x.DisplayName}")));
                    }

                    embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Good Luck & Have Fun! {DiscordEmoji.FromName(discord, ":wink:")}"
                    };

                    embed.Fields.ToList().ForEach(x => Log.Information($"\n{x.Name}\n{x.Value}"));
                    await message.Channel.SendMessageAsync(embed: embed);
                }

                if (message.Message.Content.Equals("!split"))
                {
                    Log.Information("Splitting teams...");
                    
                    var team1Channel = discord.Guilds.FirstOrDefault().Value.Channels
                        .FirstOrDefault(channel => channel.Id == 399703488008945664);
                    
                    var team2Channel = discord.Guilds.FirstOrDefault().Value.Channels
                        .FirstOrDefault(channel => channel.Id == 399703457721745418);
                    
                    foreach (var member in team1)
                    {
                        try
                        {
                            await team1Channel!.PlaceMemberAsync(member);
                            Log.Information($"Moved {member.DisplayName} to {team1Channel!.Name}");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, e.Message);
                        }
                    }
                    
                    foreach (var member in team2)
                    {
                        try
                        {
                            await team2Channel!.PlaceMemberAsync(member);
                            Log.Information($"Moved {member.DisplayName} to {team2Channel!.Name}");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, e.Message);
                        }
                    }
                }

                if (message.Message.Content.Equals("!end"))
                {
                    Log.Information("Ending...");
                    
                    var teams = team1.Concat(team2).ToList();
                    
                    team1 = default;
                    team2 = default;

                    if (teams.Any())
                    {
                        foreach (var member in teams)
                        {
                            try
                            {
                                if (originChannel == default)
                                {
                                    await discord.Guilds.FirstOrDefault().Value.Channels
                                        .FirstOrDefault(x => x.Id == 413533229728006145)!.PlaceMemberAsync(member);
                                    Log.Information($"Moved {member.DisplayName} to {originChannel!.Name}");
                                }
                                else
                                {
                                    await originChannel!.PlaceMemberAsync(member);
                                    Log.Information($"Moved {member.DisplayName} to {originChannel!.Name}");
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, e.Message);
                            }
                        }
                    }

                    originChannel = default;
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SimpleRandomTeams.Commands.Interfaces;
using SimpleRandomTeams.Services;

namespace SimpleRandomTeams.Commands
{
    public class SplitCommand : BaseCommandModule, IModule
    {
        [Command("split")]
        [Description("Split the generated teams to their individual team voice channels.")]
        public async Task Split(CommandContext ctx)
        {
            try
            {
                // if (ctx.Member.Roles.FirstOrDefault(x => x.Name == "Ducks") == null)
                // {
                //     LoggingService.LogWarning($"User {ctx.Member.DisplayName} has no access to execute this command.");
                //     return;
                // }
            
                LoggerService.LogInformation(ctx.Client, "Splitting the generated teams to their individual team voice channels.");

                if (ctx.Member.VoiceState == null)
                {
                    LoggerService.LogWarning(ctx.Client, $"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }
                
                var db = InMemoryDatabase.Instance;
                
                if (!db.Team1.Any() || !db.Team2.Any())
                {
                    LoggerService.LogWarning(ctx.Client, "There are no defined teams.");
                    await ctx.RespondAsync("There are no defined teams. Type `!teams` to generate random teams first.");
                    return;
                }

                // TODO: remove team1/team2 hardcoded voice channel ids
                var team1Channel = ctx.Guild.Channels
                    .FirstOrDefault(channel => channel.Value.Id == 399703488008945664);

                var team2Channel = ctx.Guild.Channels
                    .FirstOrDefault(channel => channel.Value.Id == 399703457721745418);

                foreach (var member in db.Team1)
                {
                    try
                    {
                        await team1Channel!.Value.PlaceMemberAsync(member);
                        LoggerService.LogInformation(ctx.Client, $"Moved {member.DisplayName} to {team1Channel!.Value.Name}");
                    }
                    catch (Exception e)
                    {
                        LoggerService.LogError(ctx.Client, e, e.Message);
                    }
                }

                foreach (var member in db.Team2)
                {
                    try
                    {
                        await team2Channel!.Value.PlaceMemberAsync(member);
                        LoggerService.LogInformation(ctx.Client, $"Moved {member.DisplayName} to {team2Channel!.Value.Name}");
                    }
                    catch (Exception e)
                    {
                        LoggerService.LogError(ctx.Client, e, e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerService.LogError(ctx.Client, e, e.Message);
            }
        }
    }
}
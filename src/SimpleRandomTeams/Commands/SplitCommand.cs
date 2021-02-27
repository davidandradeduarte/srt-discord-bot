using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Serilog;

namespace SimpleRandomTeams.Commands
{
    public class SplitCommand : IModule
    {
        [Command("split")]
        [Description("Split the generated teams to their individual team voice channels.")]
        public async Task Split(CommandContext ctx)
        {
            try
            {
                // TODO: move to a custom "auth" attribute
                if (ctx.Member.Roles.FirstOrDefault(x => x.Name == "Ducks") == null)
                {
                    Log.Warning($"User {ctx.Member.DisplayName} has no access to execute this command.");
                    return;
                }
            
                Log.Information("Splitting the generated teams to their individual team voice channels.");

                if (ctx.Member.VoiceState == null)
                {
                    Log.Warning($"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }
                
                var db = InMemoryDatabase.Instance;
                
                if (!db.Team1.Any() || !db.Team2.Any())
                {
                    Log.Warning("There are no defined teams.");
                    await ctx.RespondAsync("There are no defined teams. Type `!teams` to generate random teams first.");
                    return;
                }

                // TODO: remove team1/team2 hardcoded voice channel ids
                var team1Channel = ctx.Guild.Channels
                    .FirstOrDefault(channel => channel.Id == 399703488008945664);

                var team2Channel = ctx.Guild.Channels
                    .FirstOrDefault(channel => channel.Id == 399703457721745418);

                foreach (var member in db.Team1)
                {
                    try
                    {
                        await team1Channel!.PlaceMemberAsync(member);
                        Log.Information($"Moved {member.DisplayName} to {team1Channel!.Name}");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.StackTrace ?? string.Empty, e.Message, e);
                    }
                }

                foreach (var member in db.Team2)
                {
                    try
                    {
                        await team2Channel!.PlaceMemberAsync(member);
                        Log.Information($"Moved {member.DisplayName} to {team2Channel!.Name}");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.StackTrace ?? string.Empty, e.Message, e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace ?? string.Empty, e.Message, e);
            }
        }
    }
}
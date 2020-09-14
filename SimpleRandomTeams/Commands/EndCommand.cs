using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Serilog;

namespace SimpleRandomTeams.Commands
{
    public class EndCommand : IModule
    {
        [Command("end")]
        [Description("Move team members to the original voice channel.")]
        public async Task End(CommandContext ctx)
        {
            try
            {
                // TODO: move to a custom "auth" attribute
                if (ctx.Member.Roles.FirstOrDefault(x => x.Name == "Ducks") == null)
                {
                    Log.Warning($"User {ctx.Member.DisplayName} has no access to execute this command.");
                    return;
                }
            
                Log.Information("Moving team members to the original voice channel.");
            
                if (ctx.Member.VoiceState == null)
                {
                    Log.Warning($"User {ctx.Member.DisplayName} is not connected to a voice channel.");
                    await ctx.RespondAsync($"@{ctx.Member.Mention} you need to be connected to a voice channel.");
                    return;
                }
                
                var db = InMemoryDatabase.Instance;

                var teams = db.Team1.Concat(db.Team2).ToList();

                db.Team1 = default;
                db.Team2 = default;

                if (teams.Any())
                {
                    foreach (var member in teams)
                    {
                        try
                        {
                            if (db.OriginChannel == default)
                            {
                                // TODO: remove hardcoded channel id and set it to default guild voice channel
                                await ctx.Guild.Channels
                                    .FirstOrDefault(x => x.Id == 413533229728006145)!.PlaceMemberAsync(member);
                                Log.Information($"Moved {member.DisplayName} to {db.OriginChannel?.Name}");
                            }
                            else
                            {
                                await db.OriginChannel.PlaceMemberAsync(member);
                                Log.Information($"Moved {member.DisplayName} to {db.OriginChannel?.Name}");
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, e.Message);
                        }
                    }
                }

                db.OriginChannel = default;
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
    }
}
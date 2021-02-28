using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using SimpleRandomTeams.Commands.Interfaces;
using SimpleRandomTeams.Configuration;

namespace SimpleRandomTeams.Services
{
    public class BotService
    {
        public readonly EventId BotEventId = new(42, "SRT");
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }

        public ConfigurationSettings Configuration { get; set; }

        public async Task InitAsync()
        {
            // Configuration
            await using var stream = File.OpenRead("config.json");
            Configuration = await JsonSerializer.DeserializeAsync<ConfigurationSettings>(stream);
            
            var cfg = new DiscordConfiguration
            {
                Token = Configuration.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            Client = new DiscordClient(cfg);

            // Events
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientError;

            // Commands
            var nextConfiguration = new CommandsNextConfiguration
            {
                StringPrefixes = new[] {Configuration.CommandPrefix},
                EnableDms = false,
                EnableMentionPrefix = true
            };
            
            Commands = Client.UseCommandsNext(nextConfiguration);

            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            var type = typeof(IModule);
            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

            var typeList = types as Type[] ?? types.ToArray();
            foreach (var t in typeList)
            {
                Commands.RegisterCommands(t);
            }

            // Connect
            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            LoggerService.LogInformation(sender, BotEventId, "Client is ready to process events");

            await sender.UpdateStatusAsync(new DiscordActivity
                {ActivityType = ActivityType.ListeningTo, Name = Configuration.CommandPrefix});
        }

        private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            LoggerService.LogInformation(sender, BotEventId, $"Guild available: {e.Guild.Name}");
            
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            LoggerService.LogError(sender, BotEventId, e.Exception, "Exception occured");
            
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            LoggerService.LogInformation(e.Context.Client, BotEventId,
                $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");

            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            LoggerService.LogError(e.Context.Client, BotEventId,
                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message}",
                DateTime.Now);

            if (e.Exception is ChecksFailedException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync(embed);
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;
using Serilog;
using SimpleRandomTeams.Commands;

namespace SimpleRandomTeams.Services
{
    public class BotService
    {
        private CancellationTokenSource Cts { get; set; }
        private IConfigurationRoot _config;
        private DiscordClient _discord;
        private CommandsNextModule _commands;
        private InteractivityModule _interactivity;
        
        public async Task InitAsync()
        {
            try
            {
                Cts = new CancellationTokenSource();

                Log.Information("Loading configuration file");
                _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                    .Build();

                Log.Information("Creating discord client");
                _discord = new DiscordClient(new DiscordConfiguration
                {
                    AutoReconnect = true,
                    LargeThreshold = 250,
                    MessageCacheSize = 2048,
                    Token = _config["discord:token"],
                    TokenType = TokenType.Bot
                });

                _interactivity = _discord.UseInteractivity(new InteractivityConfiguration
                {
                    PaginationBehaviour = TimeoutBehaviour.Delete,
                    PaginationTimeout = TimeSpan.FromSeconds(30),
                    Timeout = TimeSpan.FromSeconds(30)
                });

                var deps = BuildDeps();
                _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefix = _config["discord:CommandPrefix"],
                    Dependencies = deps
                });

                Log.Information("Loading command modules");
                var type = typeof(IModule);
                var types =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

                var typeList = types as Type[] ?? types.ToArray();
                foreach (var t in typeList)
                {
                    _commands.RegisterCommands(t);
                    Log.Information($"Adding {t.Name} module.");
                }

                Log.Information($"Loaded {typeList.Length} modules.");

                RunAsync().Wait();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }
        }
        
        private DependencyCollection BuildDeps()
        {
            Log.Information("Building dependencies");
            using var deps = new DependencyCollectionBuilder();

            deps.AddInstance(_interactivity)
                .AddInstance(Cts)
                .AddInstance(_config)
                .AddInstance(_discord);

            return deps.Build();
        }
        
        private async Task RunAsync()
        {
            Log.Information("Connecting...");
            await _discord.ConnectAsync();
            Log.Information("Connected");

            await _discord.UpdateStatusAsync(new DiscordGame{Name = "teste"});

            while (!Cts.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
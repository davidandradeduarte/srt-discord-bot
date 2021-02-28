# Simple Random Teams

Simple Random Teams is a Discord bot very useful for communities that do random csgo scrims every day.

## Running it

- Install [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- Set your bot token in [config.json](src/SimpleRandomTeams/config.json)

```bash
dotnet run
```

## Using Docker

### Build

```bash
docker build -t local/srt-discord-bot -f src/SimpleRandomTeams/Dockerfile .
```

### Build for arm32v7

```bash
docker build -t local/srt-discord-bot -f src/SimpleRandomTeams/Dockerfile.arm32v7 .
```

### Run

```bash
docker run -d --name srt-discord local/srt-discord-bot
```

## Available Commands

- `help` Get a help description and commands available.
- `commands` List available commands.
- `map` Generate a random map to play from the csgo scrim map pool.
- `teams` Generate random teams with members in the current voice channel.
- `veto` Picks one random member from each team to start a veto process.
- `ban` Bans a map from the veto available maps.
- `split` Split the generated teams to their individual team voice channels.
- `end` Move team members to the original voice channel.
- `reset` Reset in memory database.
- `yo` Test if the bot is running.

# Simple Random Teams Discord Bot

![Pipeline](https://github.com/davidandradeduarte/simple-random-teams/actions/workflows/pipeline.yml/badge.svg)

Simple Random Teams is a Discord bot very useful for communities that do random csgo scrims every day.

## Running it

- Set your bot token in [config.json](src/SimpleRandomTeams/config.json)

### With Docker _(recommended)_

```bash
make docker
make docker-arm # for arm devices
# or
docker build -t srt-discord-bot .
docker build -t srt-discord-bot -f Dockerfile.arm32v7 . # for arm devices
docker run -d --name srt-discord srt-discord-bot
```

### Without Docker

- Install [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- Set your bot token in [config.json](src/SimpleRandomTeams/config.json)

```bash
make run
# or
dotnet run
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

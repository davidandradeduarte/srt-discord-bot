# Simple Random Teams

Simple Random Teams is a Discord Bot written in C#, running on .NET Core 3.1 as a Console app.  
This bot does some very simplistic work, but very helpful for communities that do random scrims every day.  

## Features
- `!teams` Generate random teams with members in the current voice channel.
- `!map` Generate a random map to play from the CSGO scrim map pool.
- `!split` Split the generated teams to their individual team voice channels.
- `!end` Move team members to the original voice channel.

## Installation

You can set up this bot in your Discord channel by going to `bot-URL`.

Alternatively, extend this bot at will by forking the repository and deploying your own version.

There's a Dockerfile if you wish to run this in a container. 

```bash
dotnet build
```

## Prerequisites
- .NET Core 3.1 SDK [https://dotnet.microsoft.com/download/dotnet-core/3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Usage

```bash
dotnet run
```

## Contributing
Pull requests are welcome.

## License
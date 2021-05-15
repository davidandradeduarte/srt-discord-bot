run:
	(cd src/SimpleRandomTeams/ && dotnet run)

docker:
	docker build -t srt-discord-bot .
	docker run -d --name srt-discord-bot srt-discord-bot

docker-arm:
	docker build -t srt-discord-bot -f Dockerfile.arm32v7 .
	docker run -d --name srt-discord-bot srt-discord-bot

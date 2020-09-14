using System.Collections.Generic;
using DSharpPlus.Entities;

namespace SimpleRandomTeams
{
    public class InMemoryDatabase
    {
        public static InMemoryDatabase Instance { get; } = new InMemoryDatabase();

        public List<DiscordMember> Team1 { get; set; } = new List<DiscordMember>();
        public List<DiscordMember> Team2 { get; set; } = new List<DiscordMember>();
        public DiscordChannel OriginChannel { get; set; } = default;

        private InMemoryDatabase(){}
    }
}
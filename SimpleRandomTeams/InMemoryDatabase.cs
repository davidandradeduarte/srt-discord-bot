using System.Collections.Generic;
using DSharpPlus.Entities;

namespace SimpleRandomTeams
{
    public class InMemoryDatabase
    {
        public static InMemoryDatabase Instance { get; } = new InMemoryDatabase();

        public List<DiscordMember> Team1 { get; set; } = new List<DiscordMember>();
        public List<DiscordMember> Team2 { get; set; } = new List<DiscordMember>();
        public DiscordChannel OriginChannel { get; set; }
        public List<string> VetoMaps = default;
        public readonly List<string> DefaultMaps = new List<string>
        {
            "inferno", "train", "mirage", "nuke", "overpass", "dust2", "vertigo", "cache"
        };
        public DiscordMember VetoPlayerTeam1 { get; set; }
        public DiscordMember VetoPlayerTeam2 { get; set; }
        public DiscordMember LastVetoPlayer { get; set; }

        private InMemoryDatabase(){}

        public static void Reset()
        {
            Instance.Team1 = new List<DiscordMember>();
            Instance.Team2 = new List<DiscordMember>();
            Instance.OriginChannel = default;
            Instance.VetoMaps = new List<string>();
            Instance.VetoPlayerTeam1 = default;
            Instance.VetoPlayerTeam2 = default;
            Instance.LastVetoPlayer = default;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace MelonBot.Bots.Models
{
    public class Profile : Entity
    {
        //User Detail Properties
        public ulong MemberId { get; set; }
        public ulong guildId { get; set; }
        public string Username { get; set; }
        public string UserIconURL { get; set; }

        //Drip Properties
        public int DripScore { get; set; }
        public string DripInventory { get; set; }
        public DateTime DripCooldown { get; set; }
        public int Tier { get; set; }

        //Gamble Properties
        public int GambleCount { get; set; }
        public int GambleTickets { get; set; }
        public DateTime GambleCooldown { get; set; }
    }
}

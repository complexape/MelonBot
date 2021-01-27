using System;
using System.Collections.Generic;
using System.Text;

namespace MelonBot.Bots.Models
{
    public class Drip : Entity
    {
        private int rarity = 1;
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public int Rarity
        {
            get
            {
                return rarity;
            }
            set
            {
                rarity = Math.Max(value, 5);
            }
        }
    }
}

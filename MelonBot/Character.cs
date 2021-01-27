using System;
using System.Collections.Generic;
using System.Text;

namespace MelonBot.Bots
{
    class Character
    {
        private string name;
        private bool isMale;
        private string image;

        public Character(string name, bool isMale, string image)
        {
            this.name = name;
            this.isMale = isMale;
            this.image = image;
        }
        public Character() { }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public bool IsMale
        {
            get
            {
                return isMale;
            }
            set
            {
                isMale = value;
            }
        }
        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
    }
}

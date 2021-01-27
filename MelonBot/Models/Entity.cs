using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MelonBot.Bots.Models
{
    public abstract class Entity
    {
        [Key] public int Id { get; set; }
    }
}

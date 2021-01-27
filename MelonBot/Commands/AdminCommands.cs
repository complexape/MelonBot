using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using MelonBot.Bots.Context;
using Microsoft.EntityFrameworkCore;
using MelonBot.Bots.Models;

namespace MelonBot.Bots
{
    [RequirePrefixes("m!")]
    [RequireOwner]
    [Hidden]

    public class AdminCommands : BaseCommandModule
    {
        [Command("migratetest")]
        public async Task MigrateLite(CommandContext ctx)
        {
            Console.WriteLine("*** migrating... ***");

            await using SqliteContext lite = new SqliteContext();

            if (lite.Database.GetPendingMigrationsAsync().Result.Any())
            {
                await lite.Database.MigrateAsync();
            }

            await ctx.Channel.SendMessageAsync($"Sqlite Migration complete");
        }
        [Command("migratedrip")]
        public async Task MigrateDrip(CommandContext ctx)
        {
            Console.WriteLine("*** migrating... ***");

            await using DripContext lite = new DripContext();

            if (lite.Database.GetPendingMigrationsAsync().Result.Any())
            {
                await lite.Database.MigrateAsync();
            }

            await ctx.Channel.SendMessageAsync($"Sqlite Migration complete");
        }

        [Command("redo")]
        public async Task RemoveDB(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                try
                {
                    /*if (targetUser != null)
                    {
                        foreach (var item in lite.Profile)
                        {
                            if (item.MemberId == targetUser.Id)
                            {
                                lite.Profile.Remove(item);
                            }
                        }
                        await ctx.Channel.SendMessageAsync($"successfully removed all entities from SqliteDB.db for **{targetUser.Username}**");
                        return;
                    }
                    else
                    {*/
                    foreach (var item in lite.Profile)
                    {
                        lite.Profile.Remove(item);
                    }
                    await ctx.Channel.SendMessageAsync("successfully removed all entities from SqliteDB.db");

                    await lite.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await ctx.Channel.SendMessageAsync(e.ToString());
                }
            }

        }

        [Command("addalldrip")]
        public async Task AddAllDrip(CommandContext ctx)
        {
            using (DripContext drip = new DripContext())
            {
                try
                {
                    var lines = File.ReadAllLines(@"Textfiles\DripCharacters.json");

                    foreach (var line in lines)
                    {
                        var myTitle = line.Substring(0, line.IndexOf("-="));
                        var myImageUrl = line.Substring(line.IndexOf("-=") + 2);
                        drip.Drip.Add(new Drip
                        {
                            Title = myTitle,
                            ImageURL = myImageUrl
                        });
                    }
                    await drip.SaveChangesAsync();
                    await ctx.Channel.SendMessageAsync("complete");
                }
                catch (Exception e)
                {
                    await ctx.Channel.SendMessageAsync(e.ToString());
                }
            }
        }

        [Command("dripaddd")]
        public async Task AddDrip(CommandContext ctx, string imgURL, [RemainingText] string name)
        {
            var interactivity = ctx.Client.GetInteractivity();

            using (SqliteContext lite = new SqliteContext())
            {

                var dripEmbed = new DiscordEmbedBuilder
                {
                    Title = ":fire:" + name + " DRIP :fire:",
                    Description = $"Are you sure this what you want? (You cannot remove it once you confirm)",
                    ImageUrl = imgURL,
                    Color = DiscordColor.Black
                };

                await ctx.Channel.SendMessageAsync(embed: dripEmbed);
                await ctx.Channel.SendMessageAsync($"**{ctx.Member.DisplayName}**, respond with \"y\" to confirm.");
                var confirmation = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
          && x.Author.Id == ctx.Member.Id);
                if (confirmation.Result.Content.ToString() != "y")
                {
                    await ctx.Channel.SendMessageAsync("`request cancelled.`");
                    return;
                }

                using (DripContext drip = new DripContext())
                {
                    var list = drip.Drip.ToListAsync().Result;
                    Drip newDrip = new Drip
                    {
                        Title = name,
                        ImageURL = imgURL,
                    };
                    foreach (var item in list)
                    {
                        if (item.Title == name)
                        {
                            await ctx.Channel.SendMessageAsync("Drip Already Exists.");
                            return;
                        }
                    }

                    await ctx.Channel.SendMessageAsync("`success!`");
                    drip.Drip.Add(newDrip);
                    await drip.SaveChangesAsync();
                }
            }
        }

        [Command("driplist")]
        public async Task DripList(CommandContext ctx)
        {
            using (DripContext drip = new DripContext())
            {
                var list = await drip.Drip.ToListAsync();
                var listEmbed = new DiscordEmbedBuilder
                {
                    Title = "Drip List",
                    Description = $"{list.Count()} total items."
                };
                foreach (var item in drip.Drip)
                {
                    listEmbed.Description += $"\n{item.Title}";
                }
                await ctx.Channel.SendMessageAsync(embed: listEmbed);
            }
        }

        [Command("dripremove")]
        public async Task DripRemove(CommandContext ctx, [RemainingText] string name)
        {
            using (DripContext drip = new DripContext())
            {
                var interactivity = ctx.Client.GetInteractivity();

                var selectedDrip = drip.Drip.FirstOrDefault(x => x.Title == name);

                if (selectedDrip == default)
                {
                    await ctx.Channel.SendMessageAsync("drip not found.");
                    return;
                }

                var dripEmbed = new DiscordEmbedBuilder
                {
                    Title = ":fire:" + selectedDrip.Title + " DRIP :fire:",
                    Description = $"Are you sure you want to delete this one?",
                    ImageUrl = selectedDrip.ImageURL,
                    Color = DiscordColor.Black
                };

                await ctx.Channel.SendMessageAsync(embed: dripEmbed);
                await ctx.Channel.SendMessageAsync($"**{ctx.Member.DisplayName}**, respond with \"y\" to confirm.");
                var confirmation = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
          && x.Author.Id == ctx.Member.Id);
                if (confirmation.Result.Content.ToString() == "y")
                {
                    await ctx.Channel.SendMessageAsync("`success!`");
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("`request cancelled.`");
                    return;
                }

                drip.Remove(selectedDrip);
                await drip.SaveChangesAsync();
            }
        }
    }
}

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using MelonBot.Bots.Context;
using MelonBot.Bots.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelonBot.Bots.Commands
{
    [RequirePrefixes("m!")]
    public class DbCommands : BaseCommandModule
    {
        #region Non-Command Tasks
        public async Task<bool> CheckCanGamble
            (CommandContext ctx, Profile userProfile,
            int ticketsBeingUsed, int maxTickets, int minTickets = 0)
        {
            var gambleCount = userProfile.GambleCount;
            var gambleTickets = userProfile.GambleTickets;
            var name = userProfile.Username;

            //makes sure the number of tickets being used is within the correct boundaries (minTickets & maxTickets)
            if (ticketsBeingUsed > maxTickets)
            {
                await ctx.Channel.SendMessageAsync($"**{name}**, you can only wager a maximum of **{maxTickets}** ticket(s) for **this command**.");
                return false;
            }
            else if (ticketsBeingUsed < minTickets)
            {
                await ctx.Channel.SendMessageAsync($"**{name}**, you must wager a minimum of **{minTickets}** ticket(s) for **this command**.");
                return false;
            }
            else if (ticketsBeingUsed <= 0)
            {
                await ctx.Channel.SendMessageAsync($"**{name}**, please provide a **valid number**.");
                return false;
            }

            //checks to make sure user has nessesary number of tickets (userProfile.GambleTickets)
            else if (gambleTickets <= 0)
            {
                await ctx.Channel.SendMessageAsync($"**{name}**, pou have don't have any tickets.");
                return false;
            }
            else if (ticketsBeingUsed > gambleTickets)
            {

                await ctx.Channel.SendMessageAsync($"**{name}**, you don't have enough tickets.");
                return false;
            }

            //checks if command is or will go over the daily quota (userProfile.GambleCount)
            else if (gambleCount >= 20)
            {
                await ctx.Channel.SendMessageAsync
                ($"**{name}**, you have already used up all **{maxGambleCount}** of your chances, do \"**m!refreshquota**\" or \"**m!rq**\" once a day to refresh your chances.");
                return false;
            }
            else if (ticketsBeingUsed + gambleCount > maxGambleCount)
            {

                await ctx.Channel.SendMessageAsync
                ($"**{name}**, you are unable to use this amount of tickets, do \"**m!refreshquota**\" or \"**m!rq**\" once a day to refresh your chances.");
                return false;
            }

            return true;
        }

        public async Task<Profile> GetOrCreateProfileAsync(SqliteContext lite, DiscordMember user, ulong guildId)
        {
            var recordedUser = await lite.Profile
                .Where(x => x.guildId == guildId)
                .FirstOrDefaultAsync(x => x.MemberId == user.Id);

            if (recordedUser != null) { return recordedUser; }

            recordedUser = new Profile
            {
                MemberId = user.Id,
                guildId = guildId,
                Username = user.Username,
                UserIconURL = user.AvatarUrl,
                DripScore = 100,
                GambleTickets = 5,
                DripInventory = "",
            };
            lite.Add(recordedUser);
            await lite.SaveChangesAsync();

            return recordedUser;
        }

        public string GetTier(int tierNumber)
        {
            string tier = "";

            switch (tierNumber)
            {

                case 0:
                    tier = "zero";
                    break;
                case 1:
                    tier = "one";
                    break;
                case 2:
                    tier = "two;
                    break;
                case 3:
                    tier = "three";
                    break;
                case 4:
                    tier = "four";
                    break;
                case 5:
                    tier = "five";
                    break;
                case 6:
                    tier = "siz";
                    break;
                case 7:
                    tier = "seven";
                    break;
                case 8:
                    tier = "eight";
                    break;
                case 9:
                    tier = "nine;
                    break;
                default:
                    break;
            }
            return tier;
        }
        #endregion

        #region Setter Commands

        #region Gambling Commands
        int maxGambleCount = 20;

        [Command("refreshquota")]
        [Aliases("rq")]
        public async Task RefreshQuota(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id).Result;
                var currentDate = DateTime.Now;

                if (userProfile.GambleCooldown < currentDate)
                {
                    if (userProfile.GambleCount == 0)
                    {
                        await ctx.Channel.SendMessageAsync($"**{userProfile.Username}**, you have not used any tickets yet.");
                        return;
                    }
                    userProfile.GambleCount = 0;
                    await ctx.Channel.SendMessageAsync($"**{userProfile.Username}**'s your daily quota has been reset");
                    userProfile.GambleCooldown = DateTime.Now.AddDays(1);
                    await lite.SaveChangesAsync();
                }
                else
                {
                    var coolDownTimespan = userProfile.GambleCooldown - currentDate;
                    await ctx.Channel.SendMessageAsync($"**{ctx.Member.DisplayName}**, you can refresh your daily quota in " +
                        $"**{coolDownTimespan.Hours}h {coolDownTimespan.Minutes} mins**");
                }
            }
        }

        [Command("buyticket")]
        [Aliases("bt")]
        public async Task BuyTicket(CommandContext ctx, int wantedAmount = 0)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = await GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id);

                var dripCost = wantedAmount * 5;

                if (wantedAmount <= 0) { return; }
                else if (wantedAmount > 99)
                {
                    await ctx.Channel.SendMessageAsync($"**{userProfile.Username}**, you can't purchase more than **99** tickets at once.");
                    return;
                }
                else if (userProfile.DripScore < dripCost)
                {
                    await ctx.Channel.SendMessageAsync($"**{userProfile.Username}**, you don't have enough drip to purchase **{wantedAmount}** ticket(s). " +
                        $"\n(tickets cost **5** drip each)");
                    return;
                }

                var buyEmbed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Blurple
                };
                buyEmbed.AddField($"{userProfile.Username}, spend {dripCost} drip :fire: to purchase {wantedAmount} Tickets :tickets:?"
                , $"please respond with **\"y\"** to confirm your purchase:");

                //check if user confirms purchase
                var interactivity = ctx.Client.GetInteractivity();
                await ctx.Channel.SendMessageAsync(embed: buyEmbed);
                var confirmation = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
          && x.Author.Id == ctx.Member.Id);
                if (confirmation.Result.Content != "y" || confirmation.TimedOut)
                {
                    await ctx.Channel.SendMessageAsync("Purchase Cancelled.");
                    return;
                }

                await ctx.Channel.SendMessageAsync($"**Success**, you now have **{wantedAmount}** tickets(s) :tickets:");
                userProfile.DripScore -= dripCost;
                userProfile.GambleTickets += wantedAmount;
                await lite.SaveChangesAsync();
            }
        }

        [Command("exchangeticket")]
        [Aliases("et")]
        public async Task TicketExchange(CommandContext ctx, int amount = 0)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = await GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id);
                var ticketCount = userProfile.GambleTickets;
                if (amount <= 0) { return; }
                else if (amount > ticketCount)
                {
                    await ctx.Channel.SendMessageAsync($"**{userProfile.Username}**, you only have **{ticketCount}** ticket(s).");
                    return;
                }

                await ctx.Channel.SendMessageAsync("Command is in progress.");
            }
        }

        [Command("gamble")]
        public async Task GambleTickets(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id).Result;
                //the number of times the user has gambled
                var gambleCount = userProfile.GambleCount;
                var introEmbed = new DiscordEmbedBuilder
                {
                    Title = "Welcome To the Casino!",
                    Color = DiscordColor.Red,
                    ImageUrl = "https://media.giphy.com/media/c7O1w6DdCpMWlfT9P2/giphy.gif",
                    Description = $"**{userProfile.Username}** has " +
                    $"**{userProfile.GambleTickets}** casino tickets :tickets: and " +
                    $"**{maxGambleCount - gambleCount}** remaining chances to use tickets today." +
                    "\nRespond with your desired game's **corresponding number** to play!\n" +
                    "\n**1**. m!coinflip :coin:" +
                    "\n - Wager your tickets on the side the coin flip lands on! " +
                    "Each flip costs a ticket and each correct guessing correctly rewards drip!" +
                    "\n**2**. m!slotmachine :slot_machine:" +
                    "\n - Slot machine description here" +
                    "\n**3**. m!blackjack :black_joker: " +
                    "\n - Blackjack description here"
                };
                introEmbed.AddField("Multiplayer Options:",
                    "\n**4**. Horse Racing Bets"
                    );

                await ctx.Channel.SendMessageAsync(embed: introEmbed);
                var confirmation = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
          && x.Author.Id == ctx.Member.Id);

                //checks if user is able to gamble
                if (userProfile.GambleTickets <= 0)
                {
                    await ctx.Channel.SendMessageAsync("You have don't have any tickets.");
                    return;
                }
                else if (gambleCount >= 20)
                {
                    await ctx.Channel.SendMessageAsync
                    ($"you have already used up all **{maxGambleCount}** of your chances, do \"**m!refreshquota**\" or \"**m!rq**\" once a day to refresh your chances.");
                    return;
                }

                switch (confirmation.Result.Content)
                {
                    case "1":
                        await ctx.Channel.SendMessageAsync("Selected **Coin Flip** :coin:");
                        await GambleCoinFlip(ctx);
                        break;
                    case "2":
                        await ctx.Channel.SendMessageAsync("Selected **Slot Machine** :slot_machine:");
                        await GambleSlotMachine(ctx);
                        break;
                    case "3":
                        await ctx.Channel.SendMessageAsync("Selected **Blackjack** :black_joker: ");
                        await GambleBlackjack(ctx);
                        break;
                    default:
                        break;
                }
            }
        }

        [Command("coinflip")]
        [Aliases("cf")]
        public async Task GambleCoinFlip(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            int maxTickets = 4;

            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id).Result;

                var ticketPrompt = await ctx.Channel.SendMessageAsync
                    ($"Respond with the **number** of tickets :tickets: you want to wager. (max **{maxTickets}** per command)");
                var confirmation = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
                 && x.Author.Id == ctx.Member.Id);
                await ticketPrompt.DeleteAsync();

                //saves response
                int ticketsUsing;
                bool isParsable = Int32.TryParse(confirmation.Result.Content.ToString(), out ticketsUsing);

                //checks if reponse is valid and if user can gamble
                if (!isParsable)
                {
                    //code here is run if user did not respond with an integer
                    await ctx.Channel.SendMessageAsync("Please provide a **valid** response.");
                    return;
                }
                var canGamble = await CheckCanGamble(ctx, userProfile, ticketsUsing, maxTickets, 1);
                if (!canGamble) { return; } //code returns here if cannot gamble

                //gets the user's chosen side to wager on
                var sidePrompt = await ctx.Channel.SendMessageAsync("Respond with **\"h\"** to wager on the coin landing on heads and **\"t\"** for tails:");
                var chosenSide = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
          && x.Author.Id == ctx.Member.Id && (x.Content == "h" || x.Content == "t"));
                bool hasChosenHeads = true;
                if (chosenSide.Result.Content == "t")
                {
                    hasChosenHeads = false;
                }
                await sidePrompt.DeleteAsync();

                //player starts to flip the coin
                var coinEmbed = new DiscordEmbedBuilder
                {
                    Title = "React to this message to flip the coin! :coin:",
                    Description = $"- Wager your tickets :tickets: on the side the coin flip lands on! " +
                    "Each flip costs **1** ticket :tickets: and each correct guess rewards **7** drip :fire:!"
                    + $"\nYou have **{ticketsUsing}** total flips.",
                    Color = DiscordColor.Yellow,
                    ImageUrl = "https://media.giphy.com/media/6jqfXikz9yzhS/giphy.gif"
                };
                var message = await ctx.Channel.SendMessageAsync(embed: coinEmbed);
                var flipEmoji = DiscordEmoji.FromName(ctx.Client, ":arrows_clockwise:");
                await message.CreateReactionAsync(flipEmoji);
                int totalCorrect = 0;

                for (int i = 0; i < ticketsUsing; i++)
                {
                    DiscordEmoji[] emojiOptions = {
                    DiscordEmoji.FromName(ctx.Client, ":+1:"),
                    DiscordEmoji.FromName(ctx.Client, ":-1:")};
                    for (int stall = 0; stall <= 1000000; stall++) //Stall loop for correcting the timing
                    {
                        emojiOptions.Select(x => $"{x} :{emojiOptions.Where(xtd => x == xtd).Count()}").Distinct();
                    }

                    var reactionResult = await interactivity.WaitForReactionAsync(x => x.Message == message &&
                    x.Emoji == flipEmoji && x.User.Id == ctx.User.Id);
                    
                    var r = new Random().Next(0, 2);
                    string description = "";
                    string headsOrTails = "Heads";
                    string yesOrNo = ":negative_squared_cross_mark:";
                    // 2 = tails, 1 = heads

                    if (r == 0)
                    {
                        headsOrTails = "Tails";
                    }
                    if ((r == 0 && hasChosenHeads == false) || (r == 1 && hasChosenHeads == true))
                    {
                        yesOrNo = ":white_check_mark:";
                        totalCorrect++;
                    }

                    description += $"\nFlip #{i + 1}. **{headsOrTails}** {yesOrNo}";
                    DiscordEmbed newEmbed = coinEmbed.AddField($"{i + 1}. {yesOrNo}", $"coin landed on a **{headsOrTails}**.");
                    await message.ModifyAsync(embed: newEmbed);
                }
                await message.DeleteAsync();

                //Results are shown and user's attributes in the DB are edited
                var title = $"Congratulations!";
                var imageUrl = "https://media.giphy.com/media/WzJkq7MWBN8YYpurtm/giphy.gif";
                var dripGain = (totalCorrect * 7);
                var descriptionResult = $"**{userProfile.Username}**, you guessed **{totalCorrect}** correct out of **{ticketsUsing}**.";

                DiscordColor color = DiscordColor.SpringGreen;
                if ((ticketsUsing/2) >= totalCorrect) 
                { 
                    title = $"Better luck next time..."; 
                    color = DiscordColor.IndianRed;
                    imageUrl = "https://media.giphy.com/media/4QxQgWZHbeYwM/giphy.gif";
                }
                else if (new Random().Next(0, 21) == 7)
                {
                    dripGain = ticketsUsing * 30;
                    title = ":tickets::tickets: SURPRISE JACKPOT :tickets::tickets: ";
                    descriptionResult = $"Amazing! your winnings have been tripled!! **+{dripGain} Drip** :fire:";
                    imageUrl = "https://media.giphy.com/media/4GRj3pwoAJSwg/giphy.gif";
                }
                else if (ticketsUsing / totalCorrect == 1 && ticketsUsing != 1)
                {
                    title = ":first_place:  --Perfect Score-- :first_place: ";
                    color = DiscordColor.Yellow;
                    dripGain = ticketsUsing * 10;
                }
                
                var resultsEmbed = new DiscordEmbedBuilder
                {
                    Title = title + $" +{dripGain} Drip :fire:",
                    Color = color,
                    Description = descriptionResult,
                    ImageUrl = imageUrl
                };
                await ctx.Channel.SendMessageAsync(embed: resultsEmbed);

                userProfile.GambleTickets -= ticketsUsing;
                userProfile.GambleCount += ticketsUsing;
                userProfile.DripScore += dripGain;
                await lite.SaveChangesAsync();
            }
        }

        [Command("slotmachine")]
        [Aliases("sm")]
        public async Task GambleSlotMachine(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = await GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id);
                var emojiA = ":cherries:";
                var emojiB = "<:sus:804048669740761098>";
                var emojiC = ":lemon:";
                var emojiD = ":gem: ";
                var jckPot = ":moneybag:";
                var Lseven = "<:luckyseven:803861664561889331>";
                
                var message =
                    $"\n:blue_square: {emojiA}   {emojiB}   {emojiC} :blue_square:" +
                    $"\n:arrow_forward: {emojiB}   {emojiC}   {emojiA} :arrow_backward:" + 
                    $"\n:blue_square: {emojiC}   {emojiA}   {emojiB} :blue_square:";

                await ctx.Channel.SendMessageAsync(message);

                await ctx.Channel.SendMessageAsync(jckPot);
            }
        }

        [Command("blackjack")]
        [Aliases("bj")]
        public async Task GambleBlackjack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("blackjack in progress...");
        }
        [Command("horseracing")]
        [Aliases("hr")]
        public async Task GambleHorseRacing(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("horce racing in progress...");
        }

        #endregion

        #region Drip Commands
        [Command("tier")]
        public async Task CheckTier(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id).Result;

                int tier = userProfile.Tier;
                string tierName = GetTier(tier);
                string nextTierName = GetTier(tier + 1);

                await ctx.Channel.SendMessageAsync($"Current Tier: {tierName}");
                await ctx.Channel.SendMessageAsync($"Next Tier: {nextTierName}");
            }
        }

        [Command("drip")]
        public async Task DoDrip(CommandContext ctx)
        {
            var currentDate = DateTime.Now;

            using (SqliteContext lite = new SqliteContext())
            {

                var recordedUser = GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id).Result;

                //Checks if cooldown is over for the user or user is not recorded
                if (recordedUser.DripCooldown < currentDate)
                {
                    //records user and their values
                    int userDripScoreAdd = new Random().Next(4, 10);
                    int tryUltraDrip = new Random().Next(0, 200);
                    if (tryUltraDrip == 50)
                    {
                        userDripScoreAdd = 50;
                    }

                    recordedUser.DripScore += userDripScoreAdd;
                    recordedUser.DripCooldown = DateTime.Now.AddHours(4);
                    await lite.SaveChangesAsync();

                    string myTitle;
                    string myImageUrl;
                    Drip myResult;
                    using (DripContext drip = new DripContext())
                    {
                        var lineRandom = new Random().Next(0, drip.Drip.Count()+1);
                        var myList = drip.Drip.ToListAsync();
                        myResult = myList.Result.ElementAt(lineRandom);

                        myImageUrl = myResult.ImageURL;
                        myTitle = $":fire: {myResult.Title} DRIP :fire:";
                    }

                    //Seperate string elements
                    if (tryUltraDrip == 50)
                    {
                        myTitle = $":fire::fire::fire:「GOD DRIP」 :fire: :fire: :fire: ";
                        myImageUrl = "https://cdn.discordapp.com/attachments/799182994992267284/799510817405927444/ElNHS9bXIAEi5gB.png";
                    }

                    var dripEmbed = new DiscordEmbedBuilder
                    {
                        Title = myTitle,
                        Description = $"**{ctx.Member.DisplayName}**, Drip",
                        ImageUrl = myImageUrl,
                        Color = DiscordColor.Rose
                    };
                    if (userDripScoreAdd == 50) { dripEmbed.AddField("** **", $"**+{userDripScoreAdd}** Drip, **Wow!**"); }
                    else { dripEmbed.AddField("** **", $"**+{userDripScoreAdd}** Drip"); }

                    if (!recordedUser.DripInventory.Contains(myTitle))
                    {
                        recordedUser.DripInventory += $"\n{myTitle}";
                        dripEmbed.AddField("**NEW DRIP DISCOVERED**", $"**{myResult.Title}** has been added to your collection.");
                        await lite.SaveChangesAsync();
                    }

                    await ctx.Channel.SendMessageAsync(embed: dripEmbed);

                }
                else
                {
                    var coolDownTimespan = recordedUser.DripCooldown - currentDate;
                    await ctx.Channel.SendMessageAsync($"**{ctx.Member.DisplayName}**, you can do this command again in " +
                        $"**{coolDownTimespan.Hours}h {coolDownTimespan.Minutes} mins**");
                }
            }
        }

        [Command("simp")]
        public async Task Simp(CommandContext ctx, DiscordMember recipient, int amount)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var user = await GetOrCreateProfileAsync(lite, ctx.Member, ctx.Guild.Id);
                var recipientProfile = await GetOrCreateProfileAsync(lite, recipient, ctx.Guild.Id);

                if (amount <= 0)
                {
                    await ctx.Channel.SendMessageAsync($"Invalid amount given.");
                    return;
                }
                else if (user.DripScore < amount)
                {
                    await ctx.Channel.SendMessageAsync($"**{ctx.Member.Username}**, you do not have enough drip to give that amount.");
                    return;
                }

                user.DripScore -= amount;
                recipientProfile.DripScore += amount;
                await lite.SaveChangesAsync();

                var urls = new string[]
                {
                    "https://cdn.discordapp.com/attachments/799182855899971626/802711276798738442/blush1.gif",
                    "https://media.giphy.com/media/PR3wumHIdsBhu/giphy.gif",
                    "https://media.giphy.com/media/HPI9m7McNPGN2/giphy.gif",
                    "https://media.giphy.com/media/vIIbrC80HHN7O/giphy.gif",
                    "https://media.giphy.com/media/UrPxdGW62TDtS/giphy.gif",
                    "https://media.giphy.com/media/19jcWuNGzaI5W/giphy.gif",
                    "https://media.giphy.com/media/6o9roFSbffy9O/giphy.gif",
                    "https://media.giphy.com/media/l1KdaAqHtx1ojBI2c/giphy.gif"

                };
                int randUrl = new Random().Next(0, urls.Length);
                var url = urls.ElementAt(randUrl);

                var simpEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{ctx.Member.Username} has simped for {recipientProfile.Username} :heart:",
                    Color = DiscordColor.Rose,
                    Description = $"**{ctx.Member.Username}**, you have donated **{amount}** drip to **{recipientProfile.Username}**.",
                };
                simpEmbed.WithImageUrl(url);

                var r = new Random();
                if (r.Next(0, 4) == 3 && (amount >= 3))
                {
                    user.DripCooldown = DateTime.Now;
                    simpEmbed.AddField("**--KARMA BONUS--**", "Cooldown for **m!drip** has been reset. nice!");
                    await lite.SaveChangesAsync();
                }

                await ctx.Channel.SendMessageAsync(embed: simpEmbed);
            }
        }
        #endregion

        #endregion

        #region Getter Commands
        [Command("profile")]
        public async Task DisplayProfile(CommandContext ctx, DiscordMember member = null)
        {
            if (member == null)
            {
                member = ctx.Member;
            }

            using (SqliteContext lite = new SqliteContext())
            {
                var userProfile = await GetOrCreateProfileAsync(lite, member, ctx.Guild.Id);

                string tier = GetTier(userProfile.Tier);
                var profileEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{member.Username}'s Profile",
                    Color = DiscordColor.SapGreen,
                    Description = $"Current Tier: **{tier}**"
                };
                profileEmbed.WithThumbnail(member.AvatarUrl);

                var dripScore = userProfile.DripScore;
                profileEmbed.AddField("Drip Score:", $"**{dripScore}** :fire:");
                var collectedDrip = userProfile.DripInventory.Split("\n").Length;

                using (DripContext drip = new DripContext())
                {
                    profileEmbed.AddField("Discovered:", $"**{collectedDrip - 1}**/{drip.Drip.Count()} :scroll:");
                }
                profileEmbed.AddField("Casino Tickets", $"**{userProfile.GambleTickets}** :tickets:");
                if (userProfile.GambleCount >= maxGambleCount)
                {
                    profileEmbed.AddField
                        ("Tickets used:", $"You have reached your quota for spending tickets today, use **\"m!rq\"** to refresh once a daily to refresh yur quota.");
                }
                profileEmbed.AddField("Tickets used:", $"**{userProfile.GambleCount}**/{maxGambleCount}");

                await ctx.Channel.SendMessageAsync(embed: profileEmbed);
            }

        }

        [Command("driptop")]
        [Aliases("dt")]
        public async Task DisplayDripTop(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                int maxProfiles = 15;

                //filters out profiles that are not related to the guild
                List<Profile> profiles = new List<Profile> { };
                foreach (var profile in lite.Profile)
                {
                    if (profile.guildId == ctx.Guild.Id)
                    {
                        profiles.Add(profile);
                    }
                }

                //orders filtered profiles by dripscore
                var sortedProfiles = profiles.OrderByDescending(item => item.DripScore);
                string description = string.Empty;

                int i = 0;


                foreach (var item in sortedProfiles)
                {
                    if (i < maxProfiles)
                    {
                        var tierName = GetTier(item.Tier);
                        description += $"\n{i + 1}. **{item.Username}** | {item.DripScore} :fire: --**{tierName}**";
                        i++;
                    }
                }

                var topEmbed = new DiscordEmbedBuilder
                {
                    Title = $":medal:「{ctx.Guild.Name.ToUpper()}」's Drip Leaderboard ",
                    Description = description,
                    Color = DiscordColor.Gold
                };
                topEmbed.WithThumbnail(ctx.Guild.IconUrl);

                await ctx.Channel.SendMessageAsync(embed: topEmbed);
            }
        }
        #endregion


    }
}

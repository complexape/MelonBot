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



namespace MelonBot.Bots
{
    [RequirePrefixes("m!")]
    public class HelpCommands : BaseCommandModule
    {
        // wrap text with ` for code block font
        // wrap text with ** for bold font
        [Command("gamblehelp")]
        public async Task GambleHelp(CommandContext ctx)
        {
            var title = "Gambling... but in Discord! :thinking:";
            var description = "Quench your thirst in gambling! You can start by doing \"m!gamble\" or by using these following commands, " +
                "but there's a catch! Gambling has its limits and so do you as you're only able to spend **20** tickets every day.";
            var resultEmbed = await PublishHelpEmbed(ctx, title, description);
            resultEmbed.AddField("m!refreshquota/rq", 
                "Use this command to once every **24** hours to refresh your daily quota of spending tickets.");
            resultEmbed.AddField("m!buyticket/bt `<amount-wanted>`", "Use this command to purchase tickets, each ticket is worth **5** drip.");
            resultEmbed.AddField("m!exchangeticket/et `<amount-you-want-to-exchange>` **In-Progress**", " Use this command to exchange your tickets for various rewards!");
            resultEmbed.AddField("m!coinflip/cf", "Wager your tickets on the side the coin flip lands on! " +
                    "Each flip costs **1** ticket and correct guesses reward drip!");
            await ctx.Member.SendMessageAsync(embed: resultEmbed);
        }

        [Command("pollhelp")]
        public async Task PollHelp(CommandContext ctx)
        {
            var title = "Create polls for your friends to decide and vote on!";
            var description = "\n- To create a poll, do \"m!poll\", followed by the duration you want it to last (leave it blank for no time limit)." +
               "\n- IMPORTANT: it's highly recommended that you send the command in a dedicated bot channel to reduce spam." +
               "\n- For choosing your duration, write the integer followed by \"s\", \"m\" and \"d\" for seconds respectively" +
               "\n- (ex: 10s or 5m or 1d or 50m ) " +
               "\n- when the poll expires, the results will be DMed to the sender AND posted to the channel" +
               "\n- Remember, you'll also be asked for a title and the channel you want your poll to be in, so make sure you have those." +
               "\n- Note: Only a single duration can be given, and the only poll options include: :+1: or :-1: ";

            var resultEmbed = await PublishHelpEmbed(ctx, title, description);
            await ctx.Member.SendMessageAsync(embed: resultEmbed);
        }

        [Command("animehelp")]
        public async Task AnimeHelp(CommandContext ctx)
        {
            var title = "MyAnimeList Commands";
            var description = "**NOTE: you can only use the \"a!\" prefix, using \"m!\" will not do anything.**";

            var resultEmbed = await PublishHelpEmbed(ctx, title, description);
            resultEmbed.AddField("a!asearch `<anime-title>`", "**searches** for `<anime-title>`, and sends information about the anime." +
                "\n (Note: **\"RX\" rated** productions can only be shown in **NSFW channels only**.");
            resultEmbed.AddField("a!msearch `<manga-title>`", "**searches** for`<manga-title>`, and sends information about the manga");
            resultEmbed.AddField("a!csearch `<character-name>`", "**searches** for `<character-name>`, and sends information about the character." +
                "\n(Note: Make sure to provide their **Full Name**)");
            resultEmbed.AddField("a!ctop", "sends a page-by-page list of the top 100 characters from MyAnimeList.net.");
            resultEmbed.AddField("a!atop", "sends a page-by-page list of the top 100 anime from MyAnimeList.net.");
            resultEmbed.AddField("a!list `<MAL-Account-Username>`", "returns `<MAL-Account-Username>`'s anime list, along with other stats. ");
            resultEmbed.AddField("a!season `<season>` `<year>`", "sends a page-by-page list of the top 50 anime of given `<season>` and `<year>`");
            resultEmbed.AddField("a!recomm `<title>`", "sends a list of shows you might like if you liked `<title>`.");
            resultEmbed.AddField("a!news `<title>`", "sends a list of the most recent news articles related to `<title>`, from MyAnimeList.net");
            resultEmbed.WithFooter("Credits to API: https://jikan.moe/ ");
            await ctx.Member.SendMessageAsync(embed: resultEmbed);
        }

        public async Task<DiscordEmbedBuilder> PublishHelpEmbed(CommandContext ctx, string title, string description)
        {
            var helpEmbed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = DiscordColor.SpringGreen
            };
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":+1:"));
            return helpEmbed;
        }
    }
}

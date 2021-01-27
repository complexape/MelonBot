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
    public class InteractivityCommands : BaseCommandModule
    {

        [Command("poll")]
        [RequireUserPermissions(DSharpPlus.Permissions.MentionEveryone)]
        public async Task Poll(CommandContext ctx, TimeSpan duration = new TimeSpan())
        {
            var interactivity = ctx.Client.GetInteractivity();

            #region Gathers the channel of the poll
            await ctx.Channel.SendMessageAsync(
          "`Respond with the name of the your desired channel without the # (respond with anything else for this current channel): " +
          "\n(Your response will have a reaction if a channel is found)`");
           var channelInput = await interactivity.WaitForMessageAsync(
                x => x.Channel == ctx.Channel && x.Author.Id == ctx.Member.Id);

            //initialize variables
            var channels = ctx.Guild.Channels.Values.ToArray();
            DiscordChannel channel = ctx.Channel;

            foreach (var i in channels) //Checks if user sent a channel and sets channel and if not, breaks and defaults to current channel
            {
                if (i.Name == channelInput.Result.Content.ToString() && i.GuildId == ctx.Guild.Id && i.IsPrivate == false && i.Type.ToString() == "Text")
                {
                    channel = i;  // successfully detects that given channel does exist and breaks out of loop
                    break;
                    
                }
            }
            await channelInput.Result.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":+1:"));
            #endregion

            #region Gathers the title of the poll
            
            await ctx.Channel.SendMessageAsync("Respond with your desired title for the poll:\n(Posting images must follow this format: `<title> img: <discord-image-url>`");
            var Title = await interactivity.WaitForMessageAsync(
                x => x.Channel == ctx.Channel && x.Author.Id == ctx.Member.Id);
            await Title.Result.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":+1:"));
            string title = Title.Result.Content.ToString();

            await ctx.Channel.SendMessageAsync("`respond with \"y\" to confirm your selections and post.`");
            var confirmation = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel
           && x.Author.Id == ctx.Member.Id);
            if (confirmation.Result.Content.ToString() == "y")
            {
                await confirmation.Result.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":+1:"));
                await ctx.Channel.SendMessageAsync("`success!`");
            }
            else
            {
                await ctx.Channel.SendMessageAsync("`request cancelled.`");
                return;
            }
            #endregion

            #region Creates and posts the poll embed
            //calculates when poll ends (to be shown in embed)
            DateTime dt = DateTime.Now;

            //Stores emojis in variables//
            var thumbsUp = DiscordEmoji.FromName(ctx.Client, ":+1:");
            var thumbsDown = DiscordEmoji.FromName(ctx.Client, ":-1:");

            
            //checks if user has given a duration and if not, it removes the duration in description
            string description = $"by: {ctx.User.Username}";
            if (duration == new TimeSpan())
            {
              description = $"by: {ctx.User.Username} \n";
            }

            string titleNoImg = title.Remove(title.IndexOf("img:"));
            string titlejustImg = title.Substring(title.IndexOf("img:") + 4);

            //Creates poll embed 
            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = $"{titleNoImg} :thinking:",
                Description = description,
                Color = DiscordColor.SpringGreen
            };

            //creates Emoji variables
            DiscordEmoji[] emojiOptions = {
                DiscordEmoji.FromName(ctx.Client, ":+1:"),
                DiscordEmoji.FromName(ctx.Client, ":-1:")};

            if (title.Contains("https://cdn.discordapp.com/attachments/") && title.Contains(".png") && title.Contains("img:"))
            {
                pollEmbed.WithImageUrl(titlejustImg);
            }

            //sends embed and reacts with "thumbsup" and "thumbsdown"
            var pollMessage = await channel.SendMessageAsync(embed: pollEmbed);  
            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }
            #endregion

            #region Gathers and publishes the results

            for (int i = 0; i <= 50000000; i++) //Stall loop for correcting the timing
            {
                emojiOptions.Select(x => $"{x} :{emojiOptions.Where(xtd => x == xtd).Count()}").Distinct();
            }
           
            var result = await interactivity.CollectReactionsAsync(pollMessage, duration); //Collects reactions
            var distinctResults = result.Distinct();
            
            int yesCount = 0;
            int noCount = 0;
            foreach (var reaction in distinctResults) //records the number of times each option was picked in an int
            {
                if (reaction.Emoji.GetDiscordName() == ":+1:")
                {
                    yesCount++;
                }
                else if (reaction.Emoji.GetDiscordName() == ":-1:")
                {
                    noCount++;
                }
            }
            

            if (duration != new TimeSpan()) //does this if poll is given a timespan
            {
                //calculates result based on number of votes
                string resultDescription = "`looking empty... no votes.`"; 
                if (yesCount > noCount) 
                {
                    resultDescription = $":+1: `is the winner!`";
                }
                else if (yesCount < noCount)
                {
                    resultDescription = $":-1: `is the winner!`";
                }
                else if(yesCount == noCount)
                {
                    resultDescription = "A 50/50 draw.";
                }

                var resultEmbed = new DiscordEmbedBuilder
                {
                    Title = $"\"{titleNoImg}\" Poll Results:",
                    Description = resultDescription,
                    Color = DiscordColor.Yellow
                };
                await pollMessage.DeleteAsync();
                await channel.SendMessageAsync(embed: resultEmbed);
                await ctx.Member.SendMessageAsync(embed: resultEmbed);
                #endregion
            }
        }
    }
}

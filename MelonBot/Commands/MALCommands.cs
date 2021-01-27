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
using JikanDotNet;

namespace MelonBot.Bots
{
    [RequirePrefixes("a!")]
    public class MALCommands : BaseCommandModule
    {

        // wrap text with ` for code block font
        // wrap text with ** for bold font

        #region Search Commands
        [Command("asearch")]
        public async Task AnimeSearch(CommandContext ctx, [RemainingText] string title)
        {
            var isShort = await IsTitleShort(title, ctx);
            if (isShort == true) { return; }

            var interactivity = ctx.Client.GetInteractivity();
            IJikan jikan = new Jikan(true);
           
            AnimeSearchResult animeSearchResult = await jikan.SearchAnime(title); //gets the anime based on title argument

            var firstFiveResults = animeSearchResult.Results.Take(5);

            string responseContent = $"Please respond with the **number** of your desired title:";
            var forCount = 0;

            foreach (var item in firstFiveResults)
            {
                forCount++;
                responseContent = responseContent + $"\n **{forCount}**: {item.Title}";
            }
            var response = await ctx.Channel.SendMessageAsync(responseContent);
            var index = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Id == ctx.Member.Id);

            AnimeSearchEntry firstAnimeResult = new AnimeSearchEntry();
            if (index.Result.Content.ToString() == "1" || index.Result.Content.ToString() == "2" || index.Result.Content.ToString() == "3" || index.Result.Content.ToString() == "4" || index.Result.Content.ToString() == "5")
            {
                int x = Int32.Parse(index.Result.Content.ToString());
                firstAnimeResult = firstFiveResults.Take(5).ElementAt(x-1);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("`invalid input: request cancelled.`");
                return;
            }

            #region Creating and Sending Anime Embed
            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"{firstAnimeResult.Title} ({firstAnimeResult.Type})",
                Description = $"{ firstAnimeResult.Description}",
                ImageUrl = firstAnimeResult.ImageURL,
                Color = DiscordColor.Azure,
                Url = firstAnimeResult.URL,
            };
            resultEmbed.AddField("Score:", firstAnimeResult.Score.ToString());
            resultEmbed.AddField("Rating:", firstAnimeResult.Rated);
            resultEmbed.AddField("Release Date:", firstAnimeResult.StartDate.ToString());
            resultEmbed.WithThumbnail("https://cdn.discordapp.com/attachments/779566889198157825/795191771315437579/apple-touch-icon-256.png"); //MAL ICON
            resultEmbed.WithFooter("Source: MyAnimeList.net");

            if (firstAnimeResult.Title == "Boku no Pico")
            {
                await ctx.Channel.SendMessageAsync(embed: resultEmbed);
                await ctx.Channel.SendMessageAsync("`Sir, the man you're looking for is right here.`");
            }
            else if (firstAnimeResult.Rated.ToString() == "Rx" && ctx.Channel.IsNSFW == false)
            {
                await ctx.Channel.SendMessageAsync("`This anime can only be shown in a NSFW channel only.`");
            }
            else
            {
                await ctx.Channel.SendMessageAsync(embed: resultEmbed);
            }
            #endregion

        }

        [Command("msearch")]
        public async Task MangaSearch(CommandContext ctx, [RemainingText] string title)
        {
            var isShort = await IsTitleShort(title, ctx);
            if (isShort == true) { return; }

            // Initialize JikanWrapper
            IJikan jikan = new Jikan(true);

            var interactivity = ctx.Client.GetInteractivity();
           
            MangaSearchResult animeSearchResult = await jikan.SearchManga(title); //gets the anime based on title argument

            var firstFiveResults = animeSearchResult.Results.Take(5);

            string responseContent = $"Please respond with the **number** of your desired title:";
            var forCount = 0;

            foreach (var item in firstFiveResults)
            {
                forCount++;
                responseContent = responseContent + $"\n **{forCount}**: {item.Title}";
            }
            var response = await ctx.Channel.SendMessageAsync(responseContent);
            var index = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Id == ctx.Member.Id);

            MangaSearchEntry firstMangaSearchResult = new MangaSearchEntry();
            if (index.Result.Content.ToString() == "1" || index.Result.Content.ToString() == "2" || index.Result.Content.ToString() == "3" || index.Result.Content.ToString() == "4" || index.Result.Content.ToString() == "5")
            {
                int x = Int32.Parse(index.Result.Content.ToString());
                firstMangaSearchResult = firstFiveResults.Take(5).ElementAt(x - 1);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("`invalid input: request cancelled.`");
                return;
            }



            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"{firstMangaSearchResult.Title} ({firstMangaSearchResult.Type})",
                Description = $"{ firstMangaSearchResult.Description}",
                ImageUrl = firstMangaSearchResult.ImageURL,
                Color = DiscordColor.Azure,
                Url = firstMangaSearchResult.URL,
                
            };
            resultEmbed.AddField("Score:", firstMangaSearchResult.Score.ToString());
            resultEmbed.AddField("Release Date:", firstMangaSearchResult.StartDate.ToString());
            resultEmbed.AddField("** **", $"**{firstMangaSearchResult.Chapters}** Chapters / **{firstMangaSearchResult.Volumes}** Volumes");
            resultEmbed.WithThumbnail("https://cdn.discordapp.com/attachments/779566889198157825/795191771315437579/apple-touch-icon-256.png"); //MAL ICON
            resultEmbed.WithFooter("source: MyAnimeList.net");
            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }

        [Command("csearch")]
        public async Task CharacterSearch(CommandContext ctx, [RemainingText] string character)
        {
            var isShort = await IsTitleShort(character, ctx);
            if (isShort == true) { return; }

            IJikan jikan = new Jikan(true);

            var interactivity = ctx.Client.GetInteractivity();

            CharacterSearchResult characterSearchResult = await jikan.SearchCharacter(character); //gets the anime based on title argument

            var firstFiveResults = characterSearchResult.Results.Take(4);

            string responseContent = $"Please respond with the **number** of your desired character:";
            var forCount = 0;

            foreach (var item in firstFiveResults)
            {
                forCount++;
                responseContent = responseContent + $"\n **{forCount}**: {item.Name}";
            }
            var response = await ctx.Channel.SendMessageAsync(responseContent);
            var index = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Id == ctx.Member.Id);

            CharacterSearchEntry firstCharSearchResult = new CharacterSearchEntry();
            if (index.Result.Content.ToString() == "1" || index.Result.Content.ToString() == "2" || index.Result.Content.ToString() == "3" || index.Result.Content.ToString() == "4")
            {
                int x = Int32.Parse(index.Result.Content.ToString());
                firstCharSearchResult = firstFiveResults.Take(4).ElementAt(x - 1);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("`invalid input: request cancelled.`");
                return;
            }

            CharacterSearchResult charSearchResult = await jikan.SearchCharacter(character);


            string description = "";
            if (firstCharSearchResult.AlternativeNames.Count >= 1)
            {
                string alias = firstCharSearchResult.AlternativeNames.ElementAt(0);
                description = $"(Alias: {alias}) \n \n{firstCharSearchResult.URL}";
            }
            else
            {
                description = firstCharSearchResult.URL;
            }
            
            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = firstCharSearchResult.Name ,
                Description = description,
                ImageUrl = firstCharSearchResult.ImageURL,
                Color = DiscordColor.Azure,
            };
            resultEmbed.WithThumbnail("https://cdn.discordapp.com/attachments/779566889198157825/795191771315437579/apple-touch-icon-256.png"); //MAL ICON
            resultEmbed.WithFooter("source: MyAnimeList.net");
            

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }
        #endregion

        #region Display Rankings Commands
        [Command("ptop")]
        public async Task CharTop(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            int currentPage = 1;

            var leftEmoji = DiscordEmoji.FromName(ctx.Client, ":closed_book:");
            var rightEmoji = DiscordEmoji.FromName(ctx.Client, ":blue_book:");

            IJikan jikan = new Jikan(true);

            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"Top Characters",
                Color = DiscordColor.Azure
            };
            resultEmbed.WithThumbnail("https://cdn.discordapp.com/attachments/779566889198157825/795191771315437579/apple-touch-icon-256.png");
            resultEmbed.WithFooter($"source: MyAnimeList.net \npage: {currentPage}/10");

            CharactersTop charTop = await jikan.GetCharactersTop(1);
            CharactersTop charTop2 = await jikan.GetCharactersTop(2);


            for (var i = 0; i <= 10; i++)
            {
                var character = charTop.Top.ElementAt(i);
                resultEmbed.AddField($"{character.Rank}: {character.Name}", "** **");
            };

            var message = await ctx.Channel.SendMessageAsync(embed: resultEmbed);
            DiscordEmoji[] emojiOptions = {
                DiscordEmoji.FromName(ctx.Client, ":+1:"),
                DiscordEmoji.FromName(ctx.Client, ":-1:")};

            while (true)
            {
                await message.CreateReactionAsync(leftEmoji);
                await message.CreateReactionAsync(rightEmoji);

                for (int i = 0; i <= 1000000; i++) //Stall loop for correcting the timing
                {
                    emojiOptions.Select(x => $"{x} :{emojiOptions.Where(xtd => x == xtd).Count()}").Distinct();
                }

                var result = await ManagePages(ctx, leftEmoji, rightEmoji, message, currentPage);
                if (result.Item2 == true)
                {
                    currentPage = result.Item1;
                    await CreateNewPage(currentPage);
                }
                else
                {
                    continue;
                }
            }

            async Task CreateNewPage(int currentPage)
            {
                await message.DeleteAsync();
                resultEmbed.WithFooter($"source: MyAnimeList.net \npage: {currentPage}/10");
                resultEmbed.ClearFields();

                int iStart = 0;
                CharactersTop mycharTop;

                if (currentPage < 1 || currentPage > 10) { return; }
                else if (currentPage <= 5)
                {
                    mycharTop = charTop;
                    iStart = ((currentPage * 10) - 10);
                }
                else
                {
                    mycharTop = charTop2;
                    iStart = ((currentPage - 6) * 10);
                }

                for (var i = iStart; i < iStart + 10; i++)
                {
                    var character = mycharTop.Top.ElementAt(i);
                    resultEmbed.AddField($"{character.Rank}: {character.Name}", "** **");
                };

                message = await ctx.Channel.SendMessageAsync(embed: resultEmbed);



            }
        }
       
        [Command("ftop")]
        public async Task PagesTest(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            int currentPage = 1;

            var leftEmoji = DiscordEmoji.FromName(ctx.Client, ":closed_book:");
            var rightEmoji = DiscordEmoji.FromName(ctx.Client, ":blue_book:");

            IJikan jikan = new Jikan(true);
           

            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"Top Anime",
                Color = DiscordColor.Azure

            };
            resultEmbed.WithThumbnail("https://cdn.discordapp.com/attachments/779566889198157825/795191771315437579/apple-touch-icon-256.png");
            resultEmbed.WithFooter($"source: MyAnimeList.net \npage: {currentPage}/10");

            AnimeTop aniTop = await jikan.GetAnimeTop(1);
            AnimeTop aniTop2 = await jikan.GetAnimeTop(2);

            for (var i = 0; i < 10; i++)
            {
                var character = aniTop.Top.ElementAt(i);
                resultEmbed.AddField($"{character.Rank}: {character.Title}", "** **");
            };

            var message = await ctx.Channel.SendMessageAsync(embed: resultEmbed);
            DiscordEmoji[] emojiOptions = {
                DiscordEmoji.FromName(ctx.Client, ":+1:"),
                DiscordEmoji.FromName(ctx.Client, ":-1:")};

            while (true)
            {
                await message.CreateReactionAsync(leftEmoji);
                await message.CreateReactionAsync(rightEmoji);

                for (int i = 0; i <= 1000000; i++) //Stall loop for correcting the timing
                {
                    emojiOptions.Select(x => $"{x} :{emojiOptions.Where(xtd => x == xtd).Count()}").Distinct();
                }

                var result = await ManagePages(ctx, leftEmoji, rightEmoji, message, currentPage);
                if (result.Item2 == true)
                {
                    currentPage = result.Item1;
                    await CreateNewPage(currentPage);
                }
                else
                {
                    continue;
                }
            }

           async Task CreateNewPage(int currentPage)
            {
                await message.DeleteAsync();
                resultEmbed.WithFooter($"source: MyAnimeList.net \npage: {currentPage}/10");
                resultEmbed.ClearFields();

                int iStart = 0;
                AnimeTop myAnimeTop;

                if (currentPage < 1 || currentPage > 10) { return; }
                else if (currentPage <= 5) {
                    myAnimeTop = aniTop;
                    iStart = ((currentPage * 10) - 10); 
                }
                else  {
                    myAnimeTop = aniTop2;
                    iStart = ((currentPage-6) * 10); 
                }

                for (var i = iStart; i < iStart+10; i++)
                {
                    var character = myAnimeTop.Top.ElementAt(i);
                    resultEmbed.AddField($"{character.Rank}: {character.Title}", "** **");
                };

               message = await ctx.Channel.SendMessageAsync(embed: resultEmbed);
            }
        }
        #endregion

        #region Other Commands
        [Command("list")]
        public async Task GetAnimeList(CommandContext ctx, [RemainingText] string username)
        {
            var isShort = await IsTitleShort(username, ctx);
            if (isShort == true) { return; }

            IJikan jikan = new Jikan(true);

            UserProfile user = await jikan.GetUserProfile(username);
            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"`{user.Username}`'s anime list: https://myanimelist.net/animelist/{username}$",
                Description =
                $"\n:blue_circle: `{user.AnimeStatistics.Completed.ToString()}` **Completed**" +
                $"\n:green_circle: `{user.AnimeStatistics.Watching.ToString()}` **Watching**" +
                $"\n:yellow_circle: `{user.AnimeStatistics.OnHold.ToString()}` **On Hold**" +
                $"\n:red_circle: `{user.AnimeStatistics.Dropped.ToString()}` **Dropped**",
                Color = DiscordColor.Azure,
            };
            resultEmbed.AddField("Watch Time :clock9:", $"`{user.AnimeStatistics.DaysWatched.ToString()}` Days Watched");
            resultEmbed.WithFooter("source: MyAnimeList.net");

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }

        [Command("season")]
        public async Task GetTopAnimeSeason(CommandContext ctx, string _seasons = null, int _year = -1)
        {

            #region Checks Task Arguments
            Seasons seasons = Seasons.Winter;

           if (_seasons == null)
            {
                //Sets the season to current season based on DateTime.Now
                int season = getSeason(DateTime.Now); 
                switch (season)
                {
                    case 0:
                        seasons = Seasons.Spring;
                        break;
                    case 1:
                        seasons = Seasons.Summer;
                        break;
                    case 2:
                        seasons = Seasons.Fall;
                        break;
                    case 3:
                        seasons = Seasons.Winter;
                        break;
                }
            }
            else
            {
                switch (_seasons.ToLower())
                {
                    case "spring":
                        seasons = Seasons.Spring;
                        break;
                    case "summer":
                        seasons = Seasons.Summer;
                        break;
                    case "fall":
                        seasons = Seasons.Fall;
                        break;
                    case "autumn":
                        seasons = Seasons.Fall;
                        break;
                    case "winter":
                        seasons = Seasons.Winter;
                        break;
                    default:
                        await ctx.Channel.SendMessageAsync("`Invalid Season: please choose a valid season.`");
                        return;

                }
            }
            int getSeason(DateTime date)//Method for returning season
            {
                float value = (float)date.Month + date.Day / 100f;  // <month>.<day(2 digit)>    
                if (value < 3.21 || value >= 12.22) return 3;   // Winter
                if (value < 6.21) return 0; // Spring
                if (value < 9.23) return 1; // Summer
                return 2;   // Autumn
            }

            if (_year == -1) //Does this if user did not input any year
            {
                _year = System.DateTime.Today.Year;
            }
            int year = _year;
            #endregion

            
            DiscordEmoji[] emojiOptions = {
                DiscordEmoji.FromName(ctx.Client, ":+1:"),
                DiscordEmoji.FromName(ctx.Client, ":-1:")};

            IJikan jikan = new Jikan();

            Season animeSeason;
             try
             {
                 animeSeason = await jikan.GetSeason(year, seasons);
             }
             catch (Exception)
             {
                 await ctx.Channel.SendMessageAsync("`Invalid Year: please choose a valid year.`");
                 return;
             }

              var resultEmbed = new DiscordEmbedBuilder
             {
                 Title = $":star: Top Anime of {animeSeason.SeasonName} {animeSeason.SeasonYear} :star:",
                 Color = DiscordColor.Azure

             };

             for (var i = 0; i < 5; i++)
             {
                 var anime = animeSeason.SeasonEntries.ElementAt(i);
                 resultEmbed.AddField($"{anime.Title}", $"Air Date: {anime.AiringStart} \n{anime.URL}");
             };
             resultEmbed.WithFooter("source: MyAnimeList.net \npage: 1/10");
            
           var message = await ctx.Channel.SendMessageAsync(embed: resultEmbed);

            int currentPage = 1;
            var leftEmoji = DiscordEmoji.FromName(ctx.Client, ":closed_book:");
            var rightEmoji = DiscordEmoji.FromName(ctx.Client, ":blue_book:");
            var interactivity = ctx.Client.GetInteractivity();
            while (true)
            {
                await message.CreateReactionAsync(leftEmoji);
                await message.CreateReactionAsync(rightEmoji);

                for (int i = 0; i <= 1000000; i++) //Stall loop for correcting the timing
                {
                    emojiOptions.Select(x => $"{x} :{emojiOptions.Where(xtd => x == xtd).Count()}").Distinct();
                }

                var result = await ManagePages(ctx, leftEmoji, rightEmoji, message, currentPage);
                if (result.Item2 == true)
                {
                    currentPage = result.Item1;
                    await CreateNewPage(currentPage);
                }
                else
                {
                    continue;
                }
            }
            async Task CreateNewPage(int currentPage)
            {
                await message.DeleteAsync();
                resultEmbed.WithFooter($"source: MyAnimeList.net \npage: {currentPage}/10");
                resultEmbed.ClearFields();

                int iStart = 0;
                if (currentPage < 1 || currentPage > 10) { return; }
                else 
                {
                    iStart = ((currentPage * 5) - 5);
                }

               
                for (var i = iStart; i < iStart+5; i++)
                {
                    var anime = animeSeason.SeasonEntries.ElementAt(i);
                    resultEmbed.AddField($"{anime.Title}", $"Air Date: {anime.AiringStart} \n{anime.URL}");
                };

                message = await ctx.Channel.SendMessageAsync(embed: resultEmbed);
            }
        }

       

        [Command("recomm")]
        public async Task GetRecommendations(CommandContext ctx, [RemainingText] string title = "")
        {
            
            var isShort = await IsTitleShort(title, ctx);
            if (isShort == true) { return; }

            IJikan jikan = new Jikan();
            var anime = await GetAnimeFromSearch(title);
            var id = anime.MalId;

            //uses id to fetch info
            Recommendations recommendations = await jikan.GetAnimeRecommendations(id);

            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"If you liked {anime.Title} you might also like:",
                Color = DiscordColor.Azure,
            };
            for (int i = 0; i <= 5; i++)
            {
                var recommendation = recommendations.RecommendationCollection.ElementAt(i);
                resultEmbed.AddField(recommendation.Title, recommendation.Url);
            }
            resultEmbed.WithFooter("source: MyAnimeList.net");
            resultEmbed.WithThumbnail(anime.ImageURL);

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }
       
        [Command("news")]
        public async Task GetNews(CommandContext ctx, [RemainingText] string title = "")
        {
           
            var isShort = await IsTitleShort(title, ctx);
            if (isShort == true){ return; }

            IJikan jikan = new Jikan();
            var anime = await GetAnimeFromSearch(title);
            var id = anime.MalId;

            //uses id to fetch info
            AnimeNews news = await jikan.GetAnimeNews(id);
            // var newsOrdered = news.News.OrderBy(x => x.Date.Equals(DateTime.Now)).ToList(); USE THIS TO SORT COLLECTIONS

            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"News Results for: {anime.Title}",
                Description ="sorted by: newest to oldest",
                Color = DiscordColor.Azure
            };
            for (var i = 0; i <= 5; i++)
            {
                var item = news.News.ElementAt(i);
                resultEmbed.AddField($"{item.Title} ", $"published: {item.Date.Value.ToShortDateString()} {item.Url}");
            }
            resultEmbed.WithFooter("source: MyAnimeList.net");
            resultEmbed.WithThumbnail(anime.ImageURL);

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }
        #endregion

        #region Methods

        public async Task<(int, bool)> ManagePages(CommandContext ctx, DiscordEmoji leftEmoji, DiscordEmoji rightEmoji, DiscordMessage message, int currentPage)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var reactionResult = await interactivity.WaitForReactionAsync(x => x.Message == message &&
             (x.Emoji == leftEmoji || x.Emoji == rightEmoji));

            if (reactionResult.Result.Emoji == rightEmoji && currentPage < 10)
            {
                currentPage++;
                if (currentPage > 9) { currentPage = 10; }
                else if (currentPage < 2) { currentPage = 1; }
                return (currentPage, true);
            }
            else if (reactionResult.Result.Emoji == leftEmoji && currentPage > 1)
            {
                currentPage--;
                if (currentPage > 9) { currentPage = 10; }
                else if (currentPage < 2) { currentPage = 1; }
                return (currentPage, true);
            }
            else
            {
                return (currentPage, false);
            }
        }

        public async Task<bool> IsTitleShort(string title, CommandContext ctx)
        {
            if (title == null)
            {
                title = "";
            }
            try
            {
                if (title.Length < 3)
                {
                    await ctx.Channel.SendMessageAsync($"`Invalid search: input has to have 3 or more characters.`");
                    return true;
                }
            }
            catch (Exception)
            {
                await ctx.Channel.SendMessageAsync($"`Invalid search: input has to have 3 or more characters.`");
                return true;
            }
            return false;
        }

        public async Task<AnimeSearchEntry> GetAnimeFromSearch(string title)
        {
            IJikan jikan = new Jikan();
            AnimeSearchResult searches = await jikan.SearchAnime(title);
            var anime = searches.Results.First();
            return anime;
        }
#endregion
    }
}

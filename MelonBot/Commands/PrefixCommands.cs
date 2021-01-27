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
using MelonBot.Bots.Models;
using MelonBot.Bots.Context;
using System.Net;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SharkBot.Helpers;

namespace MelonBot.Bots
{
    [RequirePrefixes("m!")]
    public class PrefixCommands : BaseCommandModule
    {
        
        Random r = new Random();

        #region Array of all Character Objects
        Character[] randomWaifu = new Character[]
        {
                new Character("Zero Two", false, "https://cdn.discordapp.com/attachments/639917035891064833/779257381866569728/2Q.png"),
                new Character("David Chen", true, "https://cdn.discordapp.com/attachments/647240098219556865/779578652971106354/unknown.png"),
                new Character("Rem", false, "https://cdn.discordapp.com/attachments/639917035891064833/779257508958175242/Z.png"),
                new Character("Megumin", false, "https://cdn.discordapp.com/attachments/639917035891064833/779257641175089152/9k.png"),
                new Character("Nezuko Kamado", false, "https://cdn.discordapp.com/attachments/639917035891064833/779257759894863902/2Q.png"),
                new Character("Asuna Yuuki", false, "https://cdn.discordapp.com/attachments/639917035891064833/779257897682468884/2Q.png"),
                new Character("Mai Sakurajima", false, "https://cdn.discordapp.com/attachments/639917035891064833/779258022668009503/images.png"),
                new Character("Saber", false, "https://cdn.discordapp.com/attachments/639917035891064833/779258195695632434/2Q.png"),
                new Character("Rias Gremory", false, "https://cdn.discordapp.com/attachments/639917035891064833/779258284720390174/Z.png"),
                new Character("Aqua", false, "https://cdn.discordapp.com/attachments/639917035891064833/779258381315080253/9k.png"),
                new Character("Chika Fujiwara", false, "https://cdn.discordapp.com/attachments/639917035891064833/779258450072305674/9k.png"),
                new Character("Albedo", false, "https://cdn.discordapp.com/attachments/639917035891064833/779258506858594324/Z.png"),
                new Character("Raphtalia(Child)", false, "https://cdn.discordapp.com/attachments/639917035891064833/779259306067361832/Z.png"),
                new Character("Raphtalia", false, "https://cdn.discordapp.com/attachments/639917035891064833/779259318017064960/images.png"),
                new Character("Emilia", false, "https://cdn.discordapp.com/attachments/639917035891064833/779259458673049620/2Q.png"),
                new Character("Akame", false, "https://cdn.discordapp.com/attachments/639917035891064833/779259548254863360/Z.png"),
                new Character("Kaguya Shinomiya", false, "https://cdn.discordapp.com/attachments/639917035891064833/779259747060023316/2Q.png"),
                new Character("Akeno Himejima", false, "https://cdn.discordapp.com/attachments/639917035891064833/779259878791315506/Z.png"),
                new Character("Chizuru Ichinose", false, "https://cdn.discordapp.com/attachments/639917035891064833/779260172978618389/9k.png"),
                new Character("Erina Nakiri", false, "https://cdn.discordapp.com/attachments/639917035891064833/779260262460424212/2Q.png"),
                new Character("SHREK", true, "https://cdn.discordapp.com/attachments/717972833573011466/779260433953587210/76927939ad6134c1b5b0fa472803ca4b.png"),
                new Character("Yukino Yukinoshita", false, "https://cdn.discordapp.com/attachments/639917035891064833/779260388826677258/9k.png"),
                new Character("Miku Nakano", false, "https://cdn.discordapp.com/attachments/639917035891064833/781066423811440650/Z.png"),
                new Character("Ichigo", false, "https://cdn.discordapp.com/attachments/639917035891064833/781066974397857812/9k.png"),
                new Character("Wiz", false, "https://cdn.discordapp.com/attachments/639917035891064833/781067238920683530/9k.png"),
                new Character("xQcOW", true, "https://cdn.discordapp.com/attachments/632655028393607218/749009138259984504/1802.png"),
                new Character("100 gecs :thinking:", true , "https://cdn.discordapp.com/attachments/482407851180949504/749008879454781440/charlie_and_lola_5021.png"),
                new Character("Ais Wallenstein", false, "https://cdn.discordapp.com/attachments/639917035891064833/781068013263781908/2Q.png"),
                new Character("Kyouka Izumi", false, "https://cdn.discordapp.com/attachments/639917035891064833/781067786533076992/9k.png"),
                new Character("Mine", false, "https://cdn.discordapp.com/attachments/639917035891064833/781067411608698911/2Q.png"),
                new Character("Leone", false, "https://cdn.discordapp.com/attachments/639917035891064833/781067607285301248/images.png"),
                new Character("Chelsea", false, "https://cdn.discordapp.com/attachments/639917035891064833/781067553551155220/Z.png"),
                new Character("Rin Tohsaka", false, "https://cdn.discordapp.com/attachments/639917035891064833/781067094976364554/Z.png"),
                new Character("Nao Tomori", false, "https://cdn.discordapp.com/attachments/639917035891064833/781066784219332608/2Q.png"),
                new Character("Taiga Aisaka", false, "https://cdn.discordapp.com/attachments/639917035891064833/781066518955950150/9k.png"),
                new Character("Kirumi Tokisaki", false, "https://cdn.discordapp.com/attachments/639917035891064833/781066271655329802/9k.png"),
        };
        #endregion

        #region Simple Commands

        [Command("connorbts")]
        [Aliases("cb")]
        [Description("Very cool, and very epic command.")]   
        public async Task Connorbts(CommandContext ctx)
        {
            // use ctx. for getting arguments from the command (aka who sent it)
            await ctx.Channel.SendMessageAsync("https://cdn.discordapp.com/attachments/620056088863178755/654876394358308875/eoqahmlgx9441.png").ConfigureAwait(false);
        }

        [Command("sad")]
        [Description("When you find out your favourite Waifu isn't real.")]
        public async Task DavidUnravel(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://cdn.discordapp.com/attachments/620060829995368458/774089743083241482/David_Unravel.mp4").ConfigureAwait(false);
        }

        [Command("bigtimerush")]
        [Aliases("btr")]
        public async Task BigTimeRush(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://cdn.discordapp.com/attachments/620083637202976819/797608991236292658/connor_sings_big_time_rush.mp4").ConfigureAwait(false);
        }
        #endregion

        [Command("sonicsays")]
        public async Task SonicSays(CommandContext ctx,[RemainingText] string input = "")
        {
            var img = Image.Load(@"Images\sonicsays.jpg");

            Font font = SystemFonts.CreateFont("Times New Roman", 12);

            var img_clone = img.Clone(img_ctx =>
                ImageHelper.ApplyScalingWaterMarkWordWrap(img_ctx, font, input, Color.Black, 20));

            var outputFile = @$"temp\{ctx.Member.Id}{DateTime.UtcNow.Ticks}.jpg";

            img_clone.Save(outputFile);

            await ctx.Channel.SendFileAsync(outputFile);
            try
            {
                img_clone.Dispose();
                File.Delete(outputFile);
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(e.ToString());
            }
        } 


        [Command("family")]
        [Aliases("f")]
        [Description("Returns the future family of <user>.")]
        public async Task Spouse(CommandContext ctx, DiscordUser user = default )
        {
            string victim = ctx.Message.Author.Username;
            if (user != null){
                victim = user.Username;
            }
            //RNG for image//
            int maxArray = randomWaifu.Length;
            int waifuNum = r.Next(0, maxArray);

            //RNG for family count//
            int descNum = r.Next(0, 10);

            #region Field Prompts
            string pronoun = "it";
            string pronoun2 = "their";
            if (randomWaifu[waifuNum].IsMale == true)
            {
                pronoun = "He";
                pronoun2 = "his";
            }
            else
            {
                pronoun = "She";
                pronoun2 = "her";
            }

            string[] prompts = new string[] {
                $"{pronoun} married you just to get a green card. Yikes!",
                $"{pronoun} fell in Love with you through a dating app.",
                $"{pronoun} left and took the kids. :(",
                $"{pronoun} still ignores you and wants to divorce after 30 years of marriage.",
                $"Wants to have more kids. 0_0",
                $"Held a gender reveal party and accidently caused a raging wildfire.",
                $"{pronoun} likes long walks on the beach.",
                $"Keeps a 3ft. bamboo still in under {pronoun2} bed.",
                $"{pronoun} listens to Nickelback.",
                "(none)",
                "(none)",
                "(none)",
                "(none)",
                "I'm so sorry."
            };
            #endregion

            //RNG for the appearance of field prompts in embed//
            int fldNum = r.Next(0, prompts.Length);

            var spouseEmbed = new DiscordEmbedBuilder
            {
                Title = $"{victim} :wedding: {randomWaifu[waifuNum].Name}",
                ImageUrl = randomWaifu[waifuNum].Image,
                Color = DiscordColor.Yellow,
                Description = $"and their {Math.Abs(descNum - 2)} daugther(s) , {Math.Abs(descNum - 4)} son(s) , and {Math.Abs(descNum - 2)} pet(s)!",
            };
            
            spouseEmbed.AddField("Trait:", prompts[fldNum]);
           
            await ctx.Channel.SendMessageAsync(embed: spouseEmbed).ConfigureAwait(false);
            
        }

        [Command("echo")]
        [Description("echoes a given message for a given number of times (max 50).")]
        public async Task Echo(CommandContext ctx,
            [Description("# of times you want the message to be echoed.")] int echoAmount,
            [Description("The message you want to be echoed.")][RemainingText] string item)
        {
            if (echoAmount > 50) { echoAmount = 50; }
                for (int i = 0; i < echoAmount; i++)
                {
                    await ctx.Channel.SendMessageAsync(item).ConfigureAwait(false);
                }
        }

        [Command("femboy")]
        [Aliases("fb")]
        [Description("as the title suggests.")]
        public async Task Femboy(CommandContext ctx)
        {
            string[] messages = new string[] { };
            DateTime dt = System.DateTime.Now;
            string fridayMessage = "Fun Fact: Today is not Femboy Friday.";
            if (dt.DayOfWeek == DayOfWeek.Friday)
            {
                fridayMessage = "Fun Fact: Today is Femboy Friday.";
            }
                messages = new string[] 
                {  
                    fridayMessage,
                  "https://youtu.be/fN5NQJSqV28" ,
                  "Welcome to Femboy Hooters, may we take your order?",
                  "femboys are the future.",
                  "https://cdn.discordapp.com/attachments/779566889198157825/781060285653975050/0e97db398eae70a9f1b93ab6fce3090d_8153262961374270462.png"
                };
           
            var R = r.Next( 0, messages.Length );
            await ctx.Channel.SendMessageAsync(messages[R]);
        }

        /*
        [Command("flip")]
        [Description("Flips a coin for an given number of times and lists the results.")]
        public async Task Flip(CommandContext ctx, int iterations)
        {
            int h = 0;
            int t = 0;
            for (int i = 0; i < iterations; i++)
            {
                int R = r.Next(2);
                if (R == 0)
                {
                    h += 1;
                }
                else
                {
                    t += 1;
                }
            }
            decimal percent = (decimal)h / (decimal)iterations * (decimal)100;
            var flipEmbed = new DiscordEmbedBuilder
            {
                Title = $"{Decimal.Round(percent, 3)}% of Flips were Heads",
                Color = DiscordColor.Orange,
                Description = $"Heads: {h} Tails: {t}",
            };

            await ctx.Channel.SendMessageAsync(embed: flipEmbed).ConfigureAwait(false);
        }
        */
    }
}

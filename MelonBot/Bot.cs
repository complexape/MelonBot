using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using MelonBot.Bots.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MelonBot.Bots
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var SR = new StreamReader(fs, new UTF8Encoding(false)))
                json = SR.ReadToEnd();
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            string[] prefixes = configJson.Prefix.Split(',');

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2),
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = prefixes,
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
                IgnoreExtraArguments = true,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<HelpCommands>();
            Commands.RegisterCommands<PrefixCommands>();
            Commands.RegisterCommands<InteractivityCommands>();
            Commands.RegisterCommands<MALCommands>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<DbCommands>();
            // remember to write line above to register commands in new classes//

            await Client.ConnectAsync();

            await Task.Delay(-1);
           
        }
       
        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            // code here triggered when bot turns on //
            return Task.CompletedTask;
        }
        
    }
}
